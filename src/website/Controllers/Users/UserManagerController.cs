
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using System.Web;
using monkey.service;
using monkey.service.Logs;
using monkey.service.Users;


namespace website.Controllers.Users
{
    /// <summary>
    /// 后台用户相关
    /// </summary>
    public class UserManagerController : ApiController
    {
        /// <summary>
        /// 后台用户登录
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<UserManager> login(UserManagerLoginRequest condtion) {
            var user = UserManager.checkLogin(condtion);

            //建立身份验证票对象
            FormsAuthenticationTicket Ticket = new FormsAuthenticationTicket(
                1, 
                user.Id,                     //用户标示  这里存的用户的ID
                DateTime.Now,                //Cookie   的发出时间, 
                DateTime.Now.AddMinutes(30), //过期时间 
                false,                       //是否持久性(根据需要设置,若是设置为持久性,在发出  cookie时, cookie的Expires设置一定要设置
                user.rolesString,            // 用户自定义的一些信息  这里存放了用户的角色信息 
                "/"                          // cookie存放路径
            );

            string HashTicket = FormsAuthentication.Encrypt(Ticket);   //加密序列化验证票为字符串  
            HttpCookie UserCookie = new HttpCookie(FormsAuthentication.FormsCookieName, HashTicket);
            //生成Cookie  
            HttpContext.Current.Response.Cookies.Add(UserCookie); //输出Cookie  

            user.updateLastLoginInfo(HttpContext.Current.Request.UserHostAddress);//更新最后一次登录时间

            //记录到日志
            UserLog.create(string.Format("用户登录,IP地址：{0}，客户端：{1}", HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.UserAgent), "login", user);

            return BaseResponse.getResult(user);
        }

        /// <summary>
        /// 后台用户退出登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse logout() {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                return BaseResponse.getResult("退出登录成功");
            }
            else {
                return BaseResponse.getResult("您还没有登录");
            }
        }

        /// <summary>
        /// 创建一个后台用户(管理员角色可调用)
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles ="admin")]
        [HttpPost]
        public UserManager create(UserManagerCreateRequest condtion)
        {
            return UserManager.create(condtion);
        }
        
        /// <summary>
        /// 获取后台用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpGet]
        public BaseResponse<UserManager> getInfo(string id) {
            var item = UserManager.getUserById(id);
            return BaseResponse.getResult(item);
        }
    }
}
