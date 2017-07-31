using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using System.ComponentModel.DataAnnotations;

namespace monkey.service.Fun.Doc
{
    /// <summary>
    /// 更新文档图片集请求
    /// </summary>
    public class BaseDocImgFilesUploadRequest {

        /// <summary>
        /// 文档的ID
        /// </summary>
        public string DocId { get; set; }

        /// <summary>
        /// 图片集
        /// </summary>
        public List<BaseDocImgFile> FilesList { get; set; }
    }

    /// <summary>
    /// 基础文档图片
    /// </summary>
    public class BaseDocImgFile
    {
        #region

        /// <summary>
        /// 文档的图片记录ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 图片记录的ID
        /// </summary>
        [Required]
        public string FileId { get; set; }

        /// <summary>
        /// 文档的ID
        /// </summary>
        public string DocId { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 排序位置
        /// </summary>
        public int Seq { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        /// <summary>
        /// 空构造
        /// </summary>
        public BaseDocImgFile() { }

        /// <summary>
        /// 使用数据库的行构造
        /// </summary>
        /// <param name="row"></param>
        /// <param name="file"></param>
        public BaseDocImgFile(Db_BaseDocFile row, Db_BaseFile file) {
            this.Id = row.Id;
            this.FileId = row.FileId;
            this.DocId = row.Db_BaseDocId;
            this.FilePath = file.Path;
            this.Caption = row.Caption;
            this.Seq = row.Seq;
            this.Descript = row.Descript;
            this.CreatedOn = row.CreatedOn;
        }
    }
}
