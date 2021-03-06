﻿using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class RecipeController : Controller
    {
        private bool IsUserAuthorizedToEdit(Recipe recipe)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = recipe.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }


        // GET: Recipe
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            using (var db = new BlogDbContext())
            {
                var recipes = db.Recipes
                    .Include(r => r.Author)
                    .Include(r => r.RecipeTags)
                    .Include(r => r.Ingredients)
                    .ToList();

                return View(recipes);
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var recipe = db.Recipes
                    .Where(r => r.Id == id)
                    .Include(r => r.Author)
                    .Include(r => r.RecipeTags)
                    .Include(r => r.Ingredients)
                    .First();

                if (recipe == null)
                {
                    return HttpNotFound();
                }

                return View(recipe);
            }
        }

        //
        //GET: Recipe/Create
        [Authorize]
        public ActionResult Create()
        {
            using (var db = new BlogDbContext())
            {
                var model = new RecipeViewModel();
                model.RecipeCategories = db.RecipeCategories
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(model);
            }
        }

        //POST: Recipe/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RecipeViewModel model, HttpPostedFileBase upload)
        {
            //if (ModelState.IsValid)
            //{
            using (var db = new BlogDbContext())
            {
                //if (upload != null && upload.ContentLength > 0)
                //{
                //    var avatar = new File
                //    {
                //        FileName = System.IO.Path.GetFileName(upload.FileName),
                //        FileType = FileType.Avatar,
                //        ContentType = upload.ContentType
                //    };
                //    var reader = new System.IO.BinaryReader(upload.InputStream);

                //    avatar.Content = reader.ReadBytes(upload.ContentLength);

                //    //recipeFile.Files = new List<File> { avatar };
                //}


                

                //Get Author id
                var authorId = db.Users
                    .Where(u => u.UserName == this.User.Identity.Name)
                    .First()
                    .Id;

                var recipe = new Recipe(
                    authorId,
                    model.Title,
                    model.Content,
                    model.RecipeCategoryId);

                if (upload != null)
                {
                    const int filenameLength = 20;
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                    var random = new Random();

                    var randomFileName = new string(Enumerable.Repeat(chars, filenameLength)
                      .Select(s => s[random.Next(s.Length)]).ToArray());

                    randomFileName = randomFileName + Path.GetExtension(upload.FileName);

                    var absolutePath = Server.MapPath("~/Content/Images/Recipes/");
                    upload.SaveAs(absolutePath + randomFileName);

                    recipe.ImagePath = $"/Content/Images/Recipes/{randomFileName}";
                }

                this.SetRecipeTags(recipe, model, db);
                this.SetRecipeIngredients(recipe, model, db);
                //Set recipes author
                recipe.AuthorId = authorId;

                //Save recipe in DB
                //db.Recipes.Add(recipeFile);
                db.Recipes.Add(recipe);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            //}

            // return View(model);
        }

        //
        //GET: Recipe/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var recipe = database.Recipes
                    .Where(r => r.Id == id)
                    .Include(r => r.Author)
                    .Include(r => r.RecipeCategory)
                    .First();

                if (!IsUserAuthorizedToEdit(recipe))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                ViewBag.TagsString = string.Join(", ", recipe.RecipeTags.Select(t => t.Name));
                ViewBag.IngredientsStrings = string.Join(",", recipe.Ingredients.Select(i => i.Name));

                //Check if recipe exists
                if (recipe == null)
                {
                    return HttpNotFound();
                }

                //Pass recipe to view
                return View(recipe);
            }
        }

        //
        //POST: Recipe/Delete
        [HttpPost]
        [Authorize]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                //Get recipe from db
                var recipe = database.Recipes
                    .Where(r => r.Id == id)
                    .Include(r => r.Author)
                    .First();

                //Check if recipe exists
                if (recipe == null)
                {
                    return HttpNotFound();
                }

                //Remove article from db
                database.Recipes.Remove(recipe);
                database.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        //GET: Recipe/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                //Get recipe from database
                var recipe = db.Recipes
                    .Where(r => r.Id == id)
                    .First();

                if (!IsUserAuthorizedToEdit(recipe))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                //Check if recipe exists
                if (recipe == null)
                {
                    return HttpNotFound();
                }

                //Create the view model
                var model = new RecipeViewModel();
                model.Id = recipe.Id;
                model.Title = recipe.Title;
                model.Content = recipe.Content;
                model.RecipeCategoryId = recipe.RecipeCategoryId;
                model.RecipeCategories = db.RecipeCategories
                    .OrderBy(c => c.Name)
                    .ToList();

                model.RecipeTags = string.Join(", ", recipe.RecipeTags.Select(t => t.Name));

                model.Ingredients = string.Join(",", recipe.Ingredients.Select(i => i.Name));

                //Pass the view model to view
                return View(model);
            }
        }

        //POST: Recipe/Edit
        [HttpPost]
        [Authorize]
        public ActionResult Edit(RecipeViewModel model)
        {
            //Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    //Get recipe from database
                    var recipe = database.Recipes
                        .FirstOrDefault(r => r.Id == model.Id);

                    //Set article properties
                    recipe.Title = model.Title;
                    recipe.Content = model.Content;
                    recipe.RecipeCategoryId = model.RecipeCategoryId;
                    this.SetRecipeTags(recipe, model, database);
                    this.SetRecipeIngredients(recipe, model, database);

                    //Save recipe state in database
                    database.Entry(recipe).State = EntityState.Modified;
                    database.SaveChanges();

                    //Redirect to the index page
                    return RedirectToAction("Index");
                }
            }

            //If model is invalid, return the same view
            return View(model);
        }

        private void SetRecipeTags(Recipe recipe, RecipeViewModel model, BlogDbContext db)
        {
            // Split tags
            var tagsStrings = model.RecipeTags
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .Distinct();

            // Clear all current recipe tags
            recipe.RecipeTags.Clear();

            // Set new recipe tags
            foreach (var tagString in tagsStrings)
            {
                // Get tagfrom db by its name
                RecipeTag recipeTag = db.RecipeTags.FirstOrDefault(t => t.Name.Equals(tagString));

                // If the tag is null, create new tag
                if (recipeTag == null)
                {
                    recipeTag = new RecipeTag() { Name = tagString };
                    db.RecipeTags.Add(recipeTag);
                }

                // Add tag to recipe tags
                recipe.RecipeTags.Add(recipeTag);
            }
        }

        private void SetRecipeIngredients(Recipe recipe, RecipeViewModel model, BlogDbContext db)
        {
            // Split ingredients
            var ingredientsStrings = model.Ingredients
                .Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(i => i.ToLower())
                .Distinct();

            // Clear all current recipe ingredients
            recipe.Ingredients.Clear();

            // Set new recipe ingredients
            foreach (var ingredientString in ingredientsStrings)
            {
                // Get ingredient from db by its name
                Ingredient ingredient = db.Ingredients.FirstOrDefault(i => i.Name.Equals(ingredientString));

                // If the tag is null, create new tag
                if (ingredient == null)
                {
                    ingredient = new Ingredient() { Name = ingredientString };
                    db.Ingredients.Add(ingredient);
                }

                // Add ingredient to recipe ingredients
                recipe.Ingredients.Add(ingredient);
            }
        }
    }
}