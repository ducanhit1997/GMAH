using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Web.Helpers.Attributes;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class UserController : Controller
    {
        /// <summary>
        /// Quản lý tài khoản quản trị
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JwtAuthentication(RoleEnum.MANAGER)]
        [Route("taikhoan-quantri")]
        public ActionResult Administrator()
        {
            return View();
        }

        [HttpGet]
        [JwtAuthentication(RoleEnum.MANAGER)]
        [Route("taikhoan-taoquantri")]
        [Route("taikhoan-taoquantri/{id}")]
        public ActionResult InfoAdministrator(int? id)
        {
            ViewBag.IdUSer = id is null ? 0 : id.Value;
            ViewBag.ListRole = new List<RoleViewModel> { 
                new RoleViewModel
                {
                    IdRole = (int)RoleEnum.MANAGER,
                    RoleName = "Quản trị viên",
                },
                new RoleViewModel
                {
                    IdRole = (int)RoleEnum.ASSISTANT,
                    RoleName = "Hỗ trợ viên",
                }
            };

            return View();
        }

        /// <summary>
        /// Quản lý tài khoản giáo viên
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        [Route("taikhoan-giaovien")]
        public ActionResult Teacher()
        {
            return View();
        }

        [HttpGet]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        [Route("taikhoan-taogiaovien")]
        [Route("taikhoan-taogiaovien/{id}")]
        public ActionResult InfoTeacher(int? id)
        {
            ViewBag.IdUSer = id is null ? 0 : id.Value;
            ViewBag.ListRole = new List<RoleViewModel> {
                new RoleViewModel
                {
                    IdRole = (int)RoleEnum.TEACHER,
                    RoleName = "Giáo viên",
                },
                new RoleViewModel
                {
                    IdRole = (int)RoleEnum.HEAD_OF_SUBJECT,
                    RoleName = "Trưởng bộ môn",
                }
            };

            return View();
        }

        /// <summary>
        /// Quản lý tài khoản học sinh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        [Route("taikhoan-hocsinh")]
        public ActionResult Student()
        {
            return View();
        }

        [HttpGet]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        [Route("taikhoan-taohocsinh")]
        [Route("taikhoan-taohocsinh/{id}")]
        public ActionResult InfoStudent(int? id)
        {
            ViewBag.IdUSer = id is null ? 0 : id.Value;
            ViewBag.ListRole = new List<RoleViewModel> {
                new RoleViewModel
                {
                    IdRole = (int)RoleEnum.STUDENT,
                    RoleName = "Học sinh",
                },
            };

            return View();
        }

        /// <summary>
        /// Quản lý tài khoản phụ huynh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        [Route("taikhoan-phuhuynh")]
        public ActionResult Parent()
        {
            return View();
        }

        [HttpGet]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        [Route("taikhoan-taophuhuynh")]
        [Route("taikhoan-taophuhuynh/{id}")]
        public ActionResult InfoParent(int? id)
        {
            ViewBag.IdUSer = id is null ? 0 : id.Value;
            ViewBag.ListRole = new List<RoleViewModel> {
                new RoleViewModel
                {
                    IdRole = (int)RoleEnum.PARENT,
                    RoleName = "Phụ huynh học sinh",
                },
            };

            return View();
        }
    }
}