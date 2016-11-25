using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using monkey.service.Users;

namespace monkey.service.Logs
{
    

    /// <summary>
    /// 用户日志
    /// </summary>
    public class UserLog : BaseLog
    {
        /// <summary>
        /// 用户日志操作类型标示字符串
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 对应的用户ID
        /// </summary>
        public string userId { get; set; }

        public UserLog(Db_UserLog row) : base(row)
        {
            setValue(row);
        }
        private void setValue(Db_UserLog row)
        {
            this.code = row.code;
            this.userId = row.userId;
        }

        /// <summary>
        /// 创建一个用户日志
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="userObj">关联的用户对象</param>
        /// <param name="code">用户日志操作类型标示字符串</param>
        /// <param name="fkObj">关联的其他信息对象</param>
        /// <returns></returns>
        public static UserLog create(string message,string code ,ILogStringable userObj,  ILogStringable fkObj = null) {
            
            Db_UserLog log = new Db_UserLog() {
                code = code,
                createdOn = DateTime.Now,
                logType = (byte)BaseLogType.用户日志,
                message = message,
                userId = userObj.getIdString(),
                userName = userObj.getNameString(),
            };
            if (fkObj != null) {
                log.fkId = fkObj.getIdString();
                log.fkName = fkObj.getNameString();
            }

            using (var db = new DefaultContainer()) {
                db.Db_BaseLogSet.Add(log);
                db.SaveChanges();
                return new UserLog(log);
            }
        }
    }
}
