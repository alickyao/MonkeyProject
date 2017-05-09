using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using monkey.service;
using monkey.service.WorkFlow;
using monkey.service.Users;
using monkey.service.Logs;

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
        public BaseResponse<List<WorkFlowDefinition>> EditWorkFlowDefinition(BaseBatchRequest<WorkFlowDefinition> condtion) {

            var result = WorkFlowDefinition.EditDefs(condtion.rows);
            string msg = string.Format("已新增/编辑{0}条数据", result.Count);

            //记录到日志
            string thisUserId = User.Identity.Name;
            UserManager thisUser = UserManager.getUserById(thisUserId);
            string logMsg = string.Empty;
            foreach (var item in result) {
                if (condtion.rows.Select(p=>p.Id).Contains(item.Id))
                {
                    logMsg = "编辑工作流定义名称/描述";
                }
                else {
                    logMsg = "新增工作流定义";
                }
                UserLog.create(logMsg, "工作流定义", thisUser, item);
            }
            

            return BaseResponse.getResult(result, msg);
        }

        /// <summary>
        /// [后台角色权限]删除工作流定义（物理删除）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse DelWorkFlowDefinition(string id) {
            var info = WorkFlowDefinition.GetInstance(id);
            info.Delete();

            //记录到日志
            string thisUserId = User.Identity.Name;
            UserManager thisUser = UserManager.getUserById(thisUserId);
            UserLog.create("删除工作流定义（物理删除）", "工作流定义", thisUser, info);

            return BaseResponse.getResult("删除成功");
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

            //记录到日志
            string thisUserId = User.Identity.Name;
            UserManager thisUser = UserManager.getUserById(thisUserId);
            UserLog.create("编辑工作流定义流程详情", "工作流定义", thisUser, info);

            return BaseResponse.getResult(result, "保存成功");
        }

        /// <summary>
        /// [后台角色权限]获取步骤的后续关系线列表信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<List<WorkFlowDefLineDetail>> GetWorkFlowSetpNextLines(string id) {
            var info = WorkFlowDefStep.GetInstance(id);
            var result = info.GetNextLineDetails();
            return BaseResponse.getResult(result);
        }

        /// <summary>
        /// [后台角色权限]编辑步骤审批信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<WorkFlowDefSetpDetail> EditStepApprovalInfo(WorkFlowDefStep condtion) {
            
            var info = WorkFlowDefSetpDetail.GetDetailInstance(condtion.Id);
            var def = WorkFlowDefinition.GetInstance(info.DefinitionId);
            var result = info.EditStepApprovalInfo(condtion);

            //保存到日志
            string thisUserId = User.Identity.Name;
            UserManager thisUser = UserManager.getUserById(thisUserId);
            UserLog.create(string.Format("编辑工作流步骤[{0}]的审批方式与角色配置信息", info.name), "工作流定义", thisUser, def);

            return BaseResponse.getResult(result, "保存成功");
        }
    }
}
