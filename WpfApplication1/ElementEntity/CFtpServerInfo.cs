using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1.ElementEntity
{
    /// <summary>
    /// 登录的ftp服務器相关的信息
    /// </summary>
    class CFtpServerInfo
    {
        public readonly string FTP_PREFIX = "ftp://";

        private string server_addr;
        private string file_path;
        private string user_name;
        private string user_pwd;

        /// <summary>
        /// FTP server的地址，IP或者非IP
        /// </summary>
        public string ServerAddr
        {
            set { server_addr = value; }
            get { return server_addr; }
        }

        /// <summary>
        /// 此文件在FTP server下的路径，包括最开始的'/'
        /// </summary>
        public string FilePath
        {
            set { file_path = value; }
            get { return file_path; }
        }

        /// <summary>
        /// 登录FTP server的用户名
        /// </summary>
        public string UserName
        {
            set { user_name = value; }
            get { return user_name; }
        }

        /// <summary>
        /// 登录FTP server的密码
        /// </summary>
        public string UserPwd
        {
            set { user_pwd = value; }
            get { return user_pwd; }
        }

        public CFtpServerInfo(string url)
        {
            url = url.Trim();
            if (!url.StartsWith("ftp://", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new System.ArgumentException("url should start with \"ftp://\"");
            }

            url = url.Substring(6);
            int index = url.IndexOf('@');
            if (index != -1)
            {
                //@ found
                string name_pwd = url.Substring(0, index);
                this.decodeNamePwd(name_pwd, out user_name, out user_pwd);
                url = url.Substring(index + 1);
            }

            index = url.IndexOf('/');
            if (index != -1)
            {
                server_addr = url.Substring(0, index);
                file_path = url.Substring(index);
            }
            else
            {
                server_addr = url;
                file_path = "";
            }
        }

        /// <summary>
        /// 根据FTP的下载URL的规则，从中读出用户名和密码
        /// </summary>
        /// <param name="src"></param>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        private void decodeNamePwd(string src, out string name, out string pwd)
        {
            int index = src.IndexOf(':');
            if (index == -1)
            {
                throw new ArgumentException("下载路径非法！应该含有':'作为用户名和密码的分隔符");
            }

            name = src.Substring(0, index);
            pwd = src.Substring(index + 1);
        }

        /// <summary>
        /// 返回完整的下载URL
        /// </summary>
        /// <returns></returns>
        public string getFullUrl()
        {
            return server_addr + file_path;
        }
    }
}
