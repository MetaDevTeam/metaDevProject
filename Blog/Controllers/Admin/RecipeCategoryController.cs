using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class RecipeCategoryController : Controller
    {
        private BlogDbContext db = new BlogDbContext();

        // GET: RecipeCategory
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: RecipeCategory/List
        public ActionResult List()
        {
            using (var db = new BlogDbContext())
            {
                var recipeCaregories = db.RecipeCategories
                    .ToList();

                return View(recipeCaregories);
            }
        }

        // GET: RecipeCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RecipeCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] RecipeCategory recipeCategory)
        {
            if (ModelState.IsValid)
            {
                db.RecipeCategories.Add(recipeCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(recipeCategory);
        }

        // GET: RecipeCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var recipeCategory = db.RecipeCategories
                    .FirstOrDefault(c => c.id == id);

                if (recipeCategory == null)
                {
                    return HttpNotFound();
                }

                return View(recipeCategory);
            }
        }

        // POST: RecipeCategory/Edit
        [HttpPost]
        public ActionResult Edit(RecipeCategory recipeCategory)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    db.Entry(recipeCategory).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(recipeCategory);
        }

        // GET: RecipeCategory/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var recipeCategory = db.RecipeCategories
                       .FirstOrDefault(c => c.id == id);

                if (recipeCategory == null)
                {
                    return HttpNotFound();
                }

                return View(recipeCategory);
            }
        }

        // POST: RecipeCategory/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new BlogDbContext())
            {
                var recipeCategory = db.RecipeCategories
                       .FirstOrDefault(c => c.id == id);

                var categoryReci = recipeCategory.Recipes
                    .ToList();

                foreach (var recipe in categoryReci)
                {
                    db.Recipes.Remove(recipe);
                }

                db.RecipeCategories.Remove(recipeCategory);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}