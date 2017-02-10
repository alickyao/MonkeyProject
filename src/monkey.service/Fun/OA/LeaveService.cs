using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
    /// 请假
    /// </summary>
    public class LeaveInfo
    {
        /// <summary>
        /// 请假申请的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 请假类型
        /// </summary>
        public LeaveType type { get; set; }

        /// <summary>
        /// 请假开始时间
        /// </summary>
        public DateTime beginTime { get; set; }

        /// <summary>
        /// 请假结束时间
        /// </summary>
        public DateTime endTime { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string createBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createdOn { get; set; }

        /// <summary>
        /// 请假事由
        /// </summary>
        public string remark { get; set; }
    }
}
