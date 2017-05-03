using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service;
using monkey.service.Db;
using System.ComponentModel.DataAnnotations;

namespace monkey.service.WorkFlow
{
    /// <summary>
    /// 工作流步骤类型
    /// </summary>
    public enum WorkFlowStepType {
        /// <summary>
        /// 开始节点
        /// </summary>
        begin = 1,
        /// <summary>
        /// 非会签
        /// </summary>
        sigle = 32,
        /// <summary>
        /// 会签节点
        /// </summary>
        mut = 64,
        /// <summary>
        /// 结束节点
        /// </summary>
        end = 128,
    }

    #region - 编辑请求

    /// <summary>
    /// 工作流定义编辑基础请求
    /// </summary>
    public class WorkFlowEditBaseRequest {
        /// <summary>
        /// 步骤名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Caption { get; set; }

        /// <summary>
        /// 步骤描述
        /// </summary>
        public string Descript { get; set; }
    }

    /// <summary>
    /// 工作流定义编辑请求
    /// </summary>
    public class WorkFlowDefEditRequest : WorkFlowEditBaseRequest {
        /// <summary>
        /// 从开始到结束所有的节点包含关系线的信息
        /// </summary>
        public WorkFlowStepEditRequest Setp { get; set; }
    }

    /// <summary>
    /// 工作流定义编辑请求（节点）
    /// </summary>
    public class WorkFlowStepEditRequest: WorkFlowEditBaseRequest
    {
        /// <summary>
        /// 步骤的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 步骤的类型
        /// </summary>
        public WorkFlowStepType Seq { get; set; }

        /// <summary>
        /// 审批人的工作流角色ID
        /// </summary>
        [Required]
        public string WFConfirmRoleId { get; set; }

        /// <summary>
        /// 下一步关系线（如果不是结束类型的节点 必填该信息）
        /// </summary>
        public List<WorkFlowLineEditRequest> Lines { get; set; }
    }

    /// <summary>
    /// 工作流定义编辑请求（关系）
    /// </summary>
    public class WorkFlowLineEditRequest : WorkFlowEditBaseRequest {
        /// <summary>
        /// 目标节点信息
        /// </summary>
        public WorkFlowStepEditRequest Step { get; set; }
    }

    #endregion

    /// <summary>
    /// 工作流程定义
    /// </summary>
    public class WorkFlowDefinition {

        #region -- 属性

        /// <summary>
        /// 工作流程定义的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工作流程定义的名称
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 工作流程定义的描述信息
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 创建时间 - 文本
        /// </summary>
        public string CreatedOnString { get; set; }

        /// <summary>
        /// 工作流程定义中所有步骤的集合
        /// </summary>
        public List<WorkFlowDefStep> Steps { get; set; }

        /// <summary>
        /// 工作流程定义中所有的关系线
        /// </summary>
        public List<WorkFlowDefLine> Lines { get; set; }

        #endregion

        /// <summary>
        /// 空构造
        /// </summary>
        public WorkFlowDefinition() { }

        /// <summary>
        /// 新增工作流程定义
        /// </summary>
        /// <param name="condtion"></param>
        public void Create(WorkFlowDefEditRequest condtion) {

        }
    }

    /// <summary>
    /// 工作流步骤定义
    /// </summary>
    public class WorkFlowDefStep
    {
        /// <summary>
        /// 工作流步骤节点的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工作流步骤节点类型
        /// </summary>
        public WorkFlowStepType Seq { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 步骤描述
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 审批人的工作流角色ID
        /// </summary>
        public string WFConfirmRoleId { get; set; }

        /// <summary>
        /// 关联的工作流定义ID
        /// </summary>
        public string WorkFlowDefinitionId { get; set; }
    }

    /// <summary>
    /// 工作流步骤关系线
    /// </summary>
    public class WorkFlowDefLine {

        /// <summary>
        /// 工作流步骤关系线的Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 步骤ID - 源节点
        /// </summary>
        public string FromId { get; set; }

        /// <summary>
        /// 步骤ID - 目标节点
        /// </summary>
        public string ToId { get; set; }

        /// <summary>
        /// 线条的名称
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 线条的描述
        /// </summary>
        public string Descript { get; set; }
    }
}
