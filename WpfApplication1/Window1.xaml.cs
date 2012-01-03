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
                ftpManager.onDownloadStarted += new CExternalFtpManage.FtpDownloadHandler(onDownloadStarted);
                ftpManager.onDownloadAdvanced += new CExternalFtpManage.FtpDownloadHandler(onDownloadAdvanced);
                ftpManager.onDownloadFinished += new CExternalFtpManage.FtpDownloadHandler(onDownloadFinished);
                ftpManager.onFileSizeRetrieved += new CExternalFtpManage.FtpDownloadHandler(onFileSizeRetrieved);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            try
            {
                ftpManager.startDownloading("C:\\Users\\Andriy\\Desktop", true);
                long size = ftpManager.FileSize;

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
                MessageBox.Show("Size: " + ftpManager.FileSize);
            }
        }
    }
}
