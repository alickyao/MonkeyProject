using monkey.service;
using monkey.service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// 基础项维护
    /// </summary>
    public class BaseManagementController : BaseController
    {
        // GET: Admin/BaseMangement

        /// <summary>
        /// 基本树网格
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult BaseTreeGrid(string pageId = null)
        {
            ViewBag.pageId = getPageId(pageId);
            return View();
        }
    }
}