using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1.Exceptions
{
    /// <summary>
    /// 上传给的文件路径有问题！
    /// </summary>
    class FilePathInvalidException : Exception
    {
        public FilePathInvalidException(string info)
            : base(info)
        {
        }
    }
}
