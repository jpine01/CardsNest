﻿using System.Web;
using System.Web.Optimization;

namespace UofLConnect
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/calendarJS").Include(
                        "~/Scripts/jquery-3.4.1.min.js",
                        "~/Scripts/moment.min.js",
                        "~/Scripts/fullcalendar.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                        "~/Scripts/DataTables/jquery.dataTables.min.js",
                        "~/Scripts/DataTables/dataTables.bootstrap.min.js",
                        "~/Scripts/DataTables/dataTables.responsive.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/calendarCSS").Include(
                      "~/Content/fullcalendar.css"));

            bundles.Add(new StyleBundle("~/Content/table").Include(
                      "~/Content/DataTables/css/jquery.dataTables.min.css",
                      "~/Content/DataTables/css/responsive.dataTables.min.css"));
        }
    }
}
