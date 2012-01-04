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
        private readonly string m_info;

        public string ErrorInfo
        {
            get { return m_info; }
        }

        public CFtpErrorArgs(string info)
        {
            this.m_info = info;
        }
    }

    /// <summary>
    /// 传递文件大小的EventArgs
    /// </summary>
    class CFileSizeArgs : EventArgs
    {
        private readonly long m_size;

        public long Size
        {
            get { return m_size; }
        }

        public CFileSizeArgs(long size)
        {
            this.m_size = size;
        }
    }

    /// <summary>
    /// 表示传输进度的EventArgs
    /// </summary>
    class CTransferProgressArgs : EventArgs
    {
        private readonly long m_total;
        private readonly long m_current;

        /// <summary>
        /// 一共有多少
        /// </summary>
        public long TotalSize
        {
            get { return m_total; }
        }

        /// <summary>
        /// 已经下载了多少
        /// </summary>
        public long CurrentSize
        {
            get { return m_current; }
        }

        public CTransferProgressArgs(long total, long current)
        {
            this.m_total = total;
            this.m_current = current;
        }
    }

    /// <summary>
    /// 保存与下载相关的信息
    /// </summary>
    class TransferState
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
        public Thread TransferThread
        {
            set { m_thread = value; }
            get { return m_thread; }
        }

        /// <summary>
        /// 这次transfer完毕了
        /// 一定要最后执行！！！
        /// </summary>
        public void finish()
        {
            m_target_path = "";
            m_total_size = 0;
            m_current_size = 0;
            if (m_thread != null)
            {
                try
                {
                    m_thread.Abort();
                }
                catch (System.Exception)
                {
                }
                m_thread = null;
            }
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

        private TransferState m_download_state = new TransferState();
        private TransferState m_upload_state = new TransferState();

        /// <summary>
        /// 文件总大小
        /// </summary>
        public long FileSize
        {
            get { return m_download_state.TotalSize; }
        }

        /// <summary>
        /// 此次操作的文件名
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

        public delegate void FtpHandler(object sender, EventArgs args);
        /// <summary>
        /// 下载开始！
        /// </summary>
        public event FtpHandler onDownloadStarted;
        /// <summary>
        /// 又下载了一点！
        /// </summary>
        public event FtpHandler onDownloadAdvanced;
        /// <summary>
        /// 下载结束！
        /// </summary>
        public event FtpHandler onDownloadFinished;
        /// <summary>
        /// 取得文件大小
        /// </summary>
        public event FtpHandler onFileSizeRetrieved;
        /// <summary>
        /// 上传开始！
        /// </summary>
        public event FtpHandler onUploadStarted;
        /// <summary>
        /// 上传结束！
        /// </summary>
        public event FtpHandler onUploadFinished;
        /// <summary>
        /// 又上传了一点！
        /// 但是我突然觉得这个没有什么意义，因为是记录的是本地写到stream里的过程
        /// </summary>
        public event FtpHandler onUploadAdvanced;
        /// <summary>
        /// 发生错误了!
        /// </summary>
        public event FtpHandler onFtpError;


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

            m_download_state.TransferThread = new Thread(new ThreadStart(doDownload));
            m_download_state.TransferThread.Start();
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
                onDownloadAdvanced(this, new CTransferProgressArgs(m_download_state.TotalSize, m_download_state.CurrentSize));
            } while (readCount > 0);

            remoteStream.Close();
            response.Close();
            fs.Close();

            onDownloadFinished(this, null);
            m_download_state.finish();
        }


        /// <summary>
        /// 获得指定一个文件的大小
        /// 单位是B，通过FileSize拿，
        /// 也可在发event的时候作为参数传出去
        /// </summary>
        private void retrieveFileSize()
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

            onFileSizeRetrieved(this, new CFileSizeArgs(m_download_state.TotalSize));
        }

        /// <summary>
        /// 开始上传文件到FTP上
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="fileName"></param>
        public void startUploading(string fileUrl, string fileName)
        {
            FileInfo fileInf = null;
            try
            {
                fileInf = new FileInfo(fileUrl);
            }
            catch (System.Exception)
            {
                throw new FilePathInvalidException("传进来的文件路径有问题！");
            }

            m_upload_state.TargetPath = fileUrl;
            m_upload_state.TransferThread = new Thread(new ParameterizedThreadStart(doUpload));
            m_upload_state.TransferThread.Start(fileName);
            onUploadStarted(this, null);
        }

        /// <summary>
        /// 真正在线程里执行上传的函数
        /// </summary>
        /// <param name="fileurl"></param>
        public void doUpload(object o)
        {
            string fileName = (string) o;

            FileInfo fileInf = new FileInfo(m_upload_state.TargetPath);

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
                onUploadAdvanced(this, new CTransferProgressArgs(m_upload_state.TotalSize, m_upload_state.CurrentSize));
            } while (readCount > 0);
            stream.Close();
            fs.Close();

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (System.Exception ex)
            {
                //这个时候才报异常，可能是网络啥的问题
                throw ex;
            }

            onUploadFinished(this, null);
            m_upload_state.finish();
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
