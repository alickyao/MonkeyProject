using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;

namespace monkey.service.Logs
{
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
    }
}
