using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using System.Web;
using System.Xml.Serialization;
using System.IO;

namespace monkey.service.Users
{
    

    /// <summary>
    /// 系统用户角色类型
    /// </summary>
    public enum SysRolesType {
        /// <summary>
        /// 后台专用角色
        /// </summary>
        后台,
        /// <summary>
        /// 其他角色，会员，访客等
        /// </summary>
        其他
    }

    /// <summary>
    /// 角色
    /// </summary>
    public class SysRoles
    {
        /// <summary>
        /// 系统全部角色
        /// </summary>
        public static List<SysRoles> sysRoles { get; private set; }

        static SysRoles()
        {
            string path = HttpContext.Current.Server.MapPath("/App_Data/SysRole.xml");
            StreamReader file = new StreamReader(path);
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(List<SysRoles>));
                sysRoles = ((List<SysRoles>)reader.Deserialize(file));
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                file.Close();
            }
        }

        /// <summary>
        /// 对应类型的角色
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<SysRoles> getRolesList(SysRolesType t)
        {
            return sysRoles.Where(p => p.cat == t.GetHashCode()).ToList();
        }


        /// <summary>
        /// 角色
        /// </summary>
        public string role { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        public int cat { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        public string discribe { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}[{1}]", this.name, this.role);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (typeof(SysRoles).Equals(obj.GetType()))
            {
                return this.role.Equals(((SysRoles)obj).role);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.role.GetHashCode();
        }
    }

    /// <summary>
    /// 用户基础对象
    /// </summary>
    public class UserBase
    {
        #region -- 属性

        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool isDeleted { get; set; }

        /// <summary>
        /// 是否已禁用
        /// </summary>
        public bool isDisabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createdOn { get; set; }

        /// <summary>
        /// 格式化后的创建时间
        /// </summary>
        public string createdOnString { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime? lastLoginTime { get; set; }

        /// <summary>
        /// 格式化后的最近登录时间
        /// </summary>
        public string lastLoginTimeString { get; set; }

        /// <summary>
        /// 格式化后的最近登录时间 （X分钟/小时/天前）
        /// </summary>
        public string lastLoginShowTimeString { get; set; }

        /// <summary>
        /// 最近登录的IP地址
        /// </summary>
        public string lastLoginIpAddress { get; set; }

        /// <summary>
        /// 用户的角色列表
        /// </summary>
        public List<string> rolesList { get; set; }

        /// <summary>
        /// 用户的角色
        /// </summary>
        public string rolesString { get; set; }

        #endregion


        public UserBase(Db_BaseUser row) {
            setValue(row);
        }

        private void setValue(Db_BaseUser row)
        {
            this.Id = row.Id;
            this.createdOn = row.createdOn;
            this.lastLoginIpAddress = row.lastLoginIpAddress;
            this.lastLoginTime = row.lastLoginTime;
            this.rolesString = String.Join("," ,row.Db_BaseUserRole.Select(p=>p.roleName));
            this.rolesList = row.Db_BaseUserRole.Select(p => p.roleName).ToList();
            this.isDeleted = row.isDeleted;
            this.isDisabled = row.isDisabled;

            //时间
            this.createdOnString = this.createdOn.ToString("yyyy-MM-dd HH:mm");
            if (this.lastLoginTime != null) {
                this.lastLoginShowTimeString = SysHelps.get2TimeShowString(this.lastLoginTime.Value);
                this.lastLoginTimeString = this.lastLoginTime.Value.ToString("yyyy-MM-dd HH:mm");
            }
        }

        /// <summary>
        /// 保存用户的禁用设置
        /// </summary>
        public void saveDisabled(bool isDisabled) {
            if (this.isDisabled == isDisabled) {
                throw new ValiDataException(string.Format("当前用户已是{0}状态，不能重复设置", this.isDisabled ? "已禁用" : "未禁用"));
            }
            this.isDisabled = isDisabled;
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseUserSet.Single(p => p.Id == this.Id);
                row.isDisabled = this.isDisabled;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 将用户标记为删除
        /// </summary>
        public void delete() {
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseUserSet.Single(p => p.Id == this.Id);
                row.isDeleted = true;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新最后一次登录信息
        /// </summary>
        /// <param name="ipAddress">用户的IP地址</param>
        public void updateLastLoginInfo(string ipAddress) {
            this.lastLoginIpAddress = ipAddress;
            this.lastLoginTime = DateTime.Now;
            saveLastLoginInfo();
        }

        private void saveLastLoginInfo() {
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseUserSet.Single(p => p.Id == this.Id);
                row.lastLoginIpAddress = this.lastLoginIpAddress;
                row.lastLoginTime = this.lastLoginTime;
                db.SaveChanges();
            }
        }
    }
}
