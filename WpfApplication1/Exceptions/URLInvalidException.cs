using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1.Exceptions
{
    /// <summary>
    /// 输入的url不合法，比如给了ftp下载非FTP的URL
    /// </summary>
    class URLInvalidException : Exception
    {
        public URLInvalidException(string info)
            : base(info)
        {
        }
    }
}
