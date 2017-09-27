using System;
using System.Net;
using System.IO;

namespace monkey.app.timequartz.Service
{
    /// <summary>
    /// U上商侣 图片下载检查
    /// </summary>
    public class UsungUploadImgDirCheck: IThreading
    {

        const string fileName = "/UploadImg/test.txt";

        /// <summary>
        /// 下载域名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 保存的文件名的标识符
        /// </summary>
        public string SaveFileName { get; set; }

        /// <summary>
        /// U上商侣 图片下载检查
        /// </summary>
        /// <param name="host"></param>
        /// <param name="saveFileName"></param>
        public UsungUploadImgDirCheck(string host,string saveFileName) {
            this.Host = host;
            this.SaveFileName = saveFileName;
        }

        /// <summary>
        /// 是否发送错误报告
        /// </summary>
        public static bool IsSendMsg
        {
            get; set;
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void Run()
        {
            try
            {
                WebClient webClient = new WebClient();
                string fileUrl = this.Host + fileName;
                string savePath = "D:\\OfficeLog\\temp";
                DirectoryInfo sd = new DirectoryInfo(savePath);
                if (!sd.Exists) {
                    sd.Create();
                }
                string filePath = savePath + "\\test_"+ this.SaveFileName +".txt";
                FileInfo file = new FileInfo(filePath);
                if (file.Exists) {
                    file.Delete();
                }
                webClient.DownloadFile(fileUrl, filePath);
                SysLog.CreateTextLog(LogType.runing, string.Format("Download file from [{0}] is success", fileUrl));
                if (IsSendMsg) {
                    //曾经发送过错误报告，发送已修复报告
                    UshangService.UploadNotice(string.Format("U上商侣图片已经可正常通过 {0} 进行访问",this.Host), true);
                }
                IsSendMsg = false;
            }
            catch (Exception e) {
                //记录到错误日志
                SysLog.CreateTextLog(LogType.error, string.Format("Download file from [{0}] is fail,Error message is [{1}]", this.Host, e.Message));
                if (!IsSendMsg) {
                    //没有发送错误报告-立即发送
                    IsSendMsg = true;
                    UshangService.UploadNotice(string.Format("发现U上商侣图片无法从 {0} 进行访问，错误提示：{1}", this.Host, e.Message
                        ), true);
                }
            }
        }
    }
}
