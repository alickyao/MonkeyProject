using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using System.ComponentModel.DataAnnotations;

namespace monkey.service
{
    /// <summary>
    /// 基本树的属性
    /// </summary>
    public class BaseTreeAttr {

        #region -- 基本属性

        /// <summary>
        /// 树节点的ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 树节点名称
        /// </summary>
        [Required]
        public string text { get; set; }

        /// <summary>
        /// 排序依据
        /// </summary>
        public int Seq { get; set; }

        /// <summary>
        /// 树节点的编号
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 父节点的ID
        /// </summary>
        public string ParentId { get; set; }

        #endregion

        public BaseTreeAttr() { }

    }

    /// <summary>
    /// 基本树
    /// </summary>
    public class BaseTree:BaseTreeAttr
    {

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 创建时间-文本
        /// </summary>
        public string CreatedOnString { get; set; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<BaseTree> children { get; set; }


        /// <summary>
        /// 构造方法 - BaseTree表
        /// </summary>
        /// <param name="row"></param>
        public BaseTree(Db_BaseTree row) {
            this.id = row.Id;
            this.text = row.Name;
            this.Code = row.Code;
            this.ParentId = row.ParentId;
            this.CreatedOn = row.CreatedOn;
            this.CreatedOnString = this.CreatedOn.ToString("yyyy-MM-dd HH:mm");
            this.Seq = row.Seq;
        }


        /// <summary>
        /// 获取子节点
        /// </summary>
        public void GetBaseTreeChilderns() {
            using (var db = new DefaultContainer()) {
                this.children = (from c in db.Db_BaseTreeSet.AsEnumerable()
                                 where c.ParentId == this.id
                                 orderby c.Seq ascending
                                 select new BaseTree(c)).ToList();
                if (this.children.Count > 0) {
                    foreach (var ch in this.children) {
                        ch.GetBaseTreeChilderns();
                    }
                }
            }
        }

        /// <summary>
        /// 根据ID获取树的节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BaseTree GetBaseTreeById(string id)
        {
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseTreeSet.SingleOrDefault(p => p.Id == id);
                if (row == null)
                {
                    throw new DataNotFundException(string.Format("传入的数节点的Id[{0}]不正确", id));
                }
                return new BaseTree(row);
            }
        }

        /// <summary>
        /// 根据CODE获取树的节点
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static BaseTree GetBaseTreeByCode(string code) {
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseTreeSet.SingleOrDefault(p => p.Code == code);
                if (row == null) {
                    throw new DataNotFundException(string.Format("传入的数节点的CODE[{0}]不正确", code));
                }
                return new BaseTree(row);
            }
        }

        /// <summary>
        /// 获取树的根节点集合
        /// </summary>
        /// <returns></returns>
        public static List<BaseTree> GetBaseTreeRoots() {
            using (var db = new DefaultContainer()) {
                List<BaseTree> result = (from c in db.Db_BaseTreeSet.AsEnumerable()
                                         where c.ParentId == null || c.ParentId == ""
                                         orderby c.Seq ascending
                                         select new BaseTree(c)
                              ).ToList();
                return result;
            }
            
        }

        /// <summary>
        /// 新增树的节点
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseTree CreateBaseTree(BaseTreeAttr condtion) {
            ValiDatas.valiData(condtion);
            var r = CheckCode(condtion.Code);
            if (r > 0) {
                throw new ValiDataException(string.Format("code [{0}] 已存在，新增失败",condtion.Code));
            }
            using (var db = new DefaultContainer()) {
                Db_BaseTree dbTree = new Db_BaseTree() {
                    Id = Guid.NewGuid().ToString(),
                    Code = condtion.Code,
                    CreatedOn = DateTime.Now,
                    Name = condtion.text,
                    ParentId = string.IsNullOrEmpty(condtion.ParentId)? null :condtion.ParentId,
                    Seq = condtion.Seq
                };
                db.Db_BaseTreeSet.Add(dbTree);
                db.SaveChanges();
                return new BaseTree(dbTree);
            }
        }

        /// <summary>
        /// 编辑树的节点
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public BaseTree EditBaseTree(BaseTreeAttr condtion) {
            ValiDatas.valiData(condtion);

            var r = CheckCode(condtion.Code, this.id);
            if (r > 0)
            {
                throw new ValiDataException(string.Format("code [{0}] 已存在，保存失败",condtion.Code));
            }

            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseTreeSet.Single(p => p.Id == this.id);
                row.Code = condtion.Code;
                row.Name = condtion.text;
                row.ParentId = string.IsNullOrEmpty(condtion.ParentId) ? null : condtion.ParentId;
                row.Seq = condtion.Seq;
                db.SaveChanges();
                return BaseTree.GetBaseTreeById(this.id);
            }
        }


        /// <summary>
        /// 所有子节点的ID集合 初始化后调用 SetChildrenIdList() 方法进行赋值
        /// </summary>
        protected static List<string> IdList { get; set; }

        /// <summary>
        /// 取子节点的ID集合 到 IdList
        /// </summary>
        protected void SetChildrenIdList() {
            if (this.children != null) {
                if (this.children.Count > 0)
                {
                    foreach (var c in this.children) {
                        IdList.Add(c.id);
                        c.SetChildrenIdList();
                    }
                }
            }
            
        }

        /// <summary>
        /// 删除节点以及几点下的子节点(物理删除)
        /// </summary>
        public void DelBaseTree() {
            this.GetBaseTreeChilderns();
            IdList = new List<string>();
            IdList.Add(this.id);
            SetChildrenIdList();
            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_BaseTreeSet.AsEnumerable() where IdList.Contains(c.Id) select c);
                if (rows.Count() > 0) {
                    db.Db_BaseTreeSet.RemoveRange(rows);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 检查code出现的次数
        /// </summary>
        /// <param name="code">需要检查的CODE</param>
        /// <param name="id">需要排除的Id</param>
        /// <returns></returns>
        public static int CheckCode(string code, string id = null) {
            using (var db = new DefaultContainer()) {
                int result = (from c in db.Db_BaseTreeSet
                              where c.Code == code
                              && (string.IsNullOrEmpty(id) ? true : (c.Id != id))
                              select c).Count();
                return result;
            }
        }
    }
}
