using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;

namespace monkey.service.Logs
{
    /// <summary>
    /// 检索日志请求对象
    /// </summary>
    public class BaseLogSearchReqeust : BaseDateTimeRequest
    {
        /// <summary>
        /// 日志类型-可为空 为空则不限 可多选
        /// </summary>
        public List<BaseLogType> types { get; set; }
    }

    /// <summary>
    /// 可被记录到日志的对象
    /// </summary>
    public interface ILogStringable
    {
        /// <summary>
        /// 获取ID
        /// </summary>
        /// <returns></returns>
        string getIdString();
        /// <summary>
        /// 获取名字标识字符串
        /// </summary>
        /// <returns></returns>
        string getNameString();
    }

    /// <summary>
    /// 系统日志记录类型
    /// </summary>
    public enum BaseLogType {
        /// <summary>
        /// 消息日志
        /// </summary>
        系统日志,
        /// <summary>
        /// 运行异常
        /// </summary>
        异常日志,
        /// <summary>
        /// 用户日志
        /// </summary>
        用户日志
    }

    /// <summary>
    /// 系统基础日志
    /// </summary>
    public class BaseLog
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createdOn { get; set; }

        /// <summary>
        /// 创建时间-格式化后的字符串
        /// </summary>
        public string createdOnString { get; set; }

        /// <summary>
        /// 显示创建时间
        /// </summary>
        public string showTime {
            get; set;
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        public BaseLogType logType { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string message { get; set; }

        public BaseLog(Db_BaseLog row) {
            setValue(row);
        }

        private void setValue(Db_BaseLog row)
        {
            this.Id = row.Id;
            this.createdOn = row.createdOn;
            this.logType = (BaseLogType)row.logType;
            this.message = row.message;
            this.showTime = SysHelps.get2TimeShowString(this.createdOn);
            this.createdOnString = this.createdOn.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 创建系统日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <returns></returns>
        public static BaseLog create(string message) {
            Db_BaseLog newRow = new Db_BaseLog()
            {
                createdOn = DateTime.Now,
                logType = (byte)BaseLogType.系统日志.GetHashCode(),
                message = message
            };
            using (var db = new DefaultContainer()) {
                var newrow = db.Db_BaseLogSet.Add(newRow);
                db.SaveChanges();
                return new BaseLog(newrow);
            }
        }

        /// <summary>
        /// 检索系统日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<BaseLog> searchList(BaseLogSearchReqeust condtion) {
            BaseResponseList<BaseLog> result = new BaseResponseList<BaseLog>();
            using (var db = new DefaultContainer()) {
                DateTime? endDate = null;
                if (condtion.endDate != null) {
                    endDate = DateTime.Parse(string.Format("{0} 23:59:59", condtion.endDate.Value.Date.ToString("yyyy-MM-dd")));
                }
                DateTime? beginDate = null;
                if (condtion.beginDate != null) {
                    beginDate = condtion.beginDate.Value.Date;
                }
                List<byte> types = new List<byte>();
                if (condtion.types != null) {
                    if (condtion.types.Count > 0) {
                        types = condtion.types.Select(p => (byte)p).ToList();
                    }
                }
                var rows = (from c in db.Db_BaseLogSet
                            where (1 == 1)
                            && (types.Count == 0 ? true : types.Contains(c.logType))
                            && (beginDate == null ? true : c.createdOn >= beginDate)
                            && (endDate == null ? true : c.createdOn <= endDate)
                            select c);
                result.total = rows.Count();
                if (result.total > 0 && condtion.getRows) {
                    rows = rows.OrderByDescending(p => p.createdOn);
                    if (condtion.page > 0) {
                        rows = rows.Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.AsEnumerable().Select(p => new BaseLog(p)).ToList();
                }
            }
            return result;
        }
    }
}
