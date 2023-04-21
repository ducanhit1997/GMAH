using System.Web.Mvc;

namespace GMAH.Web.Areas.Student
{
    public class StudentAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Student";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            // Map attribute route on each controller class
            context.Routes.MapMvcAttributeRoutes();

            context.MapRoute(
                "Student_default",
                "{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}