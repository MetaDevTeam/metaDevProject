using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Blog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Blog.Controllers
{
    public class AboutController : Controller
    {
        private BlogDbContext context = new BlogDbContext();

        // GET: About
        public ActionResult Index()
        {
            using (var database = new BlogDbContext())
            {
                var users = database.Users
                    .ToList();

                var admins = GetAdminUserNames(users, database);
                ViewBag.Admins = admins;
            }
            return View();
        }

        //Try
        private HashSet<string> GetAdminUserNames(List<ApplicationUser> users, BlogDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            var admins = new HashSet<string>();

            foreach (var user in users)
            {
                if (userManager.IsInRole(user.Id, "Admin"))
                {
                    admins.Add(user.FullName);
                }
            }

            return admins;
        }
    }
}