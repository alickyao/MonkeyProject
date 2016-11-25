using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using System.ComponentModel.DataAnnotations;
using monkey.service.Logs;

namespace monkey.service.Users
{
    /// <summary>
    /// 创建后台用户请求
    /// </summary>
    public class UserManagerCreateRequest{
        /// <summary>
        /// 登录名
        /// </summary>
        [Required]
        [StringLength(50,MinimumLength =5)]
        public string loginName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string fullName { get; set; }

        /// <summary>
        /// 用户的角色
        /// </summary>
        [Required]
        public string[] roleNames { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobilePhone { get; set; }
    }

    /// <summary>
    /// 后台用户登录请求
    /// </summary>
    public class UserManagerLoginRequest {

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
    /// 后台用户
    /// </summary>
    public class UserManager: UserBase, ILogStringable
    {

        #region -- 属性

        /// <summary>
        /// 登录名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string loginName { get; set; }

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

        #endregion

        /// <summary>
        /// 根据ID获取用户详情
        /// </summary>
        /// <param name="id"></param>
        public static UserManager getUserById(string id) {
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseUserSet.OfType<Db_ManagerUser>().SingleOrDefault(p => p.Id == id);
                if (row == null) {
                    throw new DataNotFundException(string.Format("传入的ID：{0}，未能找到匹配的用户", id));
                }
                return new UserManager(row);
            }
        }

        public UserManager(Db_ManagerUser row) : base(row) {
            setValue(row);
        }

        private void setValue(Db_ManagerUser row)
        {
            this.loginName = row.loginName;
            this.fullName = row.fullName;
            this.mobilePhone = row.mobilePhone;
        }

        /// <summary>
        /// 检查登录名是否已使用
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static int getLoginNameCount(string loginName) {
            using (var db = new DefaultContainer()) {
                var count = (from c in db.Db_BaseUserSet.OfType<Db_ManagerUser>()
                             where c.loginName == loginName
                             select (c.Id)).Count();
                return count;
            }
        }


        private static object locker = new object();
        /// <summary>
        /// 创建一个后台用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static UserManager create(UserManagerCreateRequest condtion) {
            ValiDatas.valiData(condtion);

            lock (locker) {

                //验证登录名是否已存在
                var count = getLoginNameCount(condtion.loginName);
                if (count > 0)
                {
                    throw new ValiDataException("该登录名已存在，创建失败");
                }
                string newId = Guid.NewGuid().ToString();
                Db_ManagerUser user = new Db_ManagerUser()
                {
                    createdOn = DateTime.Now,
                    fullName = condtion.fullName,
                    loginName = condtion.loginName,
                    mobilePhone = condtion.mobilePhone,
                    passWord = SysSetingsHelp.DefaultAdminPwd,
                    Id = newId,
                    roleNames = string.Join(",", condtion.roleNames)
                };
                using (var db = new DefaultContainer())
                {
                    var newrow = db.Db_BaseUserSet.Add(user);
                    db.SaveChanges();
                    return getUserById(newId);
                }
            }
        }

        /// <summary>
        /// 验证用户名和密码是否正确
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns>验证通过返回后台用户对象 验证不通过直接抛出DataNotFundException异常</returns>
        public static UserManager checkLogin(UserManagerLoginRequest condtion) {
            ValiDatas.valiData(condtion);
            using (var db = new DefaultContainer()) {
                //判断数据库是否为空，如果为空则添加一个管理员用户
                var count = (from c in db.Db_BaseUserSet.OfType<Db_ManagerUser>()
                             select c.Id).Count();

                if (count == 0) {
                    return UserManager.create(new UserManagerCreateRequest()
                    {
                        fullName = "管理员",
                        loginName = "admin",
                        roleNames = new string[] { "admin" }
                    });
                }

                var row = db.Db_BaseUserSet.OfType<Db_ManagerUser>().SingleOrDefault(p => 
                p.loginName == condtion.loginName 
                && p.passWord.ToLower() == condtion.passWord.ToLower()
                && p.isDeleted == false
                );
                if (row == null) {
                    throw new DataNotFundException("用户名或者密码错误");
                }
                if (row.isDisabled) {
                    throw new ValiDataException("已被禁用的用户无法登录");
                }
                return new UserManager(row);
            }
        }

        public string getIdString()
        {
            return this.Id;
        }

        public string getNameString()
        {
            return string.Format("{0}[{1}]", this.fullName, this.rolesString);
        }
    }
}
