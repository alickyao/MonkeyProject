using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.app.timequartz
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 正常运行
        /// </summary>
        runing,
        /// <summary>
        /// 错误日志
        /// </summary>
        error,
        /// <summary>
        /// 警告
        /// </summary>
        warning
    }

    /// <summary>
    /// 系统日志
    /// </summary>
    public class SysLog
    {
        /// <summary>
        /// 保存路径
        /// </summary>
        const string logdir = "d:\\OfficeLog\\";

        public static object locker = new object();

        public static void CreateTextLog(LogType type, string msg)
        {
            lock (locker) {
                string dir = string.Format("{0}{1}\\{2}", logdir, type.ToString(), DateTime.Now.ToString("yyyyMM"));
                DirectoryInfo d = new DirectoryInfo(dir);
                if (!d.Exists)
                {
                    d.Create();
                }
                string fileName = string.Format("{0}\\{1}.txt", dir, DateTime.Now.ToString("MMdd"));
                FileInfo finfo = new FileInfo(fileName);
                using (FileStream fs = finfo.OpenWrite())
                {
                    StreamWriter w = new StreamWriter(fs);
                    w.BaseStream.Seek(0, SeekOrigin.End);
                    w.Write("\r\n ");
                    w.Write("{0} {1} \r\n ", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    w.Write(msg + " \r\n ");
                    w.Write("------------------------------------ \r\n ");
                    //清空缓冲区内容，并把缓冲区内容写入基础流 
                    w.Flush();
                    //关闭写数据流 
                    w.Close();
                }
            }
        }
    }
}
