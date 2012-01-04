using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfApplication1.ElementEntity;
using System.IO;
using System.Net;
using WpfApplication1.Exceptions;

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

        /// <summary>
        /// 上传文件到FTP上
        /// </summary>
        /// <param name="fileurl"></param>
        public void upload(string fileurl, string fileName)
        {
            FileInfo fileInf = null;
            try
            {
                fileInf = new FileInfo(fileurl);
            }
            catch (System.Exception)
            {
                throw new FilePathInvalidException("传进来的文件路径有问题！");
            }

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(m_ftpInfo.getFullUrl() + fileName);
            request.Credentials = new NetworkCredential(m_ftpInfo.UserName, m_ftpInfo.UserPwd);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.ContentLength = fileInf.Length;
            request.EnableSsl = m_enable_ssh;

            byte[] buff = new byte[BUFF_SIZE];
            int readCount;

            FileStream fs = fileInf.OpenRead();
            Stream stream = request.GetRequestStream();
            do
            {
                readCount = fs.Read(buff, 0, BUFF_SIZE);
                stream.Write(buff, 0, readCount);
            } while (readCount > 0);
            stream.Close();
            fs.Close();

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Exception ex)
            {
                //这个时候才报异常，可能是网络啥的问题
                throw ex;
            }
        }

        /// <summary>
        /// 拿到ftp上此目录下所有文件名
        /// </summary>
        /// <returns></returns>
        public string[] getFileList()
        {
            List<string> files = new List<string>();

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(m_ftpInfo.getFullUrl());
            request.UseBinary = true;
            request.EnableSsl = m_enable_ssh;
            request.Credentials = new NetworkCredential(m_ftpInfo.UserName, m_ftpInfo.UserPwd);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            FtpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    files.Add(line);
                    line = reader.ReadLine();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return files.ToArray();
        }

        /// <summary>
        /// 删除指定的文件
        /// </summary>
        public void delete(string fileName)
        {
            FtpWebRequest request = (FtpWebRequest) WebRequest.Create(m_ftpInfo.getFullUrl() + fileName);
            request.Credentials = new NetworkCredential(m_ftpInfo.UserName, m_ftpInfo.UserPwd);
            request.EnableSsl = m_enable_ssh;
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.UseBinary = true;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Exception ex)
            {
            	throw ex;
            }
        }

        /// <summary>
        /// 在FTP上创建一个目录
        /// </summary>
        /// <param name="dir"></param>
        public void makeDir(string dir)
        {

        }

        /// <summary>
        /// 在FTP上删除一个目录
        /// </summary>
        public void deleteDir()
        {

        }

        /// <summary>
        /// 改名！
        /// </summary>
        public void renameFile(string fileName, string newName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(m_ftpInfo.getFullUrl() + fileName);
            request.Credentials = new NetworkCredential(m_ftpInfo.UserName, m_ftpInfo.UserPwd);
            request.EnableSsl = m_enable_ssh;
            request.Method = WebRequestMethods.Ftp.Rename;
            request.UseBinary = true;
            request.RenameTo = newName;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获得一个含有详细信息的string[]
        /// </summary>
        /// <returns></returns>
        public string[] getFileDetailList()
        {
            List<string> details = new List<string>();

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(m_ftpInfo.getFullUrl());
            request.UseBinary = true;
            request.EnableSsl = m_enable_ssh;
            request.Credentials = new NetworkCredential(m_ftpInfo.UserName, m_ftpInfo.UserPwd);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            FtpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    details.Add(line);
                    line = reader.ReadLine();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return details.ToArray();
        }
    }
}
