using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfApplication1.ElementEntity;
using System.IO;
using System.Net;
using WpfApplication1.Exceptions;
using System.Threading;

namespace WpfApplication1.BaseController
{
    /// <summary>
    /// FTP的管理（如同济FTP），操作下载之外的管理
    /// 主要是对某个文件夹的操作
    /// </summary>
    class CPrivateFtpManage : CAbstractFtpManager
    {
        public CPrivateFtpManage(CFtpServerInfo ftpInfo, bool enable_ssh = true)
            : base(ftpInfo, enable_ssh)
        {
        }


    }
}
