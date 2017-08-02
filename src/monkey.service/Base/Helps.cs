using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;

namespace monkey.service
{
    /// <summary>
    /// 通用组合
    /// </summary>
    public class ComboBox
    {
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        public string text { get; set; }
    }

    /// <summary>
    /// 常用帮助方法集合
    /// </summary>
    public class SysHelps
    {
        private static char[] code = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        private static Random random = new Random();

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="c">获取随机字符串的数量</param>
        /// <returns></returns>
        public static string getRandmonStirng(int c = 4)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < c; i++)
            {
                sb.Append(code[random.Next(code.Length - 1)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取一个guid()  "-"已被替换为""
        /// </summary>
        /// <returns></returns>
        public static string GetNewId() {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// 获取枚举的详细可选项列表
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <returns></returns>
        public static List<ComboBox> getEnumTypeList(Type t)
        {
            var values = Enum.GetValues(t);
            List<ComboBox> result = new List<ComboBox>();
            foreach (var value in values)
            {
                result.Add(new ComboBox()
                {
                    value = Convert.ToInt32(value).ToString(),
                    text = value.ToString()
                });
            }
            return result;
        }


        /// <summary>
        /// 返回与当前时间之间的间隔 分钟 小时 天 超过三天则显示 月日
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string get2TimeShowString(DateTime d)
        {
            var s = DateTime.Now - d;
            StringBuilder sb = new StringBuilder();
            if (s.TotalMinutes < 0)
            {
                sb.Append("刚刚");
            }
            else if (s.TotalMinutes < 60)
            {
                sb.AppendFormat("{0}分钟前", s.TotalMinutes.ToString("0"));
            }
            else if (s.TotalHours < 24)
            {
                sb.AppendFormat("{0}小时前", s.TotalHours.ToString("0"));
            }
            else if (s.TotalDays < 4)
            {
                sb.AppendFormat("{0}天前", s.TotalDays.ToString("0"));
            }
            else {
                sb.Append(d.ToShortDateString());
            }
            return sb.ToString();
        }


        /// <summary>
        /// 发送HttpPost请求
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            //request.CookieContainer = cookie;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //response.Cookies = cookie.GetCookies(response.ResponseUri);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }


        /// <summary>
        /// 发送HttpGet请求
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpGet(string Url, string postDataStr = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (string.IsNullOrEmpty(postDataStr) ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// 检查数据库连接是否正常
        /// </summary>
        /// <returns></returns>
        public static BaseResponse CheckDb() {
            BaseResponse result = new BaseResponse();
            try
            {
                using (var db = new DefaultContainer()) {
                    if (db.Database.Exists())
                    {
                        return BaseResponse.getResult("数据库连接正常");
                    }
                    else {
                        result.Message = "500";
                        result.MessageDetail = "数据库连接失败";
                    }
                }
            }
            catch (Exception e) {
                result.Message = "500";
                result.MessageDetail = e.Message;
            }
            return result;
        }
    }
}
