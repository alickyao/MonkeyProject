using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using monkey.service;
using monkey.service.Logs;
using System.Data.SqlClient;

namespace monkey.service.Fun.Doc
{

    /// <summary>
    /// 文档类型
    /// </summary>
    public enum BaseDocType {
        /// <summary>
        /// 包含图片与文本描述的图文集
        /// </summary>
        图文集
    }

    /// <summary>
    /// 文档 基类
    /// </summary>
    public class BaseDoc:ILogStringable
    {
        #region -- 基本属性


        /// <summary>
        /// 文档的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 文档类型
        /// </summary>
        public BaseDocType DocType { get; set; }

        /// <summary>
        /// 文档类型 - 文本
        /// </summary>
        public string DocTypeString { get; set; }

        /// <summary>
        /// 标识名称 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 创建时间 - 文本
        /// </summary>
        public string CreatedOnString { get; set; }

        /// <summary>
        /// 排序依据
        /// </summary>
        public int Seq { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 是否已禁用
        /// </summary>
        public bool IsDisabled { get; set; }


        #endregion

        /// <summary>
        /// 空构造
        /// </summary>
        public BaseDoc() { }

        /// <summary>
        /// 使用ID构造
        /// </summary>
        /// <param name="id"></param>
        public BaseDoc(string id) {

            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseDocSet.SingleOrDefault(p => p.Id == id);
                if (row == null)
                {
                    throw new DataNotFundException(string.Format("未能通过ID[{0}]找到该信息", id));
                }
                SetValue(row);
            }
        }

        /// <summary>
        /// 使用数据库对象构造
        /// </summary>
        /// <param name="row"></param>
        public BaseDoc(Db_BaseDoc row) {
            SetValue(row);
        }

        private void SetValue(Db_BaseDoc row) {
            this.Id = row.Id;
            this.DocType = (BaseDocType)row.DocType;
            this.DocTypeString = this.DocType.ToString();
            this.Code = row.Code;
            this.Caption = row.Caption;
            this.CreatedOn = row.CreatedOn;
            this.CreatedOnString = this.CreatedOn.ToString("yyyy-MM-dd HH:mm");
            this.Seq = row.Seq;
            this.IsDeleted = row.IsDeleted;
            this.IsDisabled = row.IsDisabled;
        }

        /// <summary>
        /// 获取所在的分类（所在分类树的集合）
        /// </summary>
        /// <returns></returns>
        public List<BaseTree> GetTreeListInfo() {
            List<BaseTree> result = new List<BaseTree>();

            using (var db = new DefaultContainer()) {
                var treeIds = db.Db_BaseDocTreeSet.Where(p => p.Db_BaseDocId == this.Id).Select(p => p.TreeId).ToList();
                if (treeIds.Count > 0) {
                    var rows = (from c in db.Db_BaseTreeSet
                                where treeIds.Contains(c.Id)
                                select c);
                    result = rows.AsEnumerable().Select(p => new BaseTree(p)).ToList();
                }
            }

            return result;
        }

        /// <summary>
        /// 更新文档图片集
        /// </summary>
        /// <param name="filesList"></param>
        public void UpdatImgFileList(List<BaseDocImgFile> filesList) {
            if (filesList == null) {
                filesList = new List<BaseDocImgFile>();
            }
            foreach (var item in filesList) {
                ValiDatas.valiData(item);
            }
            using (var db = new DefaultContainer()) {
                //删除原来的
                db.Database.ExecuteSqlCommand("delete from Db_BaseDocFileSet where Db_BaseDocId =@docId", new SqlParameter("@docId", this.Id));
                if (filesList.Count > 0) {
                    List<Db_BaseDocFile> dbFiles = new List<Db_BaseDocFile>();
                    foreach (var item in filesList) {
                        dbFiles.Add(new Db_BaseDocFile() {
                            Caption = item.Caption,
                            CreatedOn = DateTime.Now,
                            Db_BaseDocId = this.Id,
                            Descript = item.Descript,
                            Seq = item.Seq,
                            FileId = item.FileId,
                            Id = Guid.NewGuid().ToString()
                        });
                    }
                    db.Db_BaseDocFileSet.AddRange(dbFiles);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 标记为删除
        /// </summary>
        public void SelDel() {
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseDocSet.Single(p => p.Id == this.Id);
                row.IsDeleted = true;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 切换禁用状态
        /// </summary>
        /// <returns></returns>
        public BaseDoc SetDisable() {
            using (var db = new DefaultContainer())
            {
                var row = db.Db_BaseDocSet.Single(p => p.Id == this.Id);
                row.IsDisabled = row.IsDisabled ? false : true;
                db.SaveChanges();
                return new BaseDoc(row);
            }
        }

        /// <summary>
        /// 验证CODE是否可用 确认其唯一性 （已排除被删除的项目）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id"></param>
        public static void ValiCode(string code, string id = "") {
            if (!string.IsNullOrEmpty(code))
            {
                var cc = GetCodeCount(code,id);
                if (cc > 0)
                {
                    throw new ValiDataException(string.Format("标示名称【{0}】已存在", code));
                }
            }
        }

        /// <summary>
        /// 获取编号code出现的次数 （已排除被删除的项目）
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="id">需排除的ID</param>
        /// <returns></returns>
        private static int GetCodeCount(string code, string id = "") {
            using (var db = new DefaultContainer()) {
                int result = (from c in db.Db_BaseDocSet
                              where c.IsDeleted == false
                              && (c.Code == code)
                              && (string.IsNullOrEmpty(id) ? true : c.Id != id)
                              select c.Id
                              ).Count();
                return result;
            }
        }

        public string getIdString()
        {
            return this.Id;
        }

        public virtual string getNameString()
        {
            return string.Format("{0}", this.DocTypeString);
        }
    }

    /// <summary>
    /// 基础文档搜索请求
    /// </summary>
    public class BaseDocSerarchRequest : BaseRequest {

        /// <summary>
        /// 所在分类的ID
        /// </summary>
        public List<string> TreeId { get; set; }

        /// <summary>
        /// 关键字（标题）
        /// </summary>
        public string q { get; set; }
    }

    /// <summary>
    /// 基础文档列表
    /// </summary>
    public class BaseDocDetailList : BaseDoc {

        /// <summary>
        /// 所在分类描述
        /// </summary>
        public string TreeNames { get; set; }


        /// <summary>
        /// 通过数据库构造
        /// </summary>
        /// <param name="row"></param>
        /// <param name="tree"></param>
        public BaseDocDetailList(Db_BaseDoc row, List<Db_BaseTree> tree) : base(row)
        {
            if (tree.Count > 0) {
                this.TreeNames = string.Join(",", tree.Select(p => p.Name));
            }
        }

        /// <summary>
        /// 检索基础文档列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<BaseDocDetailList> SearchBaseDocList(BaseDocSerarchRequest condtion) {
            BaseResponseList<BaseDocDetailList> result = new BaseResponseList<BaseDocDetailList>();

            List<string> treeId = new List<string>();
            if (condtion.TreeId != null) {
                if (condtion.TreeId.Count > 0) {
                    var t = condtion.TreeId.Where(p => !string.IsNullOrEmpty(p)).ToList();
                    if (t.Count > 0) {
                        treeId = t;
                    }
                }
            }

            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_BaseDocSet
                            where c.IsDeleted == false
                            && (treeId.Count == 0 ? true:treeId.Intersect(c.Db_BaseDocTree.Select(p=>p.TreeId)).Count() >0)
                            && (string.IsNullOrEmpty(condtion.q) ? true : c.Caption.Contains(condtion.q))
                            orderby c.CreatedOn descending
                            select new
                            {
                                row = c,
                                tree = (from t in db.Db_BaseTreeSet where c.Db_BaseDocTree.Select(p=>p.TreeId).Contains(t.Id) select t)
                            }
                            );
                result.total = rows.Count();
                if (result.total > 0 && condtion.getRows) {
                    var rowsList = rows.Skip(condtion.getSkip()).Take(condtion.pageSize).AsEnumerable().Select(p => new BaseDocDetailList(p.row, p.tree.ToList())).ToList();
                    result.rows = rowsList;
                }
            }

            return result;
        }
    }
}
