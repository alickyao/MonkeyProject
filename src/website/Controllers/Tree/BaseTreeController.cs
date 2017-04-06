using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using monkey.service;
using monkey.service.Users;

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
        public BaseResponseList<BaseTree> GetBaseTreeRootsList() {
            BaseResponseList<BaseTree> result = new BaseResponseList<BaseTree>();
            result.rows = BaseTree.GetBaseTreeRoots();
            if (result.rows.Count > 0) {
                foreach (var row in result.rows) {
                    row.GetBaseTreeChilderns();
                }
            }
            result.total = result.rows.Count;
            return result;
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
            if (string.IsNullOrEmpty(condtion.id))
            {
                //新增
                var info  = BaseTree.CreateBaseTree(condtion);
                return BaseResponse.getResult(info, "新增成功");
            }
            else {
                //编辑
                var info = BaseTree.GetBaseTreeById(condtion.id);
                var result = info.EditBaseTree(condtion);
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
            var info = BaseTree.GetBaseTreeById(id);
            info.DelBaseTree();
            return BaseResponse.getResult("删除成功");
        }
    }
}
