using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using monkey.service;
using monkey.service.WorkFlow;
using monkey.service.Users;

namespace website.Controllers.WorkFlow
{
    /// <summary>
    /// 工作流 - 流程定义相关
    /// </summary>
    public class WorkFlowDefinitionController : ApiController
    {
        /// <summary>
        /// [后台角色权限]获取工作流程定义列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<WorkFlowDefinition>> GetWorkFlowDefinitionList(BaseRequest condtion) {
            return BaseResponse.getResult(WorkFlowDefinition.SearchList(condtion));
        }

        /// <summary>
        /// [匿名访问]获取工作流程详情
        /// </summary>
        /// <param name="id">工作流定义的ID</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<WorkFlowDefinition> GetWorkFlowDefinitionById(string id) {
            var info = WorkFlowDefinition.GetInstance(id);
            info.SetUnits();
            return BaseResponse.getResult(info);
        }

        /// <summary>
        /// [后台角色权限]批量新增/编辑业务流程定义
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse EditWorkFlowDefinition(BaseBatchRequest<WorkFlowDefinition> condtion) {

            int total = WorkFlowDefinition.EditDefs(condtion.rows);
            string msg = string.Format("已新增/编辑{0}条数据", total);
            return BaseResponse.getResult(msg);
        }

        /// <summary>
        /// [后台角色权限]编辑工作流程图定义
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<WorkFlowDefinition> EditWorkFlowDefUnits(WorkFlowDefEditRequest condtion) {
            var info = WorkFlowDefinition.GetInstance(condtion.Id);
            var result = info.EditDefUnit(condtion);
            return BaseResponse.getResult(result, "保存成功");
        }
    }
}
