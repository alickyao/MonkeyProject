using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using monkey.service;
using monkey.service.Users;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// 后台用户管理相关
    /// </summary>
    public class UserManagerController : BaseController
    {

        /// <summary>
        /// 用户信息展示界面
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult userInfo()
        {
            var user = UserManager.getUserById(User.Identity.Name);
            return View(user);
        }

        /// <summary>
        /// 创建/编辑用户界面
        /// </summary>
        /// <param name="condtion">新增/编辑对象</param>
        /// <param name="Id">用户的ID</param>
        /// <param name="pageId">页面ID</param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public ActionResult editUser(UserManagerCreateRequest condtion, string Id = null, string pageId = null)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                UserManager user = UserManager.getUserById(Id);
                condtion.fullName = user.fullName;
                condtion.mobilePhone = user.mobilePhone;
                condtion.roleNames = user.rolesList.ToArray();
            }
            ViewBag.Id = Id;
            ViewBag.pageId = getPageId(pageId);
            return View(condtion);
        }

        /// <summary>
        /// 后台用户列表网格
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult userList(UserManagerSearchRequest condtion, string pageId) {
            ViewBag.pageId = getPageId(pageId);
            return View(condtion);
        }
    }
}