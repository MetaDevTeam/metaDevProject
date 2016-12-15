﻿using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            using (var database = new BlogDbContext())
            {
                var recipes = database.Articles
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
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

            using (var database = new BlogDbContext())
            {
                var recipe = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
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
                var model = new ArticleViewModel();
                model.Categories = db.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(model);
            }
        }

        //POST: Recipe/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(RecipeViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    //Get Author id
                    var authorId = db.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    var recipe = new Recipe(
                        authorId,
                        model.Title,
                        model.Content,
                        model.CategoryId);

                    this.SetRecipeTags(recipe, model, db);
                    //Set recipes author
                    recipe.AuthorId = authorId;

                    //Save recipe in DB
                    db.Recipes.Add(recipe);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        //
        //GET: Recipe/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var recipe = database.Recipes
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Category)
                    .First();

                if (!IsUserAuthorizedToEdit(recipe))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                ViewBag.TagsString = string.Join(", ", recipe.Tags.Select(t => t.Name));

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
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
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
                    .Where(a => a.Id == id)
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
                var model = new ArticleViewModel();
                model.Id = recipe.Id;
                model.Title = recipe.Title;
                model.Content = recipe.Content;
                model.CategoryId = recipe.CategoryId;
                model.Categories = db.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                model.Tags = string.Join(", ", recipe.Tags.Select(t => t.Name));

                //Pass the view model to view
                return View(model);
            }
        }

        //POST: Recipe/Edit
        [HttpPost]
        public ActionResult Edit(RecipeViewModel model)
        {
            //Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    //Get recipe from database
                    var recipe = database.Recipes
                        .FirstOrDefault(a => a.Id == model.Id);

                    //Set article properties
                    recipe.Title = model.Title;
                    recipe.Content = model.Content;
                    recipe.CategoryId = model.CategoryId;
                    this.SetRecipeTags(recipe, model, database);

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
            var tagsStrings = model.Tags
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .Distinct();

            // Clear all current article tags
            recipe.Tags.Clear();

            // Set new article tags
            foreach (var tagString in tagsStrings)
            {
                // Get tagfrom db by its name
                Tag tag = db.Tags.FirstOrDefault(t => t.Name.Equals(tagString));

                // If the tag is null, create new tag
                if (tag == null)
                {
                    tag = new Tag() { Name = tagString };
                    db.Tags.Add(tag);
                }

                // Add tag to article tags
                recipe.Tags.Add(tag);
            }
        }
    }
}