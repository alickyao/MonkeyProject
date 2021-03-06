﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using monkey.service;
using monkey.service.Users;
using monkey.service.Fun.Doc;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// 文档内容维护
    /// </summary>
    public class DocController : BaseController
    {
        /// <summary>
        /// 所有文档列表
        /// </summary>
        /// <param name="treeId"></param>
        /// <param name="pageId"></param>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult BaseDocList(string treeId, string pageId, BaseRequest condtion)
        {
            ViewBag.treeId = treeId;
            ViewBag.pageId = getPageId(pageId);
            if (condtion == null) {
                condtion = new BaseRequest();
            }
            return View(condtion);
        }

        /// <summary>
        /// 管理文档的图片
        /// </summary>
        /// <param name="id">文档的ID</param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult DocImgFiles(string id, string pageId) {
            ViewBag.pageId = getPageId(pageId);
            var info = new BaseDoc(id);
            return View(info);
        }

        /// <summary>
        /// 显示一张图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult ShowDocImgFile(BaseDocImgFile file) {
            return View(file);
        }

        /// <summary>
        /// 图文文档编辑界面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="treeId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult EditDocPic(string id,string treeId, string pageId) {
            DocPic info = new DocPic();
            if (!string.IsNullOrEmpty(id)) {
                info = new DocPic(id);
            }
            ViewBag.treeId = treeId;
            ViewBag.pageId = getPageId(pageId);
            return View(info);
        }
    }
}