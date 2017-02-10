using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.service.WorkFlow
{
    /// <summary>
    /// 工作流步骤审批用户类型
    /// </summary>
    public enum WorkFlowConfirmUserType {
        /// <summary>
        /// 由指定的一个或者多个用户审批
        /// </summary>
        用户,
        /// <summary>
        /// 由某一个角色的用户来审批
        /// </summary>
        角色
    }

    /// <summary>
    /// 工作流步骤
    /// </summary>
    public class WorkFlowStep
    {
        /// <summary>
        /// 工作流步骤的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工作流的执行序号
        /// </summary>
        public int serialNumber { get; set; }

        /// <summary>
        /// 步骤的名称
        /// </summary>
        public string stepName { get; set; }

        /// <summary>
        /// 审批用户类型
        /// </summary>
        public WorkFlowConfirmUserType confirmUserType { get; set; }

        /// <summary>
        /// 用户的ID或者角色的名称
        /// </summary>
        public List<string> users { get; set; }
    }
}
