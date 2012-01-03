using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfApplication1.ElementEntity;
using System.IO;
using System.Net;

namespace WpfApplication1.BaseController
{
    /// <summary>
    /// FTP的管理（如同济FTP），操作下载之外的管理
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
        public void upload(string fileurl)
        {

            FileInfo fileInf = null;
            try
            {
                fileInf = new FileInfo(fileurl);
            }
            catch (System.Exception ex)
            {
                //传进来的file's url 有问题
                throw ex;
            }

            FtpWebRequest request;

            request = (FtpWebRequest)FtpWebRequest.Create("ftp://10.60.0.122/" + fileInf.Name);
            request.Credentials = new NetworkCredential(ftpInfo.UserName, ftpInfo.UserPwd);
            //            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.ContentLength = fileInf.Length;
            request.EnableSsl = enable_ssh;

            byte[] buff = new byte[BUFF_SIZE];
            int contentLen;

            FileStream fs = fileInf.OpenRead();
            Stream stream = null;
            try
            {
                stream = request.GetRequestStream();
                do
                {
                    contentLen = fs.Read(buff, 0, BUFF_SIZE);
                    stream.Write(buff, 0, contentLen);
                } while (contentLen != 0);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                fs.Close();
            }
        }

        /// <summary>
        /// 拿到ftp上此目录下所有文件名
        /// </summary>
        /// <returns></returns>
        public string[] getFileList()
        {
            List<string> files = new List<string>();

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpInfo.getFullUrl());
            request.UseBinary = true;
            request.EnableSsl = enable_ssh;
            request.Credentials = new NetworkCredential(ftpInfo.UserName, ftpInfo.UserPwd);
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
        public void delete()
        {

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
        public void renameFile()
        {
            FtpWebRequest request;
            //request.RenameTo
        }

        /// <summary>
        /// 获得一个含有详细信息的string[]
        /// </summary>
        /// <returns></returns>
        public string[] getFileDetailList()
        {
            return null;
        }
    }
}
