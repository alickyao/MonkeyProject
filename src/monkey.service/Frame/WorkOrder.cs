using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.service.Frame
{
    /// <summary>
    /// 基础工单
    /// </summary>
    public class WorkOrder
    {
        /// <summary>
        /// 记录的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 使用工作流的ID
        /// </summary>
        public string WorkFlowDefinitionId { get; set; }

        /// <summary>
        /// 工作流书签ID
        /// </summary>
        public string WorkFlowBookMarkId { get; set; }

        /// <summary>
        /// 工单备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 工单创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
