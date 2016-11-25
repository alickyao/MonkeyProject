﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.service
{
    /// <summary>
    /// 标准列表请求参数
    /// </summary>
    public class BaseRequest
    {
        private int _page = 1;
        /// <summary>
        /// 调取的页码 默认1，如果不需要系统进行翻页则需要传入0
        /// </summary>
        public int page
        {
            get { return _page; }
            set { _page = value; }
        }

        private int _pageSize = 20;
        /// <summary>
        /// 每页的数量 默认 20 
        /// </summary>
        [Range(1,200)]
        public int pageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
        /// <summary>
        /// 根据页码获取需要跳过的行的数量
        /// </summary>
        /// <returns></returns>
        public int getSkip()
        {
            return (this.page - 1) * this.pageSize;
        }

        private bool _getRows = true;
        /// <summary>
        /// 是否获取详细的行列表信息 默认为true 设置为false 只会返回根据当前查询条件查询到的总数
        /// </summary>
        public bool getRows
        {
            get { return _getRows; }
            set { _getRows = value; }
        }
    }
}
