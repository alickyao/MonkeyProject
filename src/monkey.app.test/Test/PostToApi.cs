using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace monkey.app.test.Test
{
    public class PostToApi
    {
        public static void test() {
            //QrScCreateBoxInfoRequest2 request = new QrScCreateBoxInfoRequest2()
            //{
            //    Br = new List<string>(),
            //    Qr = new List<string>()
            //};
            //request.Br.Add("123");
            //request.Br.Add("456");
            //for (int i = 0; i < 100; i++)
            //{
            //    request.Qr.Add("qr" + i.ToString().PadLeft(4, '0'));
            //}
            //string postData = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            //string result = HttpPost("https://n.iusung.com:4431/api/App/ScQrCode/UploadBoxInfoByJson", postData);
        }
    }

    /// <summary>
    /// 创建烟箱与内部烟条的关系请求 两列模式
    /// </summary>
    public class QrScCreateBoxInfoRequest2
    {
        /// <summary>
        /// 箱号列表
        /// </summary>
        public List<string> Br { get; set; }
        /// <summary>
        /// 二维码列表
        /// </summary>
        public List<string> Qr { get; set; }
    }
}
