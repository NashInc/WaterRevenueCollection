using System.Web.Optimization;

namespace SysWaterRev.ManagementPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-2.1.1.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.11.1.js"
                ));          
            bundles.Add(new ScriptBundle("~/bundles/morris").Include(
                "~/Scripts/morris.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/raphael").Include(
                "~/Scripts/raphael.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"
                ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/kendo/2014.2.903/kendo.all.min.js",
                "~/Scripts/kendo/2014.2.903/kendo.aspnetmvc.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/template").Include(              
                "~/Scripts/app.js",
                "~/Scripts/demo.js"
                ));

            //CSS Bundles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.css",
                "~/Content/ionicons.css"                
                ));           
            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}