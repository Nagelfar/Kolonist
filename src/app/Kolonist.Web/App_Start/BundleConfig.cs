using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;
using System.Linq;
using Castle.MicroKernel.Registration;

namespace Kolonist.Web
{
    public class BundleConfig : IWindsorInstaller
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            var allBundles = GetScriptBundles().Concat(GetStyleBundles());

            foreach (var bundle in allBundles)
                bundles.Add(bundle);

        }

        public static IEnumerable<Bundle> GetScriptBundles()
        {
            yield return new ScriptBundle("~/bundles/jquery")
                .Include(
                    "~/Scripts/jquery-{version}.js"
                );

            yield return new ScriptBundle("~/bundles/jqueryui")
                .Include(
                    "~/Scripts/jquery-ui-{version}.js"
                );

            yield return new ScriptBundle("~/bundles/jqueryval")
                .Include(
                    "~/Scripts/jquery.unobtrusive*",
                    "~/Scripts/jquery.validate*"
                );

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            yield return new ScriptBundle("~/bundles/modernizr")
                .Include(
                    "~/Scripts/modernizr-*"
                );

            yield return new ScriptBundle("~/bundles/webgl")
                .Include(
                    "~/Scripts/glMatrix*",
                    "~/Scripts/three.*"
                );

            yield return new ScriptBundle("~/bundles/kolonist")
                .IncludeDirectory(
                    "~/Scripts/Kolonist", "*.js"
                );

            yield return new ScriptBundle("~/bundles/bootstrap")
                .Include(
                    "~/Scripts/bootstrap.*"
                );

            yield return new ScriptBundle("~/bundles/less").Include(
                    "~/Scripts/less.*"
                );
        }

        public static IEnumerable<Bundle> GetStyleBundles()
        {
            yield return new StyleBundle("~/Content/site")
                .Include("~/Content/site.less");

            yield return new StyleBundle("~/Content/themes/base/css")
                .Include(
                    "~/Content/themes/base/jquery.ui.core.css",
                    "~/Content/themes/base/jquery.ui.resizable.css",
                    "~/Content/themes/base/jquery.ui.selectable.css",
                    "~/Content/themes/base/jquery.ui.accordion.css",
                    "~/Content/themes/base/jquery.ui.autocomplete.css",
                    "~/Content/themes/base/jquery.ui.button.css",
                    "~/Content/themes/base/jquery.ui.dialog.css",
                    "~/Content/themes/base/jquery.ui.slider.css",
                    "~/Content/themes/base/jquery.ui.tabs.css",
                    "~/Content/themes/base/jquery.ui.datepicker.css",
                    "~/Content/themes/base/jquery.ui.progressbar.css",
                    "~/Content/themes/base/jquery.ui.theme.css"
                );

            yield return new StyleBundle("~/Content/bootstrap")
                .Include(
                    "~/Content/less/bootstrap.less",
                    "~/Content/less/responsive.less"
                );

        }

        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            var bundles = BundleTable.Bundles;

            RegisterBundles(bundles);
        }
    }
}