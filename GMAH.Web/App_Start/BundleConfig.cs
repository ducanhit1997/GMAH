using System.Web;
using System.Web.Optimization;

namespace GMAH.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Create bundles for admin
            bundles.Add(new ScriptBundle("~/js/admin").Include(
                "~/Assests/Plugin/js/jquery.min.js",
                "~/Assests/Plugin/js/bootstrap.bundle.min.js",
                "~/Assests/Admin/js/sb-admin-2.min.js",
                "~/Assests/Plugin/js/moment.min.js",
                "~/Assests/Plugin/js/tempusdominus-bootstrap-4.min.js",
                "~/Assests/Plugin/js/select2.full.min.js",
                "~/Assests/Plugin/js/jquery.bootstrap-duallistbox.min.js"));

            bundles.Add(new StyleBundle("~/css/admin").Include(
                "~/Assests/Admin/css/sb-admin-2.min.css",
                "~/Assests/Plugin/css/all.min.css",
                "~/Assests/Plugin/css/icheck-bootstrap.min.css",
                "~/Assests/Student/css/attendance.css",
                "~/Assests/Plugin/css/tempusdominus-bootstrap-4.min.css",
                "~/Assests/Plugin/css/select2.min.css",
                "~/Assests/Plugin/css/select2-bootstrap4.min.css",
                "~/Assests/Plugin/css/bootstrap-duallistbox.min.css"));

            // Create bundles for student portal
            bundles.Add(new ScriptBundle("~/js/student").Include(
                "~/Assests/Plugin/js/sb-admin-2.min.js",
                "~/Assests/Plugin/js/jquery.min.js",
                "~/Assests/Student/js/bootstrap.bundle.min.js",
                "~/Assests/Plugin/js/moment.min.js",
                "~/Assests/Plugin/js/tempusdominus-bootstrap-4.min.js",
                "~/Assests/Plugin/js/select2.full.min.js"));

            bundles.Add(new StyleBundle("~/css/student").Include(
                "~/Assests/Student/css/sb-admin-2.min.css",
                "~/Assests/Student/css/bootstrap.min.css",
                "~/Assests/Student/css/main.css",
                "~/Assests/Student/css/attendance.css",
                "~/Assests/Plugin/css/all.min.css",
                "~/Assests/Plugin/css/select2.min.css",
                "~/Assests/Plugin/css/tempusdominus-bootstrap-4.min.css",
                "~/Assests/Plugin/css/select2-bootstrap4.min.css"));

            // DataTable
            bundles.Add(new ScriptBundle("~/js/datatable").Include(
                "~/Assests/Plugin/js/jquery.dataTables.min.js",
                "~/Assests/Plugin/js/dataTables.bootstrap4.min.js",
                "~/Assests/Plugin/js/dataTables.responsive.min.js",
                "~/Assests/Plugin/js/responsive.bootstrap4.min.js"));

            bundles.Add(new StyleBundle("~/css/datatable").Include(
                "~/Assests/Plugin/css/dataTables.bootstrap4.min.css",
                "~/Assests/Plugin/css/responsive.bootstrap4.min.css"));

            // Ckeditor
            bundles.Add(new Bundle("~/js/ckeditor").Include(
                "~/Assests/Plugin/js/ckeditor.js"));

            bundles.Add(new StyleBundle("~/css/ckeditor").Include(
                "~/Assests/Plugin/css/ckeditor.css"));
        }
    }
}
