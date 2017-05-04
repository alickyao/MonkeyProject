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
        start,
        /// <summary>
        /// 结束
        /// </summary>
        end,
        /// <summary>
        /// 任务结点
        /// </summary>
        task,
        /// <summary>
        /// 自动结点
        /// </summary>
        node,
        /// <summary>
        /// 决策结点
        /// </summary>
        chat,
        /// <summary>
        /// 状态结点
        /// </summary>
        state,
        /// <summary>
        /// 附加插件
        /// </summary>
        plug,
        /// <summary>
        /// 联合结点
        /// </summary>
        join,
        /// <summary>
        /// 分支结点
        /// </summary>
        fork,
        /// <summary>
        /// 复合结点
        /// </summary>
        complex
    }

    /// <summary>
    /// 工作流线段类型
    /// </summary>
    public enum WorkFlowLineType
    {
        /// <summary>
        /// 直线
        /// </summary>
        sl,
        /// <summary>
        /// 中段可上下移动的折线
        /// </summary>
        tb,
        /// <summary>
        /// 中段可左右移动的折线
        /// </summary>
        lr
    }

    #region - 编辑请求

    /// <summary>
    /// 工作流程定义编辑请求
    /// </summary>
    public class WorkFlowDefEditRequest {

        /// <summary>
        /// 工作流程定义的ID
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// 节点的集合
        /// </summary>
        public List<WorkFlowDefStep> nodes { get; set; }

        /// <summary>
        /// 线条的集合
        /// </summary>
        public List<WorkFlowDefLine> lines { get; set; }

        /// <summary>
        /// 区域的集合
        /// </summary>
        public List<WorkFlowDefArea> areas { get; set; }
    }

    #endregion

    /// <summary>
    /// 业务流程图（数组）
    /// </summary>
    public class WorkFlowArrayUnits {
        /// <summary>
        /// 工作流程定义中所有步骤的集合
        /// </summary>
        public List<WorkFlowDefStep> Steps { get; set; }

        /// <summary>
        /// 工作流程定义中所有的关系线
        /// </summary>
        public List<WorkFlowDefLine> Lines { get; set; }

        /// <summary>
        /// 工作流程定义中所有的区域集合
        /// </summary>
        public List<WorkFlowDefArea> Areas { get; set; }
    }
    /// <summary>
    /// 业务流程图（字典）
    /// </summary>
    public class WorkFlowDictionaryUnits
    {
        /// <summary>
        /// 工作流程定义中所有步骤的集合
        /// </summary>
        public Dictionary<string,WorkFlowDefStep> Steps { get; set; }

        /// <summary>
        /// 工作流程定义中所有的关系线
        /// </summary>
        public Dictionary<string,WorkFlowDefLine> Lines { get; set; }

        /// <summary>
        /// 工作流程定义中所有的区域集合
        /// </summary>
        public Dictionary<string,WorkFlowDefArea> Areas { get; set; }
    }

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
        [Required]
        [StringLength(100)]
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
        /// 流程图组件 - 数组模式
        /// </summary>
        public WorkFlowArrayUnits ArrayUnits { get; set; }

        /// <summary>
        /// 流程图组件 - 字典模式
        /// </summary>
        public WorkFlowDictionaryUnits DictionaryUnits { get; set; }

        #endregion

        /// <summary>
        /// 空构造
        /// </summary>
        public WorkFlowDefinition() { }

        /// <summary>
        /// 数据库对象构造方法
        /// </summary>
        /// <param name="row"></param>
        public WorkFlowDefinition(Db_WorkFlowDefinition row) {
            this.Id = row.Id;
            this.Caption = row.Caption;
            this.Descript = row.Descript;
            this.CreatedOn = row.CreatedOn;
            this.CreatedOnString = this.CreatedOn.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 通过Id获取流程定义详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WorkFlowDefinition GetInstance(string id) {
            using (var db = new DefaultContainer()) {
                var row = db.Db_WorkFlowDefinitionSet.SingleOrDefault(p => p.Id == id);
                if (row == null)
                {
                    throw new DataNotFundException(string.Format("传入的工作流ID：[{0}]，不正确", id));
                }
                return new WorkFlowDefinition(row);
            }
        }

        /// <summary>
        /// 获取工作流程定义列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<WorkFlowDefinition> SearchList(BaseRequest condtion) {
            BaseResponseList<WorkFlowDefinition> result = new BaseResponseList<WorkFlowDefinition>();

            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_WorkFlowDefinitionSet.AsEnumerable() select c);
                result.total = rows.Count();
                if (condtion.getRows && result.total > 0) {
                    if (condtion.page > 0) {
                        rows = rows.OrderByDescending(p => p.CreatedOn).Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.Select(p => new WorkFlowDefinition(p)).ToList();
                }
            }

            return result;
        }


        /// <summary>
        /// 批量新增/编辑业务流程定义
        /// </summary>
        /// <param name="condtion"></param>
        public static int EditDefs(List<WorkFlowDefinition> condtion) {
            int total = 0;

            if (condtion != null) {
                if (condtion.Count > 0) {


                    foreach (var item in condtion)
                    {
                        ValiDatas.valiData(condtion);
                    }



                    using (var db = new DefaultContainer())
                    {
                        foreach (var item in condtion)
                        {

                            if (string.IsNullOrEmpty(item.Id))
                            {
                                //新增
                                Db_WorkFlowDefinition dbDef = new Db_WorkFlowDefinition() {
                                    Id = SysHelps.GetNewId(),
                                    Caption = item.Caption,
                                    Descript = string.IsNullOrEmpty(item.Descript)? null :item.Descript,
                                    CreatedOn = DateTime.Now
                                };
                                db.Db_WorkFlowDefinitionSet.Add(dbDef);
                            }
                            else {
                                //编辑
                                var dbDef = db.Db_WorkFlowDefinitionSet.Single(p => p.Id == item.Id);
                                dbDef.Caption = item.Caption;
                                dbDef.Descript = item.Descript;
                            }

                            total++;
                        }
                        db.SaveChanges();
                    }

                }
            }

            
            
            return total;
        }

        /// <summary>
        /// 为流程图组件赋值
        /// </summary>
        public void SetUnits() {
            this.ArrayUnits = new WorkFlowArrayUnits();
            this.DictionaryUnits = new WorkFlowDictionaryUnits();
            using (var db = new DefaultContainer()) {
                var row = db.Db_WorkFlowDefinitionSet.Single(p => p.Id == this.Id);
                //数组部分
                //步骤
                this.ArrayUnits.Steps = row.Db_WorkFlowDefStep.Select(p => new WorkFlowDefStep(p)).ToList();
                //线条
                this.ArrayUnits.Lines = row.Db_WorkFlowDefLine.Select(p => new WorkFlowDefLine(p)).ToList();
                //区域
                this.ArrayUnits.Areas = row.Db_WorkFlowDefArea.Select(p => new WorkFlowDefArea(p)).ToList();
            }
            //字典部分
            this.DictionaryUnits.Areas = new Dictionary<string, WorkFlowDefArea>();
            this.DictionaryUnits.Lines = new Dictionary<string, WorkFlowDefLine>();
            this.DictionaryUnits.Steps = new Dictionary<string, WorkFlowDefStep>();

            foreach (var item in this.ArrayUnits.Steps) {
                this.DictionaryUnits.Steps.Add(item.Id, item);
            }
            foreach (var item in this.ArrayUnits.Lines) {
                this.DictionaryUnits.Lines.Add(item.Id, item);
            }
            foreach (var item in this.ArrayUnits.Areas)
            {
                this.DictionaryUnits.Areas.Add(item.Id, item);
            }
        }

        /// <summary>
        /// 编辑流程图结构
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public WorkFlowDefinition EditDefUnit(WorkFlowDefEditRequest condtion) {

            using (var db = new DefaultContainer()) {
                //三从表 创建新增/编辑已有/删除多余
                foreach (var item in condtion.nodes) {
                    var row = db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefStep>().SingleOrDefault(p => p.Id == item.Id);
                    if (row == null)
                    {
                        var newRow = new Db_WorkFlowDefStep()
                        {
                            Id = SysHelps.GetNewId(),
                            Db_WorkFlowDefinitionId = this.Id,
                            Name = item.name,
                            Type = (int)item.type,
                            Height = item.height,
                            Width = item.width,
                            Left = item.left,
                            Top = item.top
                        };
                        db.Db_WorkFlowDefBaseUnitSet.Add(newRow);
                    }
                    else {
                        row.Name = item.name;
                        row.Type = (int)item.type;
                        row.Height = item.height;
                        row.Width = item.width;
                        row.Left = item.left;
                        row.Top = item.top;
                    }
                }
                var delSetpRows = (from c in db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefStep>().AsEnumerable() where !condtion.nodes.Select(p => p.Id).Contains(c.Id) select c);
                if (delSetpRows.Count() > 0) {
                    db.Db_WorkFlowDefBaseUnitSet.RemoveRange(delSetpRows);
                }
                db.SaveChanges();
            }

            var info = WorkFlowDefinition.GetInstance(this.Id);
            info.SetUnits();
            return info;
        }
        
    }

    /// <summary>
    /// 业务流程定义基础组件对象
    /// </summary>
    public abstract class WorkFlowDefBaseUnit {

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        #region -- 图形位置与大小

        /// <summary>
        /// 高度
        /// </summary>
        public int height { get; set; }

        /// <summary>
        /// 左位移
        /// </summary>
        public int left { get; set; }

        /// <summary>
        /// 顶位移
        /// </summary>
        public int top { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int width { get; set; }

        #endregion

        /// <summary>
        /// 空构造
        /// </summary>
        public WorkFlowDefBaseUnit() { }

        /// <summary>
        /// 从数据库构造
        /// </summary>
        /// <param name="row"></param>
        public WorkFlowDefBaseUnit(Db_WorkFlowDefBaseUnit row) {
            this.name = row.Name;
            this.width = row.Width;
            this.height = row.Height;
            this.left = row.Left;
            this.top = row.Top;
        }
    }

    /// <summary>
    /// 业务流程定义区域信息
    /// </summary>
    public class WorkFlowDefArea: WorkFlowDefBaseUnit
    {
        /// <summary>
        /// 业务流程定义区域信息ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 区域的颜色
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// 空构造
        /// </summary>
        public WorkFlowDefArea() { }

        /// <summary>
        /// 从数据库进行构造
        /// </summary>
        /// <param name="row"></param>
        public WorkFlowDefArea(Db_WorkFlowDefArea row) : base(row) {
            this.Id = row.Id;
            this.color = row.Color;
        }
    }

    /// <summary>
    /// 工作流步骤定义
    /// </summary>
    public class WorkFlowDefStep: WorkFlowDefBaseUnit
    {
        /// <summary>
        /// 工作流步骤节点的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工作流步骤节点类型
        /// </summary>
        public WorkFlowStepType type { get; set; }

        /// <summary>
        /// 工作流步骤节点类型 - 文本
        /// </summary>
        public string typeString { get; set; }

        /// <summary>
        /// 空构造
        /// </summary>
        public WorkFlowDefStep() { }

        /// <summary>
        /// 从数据库构造
        /// </summary>
        /// <param name="row"></param>
        public WorkFlowDefStep(Db_WorkFlowDefStep row) : base(row) {
            this.Id = row.Id;
            this.type = (WorkFlowStepType)row.Type;
            this.typeString = this.type.ToString();
        }
    }

    /// <summary>
    /// 工作流步骤关系线
    /// </summary>
    public class WorkFlowDefLine {


        #region

        /// <summary>
        /// 工作流步骤关系线的Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 连线的开始节点ID
        /// </summary>
        public string from { get; set; }

        /// <summary>
        /// 连线的结束节点ID
        /// </summary>
        public string to { get; set; }

        /// <summary>
        /// 当type=”lr”时,为中段的相对于工作区的X坐标值,当type=”tb”时,为中段的相对于工作区的Y坐标值.
        /// </summary>
        public int? M { get; set; }

        /// <summary>
        /// 布尔值,表示是否已被用橙色标注
        /// </summary>
        public bool marked { get; set; }

        /// <summary>
        /// 连线的显示名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 连线类型,取值有”sl”直线,”lr”中段可左右移动的折线,”tb”中段可上下移动的折线
        /// </summary>
        public WorkFlowLineType type { get; set; }

        /// <summary>
        /// 连线类型 - String
        /// </summary>
        public string typeString { get; set; }

        #endregion

        /// <summary>
        /// 空构造
        /// </summary>
        public WorkFlowDefLine() { }

        /// <summary>
        /// 从数据库构造
        /// </summary>
        /// <param name="row"></param>
        public WorkFlowDefLine(Db_WorkFlowDefLine row) {
            this.Id = row.Id;
            this.from = row.From;
            this.to = row.To;
            this.M = row.M;
            this.name = row.Name;
            this.type = (WorkFlowLineType)row.Type;
            this.typeString = this.type.ToString();
        }
    }
}
