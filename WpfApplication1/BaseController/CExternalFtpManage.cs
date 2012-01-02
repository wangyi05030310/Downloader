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

        private bool enable_ssh;

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
        public CExternalFtpManage(string url, string name, string pwd, bool enable_ssh = true)
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
//                throw new ArgumentException("URL should contain a file");
            }

            this.url = url;
            this.user_name = name;
            this.user_pwd = pwd;
            this.enable_ssh = enable_ssh;
        }


        /// <summary>
        /// 下载到指定路径
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="overrite"></param>
        public void download(string targetPath, bool overrite = false)
        {
            if (!overrite && File.Exists(targetPath + file_name))
            {
                throw new ArgumentException("target path existed! Not allowed to overwrite!");
            }

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

            request.EnableSsl = enable_ssh;
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

            FtpWebRequest request = (FtpWebRequest) WebRequest.Create(url);
            request.UseBinary = true;
            request.EnableSsl = enable_ssh;
            request.Credentials = new NetworkCredential(user_name, user_pwd);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            FtpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                response = (FtpWebResponse) request.GetResponse();
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
        /// 获得指定一个文件的大小
        /// </summary>
        /// <returns></returns>
        public long getFileSize()
        {
            return 0;
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
