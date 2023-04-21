using GMAH.Models.Consts;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GMAH.Web.Helpers.Attributes
{
    public class JwtAuthenticationAttribute : AuthorizeAttribute
    {
        private readonly RoleEnum[] _allowRoles;
        private readonly string _action;
        private string _errorMsg;

        // Default constructor, chấp nhận mọi role, mọi action, chỉ cần login
        public JwtAuthenticationAttribute()
        {
        }

        // Define role và action name được phép execute method
        public JwtAuthenticationAttribute(string action)
        {
            _action = action;
        }

        public JwtAuthenticationAttribute(params RoleEnum[] allowRoles)
        {
            _allowRoles = allowRoles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Kiểm tra nếu có jwt token từ session thì dùng của session
            string jwtToken = SessionHelper.GetSession("JWT_TOKEN");
            if (jwtToken is null)
            {
                // Lấy từ header
                jwtToken = httpContext.Request.Headers["Authorization"]?.Replace("Bearer", string.Empty).Trim();
            }

            // Nếu tiếp tục null thì return false
            if (jwtToken is null)
            {
                return false;
            }

            // Check token hợp lệ hay không
            try
            {
                // Decode token thành object
                var jwtHelper = new JWTHelper();
                var userIdentity = jwtHelper.DecodeToken(jwtToken);

                _errorMsg = "Bạn không có quyền truy cập chức năng này";

                // Kiểm tra role và action nếu cần
                if (_allowRoles != null)
                {
                    return _allowRoles.Any(x => (int)x == userIdentity.IdRole);
                }

                if (_action != null)
                {
                    return userIdentity.Permission is null ||
                        userIdentity.Permission.Any(x => x.Equals(_action, StringComparison.OrdinalIgnoreCase));
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Controller.TempData["Error"] = _errorMsg;
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Login" }));
        }
    }
}