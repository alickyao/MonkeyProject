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

    #region - 类型枚举

    /// <summary>
    /// 工单类型
    /// </summary>
    public enum WorkOrderType
    {
        /// <summary>
        /// 基础工单 - 无类具体类型
        /// </summary>
        无类型,
        请假申请
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
    /// 用户审核状态
    /// </summary>
    public enum WorkOrderUserConfirmType {
        /// <summary>
        /// 等待用户审核
        /// </summary>
        待审,
        /// <summary>
        /// 用户已经审核
        /// </summary>
        已审
    }

    #endregion

    #region -- 请求对象

    /// <summary>
    /// 工单检索请求
    /// </summary>
    public class BaseWorkOrderSearchRequest : BaseRequest {

        /// <summary>
        /// 待审/已审用户ID
        /// </summary>
        public string TaskUserId { get; set; }

        /// <summary>
        /// 待审/已审状态
        /// </summary>
        public WorkOrderUserConfirmType TaskUserConfirmType { get; set; }
    }

    /// <summary>
    /// 工单用户审核/终止请求
    /// </summary>
    public class BaseWorkOrderUserConfirmReqeust {

        /// <summary>
        /// 审批的工单ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }
    }

    #endregion

    /// <summary>
    /// 基础工单
    /// </summary>
    public class BaseWorkOrder : ILogStringable
    {

        #region
        /// <summary>
        /// 工单记录的ID
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
        /// 后台默认的系统管理员用户
        /// </summary>
        protected UserManager adminUser = UserManager.getSysAdminUser();

        #region -- 启动

        /// <summary>
        /// 工作流提交
        /// </summary>
        /// <param name="defId">工作流定义的ID</param>
        /// <param name="user">提交工作流的用户</param>
        public void WorkFlowBegin(string defId, ICommunicationable user) {
            if (this.OrderStatus == WorkOrderStatus.待提交)
            {
                DoWorkFlowBegin(defId, user);
            }
            else {
                throw new ValiDataException("只有待提交的工单才可进行提交操作");
            }
        }

        /// <summary>
        /// 执行工作流提交
        /// </summary>
        /// <param name="defId">工作流定义的ID</param>
        /// <param name="user">提交工作流的用户</param>
        protected virtual void DoWorkFlowBegin(string defId, ICommunicationable user) {

            this.WorkFlowDefinitionId = defId;

            if (!DoWorkFlowBeginBefore(defId, user)) {
                throw new ValiDataException("当前工作流无法启动");
            }

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
            UserLog.create("启动工作流", "工作流", user, this);

            //执行
            //获取下一步
            var nextList = beginStep.GetNextLineDetails();
            if (nextList.Count > 0) {
                DoWorkFlowConfim(nextList.First());
            }

            DoWorkFlowBeginAfter(defId, user);
        }

        /// <summary>
        /// 【具体业务根据需要重写】工作流启动前 - 一般用于 验证
        /// </summary>
        /// <param name="defId">工作流定义的ID</param>
        /// <param name="user">提交工作流的用户</param>
        /// <returns></returns>
        protected virtual bool DoWorkFlowBeginBefore(string defId, ICommunicationable user) {
            return true;
        }

        /// <summary>
        /// 【具体业务根据需要重写】工作流启动后 - 一般用户发送通知 默认什么也没干
        /// </summary>
        /// <param name="defId">工作流定义的ID</param>
        /// <param name="user">提交工作流的用户</param>
        protected virtual void DoWorkFlowBeginAfter(string defId, ICommunicationable user) {
            return;
        }

        #endregion


        #region -- 自动执行 流程控制

        /// <summary>
        /// 执行工作流
        /// </summary>
        /// <param name="line"></param>
        public virtual void DoWorkFlowConfim(WorkFlowDefLineDetail line) {
            if (line.ToStep != null) {

                if (!DoWorkFlowConfimBefore())
                {
                    throw new ValiDataException("当前工作流无法执行");
                }

                LogToApprovalHistory(line);

                //记录流程走向
                //判断下一步 是否为结束节点 如果是 则完成工作流
                if (line.ToStep.type == WorkFlowStepType.end)
                {
                    SetWorkFlowToEndStatus();
                    UserLog.create("流程已结束", "工作流", adminUser, this);
                    DoWorkFlowEndAfter();
                    return;
                }
                else if (line.ToStep.type == WorkFlowStepType.task) {
                    SetWorkFlowBookMark(line);
                    //保存执行人
                    var userList = DoWorkFlowGetTaskUserList(line);
                    List<string> userIdList = new List<string>();
                    List<string> userFullNameList = new List<string>();
                    foreach (var item in userList) {
                        userIdList.Add(item.getIdString());
                        userFullNameList.Add(item.getFullNameString());
                    }

                    //if (!userIdList.Contains(adminUser.Id)) {
                    //    userList.Add(adminUser);
                    //}

                    //保存到数据库
                    using (var db = new DefaultContainer()) {
                        List<Db_BaseWorkOrderTaskUser> taskDbUsers = new List<Db_BaseWorkOrderTaskUser>();
                        foreach (var item in userList) {
                            taskDbUsers.Add(new Db_BaseWorkOrderTaskUser() {
                                Id = Guid.NewGuid().ToString(),
                                CreatedOn = DateTime.Now,
                                IsConfirm = false,
                                UserId = item.getIdString(),
                                Db_BaseWorkOrderId = this.Id,
                                Db_WorkFlowDefLineId = line.Id,
                                Db_WorkFlowDefinitionId = this.WorkFlowDefinitionId,
                                Db_WorkFlowDefStepId = line.ToStep.Id,
                                userName = item.getFullNameString()
                            });
                        }
                        db.Db_BaseWorkOrderTaskUserSet.AddRange(taskDbUsers);
                        db.SaveChanges();
                    }
                    DoWorkFlowSetTaskUserListAfter(userList);
                    string taskUsers = "未指定的审批用户";
                    if (userFullNameList.Count > 0) {
                        taskUsers = string.Join(",", userFullNameList);
                    }
                    UserLog.create(string.Format("执行已到任务节点[{0}]，等待用户[{1}]审批", line.ToStep.name, taskUsers), "工作流", adminUser, this);
                    return;
                }
                else {
                    string msg = string.Format("执行流程，步骤：[{0}]", line.ToStep.name);
                    UserLog.create(msg, "工作流", adminUser, this);
                    //获取下一步
                    var nextLines = line.ToStep.GetNextLineDetails();
                    if (nextLines.Count > 0)
                    {
                        var next = DoWorkFlowSelectLine(line, nextLines);
                        //递归继续
                        DoWorkFlowConfim(next);
                    }
                }
            }
        }

        /// <summary>
        /// 记录到审批历史记录
        /// </summary>
        /// <param name="line"></param>
        private void LogToApprovalHistory(WorkFlowDefLineDetail line) {
            using (var db = new DefaultContainer()) {
                var dbRow = new Db_BaseWorkOrderApprovalHistory()
                {
                    CreatedOn = DateTime.Now,
                    Db_BaseWorkOrderId = this.Id,
                    Id = Guid.NewGuid().ToString(),
                    WorkFlowDefLineId = line.Id
                };
                db.Db_BaseWorkOrderApprovalHistorySet.Add(dbRow);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 【具体业务根据需要重写】执行审批工作流前 - 一般 可进行一些验证 默认返回为True
        /// </summary>
        public virtual bool DoWorkFlowConfimBefore() {
            return true;
        }

        /// <summary>
        /// 【具体业务根据需要重写】工作流执行结束后 - 一般 进行审批后的一些操作 默认什么都没做
        /// </summary>
        public virtual void DoWorkFlowEndAfter() {
            return;
        }

        /// <summary>
        /// 【具体业务根据需要重写】工作流审批 任务节点 返回执行人 - 默认返回为工作流角色设置中的用户列表
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual List<ICommunicationable> DoWorkFlowGetTaskUserList(WorkFlowDefLineDetail line) {
            var workflowRole = WorkFlowRole.GetInstance(line.ToStep.WorkFlowRoleId);
            var userIds = workflowRole.GetDescripUserId();
            BaseResponseList<UserManager> userManagers = UserManager.searchList(new UserManagerSearchRequest()
            {
                cId = new BaseBatchRequest<string>() { rows = userIds },
                page = 0
            });
            List<ICommunicationable> result = new List<ICommunicationable>();
            if (userManagers.rows != null) {
                if (userManagers.rows.Count > 0) {
                    foreach (var item in userManagers.rows) {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 【具体业务根据需要重写】工作流审批 保存执行人后执行 - 默认什么都没做 - 可用于发送通知什么的
        /// </summary>
        /// <param name="userId"></param>
        public virtual void DoWorkFlowSetTaskUserListAfter(List<ICommunicationable> userId) {
            return;
        }

        /// <summary>
        /// 【具体业务根据需要重写】选择下一步操作线路 与 其他附加的操作 - 默认返回后续多条分后续分支中默认的第一条
        /// </summary>
        /// <param name="line"></param>
        /// <param name="nextLines"></param>
        /// <returns></returns>
        public virtual WorkFlowDefLineDetail DoWorkFlowSelectLine(WorkFlowDefLineDetail line, List<WorkFlowDefLineDetail> nextLines) {
            //默认就是返回默认的那一个
            return nextLines.First();
        }

        #endregion


        #region -- 工单审核

        /// <summary>
        /// 【具体业务根据需要重写】工单用户审批前 - 默认为权限验证
        /// </summary>
        /// <param name="condtion">审批请求</param>
        /// <param name="nowTaskUsers">当前步骤审批人列表</param>
        /// <param name="userInfo">当前审批人</param>
        /// <param name="stepInfo">步骤信息</param>
        /// <returns></returns>
        protected virtual bool DoWorkFlowUserConfirmBefore(BaseWorkOrderUserConfirmReqeust condtion,List<BaseWorkOrderTaskUserInfo> nowTaskUsers, ICommunicationable userInfo, WorkFlowDefStep stepInfo) {
            //权限验证
            if (this.OrderStatus == WorkOrderStatus.执行中)
            {
                string userId = userInfo.getIdString();
                if (!nowTaskUsers.Select(p => p.UserId).Contains(userId))
                {
                    throw new ValiDataException("您没有审批该流程的权限");
                }
                if (nowTaskUsers.Where(p => !p.IsConfirm).Select(p => p.UserId).Contains(userId))
                {
                    return true;
                }
                else {
                    throw new ValiDataException("您已经审批过了，不能再审批该流程");
                }
            }
            else {
                throw new ValiDataException("当前工作流不是执行中的状态，无法进行审批");
            }
        }

        /// <summary>
        /// 【具体业务根据需要重写】工单用户审批后 - 默认为判断是否会签 并执行下一步
        /// </summary>
        /// <param name="condtion">审批请求</param>
        /// <param name="nowTaskUsers">步骤全部审批人</param>
        /// <param name="taskUserInfo">步骤当前审批人</param>
        /// <param name="stepInfo">步骤信息</param>
        /// <param name="nextLines">步骤的下一步信息</param>
        /// <param name="userInfo">当前步骤审批人的其他信息</param>
        protected virtual void DoWorkFlowUserConfirmAfter(BaseWorkOrderUserConfirmReqeust condtion, List<BaseWorkOrderTaskUserInfo> nowTaskUsers, BaseWorkOrderTaskUserInfo taskUserInfo, WorkFlowDefStep stepInfo,List<WorkFlowDefLineDetail> nextLines,ICommunicationable userInfo) {
            if (nextLines.Count > 0)
            {
                if (!stepInfo.IsCountersign)
                {
                    UserLog.create("非会签任务节点，开始执行下一步", "工作流", adminUser, this);
                    //不是会签执行下一步
                    DoWorkFlowConfim(nextLines.First());
                }
                else {
                    //是会签
                    //判断是否都已经审批过了 如果是 执行下一步
                    if (!nowTaskUsers.Any(p => p.IsConfirm == false))
                    {
                        UserLog.create("会签节点，全部用户完成审批，开始执行下一步", "工作流", adminUser, this);
                        DoWorkFlowConfim(nextLines.First());
                    }
                    else {
                        UserLog.create("会签节点，等待其他用户审批", "工作流", adminUser, this);
                    }
                }
            }
        }

        /// <summary>
        /// 工单用户审核
        /// </summary>
        /// <param name="condtion">审批请求</param>
        /// <param name="userInfo">审批用户信息</param>
        public void DoWorkFlowUserConfirm(BaseWorkOrderUserConfirmReqeust condtion,ICommunicationable userInfo) {
            //权限验证
            var nowTaskUsers = GetTaskUsersByStepId(this.WorkFlowBookMarkId);
            string userId = userInfo.getIdString();
            var stepInfo = WorkFlowDefStep.GetInstance(this.WorkFlowBookMarkId);

            if (DoWorkFlowUserConfirmBefore(condtion, nowTaskUsers, userInfo, stepInfo))
            {
                var taskUserInfo = nowTaskUsers.Single(p => p.UserId == userId);
                taskUserInfo.IsConfirm = true;
                taskUserInfo.ConfirmTime = DateTime.Now;
                taskUserInfo.ConfirmTimeString = taskUserInfo.ConfirmTime.Value.ToString("yyyy-MM-dd HH:mm");
                taskUserInfo.Remark = condtion.Remark;
                taskUserInfo.SaveConfirm();
                string msg = string.Format("工作流用户审批，步骤：[{0}],备注：[{1}]", stepInfo.name, condtion.Remark);
                UserLog.create(msg, "工作流", userInfo, this);

                //执行下一步
                var next = stepInfo.GetNextLineDetails();
                DoWorkFlowUserConfirmAfter(condtion, nowTaskUsers, taskUserInfo, stepInfo, next, userInfo);
            }
            else {
                throw new ValiDataException("用户审批前置验证失败，无法进行审批");
            }

            if (!nowTaskUsers.Select(p => p.UserId).Contains(userId)) {
                throw new ValiDataException("您没有审批该流程的权限");
            }
        }

        

        #endregion


        #region -- 终止工作流

        /// <summary>
        /// 工作流终止（停止流程执行并设置为终止状态）
        /// </summary>
        public BaseWorkOrder WorkFlowTermination(ICommunicationable user, BaseWorkOrderUserConfirmReqeust condtion) {
            if (this.OrderStatus == WorkOrderStatus.执行中)
            {
                var nowTaskUsers = GetTaskUsersByStepId(this.WorkFlowBookMarkId);
                var stepInfo = WorkFlowDefStep.GetInstance(this.WorkFlowBookMarkId);
                string userId = user.getIdString();
                if (!DoWorkFlowTerminationBefore(condtion, nowTaskUsers, user, stepInfo))
                {
                    throw new ValiDataException("该流程不能被终止");
                }
                var taskUserInfo = nowTaskUsers.Single(p => p.UserId == userId);
                var result = DoWorkFlowTermination(user,condtion.Remark);
                //记录到日志
                UserLog.create(string.Format("流程被终止，原因：[{0}]", condtion.Remark), "工作流", user, this);
                //后续操作
                DoWorkFlowTerminationAfter(condtion, nowTaskUsers, taskUserInfo, stepInfo, user);
                return result;
            }
            else {
                throw new ValiDataException("只有执行中的工单才可进行终止操作");
            }
        }

        /// <summary>
        /// 执行工作流终止操作
        /// </summary>
        /// <returns></returns>
        protected BaseWorkOrder DoWorkFlowTermination(ICommunicationable user,string remark) {
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseWorkOrderSet.Single(p => p.Id == this.Id);
                row.OrderStatus = (byte)WorkOrderStatus.已终止.GetHashCode();
                row.WorkFlowBookMarkId = null;
                var tu = row.Db_BaseWorkOrderTaskUser.FirstOrDefault(p => p.UserId == user.getIdString());
                if (tu != null) {
                    tu.IsConfirm = true;
                    tu.ConfirmTime = DateTime.Now;
                    tu.Remark = remark;
                }
                db.SaveChanges();
                return new BaseWorkOrder(row);
            }
        }

        /// <summary>
        /// 【具体业务根据需要重写】执行工作流终止操作前 - 默认为验证用户是否有审批权限
        /// </summary>
        /// <param name="condtion">审批请求</param>
        /// <param name="nowTaskUsers">当前步骤审批人列表</param>
        /// <param name="userInfo">当前审批人</param>
        /// <param name="stepInfo">步骤信息</param>
        /// <returns></returns>
        protected virtual bool DoWorkFlowTerminationBefore(BaseWorkOrderUserConfirmReqeust condtion, List<BaseWorkOrderTaskUserInfo> nowTaskUsers, ICommunicationable userInfo, WorkFlowDefStep stepInfo)
        {
            if (this.OrderStatus == WorkOrderStatus.执行中)
            {
                string userId = userInfo.getIdString();
                if (!nowTaskUsers.Select(p => p.UserId).Contains(userId))
                {
                    throw new ValiDataException("您没有终止该流程的权限");
                }
                if (nowTaskUsers.Where(p => !p.IsConfirm).Select(p => p.UserId).Contains(userId))
                {
                    return true;
                }
                else {
                    throw new ValiDataException("您已经审批过了，不能再终止该流程");
                }
            }
            else {
                throw new ValiDataException("当前工作流不是执行中的状态，不能终止");
            }
        }

        /// <summary>
        /// 【具体业务根据需要重写】执行工作流终止操作后 - 默认什么也没做
        /// </summary>
        /// <param name="condtion">审批请求</param>
        /// <param name="nowTaskUsers">步骤全部审批人</param>
        /// <param name="taskUserInfo">步骤当前审批人</param>
        /// <param name="stepInfo">步骤信息</param>
        /// <param name="userInfo">当前步骤审批人的其他信息</param>
        protected virtual void DoWorkFlowTerminationAfter(BaseWorkOrderUserConfirmReqeust condtion, List<BaseWorkOrderTaskUserInfo> nowTaskUsers, BaseWorkOrderTaskUserInfo taskUserInfo, WorkFlowDefStep stepInfo, ICommunicationable userInfo) {
            return;
        }

        #endregion

        /// <summary>
        /// 设置工作流为完成状态
        /// </summary>
        protected void SetWorkFlowToEndStatus() {
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseWorkOrderSet.Single(p => p.Id == this.Id);
                row.OrderStatus = (byte)WorkOrderStatus.已完成.GetHashCode();
                row.WorkFlowBookMarkId = null;
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

        /// <summary>
        /// 获取某个节点下 用户审核的情况
        /// </summary>
        /// <param name="stepId">任务节点的ID</param>
        /// <returns></returns>
        public List<BaseWorkOrderTaskUserInfo> GetTaskUsersByStepId(string stepId) {
            List<BaseWorkOrderTaskUserInfo> result = new List<BaseWorkOrderTaskUserInfo>();
            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_BaseWorkOrderTaskUserSet
                            where c.Db_WorkFlowDefStepId == stepId
                            && c.Db_BaseWorkOrderId == this.Id
                            select c
                            );
                if (rows.Count() > 0) {
                    result = rows.AsEnumerable().Select(p => new BaseWorkOrderTaskUserInfo(p)).ToList();
                }
            }
            return result;
        }
        
        /// <summary>
        /// 获取当前当前流程历史记录 - lines Id
        /// </summary>
        /// <returns></returns>
        public List<string> GetApprovalHistoryLines() {
            List<string> result = new List<string>();
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseWorkOrderSet.Single(p => p.Id == this.Id);
                result = row.Db_BaseWorkOrderApprovalHistory.Select(p => p.WorkFlowDefLineId).ToList();
            }
            return result;
        }

        /// <summary>
        /// 获取审批页面地址
        /// </summary>
        /// <returns></returns>
        public virtual string GetApprovalPageUrl() {
            return "/manager/WorkFlow/WorkFlowOrderDefaultApproval?pageId={0}&id={1}";
        }

        /// <summary>
        /// 获取工单展示页面地址
        /// </summary>
        /// <returns></returns>
        public virtual string GetOrderDetailPageUrl() {
            return "/manager/WorkFlow/WorkFlowOrderDefaultDetail?pageId={0}&id={1}";
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

    /// <summary>
    /// 基础工单列表 - 包含当前执行人与执行流程信息
    /// </summary>
    public class BaseWorkOrderListDetail : BaseWorkOrder {

        /// <summary>
        /// 当前执行人
        /// </summary>
        public string TaskUserNames { get; set; }

        /// <summary>
        /// 当前等待执行步骤的ID
        /// </summary>
        public string NowTaskSetpId { get; set; }

        /// <summary>
        /// 当前等待执行步骤的名称
        /// </summary>
        public string NowTaskStepName { get; set; }

        /// <summary>
        /// 从数据库构造
        /// </summary>
        /// <param name="row">工单</param>
        /// <param name="def">执行的流程</param>
        /// <param name="users">当前待审的用户</param>
        /// <param name="step">当前等待执行步骤</param>
        public BaseWorkOrderListDetail(Db_BaseWorkOrder row,Db_WorkFlowDefinition def,List<Db_BaseWorkOrderTaskUser> users, Db_WorkFlowDefStep step) : base(row) {
            if (this.OrderStatus != WorkOrderStatus.待提交) {
                //修改执行状态文本
                this.OrderStatusString = string.Format("{0}[{1}]", this.OrderStatus.ToString(), def.Caption);
            }
            if (this.OrderStatus == WorkOrderStatus.执行中) {
                if (users.Count > 0)
                {
                    var showUsers = users.Skip(0).Take(5);
                    this.TaskUserNames = string.Join(",", showUsers.Select(p => p.userName));
                    if (users.Count > 5) {
                        this.TaskUserNames += string.Format(",等{0}个用户", users.Count);
                    }
                }
                if (step != null) {
                    this.NowTaskSetpId = step.Id;
                    this.NowTaskStepName = step.Name;
                }
            }
        }


        /// <summary>
        /// 检索工单信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<BaseWorkOrderListDetail> SearchBaseWorkOrderList(BaseWorkOrderSearchRequest condtion)
        {

            BaseResponseList<BaseWorkOrderListDetail> result = new BaseResponseList<BaseWorkOrderListDetail>();

            using (var db = new DefaultContainer())
            {
                //指定用户作为查询条件
                List<string> usersOrderId = new List<string>();
                if (!string.IsNullOrEmpty(condtion.TaskUserId)) {
                    //返回最多200调已审核或者待审记录
                    usersOrderId = (from u in db.Db_BaseWorkOrderTaskUserSet
                                    join o in db.Db_BaseWorkOrderSet on u.Db_BaseWorkOrderId equals o.Id
                                    where u.UserId == condtion.TaskUserId
                                    && (condtion.TaskUserConfirmType == WorkOrderUserConfirmType.已审 ? u.IsConfirm == true : (u.IsConfirm == false && u.Db_WorkFlowDefStepId == o.WorkFlowBookMarkId && o.OrderStatus == (byte)WorkOrderStatus.执行中))
                                    group new { u.Db_BaseWorkOrderId, u.CreatedOn } by new { u.Db_BaseWorkOrderId, u.CreatedOn } into g
                                    orderby g.Key.CreatedOn descending
                                    select g.Key.Db_BaseWorkOrderId).Skip(0).Take(200).ToList();
                }
                var rows = (from c in db.Db_BaseWorkOrderSet
                            join d in db.Db_WorkFlowDefinitionSet on c.WorkFlowDefinitionId equals d.Id into temp
                            from x in temp.DefaultIfEmpty()
                            join s in db.Db_WorkFlowDefBaseUnitSet.OfType<Db_WorkFlowDefStep>() on c.WorkFlowBookMarkId equals s.Id into temp1 from step in temp1.DefaultIfEmpty()
                            where (string.IsNullOrEmpty(condtion.TaskUserId) ? true : usersOrderId.Contains(c.Id))
                            select new
                            {
                                c,
                                x,
                                users = (from u in c.Db_BaseWorkOrderTaskUser where u.Db_WorkFlowDefStepId == c.WorkFlowBookMarkId select u),
                                step
                            }
                            );
                result.total = rows.Count();
                if (condtion.getRows && result.total > 0)
                {
                    if (condtion.page > 0)
                    {
                        rows = rows.OrderByDescending(p => p.c.CreatedOn).Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }

                    result.rows = rows.AsEnumerable().OrderByDescending(p => p.c.CreatedOn).Select(p => new BaseWorkOrderListDetail(p.c, p.x, p.users.ToList(),p.step)).ToList();
                }
            }

            return result;
        }
    }

    /// <summary>
    /// 工单用户审核信息
    /// </summary>
    public class BaseWorkOrderTaskUserInfo {

        #region

        /// <summary>
        /// 工单用户审核信息ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 对应的工单ID
        /// </summary>
        public string BaseWorkOrderId { get; set; }

        /// <summary>
        /// 对应的工作流ID
        /// </summary>
        public string WorkFlowDefinitionId { get; set; }

        /// <summary>
        /// 对应的工作流 线 （指向节点的来路）
        /// </summary>
        public string WorkFlowDefLineId { get; set; }

        /// <summary>
        /// 对应的工作流 节点
        /// </summary>
        public string WorkFlowDefStepId { get; set; }

        /// <summary>
        /// 审批用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 审批用户姓名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 创建时间 - 文本
        /// </summary>
        public string CreatedOnString { get; set; }

        /// <summary>
        /// 是否已审核
        /// </summary>
        public bool IsConfirm { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ConfirmTime { get; set; }

        /// <summary>
        /// 审核时间 - 文本
        /// </summary>
        public string ConfirmTimeString { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        public string Remark { get; set; }

        #endregion

        /// <summary>
        /// 从数据库构造
        /// </summary>
        /// <param name="row"></param>
        public BaseWorkOrderTaskUserInfo(Db_BaseWorkOrderTaskUser row) {
            this.Id = row.Id;
            this.BaseWorkOrderId = row.Db_BaseWorkOrderId;
            this.WorkFlowDefinitionId = row.Db_WorkFlowDefinitionId;
            this.WorkFlowDefLineId = row.Db_WorkFlowDefLineId;
            this.WorkFlowDefStepId = row.Db_WorkFlowDefStepId;
            this.UserId = row.UserId;
            this.userName = row.userName;
            this.CreatedOn = row.CreatedOn;
            this.CreatedOnString = this.CreatedOn.ToString("yyyy-MM-dd HH:mm");
            this.IsConfirm = row.IsConfirm;
            this.ConfirmTime = row.ConfirmTime;
            if (this.ConfirmTime.HasValue) {
                this.ConfirmTimeString = this.ConfirmTime.Value.ToString("yyyy-MM-dd HH:mm");
            }
        }

        /// <summary>
        /// 保存审批状态
        /// </summary>
        public void SaveConfirm() {
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseWorkOrderTaskUserSet.Single(p => p.Id == this.Id);
                row.ConfirmTime = this.ConfirmTime;
                row.Remark = this.Remark;
                row.IsConfirm = this.IsConfirm;
                db.SaveChanges();
            }
        }
    }
}
