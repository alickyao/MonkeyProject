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
        /// [匿名访问]触发系统自动刷新
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse DoScan() {


            #region -- 详细的执行逻辑

            List<IThreading> runList = new List<IThreading>();
            runList.Add(new TRuningLogToDataBase());

            foreach (var item in runList) {
                Thread t = new Thread(new ThreadStart(item.Run));
                t.Start();
            }

            #endregion

            return BaseResponse.getResult();
        }
    }

    /// <summary>
    /// 写入到系统日志
    /// </summary>
    public class TRuningLogToDataBase : IThreading
    {
        /// <summary>
        /// 执行
        /// </summary>
        public void Run()
        {
            BaseLog.create("后台定时触发已开始执行");
        }
    }
}
