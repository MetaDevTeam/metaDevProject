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
    public class TagController : Controller
    {
        // GET: Tag
        public ActionResult ListArticlesByTag(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                // Get article from database
                var articles = db.RecipeTags
                    .Include(t => t.Recipes.Select(a => a.RecipeTags))
                    .Include(t => t.Recipes.Select(a => a.Author))
                    .FirstOrDefault(t => t.Id == id)
                    .Recipes
                    .ToList();

                // Return the view
                return View(articles);
            }
        }

        // GET: Tag
        public ActionResult ListRecipesByTag(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                // Get article from database
                var recipes = db.Tags
                    .Include(r => r.Articles.Select(a => a.Tags))
                    .Include(r => r.Articles.Select(a => a.Author))
                    .FirstOrDefault(t => t.Id == id)
                    .Articles
                    .ToList();

                // Return the view
                return View(articles);
            }
        }
    }
}