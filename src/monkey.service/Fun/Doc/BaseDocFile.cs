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

        /// <summary>
        /// 空构造
        /// </summary>
        public BaseDocImgFile() { }
    }
}
