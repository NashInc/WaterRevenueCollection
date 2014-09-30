using System.Web.Optimization;

namespace SysWaterRev.ManagementPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-{version}.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryplugins").Include(
                "~/Scripts/jquery.sparkline-{version}.js",
                "~/Scripts/jquery.knob.js",
                "~/Scripts/jquery-jvectormap-1.2.2.js",
                "~/Scripts/jVectorMaps/world-ISO.A3/jquery-jvectormap-world-mill-en-US.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/pickers").Include(
                "~/Content/template/js/plugins/daterangepicker/daterangepicker.js",
                "~/Content/template/js/plugins/datepicker/bootstrap-datepicker.js",
                "~/Content/template/js/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                "~/Content/template/js/plugins/iCheck/icheck.min.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/morris").Include(
                "~/Scripts/morris.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/raphael").Include(
                "~/Scripts/raphael.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

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
                "~/Scripts/icheck.js",
                "~/Scripts/app.js",
                "~/Scripts/demo.js"
                ));

            //CSS Bundles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.css",
                "~/Content/ionicons.css",
                "~/Content/Site.css"
                ));

            bundles.Add(new StyleBundle("~/Content/kendo").Include(
                "~/Content/kendo/2014.2.903/kendo.common.min.css",
                "~/Content/kendo/2014.2.903/kendo.mobile.all.min.css",
                "~/Content/kendo/2014.2.903/kendo.dataviz.min.css",
                "~/Content/kendo/2014.2.903/kendo.bootstrap.min.css",
                "~/Content/kendo/2014.2.903/kendo.dataviz.bootstrap.min.css"
                ));
            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}