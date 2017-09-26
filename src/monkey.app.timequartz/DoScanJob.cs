using monkey.app.timequartz.Service;
using System.Collections.Generic;
using System.Threading;

namespace monkey.app.timequartz
{
    /// <summary>
    /// 执行
    /// </summary>
    public class DoScanJob
    {
        /// <summary>
        /// 被重复执行的方法
        /// </summary>
        public void DoJob()
        {
            //SysHelp.HttpGet("http://127.0.0.1:8701/api/Timer/DoScan");
            List<IThreading> runList = new List<IThreading>();
            runList.Add(new PingTelSwitch());
            foreach (var r in runList)
            {
                Thread t = new Thread(new ThreadStart(r.Run));
                t.Start();
            }
        }
    }
}
