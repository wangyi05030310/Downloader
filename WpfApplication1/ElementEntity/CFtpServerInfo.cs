using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1.ElementEntity
{
    /// <summary>
    /// 登陸的ftp服務器的信息
    /// </summary>
    class CFtpServerInfo
    {
        private string server_addr;
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

        public CFtpServerInfo(string server_addr, string user_name = "", string user_pwd = "")
        {
            this.server_addr = server_addr;
            this.user_name = user_name;
            this.user_pwd = user_pwd;
        }
    }
}
