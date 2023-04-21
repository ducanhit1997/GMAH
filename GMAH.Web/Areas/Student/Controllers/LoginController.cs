using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Student.Controllers
{
    [RouteArea("Student", AreaPrefix = "")]
    public class LoginController : Controller
    {
        [HttpGet]
        [Route("dangnhap")]
        public ActionResult Index()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            return View();
        }

        /// <summary>
        /// Xử lý đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("dangnhap")]
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