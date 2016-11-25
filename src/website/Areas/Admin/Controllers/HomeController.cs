using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using monkey.service;
using monkey.service.Users;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// 管理后台
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 后台主界面
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult Index()
        {
            Response.Cache.SetNoStore();
            UserManager user = UserManager.getUserById(User.Identity.Name);
            return View(user);
        }
    }
}