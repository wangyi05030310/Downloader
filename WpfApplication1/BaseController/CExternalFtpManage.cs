﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using WpfApplication1.ElementEntity;
using WpfApplication1.Exceptions;

namespace WpfApplication1.BaseController
{

    /// <summary>
    /// 外部FTP资源的下载管理，操作下载
    /// 注：此FTP下载没有多线程下载，没有断点续传
    /// 多线程、断点续传需要"REST"指令操作文件的偏移，（就像seek）
    /// 然而在WebRequestMethods.Ftp里没有提供REST的操作，
    /// 但.NET又不允许使用自定义的XXXX，
    /// 所以便只能这么单薄了...
    /// </summary>
    class CExternalFtpManage : CAbstractFtpManager
    {

        /// <summary>
        /// 如果构造函数不抛异常，url就可以用
        /// </summary>
        /// <param name="ftpInfo"></param>
        /// <param name="enable_ssh"></param>
        public CExternalFtpManage(CFtpServerInfo ftpInfo, bool enable_ssh = true)
            : base(ftpInfo, enable_ssh)
        {
        }

        private string m_target_path;

        private long m_total_size;
        private long m_current_size;

        private FileStream m_fs;
        private Thread m_downloadThread;

        /// <summary>
        /// 文件总大小
        /// </summary>
        public long FileSize
        {
            get { return m_total_size; }
        }

        /// <summary>
        /// 当前已下载大小
        /// </summary>
        public long CurrentSize
        {
            get { return m_current_size; }
        }


        /// <summary>
        /// 开始下载到指定路径
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="overrite"></param>
        public void startDownloading(string targetPath, bool overrite = false)
        {
            if (!overrite && File.Exists(targetPath + '\\' + m_ftpInfo.FileName))
            {
                throw new FileExistedException("此路径已存在，又没有说要不要覆盖");
            }

            this.retrieveFileSize();
            m_target_path = targetPath + '\\' + m_ftpInfo.FileName;

            m_fs = File.Create(m_target_path);
            m_fs.SetLength(m_total_size);

            m_downloadThread = new Thread(new ThreadStart(doDownload));
            m_downloadThread.Start();
        }

        /// <summary>
        /// 在线程内执行下载的函数
        /// </summary>
        private void doDownload()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(m_ftpInfo.getFullUrl());
            request.EnableSsl = m_enable_ssh;
            request.Credentials = new NetworkCredential(m_ftpInfo.UserName, m_ftpInfo.UserPwd);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = true;
            request.UsePassive = false;

            FtpWebResponse response = null;
            Stream remoteStream = null;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
                remoteStream = response.GetResponseStream();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            m_current_size = 0;
            byte[] buffer = new byte[BUFF_SIZE];
            int readCount;
            do
            {
                readCount = remoteStream.Read(buffer, 0, BUFF_SIZE);
                m_fs.Write(buffer, 0, readCount);
                m_current_size += readCount;
            } while (readCount > 0);

            remoteStream.Close();
            response.Close();
            m_fs.Close();
            m_fs = null;
        }


        /// <summary>
        /// 获得指定一个文件的大小
        /// 单位是B，通过FileSize拿
        /// </summary>
        public void retrieveFileSize()
        {
            FtpWebRequest request = (FtpWebRequest) WebRequest.Create(m_ftpInfo.getFullUrl());
            request.EnableSsl = m_enable_ssh;
            request.Credentials = new NetworkCredential(m_ftpInfo.UserName, m_ftpInfo.UserPwd);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.UseBinary = true;
            request.UsePassive = false;
            request.KeepAlive = true;

            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            m_total_size = response.ContentLength;
            response.Close();
        }

    }
}
