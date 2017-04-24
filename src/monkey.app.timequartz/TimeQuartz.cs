
using System;
using System.ServiceProcess;
using System.Threading;

namespace monkey.app.timequartz
{
    public partial class TimeQuartz : ServiceBase
    {
        public TimeQuartz()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 定时器
        /// </summary>
        private static Timer tim { get; set; }

        /// <summary>
        /// 执行间隔 (分钟)
        /// </summary>
        const int mint = 1;


        /// <summary>
        /// 服务启动
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            TimerCallback callBack = new TimerCallback(checkStatus);
            DoScanJob doScan = new DoScanJob();
            tim = new Timer(checkStatus, doScan, 0, mint * 60000);
        }

        /// <summary>
        /// 服务停止
        /// </summary>
        protected override void OnStop()
        {
            if (tim != null)
            {
                tim.Dispose();
                tim = null;
            }
        }

        private void checkStatus(object state)
        {
            DoScanJob s = (DoScanJob)state;
            s.DoJob();
        }
    }
}
