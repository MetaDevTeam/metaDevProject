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
    public class RecipeTagController : Controller
    {
        // GET: RecipeTag
        public ActionResult ListRecipesByTag(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                // Get recipe from database
                var recipes = db.RecipeTags
                    .Include(t => t.Recipes.Select(a => a.RecipeTags))
                    .Include(t => t.Recipes.Select(a => a.Author))
                    .FirstOrDefault(t => t.Id == id)
                    .Recipes
                    .ToList();

                // Return the view
                return View(recipes);
            }
        }
    }
}