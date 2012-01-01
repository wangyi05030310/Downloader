﻿using System;
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
                ftpManager = new CExternalFtpManage("ftp://10.60.0.122/server.xml", "", "");

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            try
            {
//                ftpManager.upload("C:\\Users\\Andriy\\Desktop\\如何阅读一本书.txt");
                ftpManager.download("C:\\Users\\Andriy\\Desktop\\");
                MessageBox.Show("success");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("failed  : " + ex.ToString());
            }
        }
    }
}
