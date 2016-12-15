using Blog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers.Admin
{

    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {

        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        //GET: User/List
        public ActionResult List()
        {
            using(var database = new BlogDbContext())
            {
                var users = database.Users
                    .ToList();

                var admins = GetAdminUserNames(users, database);
                ViewBag.Admins = admins;

                return View(users);
            }
        }

        private HashSet<string> GetAdminUserNames(List<ApplicationUser> users, BlogDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            var admins = new HashSet<string>();

            foreach (var user in users)
            {
                if (userManager.IsInRole(user.Id, "Admin"))
                {
                    admins.Add(user.UserName);
                }
            }

            return admins;
        }

        //
        //GET: User/Edit
        public ActionResult Edit(string id)
        {
            //Validate Id
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using(var db = new BlogDbContext())
            {
                //GET: user from database
                var user = db.Users.FirstOrDefault(u => u.Id.Equals(id));

                //Check if user exist
                if(user == null)
                {
                    return HttpNotFound();
                }
                
                var viewModel = new EditUserViewModel();
                viewModel.Email = user.Email;
                viewModel.FullName = user.FullName;
                viewModel.Roles = GetUserRoles(user, db);

                //Pass the model to the view
                return View(viewModel);
            }
        }

        //
        //POST: User/Edit
        [HttpPost]
        public ActionResult Edit(string id, EditUserViewModel viewModel)
        {
            // Check if model is valid
            if (ModelState.IsValid)
            {
                using(var db = new BlogDbContext())
                {
                    // GET user from database
                    var user = db.Users.FirstOrDefault(u => u.Id == id);

                    // Check if user exists
                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    // If password fild is not empty, change password
                    if (!string.IsNullOrEmpty(viewModel.Password))
                    {
                        var hasher = new PasswordHasher();
                        var PasswordHash = hasher.HashPassword(viewModel.Password);
                        user.PasswordHash = PasswordHash;
                    }

                    // Set user properties
                    user.Email = viewModel.Email;
                    user.FullName = viewModel.FullName;
                    user.UserName = viewModel.Email;
                    this.SetUserRoles(viewModel, user, db);

                    // Save changes
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("List");
                }
            }

            return View(viewModel);
        }

        //
        // GET: User/Delete
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id.Equals(id));

                if (user == null)
                {
                    return HttpNotFound();
                }

                return View(user);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id.Equals(id));

                var userArticles = db.Articles
                    .Where(a => a.Author.Id == user.Id);

                foreach (var article in userArticles)
                {
                    db.Articles.Remove(article);
                }

                db.Users.Remove(user);
                db.SaveChanges();

                return RedirectToAction("List");
            }

            //return View();
        }

        private void SetUserRoles(EditUserViewModel viewModel, ApplicationUser user, BlogDbContext db)
        {
            var userManager = Request.GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            foreach (var role in viewModel.Roles)
            {
                if (role.IsSelected && !userManager.IsInRole(user.Id, role.Name))
                {
                    userManager.AddToRole(user.Id, role.Name);
                }

                else if (!role.IsSelected && userManager.IsInRole(user.Id, role.Name))
                {
                    userManager.RemoveFromRole(user.Id, role.Name);
                }
            }
        }

        private List<Role> GetUserRoles(ApplicationUser user, BlogDbContext db)
        {
            //Get all application roles
            var roles = db.Roles
                .Select(r => r.Name)
                .OrderBy(r => r)
                .ToList();

            // Create user manager
            var userManager = Request
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            // For each application role, check if the user has it
            List<Role> userRoles = new List<Role>();

            foreach (var roleName in roles)
            {
                Role role = new Role() { Name = roleName };

                if (userManager.IsInRole(user.Id, roleName))
                {
                    role.IsSelected = true;
                }

                userRoles.Add(role);
            }

            // Return a list with all roles
            return userRoles;
        }
    }
}