using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service;
using monkey.service.Db;
using monkey.service.Logs;
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

    #region - 数据整理

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

    /// <summary>
    /// 业务流程图（数组）
    /// </summary>
    public class WorkFlowArrayUnits {
        /// <summary>
        /// 工作流程定义中所有步骤的集合
        /// </summary>
        public List<WorkFlowDefSetpDetail> Steps { get; set; }

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
        public Dictionary<string, WorkFlowDefSetpDetail> nodes { get; set; }

        /// <summary>
        /// 工作流程定义中所有的关系线
        /// </summary>
        public Dictionary<string,WorkFlowDefLine> lines { get; set; }

        /// <summary>
        /// 工作流程定义中所有的区域集合
        /// </summary>
        public Dictionary<string,WorkFlowDefArea> areas { get; set; }
    }

    #endregion

    /// <summary>
    /// 工作流程定义
    /// </summary>
    public class WorkFlowDefinition:ILogStringable {

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
        public static List<WorkFlowDefinition> EditDefs(List<WorkFlowDefinition> condtion) {
            List<WorkFlowDefinition> result = new List<WorkFlowDefinition>();

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
                                result.Add(new WorkFlowDefinition(dbDef));
                            }
                            else {
                                //编辑
                                var dbDef = db.Db_WorkFlowDefinitionSet.Single(p => p.Id == item.Id);
                                dbDef.Caption = item.Caption;
                                dbDef.Descript = item.Descript;
                                result.Add(new WorkFlowDefinition(dbDef));
                            }
                        }
                        db.SaveChanges();
                    }

                }
            }

            
            
            return result;
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
                //this.ArrayUnits.Steps = row.Db_WorkFlowDefStep.OrderBy(p=>p.Type).Select(p => new WorkFlowDefStep(p)).ToList();

                this.ArrayUnits.Steps = (from c in row.Db_WorkFlowDefStep
                                         join r in db.Db_WorkFlowRoleSet on c.WorkFlowRoleId equals r.Id into temp
                                         from role in temp.DefaultIfEmpty()
                                         select new WorkFlowDefSetpDetail(c, role)
                                         ).ToList();

                //线条
                this.ArrayUnits.Lines = row.Db_WorkFlowDefLine.Select(p => new WorkFlowDefLine(p)).ToList();
                //区域
                this.ArrayUnits.Areas = row.Db_WorkFlowDefArea.Select(p => new WorkFlowDefArea(p)).ToList();
            }
            //字典部分
            this.DictionaryUnits.areas = new Dictionary<string, WorkFlowDefArea>();
            this.DictionaryUnits.lines = new Dictionary<string, WorkFlowDefLine>();
            this.DictionaryUnits.nodes = new Dictionary<string, WorkFlowDefSetpDetail>();

            foreach (var item in this.ArrayUnits.Steps) {
                this.DictionaryUnits.nodes.Add(item.Id, item);
            }
            foreach (var item in this.ArrayUnits.Lines) {
                this.DictionaryUnits.lines.Add(item.Id, item);
            }
            foreach (var item in this.ArrayUnits.Areas)
            {
                this.DictionaryUnits.areas.Add(item.Id, item);
            }
        }

        /// <summary>
        /// 编辑流程图结构
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public WorkFlowDefinition EditDefUnit(WorkFlowDefEditRequest condtion) {

            if (condtion.lines == null) {
                condtion.lines = new List<WorkFlowDefLine>();
            }
            if (condtion.areas == null) {
                condtion.areas = new List<WorkFlowDefArea>();
            }
            if (condtion.nodes == null) {
                condtion.nodes = new List<WorkFlowDefStep>();
            }

            using (var db = new DefaultContainer()) {
                //三从表 创建新增/编辑已有/删除多余

                //节点
                Dictionary<string,string> nodesComb = new Dictionary<string, string>();//ID转换
                #region -- 节点
                foreach (var item in condtion.nodes) {
                    var row = db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefStep>().SingleOrDefault(p => p.Id == item.Id);
                    if (row == null)
                    {
                        var newId = SysHelps.GetNewId();
                        nodesComb.Add(item.Id, newId);
                        var newRow = new Db_WorkFlowDefStep()
                        {
                            Id = newId,
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
                        nodesComb.Add(item.Id, item.Id);
                        row.Name = item.name;
                        row.Type = (int)item.type;
                        row.Height = item.height;
                        row.Width = item.width;
                        row.Left = item.left;
                        row.Top = item.top;
                    }
                }
                var delSetpRows = (from c in db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefStep>().AsEnumerable() where !condtion.nodes.Select(p => p.Id).Contains(c.Id) && c.Db_WorkFlowDefinitionId==this.Id select c);
                if (delSetpRows.Count() > 0) {
                    db.Db_WorkFlowDefBaseUnitSet.RemoveRange(delSetpRows);
                }
                #endregion

                //连线

                #region -- 连线
                foreach (var item in condtion.lines) {
                    var row = db.Db_WorkFlowDefLineSet.SingleOrDefault(p => p.Id == item.Id);
                    if (row == null)
                    {
                        var newRow = new Db_WorkFlowDefLine()
                        {
                            Id = SysHelps.GetNewId(),
                            From = nodesComb[item.from],
                            To = nodesComb[item.to],
                            M = item.M,
                            Name = item.name,
                            Type = item.type,
                            Db_WorkFlowDefinitionId = this.Id
                        };
                        db.Db_WorkFlowDefLineSet.Add(newRow);
                    }
                    else {
                        row.From = nodesComb[item.from];
                        row.To = nodesComb[item.to];
                        row.M = item.M;
                        row.Name = item.name;
                        row.Type = item.type;
                    }
                }
                var delLineRows = (from c in db.Db_WorkFlowDefLineSet.AsEnumerable() where !condtion.lines.Select(p => p.Id).Contains(c.Id) && c.Db_WorkFlowDefinitionId == this.Id select c);
                if (delLineRows.Count() > 0) {
                    db.Db_WorkFlowDefLineSet.RemoveRange(delLineRows);
                }
                #endregion

                //区域

                #region -- 区域
                foreach (var item in condtion.areas) {
                    var row = db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefArea>().SingleOrDefault(p => p.Id == item.Id);
                    if (row == null)
                    {
                        var newRow = new Db_WorkFlowDefArea()
                        {
                            Id = SysHelps.GetNewId(),
                            Db_WorkFlowDefinitionId = this.Id,
                            Name = item.name,
                            Color = item.color,
                            Height = item.height,
                            Width = item.width,
                            Left = item.left,
                            Top = item.top
                        };
                        db.Db_WorkFlowDefBaseUnitSet.Add(newRow);
                    }
                    else {
                        row.Name = item.name;
                        row.Color = item.color;
                        row.Height = item.height;
                        row.Width = item.width;
                        row.Left = item.left;
                        row.Top = item.top;
                    }
                }
                var delAreaRows = (from c in db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefArea>().AsEnumerable() where !condtion.areas.Select(p => p.Id).Contains(c.Id) && c.Db_WorkFlowDefinitionId == this.Id select c);
                if (delAreaRows.Count() > 0)
                {
                    db.Db_WorkFlowDefBaseUnitSet.RemoveRange(delAreaRows);
                }

                #endregion

                db.SaveChanges();
            }

            var info = WorkFlowDefinition.GetInstance(this.Id);
            info.SetUnits();
            return info;
        }

        /// <summary>
        /// 删除（物理删除）
        /// 删除工作流信息和与其相关的流程图信息
        /// </summary>
        public void Delete() {
            using (var db = new DefaultContainer()) {
                var row = db.Db_WorkFlowDefinitionSet.Single(p => p.Id == this.Id);
                if (row.Db_WorkFlowDefArea.Count > 0)
                    db.Db_WorkFlowDefBaseUnitSet.RemoveRange(row.Db_WorkFlowDefArea);
                if (row.Db_WorkFlowDefStep.Count > 0)
                    db.Db_WorkFlowDefBaseUnitSet.RemoveRange(row.Db_WorkFlowDefStep);
                if (row.Db_WorkFlowDefLine.Count > 0)
                    db.Db_WorkFlowDefLineSet.RemoveRange(row.Db_WorkFlowDefLine);
                db.Db_WorkFlowDefinitionSet.Remove(row);

                db.SaveChanges();

            }
        }

        public string getIdString()
        {
            return this.Id;
        }

        public string getNameString()
        {
            return this.Caption;
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
            this.name = string.IsNullOrEmpty(row.Name) ? "" : row.Name;
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
    public class WorkFlowDefStep : WorkFlowDefBaseUnit
    {
        #region

        /// <summary>
        /// 类型字典
        /// </summary>
        public static Dictionary<string, string> typeShowStrings;

        static WorkFlowDefStep() {
            typeShowStrings = new Dictionary<string, string>();
            typeShowStrings.Add("start", "开始结点");
            typeShowStrings.Add("end", "结束结点");
            typeShowStrings.Add("task", "任务结点");
            typeShowStrings.Add("node", "自动结点");
            typeShowStrings.Add("chat", "决策结点");
            typeShowStrings.Add("state", "状态结点");
            typeShowStrings.Add("plug", "附加插件");
            typeShowStrings.Add("join", "联合结点");
            typeShowStrings.Add("fork", "分支结点");
            typeShowStrings.Add("complex", "复合结点");
        }


        #region -- 属性

        /// <summary>
        /// 工作流步骤节点的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工作流定义的ID
        /// </summary>
        public string DefinitionId { get; set; }

        /// <summary>
        /// 工作流步骤节点类型
        /// </summary>
        public WorkFlowStepType type { get; set; }

        /// <summary>
        /// 工作流步骤节点类型 - 文本 英文
        /// </summary>
        public string typeString { get; set; }

        /// <summary>
        /// 工作流步骤节点类型 - 文本 中文
        /// </summary>
        public string typeZHString { get; set; }

        /// <summary>
        /// 是否会签
        /// </summary>
        public bool IsCountersign { get; set; }

        /// <summary>
        /// 工作流审批角色ID
        /// </summary>
        public string WorkFlowRoleId { get; set; }

        #endregion

        #endregion

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
            this.typeZHString = typeShowStrings[this.typeString];
            this.DefinitionId = row.Db_WorkFlowDefinitionId;
            this.WorkFlowRoleId = row.WorkFlowRoleId;
            if (row.IsCountersign.HasValue) {
                this.IsCountersign = row.IsCountersign.Value;
            }
        }

        /// <summary>
        /// 根据ID获取步骤的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WorkFlowDefStep GetInstance(string id) {
            using (var db = new DefaultContainer()) {
                var row = db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefStep>().SingleOrDefault(p => p.Id == id);
                if (row == null) {
                    throw new DataNotFundException(string.Format("传入的工作流步骤ID：[{0}] 有误，未能找到匹配的数据", id));
                }
                return new WorkFlowDefStep(row);
            }
        }

        /// <summary>
        /// 获取该步骤后续的线列表
        /// </summary>
        /// <returns></returns>
        public List<WorkFlowDefLineDetail> GetNextLineDetails() {
            List<WorkFlowDefLineDetail> result = new List<WorkFlowDefLineDetail>();
            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_WorkFlowDefLineSet.AsEnumerable()
                            join s in db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefStep>() on c.To equals s.Id
                            where c.From == this.Id
                            && c.Db_WorkFlowDefinitionId == this.DefinitionId
                            select new { c,s }).ToList();
                result = rows.Select(p => new WorkFlowDefLineDetail(p.c, this, p.s)).ToList();
            }
            return result;
        }

        
    }

    /// <summary>
    /// 工作流步骤定义详情
    /// </summary>
    public class WorkFlowDefSetpDetail : WorkFlowDefStep {

        /// <summary>
        /// 审批角色
        /// </summary>
        public WorkFlowRole WorkFlowRoleInfo { get; set; }

        /// <summary>
        /// 描述文本
        /// </summary>
        public string DescriptString { get; set; }

        /// <summary>
        /// 从数据库 步骤表 与 工作流角色表
        /// </summary>
        /// <param name="row">步骤表</param>
        /// <param name="role">角色表</param>
        public WorkFlowDefSetpDetail(Db_WorkFlowDefStep row, Db_WorkFlowRole role) : base(row) {
            if (role != null) {
                this.WorkFlowRoleInfo = new WorkFlowRole(role);
                this.DescriptString = string.Format("{0},审批角色：[{1}]", this.IsCountersign ? "会签" : "非会签", this.WorkFlowRoleInfo.RoleName);
            }
        }

        /// <summary>
        /// 根据步骤获取工作流步骤详情(包含执行角色)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WorkFlowDefSetpDetail GetDetailInstance(string id) {
            using (var db = new DefaultContainer()) {
                var row = (from c in db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefStep>()
                           join r in db.Db_WorkFlowRoleSet on c.WorkFlowRoleId equals r.Id into temp
                           from role in temp.DefaultIfEmpty()

                           where c.Id == id

                           select new { c, role }
                           ).SingleOrDefault();

                if (row == null) {
                    throw new DataNotFundException(string.Format("传入的工作流步骤ID：[{0}] 有误，未能找到匹配的数据", id));
                }
                return new WorkFlowDefSetpDetail(row.c, row.role);
            }
            
        }

        /// <summary>
        /// 保存到数据库 - 角色/会签 只有任务类型可以保存
        /// </summary>
        /// <returns></returns>
        public WorkFlowDefSetpDetail EditStepApprovalInfo(WorkFlowDefStep condtion)
        {
            if (this.type != WorkFlowStepType.task)
            {
                throw new ValiDataException("只有类型为任务的接口可以编辑该信息");
            }
            using (var db = new DefaultContainer())
            {
                var row = db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefStep>().Single(p => p.Id == this.Id);
                row.IsCountersign = condtion.IsCountersign;
                row.WorkFlowRoleId = condtion.WorkFlowRoleId;
                db.SaveChanges();
                return GetDetailInstance(condtion.Id);
            }
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
        public double? M { get; set; }

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
        public string type { get; set; }

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
            this.type = row.Type;
        }
    }

    /// <summary>
    /// 工作流步骤关系线明细
    /// </summary>
    public class WorkFlowDefLineDetail : WorkFlowDefLine {
        
        /// <summary>
        /// 出发节点
        /// </summary>
        public WorkFlowDefStep FromStep { get; set; }

        /// <summary>
        /// 目标节点
        /// </summary>
        public WorkFlowDefStep ToStep { get; set; }

        /// <summary>
        /// 构造方法 从步骤获取数据后进行构造
        /// </summary>
        /// <param name="row">数据库中的线信息</param>
        /// <param name="from">来自（出发节点）</param>
        /// <param name="to">目标（数据库）</param>
        public WorkFlowDefLineDetail(Db_WorkFlowDefLine row, WorkFlowDefStep from, Db_WorkFlowDefStep to):base(row) {
            this.FromStep = from;
            this.ToStep = new WorkFlowDefStep(to);
        }
    }
}
