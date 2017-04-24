using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using monkey.service;
using monkey.service.Logs;
using System.Threading;

namespace website.Controllers.SysBase
{
    /// <summary>
    /// 自动刷新
    /// </summary>
    public class TimerController : ApiController
    {
        /// <summary>
        /// 触发系统自动刷新
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse DoScan() {
            DateTime beginScanTime = DateTime.Now;
            Thread.Sleep(100);

            #region -- 详细的执行逻辑



            #endregion

            var usedTime = (DateTime.Now - beginScanTime).Milliseconds;
            string msg = string.Format("定时触发已执行完毕耗时： {0} ms", usedTime);
            BaseLog.create(msg);
            return BaseResponse.getResult(msg);
        }
    }
}
