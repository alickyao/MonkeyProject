using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.WorkFlow;
using monkey.service.Db;
using monkey.service.Users;
using monkey.service.Logs;


namespace monkey.service.Fun.OA
{
    /// <summary>
    /// 请假类型
    /// </summary>
    public enum LeaveType {
        事假,
        病假,
        年假,
        婚假
    }

    /// <summary>
    /// 请假创建/编辑请求
    /// </summary>
    public class LeaveInfoEditRequest {

        /// <summary>
        /// 被编辑的请假申请的ID 新增时不填
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 请假类型
        /// </summary>
        public LeaveType Type { get; set; }

        /// <summary>
        /// 请假开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 请假结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 请假人
        /// </summary>
        public ICommunicationable User { get; set; }

        /// <summary>
        /// 请假事由
        /// </summary>
        public string Descript { get; set; }
    }

    /// <summary>
    /// 请假
    /// </summary>
    public class LeaveInfo : BaseWorkOrder
    {
        /// <summary>
        /// 请假类型
        /// </summary>
        public LeaveType Type { get; set; }

        /// <summary>
        /// 请假类型 - 文本
        /// </summary>
        public string TypeString { get; set; }

        /// <summary>
        /// 请假开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 请假开始时间 - 文本
        /// </summary>
        public string BeginTimeString { get; set; }

        /// <summary>
        /// 请假结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 请假结束时间 - 文本
        /// </summary>
        public string EndTimeString { get; set; }

        /// <summary>
        /// 请假小时
        /// </summary>
        public double Hours { get; set; }

        /// <summary>
        /// 请假人
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 请假事由
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 构造方法 - 空
        /// </summary>
        public LeaveInfo() { }

        /// <summary>
        /// 构造方法 - 使用ID
        /// </summary>
        /// <param name="id"></param>
        public LeaveInfo(string id) : base(id)
        {
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseWorkOrderSet.OfType<Db_OA_Leave>().SingleOrDefault(p => p.Id == id);
                if (row == null)
                {
                    throw new DataNotFundException(string.Format("未能通过ID找到执行的工单，ID：[{0}]", id));
                }
                SetValue(row);
            }
        }


        /// <summary>
        /// 构造方法 - 数据库
        /// </summary>
        /// <param name="row"></param>
        public LeaveInfo(Db_OA_Leave row) : base(row)
        {
            SetValue(row);
        }

        private void SetValue(Db_OA_Leave row) {
            this.Type = (LeaveType)row.LeaveType;
            this.TypeString = this.Type.ToString();
            this.BeginTime = row.BeginTime;
            this.EndTime = row.EndTime;
            this.UserId = row.UserId;
            this.Descript = row.Descript;
            this.Hours = (this.EndTime - this.BeginTime).TotalHours;
            this.BeginTimeString = this.BeginTime.ToString("yyyy-MM-dd HH:mm");
            this.EndTimeString = this.EndTime.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 获取我的请假申请列表
        /// </summary>
        /// <param name="userId">用户的ID</param>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<LeaveInfo> SearchMyLeavesList(string userId, BaseRequest condtion) {
            BaseResponseList<LeaveInfo> result = new BaseResponseList<LeaveInfo>();

            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_BaseWorkOrderSet.OfType<Db_OA_Leave>() where c.UserId == userId select c);
                result.total = rows.Count();
                if (result.total > 0 && condtion.getRows) {
                    result.rows = rows.OrderByDescending(p => p.CreatedOn).Skip(condtion.getSkip()).Take(condtion.pageSize).AsEnumerable().Select(p => new LeaveInfo(p)).ToList();
                }
            }

            return result;
        }

        /// <summary>
        /// 创建请假申请
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static LeaveInfo CreateLeaveInfo(LeaveInfoEditRequest info) {
            
            string remarkString = string.Format("请假，从{0}至{1}，{2}，申请人：{3}", info.BeginTime.ToString("yyyy-MM-dd HH:mm"), info.EndTime.ToString("yyyy-MM-dd HH:mm"), info.Type.ToString(), info.User.getFullNameString());
            using (var db = new DefaultContainer()) {
                Db_OA_Leave dbl = new Db_OA_Leave() {
                    BeginTime = info.BeginTime,
                    EndTime = info.EndTime,
                    CreatedOn = DateTime.Now,
                    Descript = info.Descript,
                    Id = Guid.NewGuid().ToString(),
                    LeaveType = (byte)info.Type,
                    OrderType = (byte)WorkOrderType.请假申请.GetHashCode(),
                    Remark = remarkString,
                    UserId= info.User.getIdString(),
                };
                db.Db_BaseWorkOrderSet.Add(dbl);
                db.SaveChanges();
                return new LeaveInfo(dbl);
            }
        }

        /// <summary>
        /// 编辑请假申请
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public LeaveInfo EditLeaveInfo(LeaveInfoEditRequest info)
        {
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseWorkOrderSet.OfType<Db_OA_Leave>().Single(p => p.Id == this.Id);
                row.BeginTime = info.BeginTime;
                row.EndTime = info.EndTime;
                row.LeaveType = (byte)info.Type.GetHashCode();
                row.Descript = info.Descript;
                db.SaveChanges();
                return new LeaveInfo(row);
            }
        }

        /// <summary>
        /// 删除请假申请
        /// </summary>
        public override void Del()
        {
            //验证
            if (this.OrderStatus == WorkOrderStatus.待提交 || this.OrderStatus == WorkOrderStatus.已终止)
            {
                base.Del();
            }
            else {
                throw new ValiDataException("只有待提交和已终止状态的请假申请可以被删除");
            }
        }



        #region -- 重写工单审批特殊操作

        #region -- 启动时

        /// <summary>
        /// 工作流启动前
        /// </summary>
        /// <param name="defId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        protected override bool DoWorkFlowBeginBefore(string defId, ICommunicationable user)
        {
            UserLog.create("【自定义】请假申请工作流即将启动", "请假申请", user, this);
            return base.DoWorkFlowBeginBefore(defId, user);
        }
        /// <summary>
        /// 工作流启动后
        /// </summary>
        /// <param name="defId"></param>
        /// <param name="user"></param>

        protected override void DoWorkFlowBeginAfter(string defId, ICommunicationable user)
        {
            UserLog.create("【自定义】请假申请工作流已启动完毕", "请假申请", user, this);
            base.DoWorkFlowBeginAfter(defId, user);
        }

        #endregion

        #region -- 启动后在执行所有步骤前

        /// <summary>
        /// 启动后在执行所有步骤前
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public override bool DoWorkFlowConfimBefore(WorkFlowDefLineDetail line)
        {
            UserLog.create(string.Format("【自定义】请假申请即将执行步骤[{0}]", line.ToStep.name), "请假申请", adminUser, this);
            return base.DoWorkFlowConfimBefore(line);
        }

        #endregion

        #region -- 执行到需要待用户审批的任务节点

        /// <summary>
        /// 执行到待用户审批步骤是返回执行人的列表
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public override List<ICommunicationable> DoWorkFlowGetTaskUserList(WorkFlowDefLineDetail line)
        {
            UserLog.create("【自定义】请假申请进入到任务节点，返回执行人", "请假申请", adminUser, this);
            return base.DoWorkFlowGetTaskUserList(line);
        }

        /// <summary>
        /// 执行到待用户审批步骤是返回执行人的列表 主流程处理完毕后
        /// </summary>
        /// <param name="userId"></param>
        public override void DoWorkFlowSetTaskUserListAfter(List<ICommunicationable> userId)
        {
            UserLog.create("【自定义】请假申请进入到任务节点，返回执行人后", "请假申请", adminUser, this);
            base.DoWorkFlowSetTaskUserListAfter(userId);
        }

        #endregion

        #region -- 用户审批时

        /// <summary>
        /// 用户审批前
        /// </summary>
        /// <param name="condtion">审批请求</param>
        /// <param name="nowTaskUsers">当前审批的用户列表</param>
        /// <param name="userInfo">当前审批人</param>
        /// <param name="stepInfo">当前步骤</param>
        /// <returns></returns>
        protected override bool DoWorkFlowUserConfirmBefore(BaseWorkOrderUserConfirmReqeust condtion, List<BaseWorkOrderTaskUserInfo> nowTaskUsers, ICommunicationable userInfo, WorkFlowDefStep stepInfo)
        {
            UserLog.create("【自定义】请假申请用户即将审批", "请假申请", userInfo, this);
            return base.DoWorkFlowUserConfirmBefore(condtion, nowTaskUsers, userInfo, stepInfo);
        }

        /// <summary>
        /// 用户审批后
        /// </summary>
        /// <param name="condtion">审批请求</param>
        /// <param name="nowTaskUsers">当前审批的用户列表</param>
        /// <param name="taskUserInfo">当前审批人</param>
        /// <param name="stepInfo">当前步骤</param>
        /// <param name="nextLines">后续可选步骤</param>
        /// <param name="userInfo">审批人</param>
        protected override void DoWorkFlowUserConfirmAfter(BaseWorkOrderUserConfirmReqeust condtion, List<BaseWorkOrderTaskUserInfo> nowTaskUsers, BaseWorkOrderTaskUserInfo taskUserInfo, WorkFlowDefStep stepInfo, List<WorkFlowDefLineDetail> nextLines, ICommunicationable userInfo)
        {
            UserLog.create("【自定义】请假申请用户已审批", "请假申请", userInfo, this);
            base.DoWorkFlowUserConfirmAfter(condtion, nowTaskUsers, taskUserInfo, stepInfo, nextLines, userInfo);
        }

        #endregion

        #region -- 自动执行 - 除开任务节点和结束节点外的其他节点在执行后都会调用该步骤 并返回后续步骤

        /// <summary>
        /// 自动执行 - 除开任务节点和结束节点外的其他节点在执行后都会调用该步骤
        /// </summary>
        /// <param name="line"></param>
        /// <param name="nextLines"></param>
        /// <returns></returns>
        public override WorkFlowDefLineDetail DoWorkFlowSelectLine(WorkFlowDefLineDetail line, List<WorkFlowDefLineDetail> nextLines)
        {
            UserLog.create("【自定义】请假申请自动执行", "请假申请", adminUser, this);
            //当审批执行到  天数判断 节点时
            if (line.ToStep.Id == "e76982fed81b4f13982528b375339c9b")
            {
                //判断天数
                var td = (this.EndTime - this.BeginTime).TotalDays;
                if (td >= 2)
                {
                    return nextLines.Single(p => p.Id == "507fbfb416b44301b71684dea4a498db");
                }
                else {
                    return nextLines.Single(p => p.Id == "800372357ce4495999e77875e1890b8d");
                }
            }
            return base.DoWorkFlowSelectLine(line, nextLines);
        }

        #endregion

        #region -- 工作流完成后（整个流程已结束）

        /// <summary>
        /// 工作流完结后
        /// </summary>
        public override void DoWorkFlowEndAfter()
        {
            UserLog.create("【自定义】请假申请工作流审批已经完成", "请假申请", adminUser, this);
            base.DoWorkFlowEndAfter();
        }

        #endregion

        #region -- 终止时

        /// <summary>
        /// 终止前
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="nowTaskUsers"></param>
        /// <param name="userInfo"></param>
        /// <param name="stepInfo"></param>
        /// <returns></returns>
        protected override bool DoWorkFlowTerminationBefore(BaseWorkOrderUserConfirmReqeust condtion, List<BaseWorkOrderTaskUserInfo> nowTaskUsers, ICommunicationable userInfo, WorkFlowDefStep stepInfo)
        {
            UserLog.create("【自定义】请假申请审批即将被终止", "请假申请", userInfo, this);
            return base.DoWorkFlowTerminationBefore(condtion, nowTaskUsers, userInfo, stepInfo);
        }

        /// <summary>
        /// 终止后
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="nowTaskUsers"></param>
        /// <param name="taskUserInfo"></param>
        /// <param name="stepInfo"></param>
        /// <param name="userInfo"></param>
        protected override void DoWorkFlowTerminationAfter(BaseWorkOrderUserConfirmReqeust condtion, List<BaseWorkOrderTaskUserInfo> nowTaskUsers, BaseWorkOrderTaskUserInfo taskUserInfo, WorkFlowDefStep stepInfo, ICommunicationable userInfo)
        {
            UserLog.create("【自定义】请假申请已经被终止", "请假申请", userInfo, this);
            base.DoWorkFlowTerminationAfter(condtion, nowTaskUsers, taskUserInfo, stepInfo, userInfo);
        }

        #endregion

        #endregion

        /// <summary>
        /// 请假申请详情展示页面地址
        /// </summary>
        /// <returns></returns>
        public override string GetOrderDetailPageUrl()
        {
            return "/manager/OA/LeaveDefaultDetail?pageId={0}&id={1}";
        }

        /// <summary>
        /// 获取业务的名称
        /// </summary>
        /// <returns></returns>
        public override string getNameString()
        {
            return "请假申请";
        }
    }
}
