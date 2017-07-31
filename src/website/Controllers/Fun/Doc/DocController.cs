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
            UserManager user = UserManager.getUserById(User.Identity.Name);
            var docInfo = new BaseDoc(id);
            docInfo.SelDel();
            UserLog.create(string.Format("将文档[{0},{1}]标记为删除",docInfo.Caption,docInfo.DocTypeString), "文档管理", user, docInfo);
            return BaseResponse.getResult("删除成功");
        }

        /// <summary>
        /// [后台角色权限]切换禁用状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseDoc> SetDisable(string id) {
            UserManager user = UserManager.getUserById(User.Identity.Name);
            var docInfo = new BaseDoc(id);
            var result = docInfo.SetDisable();

            UserLog.create(string.Format("将文档[{0},{1}]设置为：{2}", result.Caption, result.DocTypeString, result.IsDisabled ? "禁用" : "未禁用"), "文档管理", user, docInfo);

            return BaseResponse.getResult(result, "设置成功");
        }


        /// <summary>
        /// [后台角色权限]更新文档图片集信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse UpdateDocImgFiles(BaseDocImgFilesUploadRequest info) {
            UserManager user = UserManager.getUserById(User.Identity.Name);
            var docInfo = new BaseDoc(info.DocId);
            docInfo.UpdatImgFileList(info.FilesList);
            UserLog.create("编辑图集", "文档管理", user, docInfo);
            return BaseResponse.getResult("保存成功");
        }

        /// <summary>
        /// [后台角色权限]检索全部文档
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<BaseDocDetailList>> SearchBaseDocList(BaseDocSerarchRequest condtion) {
            var result = BaseDocDetailList.SearchBaseDocList(condtion);
            return BaseResponse.getResult(result);
        }
    }
}
