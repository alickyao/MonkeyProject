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
    /// 基础文档
    /// </summary>
    public class DocController : ApiController
    {
        /// <summary>
        /// [后台角色权限]删除一个文档（标记删除）
        /// </summary>
        /// <param name="id">文档的ID</param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse DelDoc(string id) {
            return BaseResponse.getResult();
        }
    }
}
