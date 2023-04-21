using System.Collections.Generic;
using System.Linq;

namespace GMAH.Models.Consts
{
    public static class MenuConst
    {
        /// <summary>
        /// Lấy danh sách menu theo role
        /// </summary>
        /// <returns></returns>
        public static List<MenuAdmin> GetMenuAdmin(RoleEnum role)
        {
            var listParentByRole = GetAll().Where(x => x.Roles is null || x.Roles.Any(r => r == role)).ToList();

            foreach (var menu in listParentByRole)
            {
                if (menu.ListChild is null) continue;
                menu.ListChild = menu.ListChild.Where(x => x.Roles is null || x.Roles.Any(r => r == role)).ToList();
            }

            return listParentByRole;
        }

        // Danh sách toàn bộ list menu
        public static List<MenuAdmin> GetAll()
        {
            var listMenu = new List<MenuAdmin>();

            // Define từng menu
            var menuAccount = new MenuAdmin
            {
                Icon = "fa fa-users",
                Title = "Người dùng",
                Controller = "User",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT },

                // Danh sách menu con
                ListChild = new List<MenuAdmin>
                {
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Quản trị viên",
                        Controller = "User",
                        Action = "Administrator",
                        Roles = new List<RoleEnum> { RoleEnum.MANAGER },
                    },
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Giáo viên",
                        Controller = "User",
                        Action = "Teacher"
                    },
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Phụ huynh",
                        Controller = "User",
                        Action = "Parent"
                    },
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Học sinh",
                        Controller = "User",
                        Action = "Student"
                    }
                }
            };

            var menuSemester = new MenuAdmin
            {
                Icon = "fas fa-flag",
                Title = "Học kỳ",
                Controller = "Semester",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT },
            };

            var menuRule = new MenuAdmin
            {
                Icon = "fas fa-ruler-vertical",
                Title = "Luật xếp hạng",
                Controller = "Grade",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT },
            };

            var menuSubject = new MenuAdmin
            {
                Icon = "fas fa-book",
                Title = "Môn học",
                Controller = "Subject",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT },
                // Danh sách menu con
                ListChild = new List<MenuAdmin>
                {
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Danh sách bộ môn",
                        Controller = "Subject",
                        Action = "Index",
                        Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT },
                    },
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Giáo viên bộ môn",
                        Controller = "Subject",
                        Action = "Teacher"
                    },
                }
            };

            var menuClass = new MenuAdmin
            {
                Icon = "fas fa-university",
                Title = "Lớp học",
                Controller = "Class",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER },
                // Danh sách menu con
                ListChild = new List<MenuAdmin>
                {
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Danh sách lớp",
                        Controller = "Class",
                        Action = "Index",
                        Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT },
                    },
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Danh sách học sinh",
                        Controller = "Class",
                        Action = "Student"
                    },
                    new MenuAdmin
                    {
                        Icon = "fas fa-calendar",
                        Title = "Thời khoá biểu",
                        Controller = "Timeline",
                        Action = "Index"
                    },
                }
            };

            var menuScoreType = new MenuAdmin
            {
                Icon = "fas fa-th-list",
                Title = "Thành phần điểm",
                Controller = "ScoreType",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER },
            };

            var menuScore = new MenuAdmin
            {
                Icon = "fas fa-graduation-cap",
                Title = "Điểm học sinh",
                Controller = "Score",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER },
            };

            var menuAttendance = new MenuAdmin
            {
                Icon = "fas fa-user-check",
                Title = "Điểm danh",
                Controller = "Attendance",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER },
                // Danh sách menu con
                ListChild = new List<MenuAdmin>
                {
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Điểm danh học sinh",
                        Controller = "Attendance",
                        Action = "Index",
                        Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT },
                    },
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Danh sách điểm danh",
                        Controller = "Attendance",
                        Action = "List"
                    },
                }
            };

            var menuReport = new MenuAdmin
            {
                Icon = "fas fa-sticky-note",
                Title = "Báo cáo",
                Controller = "Report",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER },
                // Danh sách menu con
                ListChild = new List<MenuAdmin>
                {
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Xem toàn bộ",
                        Controller = "Report",
                        Action = "Index",
                        Roles = new List<RoleEnum> { RoleEnum.MANAGER, RoleEnum.ASSISTANT },
                    },
                    new MenuAdmin
                    {
                        Icon = "fas fa-chevron-circle-right",
                        Title = "Báo cáo cần duyệt",
                        Controller = "Report",
                        Action = "Review"
                    },
                }
            };

            var menuSetting = new MenuAdmin
            {
                Icon = "fas fa-cogs",
                Title = "Thiết lập",
                Controller = "Setting",
                Action = "Index",
                Roles = new List<RoleEnum> { RoleEnum.MANAGER },
            };

            var menuLogout = new MenuAdmin
            {
                Icon = "fas fa-sign-out-alt",
                Title = "Đăng xuất",
                Controller = "Logout",
                Action = "Index",
            };

            // Add danh sách menu parent vào
            listMenu.Add(menuAccount);
            listMenu.Add(menuSemester);
            listMenu.Add(menuSubject);
            listMenu.Add(menuRule);
            listMenu.Add(menuClass);
            listMenu.Add(menuScoreType);
            listMenu.Add(menuScore);            
            listMenu.Add(menuAttendance);
            listMenu.Add(menuReport);
            listMenu.Add(menuSetting);
            listMenu.Add(menuLogout);

            return listMenu;
        }
    }

    public class MenuAdmin
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        // Role có thể xem được menu
        public List<RoleEnum> Roles { get; set; }
        public List<MenuAdmin> ListChild { get; set; }
    }
}
