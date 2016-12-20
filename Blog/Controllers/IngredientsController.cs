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
    public class IngredientsController : Controller
    {
        // GET: Tag
        public ActionResult ListRecipesByIngredient(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                // Get article from database
                var recipes = db.Ingredients
                    .Include(i => i.Recipes.Select(r => r.Ingredients))
                    .Include(i => i.Recipes.Select(r => r.Author))
                    .FirstOrDefault(i => i.Id == id)
                    .Recipes
                    .ToList();

                // Return the view
                return View(recipes);
            }
        }
    }
}