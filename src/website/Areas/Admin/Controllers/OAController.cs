using monkey.service;
using monkey.service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using monkey.service.Fun.OA;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// OA办公管理
    /// </summary>
    public class OAController : BaseController
    {

        /// <summary>
        /// 我的请假申请列表
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult MyLeavesList(string pageId, BaseRequest condtion)
        {
            if (condtion == null) {
                condtion = new BaseRequest();
            }
            ViewBag.pageId = getPageId(pageId);
            return View(condtion);
        }

        /// <summary>
        /// 请假申请详情信息界面
        /// </summary>
        /// <param name="id">请假申请的ID</param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult LeaveDefaultDetail(string id, string pageId) {
            LeaveInfo info = new LeaveInfo(id);
            ViewBag.pageId = getPageId(pageId);
            return View(info);
        }

        /// <summary>
        /// 请假申请 新增/编辑界面
        /// </summary>
        /// <param name="id">请假申请的ID 新增传空字符串</param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult LeaveEdit(string id, string pageId) {
            ViewBag.pageId = getPageId(pageId);
            LeaveInfo info = new LeaveInfo();
            if (!string.IsNullOrEmpty(id)) {
                info = new LeaveInfo(id);
            }
            return View(info);
        }
    }
}