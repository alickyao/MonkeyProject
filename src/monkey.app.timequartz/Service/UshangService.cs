using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.app.timequartz.Service
{
    /// <summary>
    /// U上商侣相关
    /// </summary>
    public class UshangService
    {
        /// <summary>
        /// 上报信息到U上商侣，可选是否发送短信给管理员
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sendSms"></param>
        public static void UploadNotice(string msg, bool sendSms) {
            try
            {
                string sendMsg = string.Format("msg={0}&sendSms={1}&key={2}", msg, sendSms.ToString().ToLower(), "ab123456ab");
                string result = SysHelp.HttpPost("https://api.iusung.com/api/app/ExceptionLog/CreateSysNoticeLog", sendMsg);
                SysLog.CreateTextLog(LogType.error, string.Format("上报信息至U上商侣，反馈结果为：[{0}]", result));
            }
            catch (Exception e) {
                SysLog.CreateTextLog(LogType.error, string.Format("上报信息至U上商侣，反馈结果为：[{0}]", e.Message));
            }
        }
    }
}
