using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;

namespace WpfApplication1.BaseController
{
    class CFTPState
    {
            
    }

    /// <summary>
    /// 外部FTP资源的下载管理
    /// </summary>
    class CExternalFtpManage
    {
        private static readonly int BUFF_SIZE = 4096;

        private string url;
        private string user_name;
        private string user_pwd;

        /// <summary>
        /// 包含在url里的文件名
        /// </summary>
        private string file_name;

        /// <summary>
        /// 如果构造函数不抛异常，url就可以用
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        public CExternalFtpManage(string url, string name, string pwd)
        {
            if (url == null)
            {
                throw new System.ArgumentNullException("url should NOT be null!");
            }

            url.Trim();
            if (!url.StartsWith("ftp://", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new System.ArgumentException("url should start with \"ftp://\"");
            }

            try
            {
                WebRequest.Create(url);
            }
            catch (System.Exception ex)
            {
            	throw ex;
            }

            this.file_name = url.Substring(url.LastIndexOf('/') + 1);
            if (file_name == "")
            {
                throw new ArgumentException("URL should contain a file");
            }

            this.url = url;
            this.user_name = name;
            this.user_pwd = pwd;
        }


        /// <summary>
        /// 下载到指定路径
        /// </summary>
        /// <param name="targetPath"></param>
        public void download(string targetPath)
        {
            FileStream fs;
            try
            {
                fs = File.Create(targetPath + file_name);
            }
            catch (System.Exception)
            {
                throw new System.ArgumentException("target path not available!");
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);

            //request.EnableSsl = true;
            request.Credentials = new NetworkCredential(user_name, user_pwd);
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
            request.Credentials = new NetworkCredential(user_name, user_pwd);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.ContentLength = fileInf.Length;
//            request.EnableSsl = true;

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
    }
}
