using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey.service
{
    /// <summary>
    /// 可用于多线程执行
    /// </summary>
    public interface IThreading
    {
        /// <summary>
        /// 执行
        /// </summary>
        void Run();
    }
}
