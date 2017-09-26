using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.app.timequartz
{
    /// <summary>
    /// 多线执行
    /// </summary>
    public interface IThreading
    {
        /// <summary>
        /// 执行
        /// </summary>
        void Run();
    }
}
