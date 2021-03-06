﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using monkey.service;
using monkey.service.Users;
using monkey.service.WorkFlow;
using monkey.service.Fun.OA;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// 工作流程相关
    /// </summary>
    public class WorkFlowController : BaseController
    {
        /// <summary>
        /// 工作流程列表 （列表与编辑）
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
        /// 工作流程列表 （网格列表）
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult DefinitionSimpList(string pageId)
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

        /// <summary>
        /// 工作流审批角色用户设置
        /// </summary>
        /// <param name="Id">工作流角色的ID</param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult WorkFlowUserRoleSet(string Id, string pageId)
        {
            ViewBag.pageId = getPageId(pageId);
            var roleInfo = WorkFlowRole.GetInstance(Id);
            roleInfo.GetDescripUserId();
            ViewBag.selectedUserId = roleInfo.DescripUserId;
            return View("SelectManagerUsers");
        }

        /// <summary>
        /// 所有工单列表
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(Roles = "admin")]
        public ActionResult WorkFlowBaseOrders(string pageId) {
            ViewBag.pageId = getPageId(pageId);
            BaseWorkOrderSearchRequest condtion = new BaseWorkOrderSearchRequest();
            return View(condtion);
        }

        /// <summary>
        /// 获取当前登录用户[待审核]的网格列表
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult WorkFlowMyOrders(string pageId) {
            ViewBag.pageId = getPageId(pageId);
            BaseWorkOrderSearchRequest condtion = new BaseWorkOrderSearchRequest()
            {
                TaskUserId = User.Identity.Name,
                TaskUserConfirmType = WorkOrderUserConfirmType.待审
            };
            return View(condtion);
        }

        /// <summary>
        /// 获取当前登录用户[已审核]的工单列表
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult WorkFlowMyApprovalHistoryOrders(string pageId)
        {
            ViewBag.pageId = getPageId(pageId);
            BaseWorkOrderSearchRequest condtion = new BaseWorkOrderSearchRequest()
            {
                TaskUserId = User.Identity.Name,
                TaskUserConfirmType = WorkOrderUserConfirmType.已审
            };
            return View("WorkFlowMyOrders", condtion);
        }

        /// <summary>
        /// 工单详情
        /// </summary>
        /// <param name="id">工单的ID</param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult WorkFlowOrderInfo(string id, string pageId) {
            ViewBag.pageId = getPageId(pageId);
            var info = new BaseWorkOrder(id);
            if (info.OrderType == WorkOrderType.请假申请) {
                info = new LeaveInfo(id);
            }
            return View(info);
        }

        /// <summary>
        /// 工单历史流程图 - 展示流程历史记录图形
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult WorkFlowOrderWorkFlowDefInfo(string id, string pageId) {
            ViewBag.pageId = getPageId(pageId);
            var info = new BaseWorkOrder(id);
            return View(info);
        }

        /// <summary>
        /// 工单审批 - 默认页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult WorkFlowOrderDefaultApproval(string id, string pageId) {
            ViewBag.pageId = getPageId(pageId);
            ViewBag.id = id;
            return View();
        }

        /// <summary>
        /// 工单详情展示 - 默认工单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult WorkFlowOrderDefaultDetail(string id, string pageId)
        {
            ViewBag.pageId = getPageId(pageId);
            var info = new BaseWorkOrder(id);
            return View(info);
        }
    }
}