using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using monkey.service;
using monkey.service.Users;
using monkey.service.Logs;

namespace website.Controllers.Tree
{
    /// <summary>
    /// 基础树服务
    /// </summary>
    public class BaseTreeController : ApiController
    {
        /// <summary>
        /// [后台角色权限]获取所有树的列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<BaseTree>> GetBaseTreeRootsList() {
            BaseResponseList<BaseTree> result = new BaseResponseList<BaseTree>();
            result.rows = BaseTree.GetBaseTreeRoots();
            if (result.rows.Count > 0) {
                foreach (var row in result.rows) {
                    row.GetBaseTreeChilderns();
                }
            }
            result.total = result.rows.Count;
            return BaseResponse.getResult(result);
        }


        /// <summary>
        /// [匿名访问]根据树节点的Code获取树的详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<BaseTree> GetBaseTreeByCode(string code) {
            var info = BaseTree.GetBaseTreeByCode(code);
            info.GetBaseTreeChilderns();
            return BaseResponse.getResult(info);
        }


        /// <summary>
        /// [匿名访问]根据树节点的Id获取树的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<BaseTree> GetBaseTreeById(string id)
        {
            var info = BaseTree.GetBaseTreeById(id);
            info.GetBaseTreeChilderns();
            return BaseResponse.getResult(info);
        }

        /// <summary>
        /// [后台角色权限]新增、编辑基本树
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseTree> EditBaseTree(BaseTreeAttr condtion) {
            BaseTree t;
            var thisUser = UserManager.getUserById(User.Identity.Name);
            string msg = "{0}基本树项目，[{1}]";
            if (string.IsNullOrEmpty(condtion.id))
            {
                //新增
                t  = BaseTree.CreateBaseTree(condtion);

                //日志
                msg = string.Format(msg, "新增", condtion.text);
                UserLog.create(msg, "基本树维护", thisUser, t);
                
                return BaseResponse.getResult(t, "新增成功");
            }
            else {
                //编辑
                var info = BaseTree.GetBaseTreeById(condtion.id);
                t = info.EditBaseTree(condtion);

                //日志
                msg = string.Format(msg, "编辑", condtion.text);
                UserLog.create(msg, "基本树维护", thisUser, t);

                return BaseResponse.getResult(info, "保存成功");
            }
        }

        /// <summary>
        /// [后台角色权限]删除分类树（物理删除包含其子节点）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse DelBaseTree(string id) {

            var thisUser = UserManager.getUserById(User.Identity.Name);
            var info = BaseTree.GetBaseTreeById(id);
            info.DelBaseTree();
            UserLog.create(string.Format("物理删除基本树项目：{0}",info.text), "基本树维护", thisUser);
            return BaseResponse.getResult("删除成功");
        }
    }
}
