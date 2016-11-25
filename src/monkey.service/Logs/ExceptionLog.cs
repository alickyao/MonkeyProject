using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using System.Net;

namespace monkey.service.Logs
{
    /// <summary>
    /// 异常日志
    /// </summary>
    public class ExceptionLog:BaseLog
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string codeString { get; set; }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        public string stackTrace { get; set; }

        public ExceptionLog(Db_ExceptionLog row) : base(row) {
            setValue(row);
        }

        private void setValue(Db_ExceptionLog row)
        {
            this.code = row.code;
            this.codeString = row.codeString;
            this.stackTrace = row.stackTrace;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="c">错误类型</param>
        /// <param name="message">消息</param>
        /// <param name="stackTrace">堆栈信息</param>
        /// <returns></returns>
        public static ExceptionLog create(HttpStatusCode c, string message, string stackTrace)
        {
            Db_ExceptionLog log = new Db_ExceptionLog()
            {
                code = c.GetHashCode().ToString(),
                codeString = c.ToString(),
                createdOn = DateTime.Now,
                message = message,
                logType = (byte)BaseLogType.异常日志.GetHashCode(),
                stackTrace = stackTrace
            };
            using (var db = new DefaultContainer())
            {
                db.Db_BaseLogSet.Add(log);
                db.SaveChanges();
                return new ExceptionLog(log);
            }
        }
    }
}
