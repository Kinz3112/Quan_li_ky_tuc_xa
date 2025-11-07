using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Quan_li_ky_tuc_xa.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeByRoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;

        /// <summary>
        /// Sử dụng: [AuthorizeByRole("Manager")] hoặc [AuthorizeByRole("Student")]
        /// </summary>
        public AuthorizeByRoleAttribute(params string[] roles)
        {
            _roles = roles ?? new string[0];
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var http = context.HttpContext;
            var user = http.User;

            bool authorized = false;

            // 1) Nếu đã auth bằng cookie/claims -> kiểm tra role trong claims
            if (user?.Identity?.IsAuthenticated == true && _roles.Length > 0)
            {
                foreach (var r in _roles)
                {
                    if (user.IsInRole(r))
                    {
                        authorized = true;
                        break;
                    }
                }
            }

            // 2) Nếu chưa authorized bởi claims -> kiểm tra session (fallback)
            if (!authorized)
            {
                var sessionRole = http.Session.GetString("Role");
                if (!string.IsNullOrEmpty(sessionRole) && _roles.Length > 0)
                {
                    foreach (var r in _roles)
                    {
                        if (string.Equals(sessionRole, r, StringComparison.OrdinalIgnoreCase))
                        {
                            authorized = true;
                            break;
                        }
                    }
                }
            }

            // 3) Nếu không authorized:
            if (!authorized)
            {
                // Nếu chưa đăng nhập (cả cookie và session đều không có) => redirect tới login (kèm returnUrl)
                bool isAuthenticated = (user?.Identity?.IsAuthenticated == true)
                                       || !string.IsNullOrEmpty(http.Session.GetString("TenDangNhap"));

                if (!isAuthenticated)
                {
                    var returnUrl = http.Request.Path + http.Request.QueryString;
                    context.Result = new RedirectToActionResult("Login", "Acc", new { returnUrl });
                    return;
                }

                // Nếu đã đăng nhập nhưng không có quyền => redirect tới trang AccessDenied
                context.Result = new RedirectToActionResult("AccessDenied", "Acc", null);
            }

            await Task.CompletedTask;
        }
    }
}
