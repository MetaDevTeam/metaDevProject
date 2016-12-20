using System.Web;
using System.Web.Optimization;

namespace Blog
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.1.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                        "~/Scripts/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/fancybox").Include(
                      "~/Scripts/jquery.fancybox/jquery.fancybox.pack.js"));

            bundles.Add(new ScriptBundle("~/bundles/responsive-nav").Include(
                      "~/Scripts/responsive-nav/responsive-nav.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootsrap.min.css",
                      "~/Content/bootsrap-responsive.min.css",
                      "~/Content/main.css",
                      "~/Content/fonts.css",
                      "~/Content/common.css",
                      "~/Content/forms.css",
                      "~/Content/gallery.css",
                      "~/Content/jquery.fancybox.css",
                      "~/Content/media.css",
                      "~/Content/navigation.css",
                      "~/Content/responsive-nav.css",
                      "~/Content/typography.css",
                      "~/Content/admin-user-panel.css"));
        }
    }
}
