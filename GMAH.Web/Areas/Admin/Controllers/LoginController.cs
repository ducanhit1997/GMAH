using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class LoginController : Controller
    {
        [Route("dangnhap")]
        [HttpGet]
        public ActionResult Index()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            return View();
        }

        [Route("dangnhap")]
        [HttpPost]
        public ActionResult Index(LoginRequest data)
        {
            var loginService = new LoginService();
            var loginResponse = loginService.LoginGetToken(data.Username, data.Password, new JWTHelper());

            if (loginResponse.IsSuccess)
            {
                // Storge token & user object
                SessionHelper.SaveSession("JWT_TOKEN", loginResponse.Token);
                SessionHelper.SaveSession("USER", loginResponse.User);

                // Redirect to homepage
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = loginResponse.Message;
            }

            return View();
        }
    }
}