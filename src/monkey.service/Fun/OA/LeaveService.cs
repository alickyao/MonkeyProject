using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.WorkFlow;
using monkey.service.Db;
using monkey.service.Users;


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

        public override string getNameString()
        {
            return "请假申请";
        }

        /// <summary>
        /// 请假申请详情展示页面地址
        /// </summary>
        /// <returns></returns>
        public override string GetOrderDetailPageUrl()
        {
            return "/manager/OA/LeaveDefaultDetail?pageId={0}&id={1}";
        }

        /// <summary>
        /// 天数判断步骤 返回对应的审批线
        /// </summary>
        /// <param name="line"></param>
        /// <param name="nextLines"></param>
        /// <returns></returns>
        public override WorkFlowDefLineDetail DoWorkFlowSelectLine(WorkFlowDefLineDetail line, List<WorkFlowDefLineDetail> nextLines)
        {
            if (line.ToStep.Id == "e76982fed81b4f13982528b375339c9b") {
                //判断天数
                var td = (this.EndTime - this.BeginTime).TotalDays;
                if (td >= 2) {
                    return nextLines.Single(p => p.Id == "507fbfb416b44301b71684dea4a498db");
                }
                else {
                    return nextLines.Single(p => p.Id == "800372357ce4495999e77875e1890b8d");
                }
            }
            return base.DoWorkFlowSelectLine(line, nextLines);
        }
    }
}
