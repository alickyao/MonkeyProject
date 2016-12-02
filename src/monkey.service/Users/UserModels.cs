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
}
