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
    }
}
