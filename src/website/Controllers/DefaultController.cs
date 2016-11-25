using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace website.Controllers
{
    /// <summary>
    /// 默认跳转登录与错误界面
    /// </summary>
    public class DefaultController : Controller
    {
        /// <summary>
        /// 跳转登录
        /// </summary>
        /// <returns></returns>
        public ActionResult loginJump(string ReturnUrl)
        {
            if (ReturnUrl.IndexOf("/manager") != -1)
            {
                //后台
                return RedirectToAction("login", "Login", new { area = "Admin", ReturnUrl = ReturnUrl });
            }
            else {
                throw new Exception("出错了");
            }
        }


        /// <summary>
        /// 404错误
        /// </summary>
        /// <param name="aspxerrorpath">用户访问的路径</param>
        /// <returns></returns>
        public ActionResult error_404(string aspxerrorpath)
        {
            ViewBag.aspxerrorpath = aspxerrorpath;
            return View();
        }

        /// <summary>
        /// 500错误
        /// </summary>
        /// <param name="aspxerrorpath">用户访问的路径</param>
        /// <returns></returns>
        public ActionResult error_500(string aspxerrorpath)
        {
            ViewBag.aspxerrorpath = aspxerrorpath;
            return View();
        }

    }
}