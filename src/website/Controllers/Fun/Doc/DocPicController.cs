using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using monkey.service;
using monkey.service.Logs;
using monkey.service.Users;
using monkey.service.Fun.Doc;

namespace website.Controllers.Fun.Doc
{
    /// <summary>
    /// 图文内容 - 继承自 基础文档
    /// </summary>
    public class DocPicController : ApiController
    {
        /// <summary>
        /// 创建/编辑 图文消息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public BaseResponse EditDocPic(DocPicEditReqeust info) {
            var result = DocPic.CreateDocPic(info);
            return BaseResponse.getResult(result);
        }
    }
}
