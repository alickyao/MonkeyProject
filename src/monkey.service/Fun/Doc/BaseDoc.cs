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
    /// 文档 基类
    /// </summary>
    public class BaseDoc
    {
        /// <summary>
        /// 文档的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 标识名称 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 所在分类ID
        /// </summary>
        public string TreeId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

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
    }
}
