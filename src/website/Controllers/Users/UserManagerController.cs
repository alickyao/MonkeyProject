
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
        /// [匿名访问]后台用户登录
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
            UserLog.create(string.Format("用户登录,IP地址：{0}，客户端：{1}", HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.UserAgent), "后台用户登录", user);

            return BaseResponse.getResult(user);
        }

        /// <summary>
        /// [匿名访问]后台用户退出登录
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
        /// [管理员角色权限]创建/编辑一个后台用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="Id">用户的ID 不填为编辑</param>
        /// <returns></returns>
        [Authorize(Roles ="admin")]
        [HttpPost]
        public BaseResponse<UserManager> editUser(UserManagerCreateRequest condtion,string Id = null)
        {
            var thisUser = UserManager.getUserById(User.Identity.Name);
            UserManager result = null;
            if (string.IsNullOrEmpty(Id))
            {
                //新增
                result = UserManager.create(condtion);
                //记录到日志
                //操作员
                UserLog.create(string.Format("创建用户[{0}]", result.getNameString()), "添加用户", thisUser);
                //被创建
                UserLog.create(string.Format("用户由[{0}]创建", thisUser.getNameString()), "添加用户", result);
            }
            else {
                //编辑
                result = UserManager.getUserById(Id);
                result.edit(new UserManagerEditRequest {
                    fullName = condtion.fullName,
                    mobilePhone = condtion.mobilePhone,
                    roleNames = condtion.roleNames
                });
                //操作员
                UserLog.create(string.Format("编辑用户信息[{0}]", result.getNameString()), "编辑用户信息", thisUser);
                //被编辑
                UserLog.create(string.Format("用户信息被[{0}]编辑", thisUser.getNameString()), "编辑用户信息", result);
            }
            return BaseResponse.getResult(result, "操作成功");
        }

        /// <summary>
        /// [后台角色权限]编辑自己的用户信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<UserManager> editMyUserInfo(UserManagerBaseEditRequest condtion) {
            UserManager user = UserManager.getUserById(User.Identity.Name);
            string logMsg = string.Format("用户编辑了自己的用户信息，原姓名[{0}]，新姓名[{1}]；原手机[{2}]，新手机[{3}]", user.fullName, condtion.fullName, user.mobilePhone, condtion.mobilePhone);
            user.editBaseInfo(condtion);
            //记录到日志
            UserLog.create(logMsg, "编辑用户信息", user);
            return BaseResponse.getResult(user, "保存成功");
        }

        /// <summary>
        /// [管理员角色权限]设置后台用户的禁用
        /// </summary>
        /// <param name="Id">被设置的用户的ID</param>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public BaseResponse<UserManager> setDisabled(string Id, UserManagerSetDisabledRequest condtion) {
            ValiDatas.valiData(condtion);
            var thisUser = UserManager.getUserById(User.Identity.Name);
            UserManager user = UserManager.getUserById(Id);
            user.saveDisabled(condtion.isDisabled);
            //记录到日志
            //操作员
            UserLog.create(string.Format("将用户[{0}]设置为{1}，备注：{2}", user.getNameString(), (condtion.isDisabled ? "已禁用" : "未禁用"), condtion.remark), "用户禁用设置", thisUser);
            //被编辑
            UserLog.create(string.Format("被用户[{0}]设置为{1}，备注：{2}", thisUser.getNameString(), (condtion.isDisabled ? "已禁用" : "未禁用"), condtion.remark), "用户禁用设置", user);
            return BaseResponse.getResult(user, "设置成功");
        }

        /// <summary>
        /// [管理员角色权限]将用户标记为删除状态
        /// </summary>
        /// <param name="Id">用户的ID</param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public BaseResponse deleteUser(string Id) {
            var thisUser = UserManager.getUserById(User.Identity.Name);
            UserManager user = UserManager.getUserById(Id);
            if (user.loginName == "admin") {
                throw new ValiDataException("不能删除系统默认的管理员");
            }
            user.delete();
            //记录到日志
            //操作员
            UserLog.create(string.Format("将用户[{0}]设置为已删除",user.getNameString()), "删除用户", thisUser);
            //被操作
            UserLog.create(string.Format("被用户[{0}]设置为已删除", thisUser.getNameString()), "删除用户", user);
            return BaseResponse.getResult("删除成功");
        }

        /// <summary>
        /// [管理员角色权限]将后台用户的登录密码恢复为初始设置
        /// </summary>
        /// <param name="Id">用户的ID</param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public BaseResponse reSetPwd(string Id) {
            var thisUser = UserManager.getUserById(User.Identity.Name);
            UserManager user = UserManager.getUserById(Id);
            user.resetPwd();
            //记录到日志
            //操作员
            UserLog.create(string.Format("将用户[{0}]的登录密码设置为系统默认值", user.getNameString()), "修改登录密码", thisUser);
            //被操作
            UserLog.create(string.Format("被用户[{0}]将登录密码重设为了系统初始值", thisUser.getNameString()), "修改登录密码", user);
            return BaseResponse.getResult("设置成功");
        }

        /// <summary>
        /// [后台角色权限]后台用户修改自己的登录密码
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse changePwd(UserChangePwdRequst condtion) {
            var user = UserManager.getUserById(User.Identity.Name);
            user.changePwd(condtion);
            UserLog.create("用户修改了登录密码", "修改登录密码", user);
            return BaseResponse.getResult("密码修改成功");
        }

        /// <summary>
        /// [后台角色权限]获取后台用户列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<BaseResponseList<UserManager>> searchUserList(UserManagerSearchRequest condtion) {
            var result = UserManager.searchList(condtion);
            return BaseResponse.getResult(result);
        }
    }
}
