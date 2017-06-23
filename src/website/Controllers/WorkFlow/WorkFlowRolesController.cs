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
    /// 工作流 - 用户角色相关
    /// </summary>
    public class WorkFlowRolesController : ApiController
    {
        /// <summary>
        /// [后台角色权限]获取工作流角色列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<WorkFlowRole>> GetWorkFlowRoleList(BaseRequest condtion) {
            return BaseResponse.getResult(WorkFlowRole.GetRolsList(condtion));
        }

        /// <summary>
        /// [后台角色权限]根据工作流角色ID获取角色详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<WorkFlowRole> GetWorkFlowRoleInfoById(string id) {
            var info = WorkFlowRole.GetInstance(id);
            info.GetDescripUserId();
            return BaseResponse.getResult(info);
        }

        /// <summary>
        /// [后台角色权限]创建或者编辑工作流角色对象
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<List<WorkFlowRole>> EditWorkFlowRoleInfo(BaseBatchRequest<WorkFlowRole> condtion) {
            var result = WorkFlowRole.Edit(condtion.rows);
            string msg = string.Format("已新增/编辑{0}条数据", result.Count);

            //记录到日志
            string thisUserId = User.Identity.Name;
            UserManager thisUser = UserManager.getUserById(thisUserId);
            string logMsg = string.Empty;
            foreach (var item in result)
            {
                if (condtion.rows.Select(p => p.Id).Contains(item.Id))
                {
                    logMsg = string.Format("编辑工作流角色信息[{0}]", item.RoleName);
                }
                else {
                    logMsg = string.Format("新增工作流角色信息[{0}]", item.RoleName);
                }
                UserLog.create(logMsg, "工作流角色", thisUser, item);
            }
            return BaseResponse.getResult(result, msg);
        }

        /// <summary>
        /// [管理员角色权限]删除工作流角色对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public BaseResponse DelWorkFlowRoleInfo(string id) {
            var info = WorkFlowRole.GetInstance(id);
            info.Delete();
            return BaseResponse.getResult("删除成功");
        }


        /// <summary>
        /// [后台角色权限]添加工作流角色下的用户
        /// </summary>
        /// <param name="id">工作流角色的ID</param>
        /// <param name="condtion">用户的ID集合</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse InsterWorkFlowRoleDescriptUser(string id, BaseBatchRequest<string> condtion)
        {
            var info = WorkFlowRole.GetInstance(id);
            info.InsterDescriptUserId(condtion.rows);
            return BaseResponse.getResult("保存成功");
        }

        /// <summary>
        /// [后台角色权限]移除工作流角色下的用户
        /// </summary>
        /// <param name="id">工作流角色的ID</param>
        /// <param name="condtion">用户的ID集合</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse DelWorkFlowRoleDescriptUser(string id, BaseBatchRequest<string> condtion)
        {
            var info = WorkFlowRole.GetInstance(id);
            var total = info.RemoveDescriptUserId(condtion.rows);
            string msg = string.Format("已成功移除{0}个用户", total);
            return BaseResponse.getResult(msg);
        }
    }
}
