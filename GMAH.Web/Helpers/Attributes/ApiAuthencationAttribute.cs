using GMAH.Models.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GMAH.Web.Helpers.Attributes
{
    /// <summary>
    /// The same idea with JwtAuthenticationAttribute
    /// </summary>
    public class ApiAuthenticationAttribute : AuthorizationFilterAttribute
    {
        private readonly RoleEnum[] _allowRoles;
        private readonly string _action;

        public ApiAuthenticationAttribute()
        {
        }

        // Define role và action name được phép execute method
        public ApiAuthenticationAttribute(string action)
        {
            _action = action;
        }

        public ApiAuthenticationAttribute(params RoleEnum[] allowRoles)
        {
            _allowRoles = allowRoles;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Lấy từ header
            string jwtToken = actionContext.Request.Headers.Authorization?.Parameter?.Replace("Bearer", string.Empty).Trim();

            // Nếu tiếp tục null thì return false
            if (jwtToken is null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            // Check token hợp lệ hay không
            try
            {
                // Decode token thành object
                var jwtHelper = new JWTHelper();
                var userIdentity = jwtHelper.DecodeToken(jwtToken);

                // Add vào claims
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim("IdUser", userIdentity.IdUser.ToString()));
                identity.AddClaim(new Claim("IdRole", userIdentity.IdRole.ToString()));
                actionContext.ControllerContext.RequestContext.Principal = new ClaimsPrincipal(identity);

                // Kiểm tra role và action nếu cần
                if (_allowRoles != null)
                {
                    if (!_allowRoles.Any(x => (int)x == userIdentity.IdRole))
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        return;
                    }
                }

                if (_action != null)
                {
                    if (!(userIdentity.Permission is null ||
                          userIdentity.Permission.Any(x => x.Equals(_action, StringComparison.OrdinalIgnoreCase))
                         )
                       )
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        return;
                    }
                }
            }
            catch
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }
        }
    }
}