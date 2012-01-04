using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.BaseController;
using WpfApplication1.ElementEntity;
using WpfApplication1.Util;

namespace WpfApplication1
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();


        }

        /// <summary>
        /// 用来测试的函数，界面一点，执行某个功能，只是用来测一测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void click_to_test(object sender, RoutedEventArgs e)
        {
            CExternalFtpManage ftpManager = null;
            try
            {
                CFtpServerInfo ftpInfo = new CFtpServerInfo("ftp://10.60.0.122/server.xml");
                ftpInfo = new CFtpServerInfo("ftp://10.60.0.122/各种软件/TortoiseSVN-1.6.14.21012-win32-svn-1.6.16.msi");
                ftpManager = new CExternalFtpManage(ftpInfo, false);
                ftpManager = new CExternalFtpManage(new CFtpServerInfo("ftp://10.60.0.122/"), false);

                ftpManager.onDownloadStarted += new CExternalFtpManage.FtpHandler(onDownloadStarted);
                ftpManager.onDownloadAdvanced += new CExternalFtpManage.FtpHandler(onDownloadAdvanced);
                ftpManager.onDownloadFinished += new CExternalFtpManage.FtpHandler(onDownloadFinished);

                ftpManager.onFileSizeRetrieved += new CExternalFtpManage.FtpHandler(onFileSizeRetrieved);

                ftpManager.onUploadStarted += new CExternalFtpManage.FtpHandler(onUploadStarted);
                ftpManager.onUploadAdvanced += new CExternalFtpManage.FtpHandler(onUploadAdvanced);
                ftpManager.onUploadFinished += new CExternalFtpManage.FtpHandler(onUploadFinished);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            try
            {
                //ftpManager.startDownloading("C:\\Users\\Andriy\\Desktop", true);

                ftpManager.startUploading("C:\\Users\\Andriy\\Desktop\\Advanced_Programming_in_The_Unix_Environment(2nd).chm", "Advanced_Programming_in_The_Unix_Environment(2nd).chm");
                //ftpManager.renameFile("Advanced_Programming_in_The_Unix_Environment(2nd).chm", "Unix高级环境编程.chm");
                //ftpManager.delete("Advanced_Programming_in_The_Unix_Environment(2nd).chm");
//                 string[] details = ftpManager.getFileDetailList();
//                 StringBuilder builder = new StringBuilder();
//                 foreach (string s in details)
//                 {
//                     builder.Append(s + "  |  ");
//                 }
//                 MessageBox.Show(builder.ToString());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("failed  : " + ex.ToString());
            }
        }

        private void onDownloadStarted(object o, EventArgs args)
        {
            CExternalFtpManage ftpManager = o as CExternalFtpManage;
            if (ftpManager != null)
            {
                MessageBox.Show("FTP下载" + ftpManager.FileName + "开始！");
            }
        }

        private void onDownloadAdvanced(object o, EventArgs args)
        {
            CExternalFtpManage ftpManager = o as CExternalFtpManage;
            if (ftpManager != null)
            {
                CTransferProgressArgs progressArgs = (CTransferProgressArgs) args;
                long total = progressArgs.TotalSize;
                long current = progressArgs.CurrentSize;
            }
        }

        private void onDownloadFinished(object o, EventArgs args)
        {
            CExternalFtpManage ftpManager = o as CExternalFtpManage;
            if (ftpManager != null)
            {
                MessageBox.Show("FTP下载" + ftpManager.FileName + "完成！");
            }
        }

        private void onFileSizeRetrieved(object o, EventArgs args)
        {
            CExternalFtpManage ftpManager = o as CExternalFtpManage;
            if (ftpManager != null)
            {
                CFileSizeArgs sizeArgs = (CFileSizeArgs) args;
                MessageBox.Show("Size: " + sizeArgs.Size);
            }
        }

        private void onUploadStarted(object o, EventArgs args)
        {
            CExternalFtpManage ftpManager = o as CExternalFtpManage;
            if (ftpManager != null)
            {
                MessageBox.Show(ftpManager.FileName + "上传开始");
            }
        }

        private void onUploadAdvanced(object o, EventArgs args)
        {
            CTransferProgressArgs progressArgs = (CTransferProgressArgs) args;
            long total = progressArgs.TotalSize;
            long current = progressArgs.CurrentSize;
        }

        private void onUploadFinished(object o, EventArgs args)
        {
            CExternalFtpManage ftpManager = o as CExternalFtpManage;
            if (ftpManager != null)
            {
                MessageBox.Show(ftpManager.FileName + "上传结束");
            }
        }
    }
}
