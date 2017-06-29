using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using monkey.service.Users;
using monkey.service.Logs;

namespace monkey.service.WorkFlow
{
    
    /// <summary>
    /// 工单类型
    /// </summary>
    public enum WorkOrderType
    {
        /// <summary>
        /// 基础工单 - 无类具体类型
        /// </summary>
        无类型
    }

    /// <summary>
    /// 工单状态
    /// </summary>
    public enum WorkOrderStatus {

        /// <summary>
        /// 未提交工作流执行的工单
        /// </summary>
        待提交,

        /// <summary>
        /// 工作流执行中 - 等待完成的工单
        /// </summary>
        执行中,
        
        /// <summary>
        /// 工作流正常完成的工单
        /// </summary>
        已完成,

        /// <summary>
        /// 执行中被终止的工单
        /// </summary>
        已终止
    }

    /// <summary>
    /// 工单检索请求
    /// </summary>
    public class BaseWorkOrderSearchRequest : BaseRequest {

    }

    /// <summary>
    /// 基础工单
    /// </summary>
    public class BaseWorkOrder:ILogStringable
    {

        #region
        /// <summary>
        /// 记录的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工单类型
        /// </summary>
        public WorkOrderType OrderType { get; set; }

        /// <summary>
        /// 工单类型 - 文本
        /// </summary>
        public string OrderTypeString { get; set; }

        /// <summary>
        /// 工单执行状态
        /// </summary>
        public WorkOrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 工单执行状态 - 文本
        /// </summary>
        public string OrderStatusString { get; set; }

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

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedOnString { get; set; }
        #endregion

        /// <summary>
        /// 从数据库对象构造
        /// </summary>
        /// <param name="row"></param>
        public BaseWorkOrder(Db_BaseWorkOrder row) {
            SetValue(row);
        }

        /// <summary>
        /// 用ID构造
        /// </summary>
        /// <param name="id"></param>
        public BaseWorkOrder(string id) {
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseWorkOrderSet.SingleOrDefault(p => p.Id == id);
                if (row == null) {
                    throw new DataNotFundException(string.Format("未能通过ID找到执行的工单，ID：[{0}]", id));
                }
                SetValue(row);
            }
        }

        private void SetValue(Db_BaseWorkOrder row) {
            this.Id = row.Id;
            this.OrderType = (WorkOrderType)row.OrderType;
            this.OrderTypeString = this.OrderType.ToString();
            this.OrderStatus = (WorkOrderStatus)row.OrderStatus;
            this.OrderStatusString = this.OrderStatus.ToString();
            this.WorkFlowDefinitionId = row.WorkFlowDefinitionId;
            this.WorkFlowBookMarkId = row.WorkFlowBookMarkId;
            this.Remark = row.Remark;
            this.CreatedOn = row.CreatedOn;
            this.CreatedOnString = this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 检索工单信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<BaseWorkOrder> SearchBaseWorkOrderList(BaseWorkOrderSearchRequest condtion) {

            BaseResponseList<BaseWorkOrder> result = new BaseResponseList<BaseWorkOrder>();

            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_BaseWorkOrderSet
                            select c
                            );
                result.total = rows.Count();
                if (condtion.getRows && result.total > 0)
                {
                    if (condtion.page > 1)
                    {
                        rows = rows.OrderByDescending(p => p.CreatedOn).Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }

                    result.rows = rows.AsEnumerable().OrderByDescending(p=>p.CreatedOn).Select(p => new BaseWorkOrder(p)).ToList();
                }
            }

            return result;
        }

        /// <summary>
        /// 批量添加基础工单信息
        /// </summary>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public static List<BaseWorkOrder> CreateBaseWorkOrders(List<string> remarks) {
            List<BaseWorkOrder> result = new List<BaseWorkOrder>();
            using (var db = new DefaultContainer()) {
                List<Db_BaseWorkOrder> dbRows = new List<Db_BaseWorkOrder>();
                foreach (var item in remarks) {
                    var newRow = new Db_BaseWorkOrder()
                    {
                        CreatedOn = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        OrderType = (byte)WorkOrderType.无类型.GetHashCode(),
                        Remark = item
                    };
                    dbRows.Add(newRow);
                    result.Add(new BaseWorkOrder(newRow));
                }
                if (dbRows.Count > 0) {
                    db.Db_BaseWorkOrderSet.AddRange(dbRows);
                    db.SaveChanges();
                }
            }
            return result;
        }


        #region -- 工作流程

        /// <summary>
        /// 工作流提交
        /// </summary>
        /// <param name="defId">业务流程的ID</param>
        /// <param name="userId">操作用户的ID</param>
        public void WorkFlowBegin(string defId,string userId) {
            if (this.OrderStatus == WorkOrderStatus.待提交)
            {
                DoWorkFlowBegin(defId,userId);
            }
            else {
                throw new ValiDataException("只有待提交的工单才可进行提交操作");
            }
        }

        /// <summary>
        /// 执行工作流提交
        /// </summary>
        /// <param name="defId"></param>
        /// <param name="userId"></param>
        protected virtual void DoWorkFlowBegin(string defId,string userId) {
            var defInfo = WorkFlowDefinition.GetInstance(defId);

            //获取启动步骤 
            defInfo.SetUnits();//获取流程信息
            var beginStep = defInfo.GetBegin();
            if (beginStep == null) {
                throw new ValiDataException(string.Format("工作流[{0}]没有起点，无法启动", defInfo.Caption));
            }

            //修改工单状态
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseWorkOrderSet.Single(p => p.Id == this.Id);
                row.WorkFlowDefinitionId = defId;
                row.OrderStatus = (byte)WorkOrderStatus.执行中.GetHashCode();
                db.SaveChanges();
            }

            //记录到日志
            var userInfo = UserManager.getUserById(userId);
            UserLog.create("启动工作流", "工作流", userInfo, this);

            //执行
            //获取下一步
            var nextList = beginStep.GetNextLineDetails();
            if (nextList.Count > 0) {
                DoWorkFlowConfim(nextList.First());
            }
        }

        /// <summary>
        /// 后台默认的系统管理员用户
        /// </summary>
        protected UserManager adminUser = UserManager.getSysAdminUser();

        /// <summary>
        /// 执行工作流
        /// </summary>
        /// <param name="line"></param>
        public virtual void DoWorkFlowConfim(WorkFlowDefLineDetail line) {
            if (line.ToStep != null) {

                #region -- 自定义的流程处理方式



                #endregion

                //记录流程走向
                //判断下一步 是否为结束节点 如果是 则完成工作流
                if (line.ToStep.type == WorkFlowStepType.end)
                {
                    SetWorkFlowToEndStatus();
                    UserLog.create("流程已结束", "工作流", adminUser, this);
                    return;
                }
                else if (line.ToStep.type == WorkFlowStepType.task) {
                    SetWorkFlowBookMark(line);
                    //保存执行人

                    UserLog.create(string.Format("执行已到任务节点[{0}]，等待用户审批",line.ToStep.name), "工作流", adminUser, this);
                    return;
                }
                else {
                    string msg = string.Format("执行流程，步骤：[{0}]", line.ToStep.name);
                    UserLog.create(msg, "工作流", adminUser, this);
                    //获取下一步
                    var nextLines = line.ToStep.GetNextLineDetails();
                    if (nextLines.Count > 0)
                    {

                        #region -- 选择下一步

                        //获取默认的那一个
                        var next = nextLines.First();

                        #endregion


                        //递归继续
                        DoWorkFlowConfim(next);
                    }
                }
            }
        }

        /// <summary>
        /// 工作流终止（停止流程执行并设置为终止状态）
        /// </summary>
        public BaseWorkOrder WorkFlowTermination() {
            if (this.OrderStatus == WorkOrderStatus.执行中)
            {
                return DoWorkFlowTermination();
            }
            else {
                throw new ValiDataException("只有执行中的工单才可进行终止操作");
            }
        }

        /// <summary>
        /// 执行工作流终止操作
        /// </summary>
        /// <returns></returns>
        protected virtual BaseWorkOrder DoWorkFlowTermination() {
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseWorkOrderSet.Single(p => p.Id == this.Id);
                row.OrderStatus = (byte)WorkOrderStatus.已终止.GetHashCode();
                db.SaveChanges();
                return new BaseWorkOrder(row);
            }
        }

        /// <summary>
        /// 设置工作流为完成状态
        /// </summary>
        protected void SetWorkFlowToEndStatus() {
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseWorkOrderSet.Single(p => p.Id == this.Id);
                row.OrderStatus = (byte)WorkOrderStatus.已完成.GetHashCode();
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新流程的工作流书签ID 保存为toStep.Id 用于下次流程启动时恢复到当前位置
        /// </summary>
        /// <param name="line">工作流关系线信息</param>
        protected void SetWorkFlowBookMark(WorkFlowDefLineDetail line) {
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseWorkOrderSet.Single(p => p.Id == this.Id);
                row.WorkFlowBookMarkId = line.ToStep.Id;
                db.SaveChanges();
            }
        }

        #endregion

        public string getIdString()
        {
            return this.Id;
        }

        public virtual string getNameString()
        {
            return "基础工单";
        }
    }
}
