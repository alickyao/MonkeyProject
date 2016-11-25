using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.service
{
    /// <summary>
    /// 验证数据
    /// </summary>
    public class ValiDatas
    {
        /// <summary>
        /// 参数验证
        /// </summary>
        /// <param name="condtion">参数</param>
        /// <param name="throwException">是否直接抛出异常，默认为true</param>
        /// <returns></returns>
        public static BaseResponse<List<ValidationResult>> valiData(object condtion, bool throwException = true)
        {
            BaseResponse<List<ValidationResult>> result = new BaseResponse<List<ValidationResult>>();

            var context = new ValidationContext(condtion, null, null);

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(condtion, context, results, true);

            result.item = results;

            if (results != null && results.Count > 0)
            {
                if (throwException)
                {
                    throw new ValiDataException(results[0].ErrorMessage);
                }
                result.Message = "403";
                result.MessageDetail = results[0].ErrorMessage;
            }
            return result;
        }
    }
}
