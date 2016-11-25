using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Users;
using System.Web;
using System.Web.Http.Controllers;

namespace monkey.service
{
    /// <summary>
    /// 扩展的系统角色验证【API】
    /// </summary>
    public class ApiAuthorize : System.Web.Http.AuthorizeAttribute
    {
        /// <summary>
        /// 角色类型，如果指定了特定的角色，该值无效
        /// </summary>
        public SysRolesType RoleType { get; set; }

        /// <summary>
        /// 自定义权限验证
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(this.Roles))
                {
                    List<SysRoles> roles = SysRoles.getRolesList(RoleType);
                    foreach (SysRoles role in roles)
                    {
                        if (HttpContext.Current.User.IsInRole(role.role))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else {
                    return base.IsAuthorized(actionContext);
                }
            }
            else {
                return false;
            }
        }
    }

    /// <summary>
    /// 扩展的系统角色验证【UI】
    /// </summary>
    public class SysAuthorize : System.Web.Mvc.AuthorizeAttribute
    {
        /// <summary>
        /// 角色类型，如果指定了特定的角色，该值无效
        /// </summary>
        public SysRolesType RoleType { get; set; }

        /// <summary>
        /// 自定义授权验证
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(this.Roles))
                {
                    List<SysRoles> roles = SysRoles.getRolesList(RoleType);
                    foreach (SysRoles role in roles)
                    {
                        if (httpContext.User.IsInRole(role.role))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else {
                    return base.AuthorizeCore(httpContext);
                }
            }
            else {
                return false;
            }

        }
    }
}
