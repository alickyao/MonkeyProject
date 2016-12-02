using monkey.service;
using monkey.service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// 登录与注销
    /// </summary>
    public class LoginController : BaseController
    {
        /// <summary>
        /// 用户登录界面
        /// </summary>
        /// <returns></returns>
        public ActionResult login(string ReturnUrl)
        {
            return View();
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <returns></returns>
        public ActionResult logout() {
            FormsAuthentication.SignOut();
            return RedirectToAction("login");
        }

        /// <summary>
        /// 修改密码界面
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult changePwd() {
            return View();
        }
    }
}