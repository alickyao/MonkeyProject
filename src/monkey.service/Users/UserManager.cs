using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using System.ComponentModel.DataAnnotations;
using monkey.service.Logs;
using monkey.service.Frame;

namespace monkey.service.Users
{
    /// <summary>
    /// 可进行通信的 用户
    /// </summary>
    public interface ICommunicationable : ILogStringable {
        /// <summary>
        /// 获取姓名
        /// </summary>
        /// <returns></returns>
        string getFullNameString();
        /// <summary>
        /// 获取手机号
        /// </summary>
        /// <returns></returns>
        string getMobilePhone();
        /// <summary>
        /// 获取邮件地址
        /// </summary>
        /// <returns></returns>
        string getEmail();
    }

    /// <summary>
    /// 后台用户
    /// </summary>
    public class UserManager : UserBase, ICommunicationable
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

        /// <summary>
        /// 空构造
        /// </summary>
        public UserManager() { }

        /// <summary>
        /// 数据库构造
        /// </summary>
        /// <param name="row"></param>
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

        /// <summary>
        /// 获取系统管理员
        /// </summary>
        /// <returns></returns>
        public static UserManager getSysAdminUser() {
            return new UserManager()
            {
                Id = System.Configuration.ConfigurationManager.AppSettings["defaultAdminId"].ToString(),
                fullName = "系统",
                rolesString = "admin"
            };
        }

        #region -- 编辑/创建

        /// <summary>
        /// 创建一个后台用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static UserManager create(UserManagerCreateRequest condtion)
        {
            ValiDatas.valiData(condtion);

            lock (locker)
            {

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
                };
                //角色
                foreach (var role in condtion.roleNames)
                {
                    user.Db_BaseUserRole.Add(new Db_BaseUserRole()
                    {
                        Id = Guid.NewGuid().ToString(),
                        roleName = role,
                    });
                }
                using (var db = new DefaultContainer())
                {
                    var newrow = db.Db_BaseUserSet.Add(user);
                    db.SaveChanges();
                    return getUserById(newId);
                }
            }
        }

        /// <summary>
        /// 编辑用户信息 信息与角色
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public UserManager edit(UserManagerEditRequest condtion) {
            ValiDatas.valiData(condtion);
            this.fullName = condtion.fullName;
            this.mobilePhone = condtion.mobilePhone;
            this.rolesList = condtion.roleNames.ToList();
            save();
            return this;
        }

        /// <summary>
        /// 更新数据库的信息 可更新【姓名，手机与角色】
        /// </summary>
        private void save() {
            using (var db = new DefaultContainer()) {
                var dbuser = db.Db_BaseUserSet.OfType<Db_ManagerUser>().Single(p => p.Id == this.Id);
                db.Db_BaseUserRoleSet.RemoveRange(dbuser.Db_BaseUserRole);
                dbuser.fullName = this.fullName;
                dbuser.mobilePhone = this.mobilePhone;
                foreach (var role in this.rolesList) {
                    dbuser.Db_BaseUserRole.Add(new Db_BaseUserRole()
                    {
                        Id = Guid.NewGuid().ToString(),
                        roleName = role
                    });
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 编辑基础信息  -  姓名与手机号
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public UserManager editBaseInfo(UserManagerBaseEditRequest condtion) {
            ValiDatas.valiData(condtion);
            this.fullName = condtion.fullName;
            this.mobilePhone = condtion.mobilePhone;
            saveBaseInfo();
            return this;
        }

        /// <summary>
        /// 保存基本信息-姓名与手机号
        /// </summary>
        private void saveBaseInfo() {
            using (var db = new DefaultContainer())
            {
                var dbuser = db.Db_BaseUserSet.OfType<Db_ManagerUser>().Single(p => p.Id == this.Id);
                dbuser.fullName = this.fullName;
                dbuser.mobilePhone = this.mobilePhone;
                db.SaveChanges();
            }
        }

        #endregion

        #region -- 密码

        /// <summary>
        /// 修改用户的密码
        /// </summary>
        /// <param name="condtion"></param>
        public void changePwd(UserChangePwdRequst condtion) {
            ValiDatas.valiData(condtion);
            using (var db = new DefaultContainer()) {
                var dbuser = db.Db_BaseUserSet.OfType<Db_ManagerUser>().Single(p => p.Id == this.Id);
                if (dbuser.passWord.ToLower() != condtion.oldPwd.ToLower()) {
                    throw new ValiDataException("您提供的旧密码错误");
                }
                dbuser.passWord = condtion.newPwd;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 重设用户的登录密码 -- 使用配置文件中的初始登录密码
        /// </summary>
        public void resetPwd() {
            using (var db = new DefaultContainer()) {
                var dbuser = db.Db_BaseUserSet.OfType<Db_ManagerUser>().Single(p => p.Id == this.Id);
                dbuser.passWord = SysSetingsHelp.DefaultAdminPwd;
                db.SaveChanges();
            }
        }

        #endregion

        #region -- 用户菜单
        private List<ManagerMenu> _userMenu = new List<ManagerMenu>();

        /// <summary>
        /// 用户的菜单项目
        /// </summary>
        public List<ManagerMenu> userMenu {
            get { return _userMenu; }
            set { _userMenu = value; }
        }

        /// <summary>
        /// 获取用户的菜单(根据用户的角色)
        /// </summary>
        /// <returns></returns>
        public List<ManagerMenu> getUserMenu()
        {
            List<ManagerMenu> m = ManagerMenu.managerMenu;
            foreach (var s in m)
            {
                if (s.roles.Count == 0 ? true : s.roles.Intersect(this.rolesList).Count() > 0)
                {
                    this.userMenu.Add(new ManagerMenu()
                    {
                        icon = s.icon,
                        roles = s.roles,
                        text = s.text,
                        url = s.url,
                        children = s.children == null ? new List<ManagerMenu>() : getUserCheldMenu(s.children)
                    });
                }
            }
            return this.userMenu;
        }

        /// <summary>
        /// 根据权限递归子菜单
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        private List<ManagerMenu> getUserCheldMenu(List<ManagerMenu> children)
        {
            List<ManagerMenu> child = new List<ManagerMenu>();
            foreach (var s in children)
            {
                if (s.roles.Count == 0 ? true : s.roles.Intersect(this.rolesList).Count() > 0)
                {
                    child.Add(new ManagerMenu()
                    {
                        icon = s.icon,
                        roles = s.roles,
                        text = s.text,
                        url = s.url,
                        children = s.children == null ? new List<ManagerMenu>() : getUserCheldMenu(s.children)
                    });
                }
            }
            return child;
        }

        #endregion

        #region -- 日志

        public string getIdString()
        {
            return this.Id;
        }

        public string getNameString()
        {
            return string.Format("{0}[{1}]", this.fullName, this.rolesString);
        }

        #endregion

        #region -- 通信

        public string getMobilePhone()
        {
            return this.mobilePhone;
        }

        public string getEmail()
        {
            return null;
        }

        public string getFullNameString() {
            return this.fullName;
        }

        #endregion

        #region -- 检索

        /// <summary>
        /// 检索后台用户列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<UserManager> searchList(UserManagerSearchRequest condtion) {
            BaseResponseList<UserManager> result = new BaseResponseList<UserManager>();

            List<string> wId = new List<string>();
            if (condtion.wId != null) {
                if (condtion.wId.rows != null) {
                    wId = condtion.wId.rows;
                }
            }

            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_BaseUserSet.OfType<Db_ManagerUser>().AsEnumerable()
                            where (c.isDeleted == false)
                            && (string.IsNullOrEmpty(condtion.q) ? true : (c.fullName.Contains(condtion.q) || (string.IsNullOrEmpty(c.mobilePhone) ? false : c.mobilePhone.Contains(condtion.q)) || (c.loginName.Contains(condtion.q))))
                            && (string.IsNullOrEmpty(condtion.role) ? true : (c.Db_BaseUserRole.Select(x => x.roleName).Contains(condtion.role)))
                            && (condtion.cId.rows == null ? true : condtion.cId.rows.Contains(c.Id))
                            && (wId.Count == 0 ? true : !condtion.wId.rows.Contains(c.Id))
                            select c
                            );
                result.total = rows.Count();
                if (result.total > 0 && condtion.getRows) {
                    rows = rows.OrderByDescending(p => p.createdOn);
                    if (condtion.page > 0) {
                        rows = rows.Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.Select(p => new UserManager(p)).ToList();
                }
            }

            return result;
        }

        

        #endregion
    }
}
