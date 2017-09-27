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
            runList.Add(new UsungUploadImgDirCheck("https://api.iusung.com", "from_api"));
            runList.Add(new UsungUploadImgDirCheck2("https://adm.iusung.com", "from_adm"));
            foreach (var r in runList)
            {
                Thread t = new Thread(new ThreadStart(r.Run));
                t.Start();
            }
        }
    }
}
