using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace monkey.TTP.sms
{
    /// <summary>
    /// 发送短信请求
    /// </summary>
    public class SendSmsRequest
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        public string[] phones { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Required]
        [StringLength(140)]
        public string meg { get; set; }
    }
    /// <summary>
    /// 漫道短息平台
    /// </summary>
    public class MandaoSmsApi
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phones">多个手机用,隔开</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        public static string send(string phones , string msg) {
            return "";
        }
    }
}
