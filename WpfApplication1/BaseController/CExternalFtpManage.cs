using System;
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


        /// <summary>
        /// 下载到指定路径
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="overrite"></param>
        public void download(string targetPath, bool overrite = false)
        {
            if (!overrite && File.Exists(targetPath + '\\' + ftpInfo.FileName))
            {
                throw new FileExistedException("target path existed! Not allowed to overwrite!");
            }

            FileStream fs;
            try
            {
                fs = File.Create(targetPath + '\\' + ftpInfo.FileName);
            }
            catch (System.Exception)
            {
                throw new System.ArgumentException("target path not available!");
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpInfo.getFullUrl());
            request.EnableSsl = enable_ssh;
            request.Credentials = new NetworkCredential(ftpInfo.UserName, ftpInfo.UserPwd);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = true;
            request.UsePassive = true;

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

            long length = response.ContentLength;
            byte[] buffer = new byte[BUFF_SIZE];
            int readCount;
            do 
            {
                readCount = remoteStream.Read(buffer, 0, BUFF_SIZE);
                fs.Write(buffer, 0, readCount);
            } while (readCount > 0);

            remoteStream.Close();
            response.Close();
            fs.Close();
        }


        /// <summary>
        /// 获得指定一个文件的大小
        /// 单位是B
        /// </summary>
        /// <returns></returns>
        public long getFileSize()
        {
            string url = ftpInfo.getFullUrl();
            FtpWebRequest request = (FtpWebRequest) WebRequest.Create(ftpInfo.getFullUrl());
            request.EnableSsl = enable_ssh;
            request.Credentials = new NetworkCredential(ftpInfo.UserName, ftpInfo.UserPwd);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.UseBinary = true;
            request.UsePassive = true;
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
            long size = response.ContentLength;
            response.Close();

            return size;
        }


    }
}
