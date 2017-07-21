using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using monkey.service;

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
    public class BaseDoc
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
        public BaseDoc(string id) { }

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
    }
}
