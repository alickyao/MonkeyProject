﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service;
using monkey.service.Logs;
using monkey.service.Db;
using System.ComponentModel.DataAnnotations;

namespace monkey.service.WorkFlow
{
    /// <summary>
    /// 工作流审批角色
    /// </summary>
    public class WorkFlowRole:ILogStringable
    {
        /// <summary>
        /// 工作流审批角色ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工作流审批角色名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }

        /// <summary>
        /// 工作流审批角色描述
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 创建时间 - 文本
        /// </summary>
        public string CreateOnString { get; set; }

        /// <summary>
        /// 空构造
        /// </summary>
        public WorkFlowRole() { }

        /// <summary>
        /// 通过数据库中的表构造
        /// </summary>
        /// <param name="row"></param>
        public WorkFlowRole(Db_WorkFlowRole row) {
            this.Id = row.Id;
            this.RoleName = row.RoleName;
            this.Descript = row.Descript;
            this.CreatedOn = row.CreatedOn;
            this.CreateOnString = this.CreatedOn.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 根据ID获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WorkFlowRole GetInstance(string id) {
            using (var db = new DefaultContainer()) {
                var row = db.Db_WorkFlowRoleSet.SingleOrDefault(p => p.Id == id);
                if (row == null) {
                    throw new DataNotFundException(string.Format("传入的工作流角色ID：[{0}],不正确", id));
                }
                return new WorkFlowRole(row);
            }
        }

        /// <summary>
        /// 新增/编辑
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static List<WorkFlowRole> Edit(List<WorkFlowRole> rows) {
            List<WorkFlowRole> result = new List<WorkFlowRole>();
            foreach (var item in rows) {
                ValiDatas.valiData(item);
            }
            using (var db = new DefaultContainer()) {
                foreach (var item in rows)
                {
                    if (string.IsNullOrEmpty(item.Id))
                    {
                        Db_WorkFlowRole dbRole = new Db_WorkFlowRole()
                        {
                            CreatedOn = DateTime.Now,
                            Descript = string.IsNullOrEmpty(item.Descript) ? null : item.Descript,
                            Id = Guid.NewGuid().ToString(),
                            RoleName = item.RoleName
                        };
                        db.Db_WorkFlowRoleSet.Add(dbRole);
                        result.Add(new WorkFlowRole(dbRole));
                    }
                    else {
                        var row = db.Db_WorkFlowRoleSet.Single(p => p.Id == item.Id);
                        row.RoleName = item.RoleName;
                        row.Descript = item.Descript;
                        result.Add(new WorkFlowRole(row));
                    }
                }
                db.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// 删除（物理删除）包括从表
        /// </summary>
        /// <returns></returns>
        public WorkFlowRole Delete() {
            using (var db = new DefaultContainer()) {
                var row = db.Db_WorkFlowRoleSet.Single(p => p.Id == this.Id);
                db.Db_WorkFlowRoleSet.Remove(row);
                var descriptsRows = (from c in db.Db_WorkFlowRoleDescriptSet where c.WorkFlowRoleId == this.Id select c).ToList();
                if (descriptsRows.Count > 0) {
                    db.Db_WorkFlowRoleDescriptSet.RemoveRange(descriptsRows);
                }
                db.SaveChanges();
            }
            return this;
        }


        /// <summary>
        /// 获取工作流角色列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<WorkFlowRole> GetRolsList(BaseRequest condtion) {
            BaseResponseList<WorkFlowRole> result = new BaseResponseList<WorkFlowRole>();
            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_WorkFlowRoleSet.AsEnumerable() select c);
                result.total = rows.Count();
                if (condtion.getRows && result.total > 0) {
                    if (condtion.page > 1)
                    {
                        rows = rows.OrderByDescending(p => p.CreatedOn).Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    
                    result.rows = rows.Select(p => new WorkFlowRole(p)).ToList();
                }
            }
            return result;
        }

        /// <summary>
        /// 工作流角色下的用户ID
        /// </summary>
        public List<string> DescripUserId { get; set; }

        /// <summary>
        /// 获取工作流角色下的用户ID
        /// </summary>
        /// <returns></returns>
        public List<string> GetDescripUserId() {
            using (var db = new DefaultContainer())
            {
                var dbRows = (from c in db.Db_WorkFlowRoleDescriptSet where c.WorkFlowRoleId == this.Id select c);
                this.DescripUserId = dbRows.Select(p => p.UserId).ToList();
            }
            return this.DescripUserId;
        }

        /// <summary>
        /// 更新角色的用户信息
        /// </summary>
        /// <param name="userIds"></param>
        public void UpdataDescriptUsers(List<string> userIds) {
            using (var db = new DefaultContainer()) {
                //先清空在新增
                var delRows = (from c in db.Db_WorkFlowRoleDescriptSet where c.WorkFlowRoleId == this.Id select c);
                db.Db_WorkFlowRoleDescriptSet.RemoveRange(delRows);
                List<Db_WorkFlowRoleDescript> newRows = new List<Db_WorkFlowRoleDescript>();
                foreach (var item in userIds) {
                    newRows.Add(new Db_WorkFlowRoleDescript()
                    {
                        CreatedOn = DateTime.Now,
                        UserId = item,
                        WorkFlowRoleId = this.Id
                    });
                }
                db.Db_WorkFlowRoleDescriptSet.AddRange(newRows);
                db.SaveChanges();
            }
        }

        public string getIdString()
        {
            return this.Id;
        }

        public string getNameString()
        {
            return this.RoleName;
        }
    }
}
