using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;

namespace Blog.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private BlogDbContext db = new BlogDbContext();

        // GET: Category
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: Category/List
        public ActionResult List()
        {
            using (var db = new BlogDbContext())
            {
                var caregories = db.Categories
                    .ToList();

                return View(caregories);
            }
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var category = db.Categories
                    .FirstOrDefault(c => c.id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        // POST: Category/Edit
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        // GET: Category/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var category = db.Categories
                       .FirstOrDefault(c => c.id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        // POST: Category/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new BlogDbContext())
            {
                var category = db.Categories
                       .FirstOrDefault(c => c.id == id);

                var categoryArticles = category.Articles
                    .ToList();

                foreach (var article in categoryArticles)
                {
                    db.Articles.Remove(article);
                }

                db.Categories.Remove(category);
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
