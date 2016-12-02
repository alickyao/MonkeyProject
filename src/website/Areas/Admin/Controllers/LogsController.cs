using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using monkey.service;
using monkey.service.Logs;
using monkey.service.Users;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// 日志列表
    /// </summary>
    public class LogsController : BaseController
    {

        /// <summary>
        /// 所有日志列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult baseLogList(BaseLogSearchReqeust condtion, string pageId = null)
        {
            ViewBag.pageId = getPageId(pageId);
            return View(condtion);
        }

        /// <summary>
        /// 用户日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult userLogList(UserLogSearchRequest condtion, string pageId = null) {
            ViewBag.pageId = getPageId(pageId);
            return View(condtion);
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult exceptionLogList(ExceptionLogSearchRequest condtion, string pageId = null) {
            ViewBag.pageId = getPageId(pageId);
            return View(condtion);
        }
    }
}