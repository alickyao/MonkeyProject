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
    /// 用户日志检索请求对象
    /// </summary>
    public class UserLogSearchRequest : BaseDateTimeRequest
    {
        /// <summary>
        /// 对应用户信息 - 模糊匹配 用户姓名/角色
        /// </summary>
        public string userName { get; set; }
        
        /// <summary>
        /// 关键字 - 标示符 消息内容
        /// </summary>
        public string q { get; set; }

        /// <summary>
        /// 关联关联的其他信息ID
        /// </summary>
        public string fkId { get; set; }

        /// <summary>
        /// 关联的用户ID
        /// </summary>
        public string userId { get; set; }
    }

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
        /// <summary>
        /// 对应的其他信息ID
        /// </summary>
        public string fkId { get; set; }
        /// <summary>
        /// 对应的其他信息名称
        /// </summary>
        public string fkName { get; set; }
        /// <summary>
        /// 对应用户名称
        /// </summary>
        public string userName { get; set; }

        public UserLog(Db_UserLog row) : base(row)
        {
            setValue(row);
        }
        private void setValue(Db_UserLog row)
        {
            this.code = row.code;
            this.userId = row.userId;
            this.fkId = row.fkId;
            this.userName = row.userName;
            this.fkName = row.fkName;
        }

        /// <summary>
        /// 创建一个用户日志
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="userObj">关联的用户对象</param>
        /// <param name="code">用户日志操作类型标示字符串 建议4-6个字符 中文</param>
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

        /// <summary>
        /// 检索用户日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<UserLog> searchList(UserLogSearchRequest condtion) {
            BaseResponseList<UserLog> result = new BaseResponseList<UserLog>();
            DateTime? endDate = null;
            if (condtion.endDate != null)
            {
                endDate = DateTime.Parse(string.Format("{0} 23:59:59", condtion.endDate.Value.Date.ToString("yyyy-MM-dd")));
            }
            DateTime? beginDate = null;
            if (condtion.beginDate != null)
            {
                beginDate = condtion.beginDate.Value.Date;
            }
            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_BaseLogSet.OfType<Db_UserLog>()
                            where (1 == 1)
                            && (string.IsNullOrEmpty(condtion.userName) ? true : c.userName.Contains(condtion.userName))
                            && (beginDate == null ? true : c.createdOn >= beginDate)
                            && (endDate == null ? true : c.createdOn <= endDate)
                            && (string.IsNullOrEmpty(condtion.userId) ? true : c.userId == condtion.userId)
                            && (string.IsNullOrEmpty(condtion.fkId) ? true : c.fkId == condtion.fkId)
                            && (string.IsNullOrEmpty(condtion.q) ? true : (c.code == condtion.q || c.message.Contains(condtion.q)))
                            select c);
                result.total = rows.Count();
                if (result.total > 0 && condtion.getRows) {
                    rows = rows.OrderByDescending(p => p.createdOn);
                    if (condtion.page > 0) {
                        rows = rows.Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.AsEnumerable().Select(p => new UserLog(p)).ToList();
                }
            }
            return result;
        }
    }
}
