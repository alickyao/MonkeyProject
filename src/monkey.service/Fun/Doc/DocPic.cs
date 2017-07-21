using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using monkey.service;
using System.ComponentModel.DataAnnotations;

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
                Db_DocPic newRow = new Db_DocPic() {
                    Caption = info.Caption,
                    Code = info.Code,
                    Content = info.Content,
                    CreatedOn = DateTime.Now,
                    Descript = info.Descript,
                    DocType = BaseDocType.图文集.GetHashCode(),
                    Id = Guid.NewGuid().ToString()
                };
                db.Db_BaseDocSet.Add(newRow);
                db.SaveChanges();
                return new DocPic(newRow);
            }
        }
    }
}
