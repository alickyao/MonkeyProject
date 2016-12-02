using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
