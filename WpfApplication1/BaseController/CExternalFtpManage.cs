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
    /// 包含某个错误的EventArgs
    /// </summary>
    class CFtpErrorArgs : EventArgs
    {
        private string info;

        public string ErrorInfo
        {
            get { return info; }
        }

        public CFtpErrorArgs(string info)
        {
            this.info = info;
        }
    }

    class DownloadState
    {
        private string m_target_path;
        private long m_total_size;
        private long m_current_size;
        private Thread m_thread;

        /// <summary>
        /// 下载到哪里
        /// </summary>
        public string TargetPath
        {
            set { m_target_path = value; }
            get { return m_target_path; }
        }

        /// <summary>
        /// 文件总大小
        /// </summary>
        public long TotalSize
        {
            set { m_total_size = value; }
            get { return m_total_size; }
        }

        /// <summary>
        /// 当前已下载了多少
        /// </summary>
        public long CurrentSize
        {
            set { m_current_size = value; }
            get { return m_current_size; }
        }

        /// <summary>
        /// 用于下载的线程
        /// </summary>
        public Thread DownloadThread
        {
            set { m_thread = value; }
            get { return m_thread; }
        }
    }

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

        private DownloadState m_download_state = new DownloadState();
        /// <summary>
        /// 文件总大小
        /// </summary>
        public long FileSize
        {
            get { return m_download_state.TotalSize; }
        }

        /// <summary>
        /// 下载的文件名
        /// </summary>
        public string FileName
        {
            get { return m_ftpInfo.FileName; }
        }

        /// <summary>
        /// 当前已下载大小
        /// </summary>
        public long CurrentSize
        {
            get { return m_download_state.CurrentSize; }
        }

        public delegate void FtpDownloadHandler(object sender, EventArgs args);
        /// <summary>
        /// 下载开始！
        /// </summary>
        public event FtpDownloadHandler onDownloadStarted;
        /// <summary>
        /// 又下载了一点！
        /// </summary>
        public event FtpDownloadHandler onDownloadAdvanced;
        /// <summary>
        /// 下载结束！
        /// </summary>
        public event FtpDownloadHandler onDownloadFinished;
        /// <summary>
        /// 取得文件大小
        /// </summary>
        public event FtpDownloadHandler onFileSizeRetrieved;
        /// <summary>
        /// 发生错误了!
        /// </summary>
        public event FtpDownloadHandler onFtpError;


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

            m_download_state.TargetPath = targetPath + '\\' + m_ftpInfo.FileName;

            m_download_state.DownloadThread = new Thread(new ThreadStart(doDownload));
            m_download_state.DownloadThread.Start();
            onDownloadStarted(this, null);
        }

        /// <summary>
        /// 在线程内执行下载的函数
        /// </summary>
        private void doDownload()
        {
            this.retrieveFileSize();

            FileStream fs = File.Create(m_download_state.TargetPath);
            fs.SetLength(m_download_state.TotalSize);

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
            m_download_state.CurrentSize = 0;
            byte[] buffer = new byte[BUFF_SIZE];
            int readCount;
            do
            {
                readCount = remoteStream.Read(buffer, 0, BUFF_SIZE);
                fs.Write(buffer, 0, readCount);
                m_download_state.CurrentSize += readCount;
                onDownloadAdvanced(this, null);
            } while (readCount > 0);

            remoteStream.Close();
            response.Close();
            fs.Close();

            onDownloadFinished(this, null);
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
            m_download_state.TotalSize = response.ContentLength;
            response.Close();

            onFileSizeRetrieved(this, null);
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
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(m_ftpInfo.getFullUrl() + fileName);
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
