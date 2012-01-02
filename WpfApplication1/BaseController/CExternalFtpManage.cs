using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using WpfApplication1.ElementEntity;

namespace WpfApplication1.BaseController
{


    /// <summary>
    /// 外部FTP资源的下载管理
    /// </summary>
    class CExternalFtpManage
    {
        private static readonly int BUFF_SIZE = 4096;

        private CFtpServerInfo ftpInfo;

        private bool enable_ssh;


        /// <summary>
        /// 如果构造函数不抛异常，url就可以用
        /// </summary>
        /// <param name="ftpInfo"></param>
        /// <param name="enable_ssh"></param>
        public CExternalFtpManage(CFtpServerInfo ftpInfo, bool enable_ssh = true)
        {
            this.ftpInfo = ftpInfo;
            this.enable_ssh = enable_ssh;

            try
            {
                WebRequest.Create(ftpInfo.getFullUrl());
            }
            catch (System.Exception ex)
            {
            	throw ex;
            }
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

            FtpWebRequest request = (FtpWebRequest) WebRequest.Create(ftpInfo.getFullUrl());
            request.UseBinary = true;
            request.EnableSsl = enable_ssh;
            request.Credentials = new NetworkCredential(ftpInfo.UserName, ftpInfo.UserPwd);
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
