using System.Web;
using System.Web.Optimization;

namespace ACT.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles( BundleCollection bundles )
        {
            bundles.Add( new ScriptBundle( "~/bundles/jquery" ).Include(
                        "~/Scripts/jquery-{version}.js" ) );

            bundles.Add( new ScriptBundle( "~/bundles/jqueryval" ).Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*" ) );

            bundles.Add( new ScriptBundle( "~/bundles/jqueryui" ).Include(
                        "~/Scripts/JQueryUI/jquery-ui-1.10.4.custom.js",
                        "~/Scripts/JQueryUI/timepicker.js" ) );

            bundles.Add( new ScriptBundle( "~/bundles/plugins" ).Include(
                        "~/Scripts/Plugins/tipsy.js",
                        "~/Scripts/Plugins/select2.js",
                        "~/Scripts/Plugins/highcharts.js",
                        "~/Scripts/Plugins/jquery.form.js",
                        "~/Scripts/Plugins/timer.jquery.js",
                        "~/Scripts/Plugins/jquery.fancybox.js",
                        //"~/Scripts/Plugins/jquery.dataTables.js",
                        "~/Scripts/Plugins/jquery.placeholder.js",
                        "~/Scripts/Plugins/jquery.dataTables.min.js",
                        "~/Scripts/Plugins/dataTables.select.min.js",
                        "~/Scripts/Plugins/toastr.min.js",
                        "~/Scripts/Plugins/moment.js",
                        "~/Scripts/Plugins/jquery.modal.min.js"
                        /*,
                        "~/Scripts/Plugins/dataTables.fixedHeader.min.js"*/ ) );

            bundles.Add( new ScriptBundle( "~/bundles/ACT" ).Include(
                        "~/Scripts/ACT/act.js",
                        "~/Scripts/ACT/menu.js",
                        "~/Scripts/ACT/ui.js",
                        "~/Scripts/ACT/modal.js",
                        "~/Scripts/ACT/loader.js",
                        "~/Scripts/ACT/stickyone.js",
                        "~/Scripts/ACT/validation.js",
                        "~/Scripts/ACT/startup.js") );

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add( new ScriptBundle( "~/bundles/modernizr" ).Include(
                        "~/Scripts/modernizr-*" ) );




            bundles.Add( new StyleBundle( "~/Content/css" ).Include(
                "~/Content/reset.css",
                "~/Content/tipsy.css",
                "~/Content/style.css",
                "~/Content/menu.css",
                "~/Content/modal.css",
                "~/Content/table.css",
                "~/Content/filter.css",
                "~/Content/ap-tabs.css",
                "~/Content/ap-tabs.css",
                "~/Content/select2.css",
                "~/Content/checkbox.css",
                "~/Content/stickyone.css",
                "~/Content/font-awesome.css",
                "~/Content/jquery.fancybox.css",
                "~/Content/fixedHeader.dataTables.min.css",
                "~/Content/toastr.min.css",
                "~/Content/jquery.modal.min.css") );

            bundles.Add( new StyleBundle( "~/Content/jqueryui" ).Include( "~/Content/jquery-ui-1.10.4.custom.css" ) );
        }
    }
}
