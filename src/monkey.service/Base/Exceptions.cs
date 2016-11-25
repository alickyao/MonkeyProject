using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.service
{
    /// <summary>
    /// 提交了为空的请求异常
    /// </summary>
    public class RequestIsNullException : ApplicationException {
        public RequestIsNullException(string message) : base(message) { }
    }
    /// <summary>
    /// 数据验证异常
    /// </summary>
    public class ValiDataException: ApplicationException
    {
        public ValiDataException(string message) : base(message) { }
    }
    /// <summary>
    /// 未能通过指定的ID或其他条件找到特定的数据时 抛出的异常
    /// </summary>
    public class DataNotFundException : ApplicationException {
        public DataNotFundException(string message) : base(message) { }
    }
}
