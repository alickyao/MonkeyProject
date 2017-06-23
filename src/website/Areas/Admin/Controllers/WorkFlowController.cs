using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using monkey.service;
using monkey.service.Users;
using monkey.service.WorkFlow;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// 工作流程相关
    /// </summary>
    public class WorkFlowController : BaseController
    {
        /// <summary>
        /// 工作流程列表
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult DefinitionList(string pageId)
        {
            BaseRequest condtion = new BaseRequest();
            ViewBag.pageId = getPageId(pageId);
            return View(condtion);
        }

        /// <summary>
        /// 编辑工作流程图的定义
        /// </summary>
        /// <param name="id">工作流的ID</param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult EditDefinition(string id,string pageId) {
            ViewBag.Id = id;
            ViewBag.pageId = getPageId(pageId);
            return View();
        }

        /// <summary>
        /// 步骤详情（编辑 审批角色与方式 ）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult DefStepDetail(string id, string pageId) {
            var info = WorkFlowDefSetpDetail.GetDetailInstance(id);
            ViewBag.pageId = getPageId(pageId);
            return View(info);
        }

        /// <summary>
        /// 工作流审批角色列表
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult WorkFlowRoleList(string pageId) {
            ViewBag.pageId = getPageId(pageId);
            BaseRequest condtion = new BaseRequest();
            return View(condtion);
        }
    }
}