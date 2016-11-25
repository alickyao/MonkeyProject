using System.Configuration;

namespace monkey.service
{
    /// <summary>
    /// 项目信息
    /// </summary>
    public class ProjectInfo {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        public string describe { get; set; }
    }

    /// <summary>
    /// 系统设置帮助
    /// </summary>
    public class SysSetingsHelp
    {
        /// <summary>
        /// 项目信息
        /// </summary>
        public static ProjectInfo ProjectInfo {
            get {
                return new ProjectInfo()
                {
                    name = ConfigurationManager.AppSettings["projectName"].ToString(),
                    describe = ConfigurationManager.AppSettings["projectDescribe"].ToString()
                };
            }
        }
        /// <summary>
        /// 新增时默认的后台用户密码
        /// </summary>
        public static string DefaultAdminPwd {
            get
            {
                return ConfigurationManager.AppSettings["defaultAdminPassword"].ToString();
            }
        }
    }
}
