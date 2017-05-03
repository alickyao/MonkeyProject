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
        /// 新增/编辑工作流程定义
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="id">编辑时传入被编辑的流程定义的ID</param>
        /// <returns></returns>
        public BaseResponse EditWorkFlowDefinition(WorkFlowDefEditRequest condtion, string id = null) {
            if (string.IsNullOrEmpty(id))
            {
                (new WorkFlowDefinition()).Create(condtion);
            }
            else {

            }
            return BaseResponse.getResult();
        }
    }
}
