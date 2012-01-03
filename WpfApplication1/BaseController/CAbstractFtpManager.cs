﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfApplication1.ElementEntity;
using System.Net;

namespace WpfApplication1.BaseController
{
    /// <summary>
    /// 统一的FTP 管理，定义好要用到的变量
    /// </summary>
    abstract class CAbstractFtpManager
    {
        protected static readonly int BUFF_SIZE = 4096;

        protected CFtpServerInfo m_ftpInfo;
        protected bool m_enable_ssh;

        public CAbstractFtpManager(CFtpServerInfo ftpInfo, bool enable_ssh = true)
        {
            this.m_ftpInfo = ftpInfo;
            this.m_enable_ssh = enable_ssh;

            try
            {
                WebRequest.Create(ftpInfo.getFullUrl());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
