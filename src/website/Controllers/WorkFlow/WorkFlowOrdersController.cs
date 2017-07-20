using monkey.service;
using monkey.service.Users;
using monkey.service.WorkFlow;
using monkey.service.Logs;
using monkey.service.Fun.OA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace website.Controllers.WorkFlow
{
    /// <summary>
    /// 工作流 - 工单相关
    /// </summary>
    public class WorkFlowOrdersController : ApiController
    {
        /// <summary>
        /// [后台角色权限]检索所有工单信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<BaseWorkOrderListDetail>> GetWorkFlowBaseOrdersList(BaseWorkOrderSearchRequest condtion)
        {
            return BaseResponse.getResult(BaseWorkOrderListDetail.SearchBaseWorkOrderList(condtion));
        }

        /// <summary>
        /// [登录权限]获取我的已审/待审工单信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public BaseResponse<BaseResponseList<BaseWorkOrderListDetail>> GetMyWorkFlowBaseOrdersList(BaseWorkOrderSearchRequest condtion)
        {
            condtion.TaskUserId = User.Identity.Name;
            return BaseResponse.getResult(BaseWorkOrderListDetail.SearchBaseWorkOrderList(condtion));
        }


        /// <summary>
        /// [后台角色权限]批量新增基础工单
        /// </summary>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse CreateWorkFlowBaseOrders(BaseBatchRequest<string> remarks) {
            var result = BaseWorkOrder.CreateBaseWorkOrders(remarks.rows);
            string thisUserId = User.Identity.Name;
            UserManager thisUser = UserManager.getUserById(thisUserId);
            //记录到日志
            if (result.Count > 0) {
                foreach (var item in result) {
                    UserLog.create("新增基础工单", "基础工单", thisUser, item);
                }
            }
            return BaseResponse.getResult(string.Format("已经成功新建{0}条工单信息", result.Count));
        }

        /// <summary>
        /// 根据工单的ID获取 其对应的具体的工单对象
        /// </summary>
        /// <param name="id">工单的ID</param>
        /// <returns></returns>
        private BaseWorkOrder GetWorkOrderById(string id) {
            BaseWorkOrder info = new BaseWorkOrder(id);
            if (info.OrderType == WorkOrderType.请假申请) {
                info = new LeaveInfo(id);
            }
            return info;
        }

        /// <summary>
        /// [后台角色权限]启动工作流
        /// </summary>
        /// <param name="id">被启动的工单的ID</param>
        /// <param name="defId">启动的业务流程定义的ID</param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse BeginWorkFlow(string id, string defId) {
            var info = GetWorkOrderById(id);
            info.WorkFlowBegin(defId, UserManager.getUserById(User.Identity.Name));
            return BaseResponse.getResult("提交成功");
        }

        /// <summary>
        /// [后台角色权限]用户审批工作流
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse WorkFlowUserConfim(BaseWorkOrderUserConfirmReqeust condtion) {
            var info = GetWorkOrderById(condtion.Id);
            info.DoWorkFlowUserConfirm(condtion, UserManager.getUserById(User.Identity.Name));
            return BaseResponse.getResult("审批成功");
        }

        /// <summary>
        /// [后台角色权限]用户终止工作流
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse WorkFlowUserTermination(BaseWorkOrderUserConfirmReqeust condtion)
        {
            var info = GetWorkOrderById(condtion.Id);
            info.WorkFlowTermination(UserManager.getUserById(User.Identity.Name), condtion);
            return BaseResponse.getResult("工作流已终止");
        }

        /// <summary>
        /// [管理员角色权限]管理员无条件终止工作流
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(Roles ="admin")]
        public BaseResponse WorkFlowAdminUserTermination(BaseWorkOrderUserConfirmReqeust condtion)
        {
            var info = new BaseWorkOrder(condtion.Id);
            info.WorkFlowTerminationForAdmin(UserManager.getUserById(User.Identity.Name), condtion);
            return BaseResponse.getResult("工作流已终止");
        }

        /// <summary>
        /// [管理员角色权限]删除各类工单 （物理删除，包含子类的数据）
        /// </summary>
        /// <param name="id">工单的ID</param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(Roles ="admin")]
        public BaseResponse WorkFlowDelOrder(string id) {
            var userInfo = UserManager.getUserById(User.Identity.Name);
            var info = new BaseWorkOrder(id);
            info.Del();
            UserLog.create(string.Format("删除工单，类型[{0}]",info.OrderTypeString), "基础工单", userInfo, info);
            return BaseResponse.getResult("删除成功");
        }
    }
}
