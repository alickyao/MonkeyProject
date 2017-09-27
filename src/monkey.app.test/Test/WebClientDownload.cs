using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace monkey.app.test.Test
{
    class WebClientDownload
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        public static void DownloadFile() {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile("https://adm.iusung.com/UploadImg/test.txt", "D:\\test.txt");
                Console.WriteLine("下载已完成");
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

        }
    }
}
