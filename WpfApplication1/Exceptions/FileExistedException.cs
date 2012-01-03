using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1.Exceptions
{
    /// <summary>
    /// 下载要创建文件的时候，目标文件已经存在了
    /// </summary>
    class FileExistedException : Exception
    {
        public FileExistedException(string info)
            : base(info)
        {
        }
    }
}
