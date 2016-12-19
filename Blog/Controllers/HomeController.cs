using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("ListRecipeCategories");
        }

        public ActionResult ListCategories()
        {
            using (var db = new BlogDbContext())
            {
                var categories = db.Categories
                    .Include(c => c.Articles)
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(categories);
            }
        }

        public ActionResult ListArticles(int? categoryId)
        {
            if (categoryId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var articles = db.Articles
                    .Where(a => a.CategoryId == categoryId)
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .ToList();

                return View(articles);
            }
        }

        public ActionResult ListRecipes(int? recipeCategoryId)
        {
            if (recipeCategoryId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var recipes = db.Recipes
                    .Where(a => a.RecipeCategoryId == recipeCategoryId)
                    .Include(a => a.Author)
                    .Include(a => a.RecipeTags)
                    .ToList();

                return View(recipes);
            }
        }

        public ActionResult ListRecipeCategories()
        {
            using (var db = new BlogDbContext())
            {
                var recipeCategories = db.RecipeCategories
                    .Include(c => c.Recipes)
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(recipeCategories);
            }
        }
    }
}