using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.service.Users
{
    /// <summary>
    /// 用户修改密码请求
    /// </summary>
    public class UserChangePwdRequst
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "旧密码需要md5 32位加密")]
        public string oldPwd { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "新密码需要md5 32位加密")]
        public string newPwd { get; set; }
    }

    /// <summary>
    /// 编辑后台用户信息基础请求对象
    /// </summary>
    public class UserManagerBaseEditRequest {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string fullName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobilePhone { get; set; }
    }

    /// <summary>
    /// 编辑后台用户信息请求对象
    /// </summary>
    public class UserManagerEditRequest: UserManagerBaseEditRequest
    {
        /// <summary>
        /// 用户的角色
        /// </summary>
        [Required]
        public string[] roleNames { get; set; }
    }

    /// <summary>
    /// 新增后台用户请求对象
    /// </summary>
    public class UserManagerCreateRequest: UserManagerEditRequest
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string loginName { get; set; }
    }

    /// <summary>
    /// 后台用户登录请求
    /// </summary>
    public class UserManagerLoginRequest
    {

        /// <summary>
        /// 登录名
        /// </summary>
        [Required]
        public string loginName { get; set; }

        /// <summary>
        /// 密码 已通过MD5加密后的字符串  不区分大小写
        /// </summary>
        [Required]
        public string passWord { get; set; }
    }

    /// <summary>
    /// 检索后台用户请求
    /// </summary>
    public class UserManagerSearchRequest : BaseRequest {

        /// <summary>
        /// 用户 登录名/姓名/手机号
        /// </summary>
        public string q { get; set; }

        /// <summary>
        /// 用户 角色
        /// </summary>
        public string role { get; set; }
    }

    /// <summary>
    /// 禁用设置请求
    /// </summary>
    public class UserManagerSetDisabledRequest {
        /// <summary>
        /// 用户设置禁用 true=已禁用 false=未禁用
        /// </summary>
        public bool isDisabled { get; set; }
        /// <summary>
        /// 备注文本信息
        /// </summary>
        [Required]
        public string remark { get; set; }
    }
}
