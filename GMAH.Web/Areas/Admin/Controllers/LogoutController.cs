using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    [JwtAuthentication]
    public class LogoutController : Controller
    {
        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("thoat")]
        public ActionResult Index()
        {
            // Xoá toàn bộ session
            Session.Clear();

            // Trở về trang đăng nhập
            return RedirectToAction("Index", "Login");
        }
    }
}