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
        /// [后台角色权限]创建/编辑 图文消息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse EditDocPic(DocPicEditReqeust info) {
            UserManager user = UserManager.getUserById(User.Identity.Name);
            if (string.IsNullOrEmpty(info.Id))
            {
                var result = DocPic.CreateDocPic(info);
                UserLog.create("新增图文集", "图文集", user, result);
                return BaseResponse.getResult(result,"新增成功");
            }
            else {
                var docPicInfo = new DocPic(info.Id);
                var result = docPicInfo.EditDocPic(info);
                UserLog.create("编辑图文集信息", "图文集", user, result);
                return BaseResponse.getResult(result,"保存成功");
            }
        }
    }
}
