using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    [JwtAuthentication(RoleEnum.MANAGER)]
    public class SettingController : Controller
    {
        private SystemSettingService settingService;

        public SettingController()
        {
            settingService = new SystemSettingService();
        }

        [HttpGet]
        [Route("thietlap")]
        public ActionResult Index()
        {
            var settings = settingService.GetAllSetting();
            return View(settings);
        }

        [HttpPost]
        [Route("thietlap")]
        [ValidateInput(false)]
        public ActionResult Index(List<SettingViewModel> settings)
        {
            // Lưu dữ liệu
            var result = settingService.SaveAllSetting(settings);

            // Báo lỗi nếu lưu ko thành công
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Message;
            }
            else
            {
                ViewBag.Success = "Lưu thiết lập thành công";
            }

            // Trả ViewModel cho view
            return View(settings);
        }
    }
}