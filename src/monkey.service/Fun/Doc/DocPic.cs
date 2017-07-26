using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using monkey.service;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace monkey.service.Fun.Doc
{

    /// <summary>
    /// 图文集基础信息编辑请求
    /// </summary>
    public class DocPicEditReqeust {

        /// <summary>
        /// ID 新增时传入空字符串
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public string Caption { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 描述摘要
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 文字内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 所在分类集合
        /// </summary>
        public List<string> TreeIds { get; set; }
    }

    /// <summary>
    /// 图文集
    /// </summary>
    public class DocPic: BaseDoc
    {

        /// <summary>
        /// 摘要描述
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 详情内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 空构造
        /// </summary>
        public DocPic() { }

        /// <summary>
        /// 通过ID构造
        /// </summary>
        /// <param name="id"></param>
        public DocPic(string id) : base(id) {
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseDocSet.OfType<Db_DocPic>().SingleOrDefault(p => p.Id == id);
                if (row == null) {
                    throw new DataNotFundException(string.Format("未能通过ID[{0}]找到该信息", id));
                }
                SetValue(row);
            }
        }

        /// <summary>
        /// 通过数据库表构造
        /// </summary>
        /// <param name="row"></param>
        public DocPic(Db_DocPic row) : base(row) {
            SetValue(row);
        }

        private void SetValue(Db_DocPic row) {
            this.Content = row.Content;
            this.Descript = row.Descript;
        }

        /// <summary>
        /// 新增图文集
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static DocPic CreateDocPic(DocPicEditReqeust info) {
            ValiDatas.valiData(info);
            ValiCode(info.Code);
            using (var db = new DefaultContainer()) {
                var newId = Guid.NewGuid().ToString();
                Db_DocPic newRow = new Db_DocPic() {
                    Caption = info.Caption,
                    Code = info.Code,
                    Content = info.Content,
                    CreatedOn = DateTime.Now,
                    Descript = info.Descript,
                    DocType = BaseDocType.图文集.GetHashCode(),
                    Id = newId
                };
                //所在分类信息

                if (info.TreeIds != null) {
                    if (info.TreeIds.Count > 0) {
                        List<Db_BaseDocTree> dbTrees = new List<Db_BaseDocTree>();
                        foreach (var item in info.TreeIds) {
                            dbTrees.Add(new Db_BaseDocTree() {
                                Id = Guid.NewGuid().ToString(),
                                Db_BaseDocId = newId,
                                TreeId = item
                            });
                        }
                        db.Db_BaseDocTreeSet.AddRange(dbTrees);
                    }
                }

                db.Db_BaseDocSet.Add(newRow);
                db.SaveChanges();
                return new DocPic(newRow);
            }
        }

        /// <summary>
        /// 编辑图文消息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DocPic EditDocPic(DocPicEditReqeust info) {
            ValiDatas.valiData(info);
            ValiCode(info.Code, info.Id);
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseDocSet.OfType<Db_DocPic>().Single(p => p.Id == this.Id);
                row.Caption = info.Caption;
                row.Code = info.Code;
                row.Content = info.Content;
                row.Descript = info.Descript;

                //删除原来的分类
                db.Database.ExecuteSqlCommand("delete from Db_BaseDocTreeSet where Db_BaseDocId=@docId", new SqlParameter("@docId", this.Id));
                //新增分类
                if (info.TreeIds != null)
                {
                    if (info.TreeIds.Count > 0)
                    {
                        List<Db_BaseDocTree> dbTrees = new List<Db_BaseDocTree>();
                        foreach (var item in info.TreeIds)
                        {
                            dbTrees.Add(new Db_BaseDocTree()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Db_BaseDocId = this.Id,
                                TreeId = item
                            });
                        }
                        db.Db_BaseDocTreeSet.AddRange(dbTrees);
                    }
                }
                db.SaveChanges();
                return new DocPic(row);
            }
        }

        public override string getNameString()
        {
            return this.Caption;
        }
    }
}
