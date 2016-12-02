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
    /// 后台基础控制器
    /// </summary>
    public class BaseController : Controller {
        /// <summary>
        /// 获取一个页面的ID
        /// </summary>
        /// <param name="pageId">可为空为空则返回一个随机的页面ID</param>
        /// <returns></returns>
        protected string getPageId(string pageId = null) {
            if (string.IsNullOrEmpty(pageId))
            {
                return string.Format("{0}_{1}", SysHelps.getRandmonStirng(4), DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            else {
                return pageId;
            }
        }
    }

    /// <summary>
    /// 管理后台
    /// </summary>
    public class HomeController : BaseController
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
            user.getUserMenu();
            return View(user);
        }

        /// <summary>
        /// 工作台
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult workTab() {
            return View();
        }
    }
}