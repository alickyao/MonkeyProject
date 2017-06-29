using monkey.service;
using monkey.service.Users;
using monkey.service.WorkFlow;
using monkey.service.Logs;
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
        /// [后台角色权限]获取所有工单信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<BaseWorkOrder>> GetWorkFlowBaseOrdersList(BaseWorkOrderSearchRequest condtion)
        {
            return BaseResponse.getResult(BaseWorkOrder.SearchBaseWorkOrderList(condtion));
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
        /// 后台角色权限]启动工作流
        /// </summary>
        /// <param name="id">被启动的工单的ID</param>
        /// <param name="defId">启动的业务流程定义的ID</param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse BeginWorkFlow(string id, string defId) {
            var info = new BaseWorkOrder(id);
            info.WorkFlowBegin(defId, User.Identity.Name);
            return BaseResponse.getResult("提交成功");
        }
    }
}
