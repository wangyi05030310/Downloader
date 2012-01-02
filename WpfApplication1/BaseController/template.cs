// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Net;
// using System.Net.Sockets;
// using System.IO;
// using System.Globalization;
// using System.Text.RegularExpressions;
// 
// namespace WpfApplication1.BaseController
// {
// 
//     public class FTPHelper : IDisposable
//     {
//         #region 声明事件
//         /// <summary>
//         /// 正在下载文件的事件
//         /// </summary>
//         public event FTPSendEventHandler FileDownloading;
//         private delegate void OnFileDownloadingDelegate(FTPConnect ftpConnect, long lTotalBytes, long lBytesTransfered);
// 
//         /// <summary>
//         /// 正在下载文件封装模式
//         /// </summary>
//         private void OnFileDownloading(FTPConnect ftpConnect, long lTotalBytes, long lBytesTransfered)
//         {
//             if (this.FileDownloading != null)
//             {
//                 if (lBytesTransfered > lTotalBytes)
//                     lBytesTransfered = lTotalBytes;
//                 System.ComponentModel.ISynchronizeInvoke aSynch = this.FileDownloading.Target as System.ComponentModel.ISynchronizeInvoke;
//                 if (aSynch != null && aSynch.InvokeRequired)
//                     aSynch.Invoke(new OnFileDownloadingDelegate(OnFileDownloading), new object[] { ftpConnect, lTotalBytes, lBytesTransfered });
//                 else
//                     this.FileDownloading(ftpConnect, new FTPSendEventArgs(lTotalBytes, lBytesTransfered));
//             }
//         }
// 
//         /// <summary>
//         /// 文件下载完成
//         /// </summary>
//         public event EventHandler FileDownloadCompleted;
//         private delegate void OnFileDownloadCompletedDelegate(FTPConnect ftpConnect);
//         /// <summary>
//         /// 文件下载完成封装模式
//         /// </summary>
//         private void OnFileDownloadCompleted(FTPConnect ftpConnect)
//         {
//             if (this.FileDownloadCompleted != null)
//             {
//                 System.ComponentModel.ISynchronizeInvoke aSynch = this.FileDownloadCompleted.Target as System.ComponentModel.ISynchronizeInvoke;
//                 if (aSynch != null && aSynch.InvokeRequired)
//                     aSynch.Invoke(new OnFileDownloadCompletedDelegate(OnFileDownloadCompleted), new object[] { ftpConnect });
//                 else
//                     this.FileDownloadCompleted(ftpConnect, new EventArgs());
//             }
//         }
//         /// <summary>
//         /// 取消正在下载的文件
//         /// </summary>
//         public event EventHandler FileDownloadCanceled;
//         private delegate void OnFileDownloadCanceledDelegate(FTPConnect ftpConnect);
//         /// <summary>
//         /// 取消正在下载的文件封装模式
//         /// </summary>
//         private void OnFileDownloadCanceled(FTPConnect ftpConnect)
//         {
//             if (this.FileDownloadCanceled != null)
//             {
//                 System.ComponentModel.ISynchronizeInvoke aSynch = this.FileDownloadCanceled.Target as System.ComponentModel.ISynchronizeInvoke;
//                 if (aSynch != null && aSynch.InvokeRequired)
//                     aSynch.Invoke(new OnFileDownloadCanceledDelegate(OnFileDownloadCanceled), new object[] { ftpConnect });
//                 else
//                     this.FileDownloadCanceled(ftpConnect, new EventArgs());
//             }
//         }
//         /// <summary>
//         /// 正在上传文件
//         /// </summary>
//         public event FTPSendEventHandler FileUploading;
//         private delegate void OnFileUploadingDelegate(FTPConnect ftpConnect, long lTotalBytes, long lBytesTransfered);
//         /// <summary>
//         /// 正在下载事件封装模式
//         /// </summary>
//         /// <param name="lTotalBytes"></param>
//         /// <param name="lBytesTransfered"></param>
//         private void OnFileUploading(FTPConnect ftpConnect, long lTotalBytes, long lBytesTransfered)
//         {
//             if (this.FileUploading != null)
//             {
//                 if (lBytesTransfered > lTotalBytes)
//                     lBytesTransfered = lTotalBytes;
//                 System.ComponentModel.ISynchronizeInvoke aSynch = this.FileUploading.Target as System.ComponentModel.ISynchronizeInvoke;
//                 if (aSynch != null && aSynch.InvokeRequired)
//                     aSynch.Invoke(new OnFileUploadingDelegate(OnFileUploading), new object[] { ftpConnect, lTotalBytes, lBytesTransfered });
//                 else
//                     this.FileUploading(ftpConnect, new FTPSendEventArgs(lTotalBytes, lBytesTransfered));
//             }
//         }
//         /// <summary>
//         /// 文件上传完成
//         /// </summary>
//         public event EventHandler FileUploadCompleted;
//         private delegate void OnFileUploadCompletedDelegate(FTPConnect ftpConnect);
//         private void OnFileUploadCompleted(FTPConnect ftpConnect)
//         {
//             if (this.FileUploadCompleted != null)
//             {
//                 System.ComponentModel.ISynchronizeInvoke aSynch = this.FileUploadCompleted.Target as System.ComponentModel.ISynchronizeInvoke;
//                 if (aSynch != null && aSynch.InvokeRequired)
//                     aSynch.Invoke(new OnFileUploadCompletedDelegate(OnFileUploadCompleted), new object[] { ftpConnect });
//                 else
//                     this.FileUploadCompleted(ftpConnect, new EventArgs());
//             }
//         }
//         /// <summary>
//         /// 取消了上传文件
//         /// </summary>
//         public event EventHandler FileUploadCanceled;
//         private delegate void OnFileUploadCanceledDelegate(FTPConnect ftpConnect);
//         private void OnFileUploadCanceled(FTPConnect ftpConnect)
//         {
//             if (this.FileUploadCanceled != null)
//             {
//                 System.ComponentModel.ISynchronizeInvoke aSynch = this.FileUploadCanceled.Target as System.ComponentModel.ISynchronizeInvoke;
//                 if (aSynch != null && aSynch.InvokeRequired)
//                     aSynch.Invoke(new OnFileUploadCanceledDelegate(OnFileUploadCanceled), new object[] { ftpConnect });
//                 else
//                     this.FileUploadCanceled(ftpConnect, new EventArgs());
//             }
//         }
//         /// <summary>
//         /// 传输过程发生错误事件
//         /// </summary>
//         public event FTPErrorEventHandler FtpError;
//         private delegate void OnFtpErrorDelegate(FTPConnect ftpConnect, Exception error);
//         public void OnFtpError(FTPConnect ftpConnect, Exception error)
//         {
//             if (this.FtpError != null)
//             {
//                 System.ComponentModel.ISynchronizeInvoke aSynch = this.FtpError.Target as System.ComponentModel.ISynchronizeInvoke;
//                 if (aSynch != null && aSynch.InvokeRequired)
//                     aSynch.Invoke(new OnFtpErrorDelegate(OnFtpError), new object[] { ftpConnect, error });
//                 else
//                     this.FtpError(ftpConnect, new FTPErrorEventArgs(error));
//             }
//         }
//         #endregion
//         #region FTPUrl结构
//         public class FTPUrl
//         {
//             private string m_Url = String.Empty;
//             private string m_RemoteHost = String.Empty;
//             private IPAddress m_RemoteHostIP = IPAddress.None;
//             private int m_RemotePort = 21;
//             private string m_UserName = String.Empty;
//             private string m_Password = String.Empty;
//             private string m_SubUrl = String.Empty;
//             public FTPUrl()
//             { }
//             public FTPUrl(string url)
//             {
//                 this.Url = url;
//             }
//             /// <summary>
//             /// FTP的全地址
//             /// </summary>
//             public string Url
//             {
//                 get { return this.m_Url; }
//                 set
//                 {
//                     if (value.IndexOf("@") < 0)
//                         throw (new Exception("FTP地址路径不合法！格式应为ftp://用户名:密码@地址[:端口]/"));
//                     string strSubUrl = "";
//                     string strRemoteHostAndPort = value.Substring(value.IndexOf("@") + 1);
//                     if (strRemoteHostAndPort.IndexOf("/") > 0)
//                     {
//                         strSubUrl = strRemoteHostAndPort.Substring(strRemoteHostAndPort.IndexOf("/"));
//                         strRemoteHostAndPort = strRemoteHostAndPort.Substring(0, strRemoteHostAndPort.IndexOf("/"));
//                     }
//                     string strRemoteHost = strRemoteHostAndPort;
//                     int iRemotePort = 21;
//                     if (strRemoteHostAndPort.IndexOf(":") > 0)
//                     {
//                         strRemoteHost = strRemoteHostAndPort.Substring(0, strRemoteHostAndPort.IndexOf(":"));
//                         string strRemotePort = strRemoteHostAndPort.Substring(strRemoteHostAndPort.IndexOf(":") + 1);
//                         if (!int.TryParse(strRemotePort, out iRemotePort))
//                             iRemotePort = 21;
//                     }
//                     string strUserNameAndPassword = value.Substring(0, value.IndexOf("@")).Trim();
//                     if (strUserNameAndPassword.ToLower().StartsWith("ftp://"))
//                         strUserNameAndPassword = strUserNameAndPassword.Substring(6).Trim();
//                     if (strUserNameAndPassword == string.Empty || strUserNameAndPassword.IndexOf(":") < 0)
//                         throw (new Exception("FTP地址路径不合法！格式应为ftp://用户名:密码@地址[:端口]/"));
//                     string strUserName = strUserNameAndPassword.Substring(0, strUserNameAndPassword.IndexOf(":"));
//                     string strPassword = strUserNameAndPassword.Substring(strUserNameAndPassword.IndexOf(":") + 1);
// 
//                     IPAddress[] ips = Dns.GetHostAddresses(strRemoteHost);
//                     if (ips.Length == 0)
//                         throw (new Exception("FTP地址路径中主机地址无效！"));
//                     strSubUrl = strSubUrl.Replace("//", "/");
//                     this.m_RemoteHostIP = ips[0];
//                     this.m_RemoteHost = strRemoteHost;
//                     this.m_RemotePort = iRemotePort;
//                     this.m_UserName = strUserName;
//                     this.m_Password = strPassword;
//                     this.m_SubUrl = strSubUrl;
//                     this.m_Url = value;
//                 }
//             }
//             /// <summary>
//             /// 主机地址
//             /// </summary>
//             public string RemoteHost
//             {
//                 get { return this.m_RemoteHost; }
//             }
//             /// <summary>
//             /// 主机IP
//             /// </summary>
//             public IPAddress RemoteHostIP
//             {
//                 get { return this.m_RemoteHostIP; }
//             }
//             /// <summary>
//             /// 主机端口
//             /// </summary>
//             public int RemotePort
//             {
//                 get { return this.m_RemotePort; }
//             }
//             public string UserName
//             {
//                 get { return this.m_UserName; }
//             }
//             public string Password
//             {
//                 get { return this.m_Password; }
//             }
//             public string SubUrl
//             {
//                 get { return this.m_SubUrl; }
//             }
//         }
//         #endregion
//         #region 传输模式
//         /// <summary>
//         /// 传输模式:二进制类型、ASCII类型
//         /// </summary>
//         public enum TransferType
//         {
//             /// <summary>
//             /// Binary
//             /// </summary>
//             Binary,
//             /// <summary>
//             /// ASCII
//             /// </summary>
//             ASCII
//         };
//         #endregion
//         #region 存贮FTP的连接结构类
//         public class FTPConnect
//         {
//             #region 私有字段
//             /// <summary>
//             /// 数据传送套接字列表
//             /// </summary>
//             private List<Socket> m_DataSocketList;
//             private string m_ID;
//             /// <summary>
//             /// 唯一ID
//             /// </summary>
//             public string ID
//             {
//                 get { return this.m_ID; }
//             }
//             private object m_Tag = null;
//             /// <summary>
//             /// 扩展标记
//             /// </summary>
//             public object Tag
//             {
//                 get { return this.m_Tag; }
//                 set { this.m_Tag = value; }
//             }
//             private bool m_DataTransmitting = false;
//             /// <summary>
//             /// 数据正在传输 标记
//             /// </summary>
//             public bool DataTransmitting
//             {
//                 get { return this.m_DataTransmitting; }
//                 set { this.m_DataTransmitting = value; }
//             }
//             private Socket m_SocketControl;
//             /// <summary>
//             /// FTPUrl
//             /// </summary>
//             private FTPUrl m_FTPUrl;
//             /// <summary>
//             /// 是否已经连接
//             /// </summary>
//             private bool m_IsConnected;
//             private Encoding m_EncodeType = Encoding.Default;
//             /// <summary>
//             /// 编码方式
//             /// </summary>
//             public Encoding EncodeType
//             {
//                 get { return this.m_EncodeType; }
//                 set { this.m_EncodeType = value; }
//             }
//             /// <summary>
//             /// 接收和发送数据的缓冲区
//             /// </summary>
//             private static int BLOCK_SIZE = 512;
//             /// <summary>
//             /// 缓冲区大小
//             /// </summary>
//             private Byte[] m_Buffer;
//             public Byte[] Buffer
//             {
//                 get { return this.m_Buffer; }
//                 set { this.m_Buffer = value; }
//             }
//             private string m_Message;
//             /// <summary>
//             /// 当前的消息
//             /// </summary>
//             public string Message
//             {
//                 get { return this.m_Message; }
//                 set { this.m_Message = value; }
//             }
//             private string m_ReplyString;
//             /// <summary>
//             /// 应答字符串
//             /// </summary>
//             public string ReplyString
//             {
//                 get { return this.m_ReplyString; }
//                 set { this.m_ReplyString = value; }
//             }
//             private int m_ReplyCode;
//             /// <summary>
//             /// 应答代码
//             /// </summary>
//             public int ReplyCode
//             {
//                 get { return this.m_ReplyCode; }
//                 set { this.m_ReplyCode = value; }
//             }
//             /// <summary>
//             /// 传输模式
//             /// </summary>
//             private TransferType trType;
//             #endregion
//             public FTPConnect()
//             {
//                 this.m_ID = System.Guid.NewGuid().ToString();
//                 this.m_DataSocketList = new List<Socket>();
//                 this.m_Buffer = new Byte[BLOCK_SIZE];
//             }
//             public FTPConnect(FTPUrl ftpUrl)
//             {
//                 this.m_ID = System.Guid.NewGuid().ToString();
//                 this.m_DataSocketList = new List<Socket>();
//                 this.m_Buffer = new Byte[BLOCK_SIZE];
//                 this.FTPUrl = ftpUrl;
//             }
//             public FTPConnect(FTPUrl ftpUrl, string ftpId)
//             {
//                 if (String.IsNullOrEmpty(ftpId))
//                     ftpId = System.Guid.NewGuid().ToString();
//                 this.m_ID = ftpId;
//                 this.m_DataSocketList = new List<Socket>();
//                 this.m_Buffer = new Byte[BLOCK_SIZE];
//                 this.FTPUrl = ftpUrl;
//             }
//             #region 公共字段
//             /// <summary>
//             /// 套接字连接
//             /// </summary>
//             public Socket SocketControl
//             {
//                 get { return this.m_SocketControl; }
//                 set { this.m_SocketControl = value; }
//             }
//             /// <summary>
//             /// 对应的URL
//             /// </summary>
//             public FTPUrl FTPUrl
//             {
//                 get { return this.m_FTPUrl; }
//                 set { this.m_FTPUrl = value; }
//             }
//             /// <summary>
//             /// 是否已经连接
//             /// </summary>
//             public bool IsConnected
//             {
//                 get { return this.m_IsConnected; }
//                 set { this.m_IsConnected = value; }
//             }
//             #endregion
//             #region 公共方法
//             #region 取消传送数据
//             public void CancelDataTransmit()
//             {
//                 this.m_DataTransmitting = false;
//             }
//             #endregion
//             #region 发送命令
//             /// <summary>
//             /// 发送命令并获取应答码和最后一行应答字符串
//             /// </summary>
//             /// <param name="strCommand">命令</param>
//             public void SendCommand(string strCommand)
//             {
//                 if (this.m_SocketControl == null)
//                     throw (new Exception("请先连接服务器再进行操作！"));
//                 Byte[] cmdBytes = m_EncodeType.GetBytes((strCommand + "/r/n").ToCharArray());
//                 this.m_SocketControl.Send(cmdBytes, cmdBytes.Length, 0);
//                 this.ReadReply();
//             }
//             #endregion
//             #region 读取最后一行的消息
//             /// <summary>
//             /// 读取Socket返回的所有字符串
//             /// </summary>
//             /// <returns>包含应答码的字符串行</returns>
//             private string ReadLine()
//             {
//                 if (this.m_SocketControl == null)
//                     throw (new Exception("请先连接服务器再进行操作！"));
//                 while (true)
//                 {
//                     int iBytes = this.m_SocketControl.Receive(m_Buffer, m_Buffer.Length, 0);
//                     m_Message += m_EncodeType.GetString(m_Buffer, 0, iBytes);
//                     if (iBytes < m_Buffer.Length)
//                     {
//                         break;
//                     }
//                 }
//                 char[] seperator = { '\n' };
//                 string[] mess = m_Message.Split(seperator);
//                 if (m_Message.Length > 2)
//                 {
//                     m_Message = mess[mess.Length - 2];
//                     //seperator[0]是10,换行符是由13和0组成的,分隔后10后面虽没有字符串,
//                     //但也会分配为空字符串给后面(也是最后一个)字符串数组,
//                     //所以最后一个mess是没用的空字符串
//                     //但为什么不直接取mess[0],因为只有最后一行字符串应答码与信息之间有空格
//                 }
//                 else
//                 {
//                     m_Message = mess[0];
//                 }
//                 if (!m_Message.Substring(3, 1).Equals(" "))//返回字符串正确的是以应答码(如220开头,后面接一空格,再接问候字符串)
//                 {
//                     return this.ReadLine();
//                 }
//                 return m_Message;
//             }
//             #endregion
//             #region 读取应答代码
//             /// <summary>
//             /// 将一行应答字符串记录在strReply和strMsg
//             /// 应答码记录在iReplyCode
//             /// </summary>
//             public void ReadReply()
//             {
//                 this.m_Message = "";
//                 this.m_ReplyString = this.ReadLine();
//                 this.m_ReplyCode = Int32.Parse(m_ReplyString.Substring(0, 3));
//             }
//             #endregion
//             #region 断开连接
//             /// <summary>
//             /// 关闭连接
//             /// </summary>
//             public void DisConnect()
//             {
//                 this.m_DataTransmitting = false;
//                 while (this.m_DataSocketList.Count > 0)
//                 {
//                     Socket socket = this.m_DataSocketList[0];
//                     if (socket != null && socket.Connected)
//                         socket.Close();
//                     this.m_DataSocketList.RemoveAt(0);
//                 }
//                 if (this.m_IsConnected && this.m_SocketControl != null)
//                     this.SendCommand("QUIT");
//                 this.CloseSocketConnect();
//                 this.m_Buffer = null;
//             }
//             /// <summary>
//             /// 关闭socket连接(用于登录以前)
//             /// </summary>
//             private void CloseSocketConnect()
//             {
//                 if (this.m_SocketControl != null && this.m_SocketControl.Connected)
//                 {
//                     this.m_SocketControl.Close();
//                     this.m_SocketControl = null;
//                 }
//                 this.m_IsConnected = false;
//             }
//             #endregion
//             #region 连接服务器
//             public void Connect()
//             {
//                 this.DisConnect();//先断开现有连接
//                 this.m_Buffer = new byte[BLOCK_SIZE];
//                 this.m_SocketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                 IPEndPoint ep = new IPEndPoint(this.m_FTPUrl.RemoteHostIP, this.m_FTPUrl.RemotePort);
//                 try
//                 {
//                     this.m_SocketControl.Connect(ep);
//                 }
//                 catch (Exception)
//                 {
//                     throw new IOException(String.Format("无法连接到远程服务器{0}！", this.m_FTPUrl.RemoteHost));
//                 }
//                 // 获取应答码
//                 this.ReadReply();
//                 if (m_ReplyCode != 220)
//                 {
//                     this.DisConnect();
//                     throw new IOException(m_ReplyString.Substring(4));
//                 }
//                 // 登陆
//                 this.SendCommand("USER " + this.m_FTPUrl.UserName);
//                 if (!(m_ReplyCode == 331 || m_ReplyCode == 230))
//                 {
//                     this.CloseSocketConnect();//关闭连接
//                     throw new IOException(m_ReplyString.Substring(4));
//                 }
//                 if (m_ReplyCode != 230)
//                 {
//                     this.SendCommand("PASS " + this.m_FTPUrl.Password);
//                     if (!(m_ReplyCode == 230 || m_ReplyCode == 202))
//                     {
//                         this.CloseSocketConnect();//关闭连接
//                         throw new IOException(m_ReplyString.Substring(4));
//                     }
//                 }
//                 this.m_IsConnected = true;
//             }
//             #endregion
//             #region 改变目录
//             /// <summary>
//             /// 改变目录
//             /// </summary>
//             /// <param name="strDirName">新的工作目录名</param>
//             public void ChangeDir(string dirName)
//             {
//                 if (!this.m_IsConnected)
//                     throw (new Exception("请先连接服务器然后再进行CWD操作！"));
//                 if (dirName.Equals(".") || dirName.Equals(""))
//                     return;
//                 this.SendCommand("CWD " + dirName);
//                 if (m_ReplyCode != 250)
//                     throw new IOException(m_ReplyString.Substring(4));
//             }
//             #endregion
//             #region 传输模式
//             /// <summary>
//             /// 设置传输模式
//             /// </summary>
//             /// <param name="ttType">传输模式</param>
//             public void SetTransferType(TransferType ttType)
//             {
//                 if (ttType == TransferType.Binary)
//                 {
//                     this.SendCommand("TYPE I");//binary类型传输
//                 }
//                 else
//                 {
//                     this.SendCommand("TYPE A");//ASCII类型传输
//                 }
//                 if (m_ReplyCode != 200)
//                 {
//                     throw new IOException(m_ReplyString.Substring(4));
//                 }
//                 else
//                 {
//                     trType = ttType;
//                 }
//             }
//             /// <summary>
//             /// 获得传输模式
//             /// </summary>
//             /// <returns>传输模式</returns>
//             public TransferType GetTransferType()
//             {
//                 return trType;
//             }
//             #endregion
//             #region 建立进行数据连接的socket
//             /// <summary>
//             /// 建立进行数据连接的socket
//             /// </summary>
//             /// <returns>数据连接socket</returns>
//             public Socket CreateDataSocket()
//             {
//                 this.SendCommand("PASV");
//                 if (this.m_ReplyCode != 227)
//                     throw new IOException(this.m_ReplyString.Substring(4));
//                 int index1 = this.m_ReplyString.IndexOf('(');
//                 int index2 = this.m_ReplyString.IndexOf(')');
//                 string ipData = this.m_ReplyString.Substring(index1 + 1, index2 - index1 - 1);
//                 int[] parts = new int[6];
//                 int len = ipData.Length;
//                 int partCount = 0;
//                 string buf = "";
//                 for (int i = 0; i < len && partCount <= 6; i++)
//                 {
//                     char ch = Char.Parse(ipData.Substring(i, 1));
//                     if (Char.IsDigit(ch))
//                         buf += ch;
//                     else if (ch != ',')
//                     {
//                         throw new IOException("Malformed PASV Reply: " + this.m_ReplyString);
//                     }
//                     if (ch == ',' || i + 1 == len)
//                     {
//                         try
//                         {
//                             parts[partCount++] = Int32.Parse(buf);
//                             buf = "";
//                         }
//                         catch (Exception)
//                         {
//                             throw new IOException("Malformed PASV Reply: " + this.m_ReplyString);
//                         }
//                     }
//                 }
//                 string ipAddress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];
//                 int port = (parts[4] << 8) + parts[5];
//                 Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                 IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
//                 try
//                 {
//                     socket.Connect(ep);
//                 }
//                 catch (Exception)
//                 {
//                     throw new IOException(String.Format("无法连接到远程服务器{0}:{1}！", ipAddress, port));
//                 }
//                 this.m_DataSocketList.Add(socket);
//                 return socket;
//             }
//             #endregion
//             #endregion
//         }
//         #endregion
//         #region 变量区
//         /// <summary>
//         /// 进行控制连接的socket
//         /// </summary>
//         private List<FTPConnect> m_FTPConnectList;
//         private object m_Tag;
//         /// <summary>
//         /// 标记
//         /// </summary>
//         public object Tag
//         {
//             get { return this.m_Tag; }
//             set { this.m_Tag = value; }
//         }
//         #endregion
// 
//         #region 实例化
//         public FTPHelper()
//         {
//             this.m_FTPConnectList = new List<FTPConnect>();
//         }
//         #endregion
//         #region Dispose
//         public void Dispose()
//         {
//             while (this.m_FTPConnectList.Count > 0)
//             {
//                 FTPConnect ftpConnect = this.m_FTPConnectList[0];
//                 ftpConnect.DisConnect();
//                 this.m_FTPConnectList.Remove(ftpConnect);
//             }
//         }
//         #endregion
//         #region 得到文件大小
//         /// <summary>
//         /// 得到文件大小
//         /// </summary>
//         /// <param name="fileUrl">目标文件，包含用户名与密码。如：ftp://username:password@127.0.0.1/1.txt</param>
//         /// <param name="UserName">用户名</param>
//         /// <param name="Password">密码</param>
//         /// <returns></returns>
//         public long GetFileSize(string fileUrl)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             try
//             {
//                 ftpUrl = new FTPUrl(fileUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 string strDirName = ftpUrl.SubUrl;
//                 if (strDirName.IndexOf("/") >= 0)
//                     strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirName);
//                 ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
//                 long lFileSize = 0;
//                 if (ftpConnect.ReplyCode == 213)
//                     lFileSize = Int64.Parse(ftpConnect.ReplyString.Substring(4));
//                 else
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 return lFileSize;
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (ftpConnect != null)
//                     ftpConnect.DisConnect();
//             }
//         }
//         #endregion
//         #region 得到文件大小
//         /// <summary>
//         /// 得到文件大小
//         /// </summary>
//         /// <param name="fileUrl">目标文件，包含用户名与密码。如：ftp://username:password@127.0.0.1/1.txt</param>
//         /// <param name="UserName">用户名</param>
//         /// <param name="Password">密码</param>
//         /// <returns></returns>
//         public DateTime GetDateTimestamp(string fileUrl)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             try
//             {
//                 ftpUrl = new FTPUrl(fileUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 string strDirName = ftpUrl.SubUrl;
//                 if (strDirName.IndexOf("/") >= 0)
//                     strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirName);
//                 ftpConnect.SendCommand("MDTM " + Path.GetFileName(ftpUrl.SubUrl));
//                 if (ftpConnect.ReplyCode == 213)
//                 {
//                     string strDateTime = ftpConnect.ReplyString.Substring(4);
//                     int iYear = 0, iMonth = 0, iDay = 0, iHour = 0, iMinute = 0, iSecond = 0;
//                     if (strDateTime.Length >= 4)
//                         int.TryParse(strDateTime.Substring(0, 4), out iYear);
//                     if (strDateTime.Length >= 6)
//                         int.TryParse(strDateTime.Substring(4, 2), out iMonth);
//                     if (strDateTime.Length >= 8)
//                         int.TryParse(strDateTime.Substring(6, 2), out iDay);
//                     if (strDateTime.Length >= 10)
//                         int.TryParse(strDateTime.Substring(8, 2), out iHour);
//                     if (strDateTime.Length >= 12)
//                         int.TryParse(strDateTime.Substring(10, 2), out iMinute);
//                     if (strDateTime.Length >= 14)
//                         int.TryParse(strDateTime.Substring(12, 2), out iSecond);
//                     return new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond).ToLocalTime();
//                 }
//                 else
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (ftpConnect != null)
//                     ftpConnect.DisConnect();
//             }
//         }
//         #endregion
//         #region 删除指定的文件
//         /// <summary>
//         /// 删除指定的文件
//         /// </summary>
//         /// <param name="fileUrl">待删除文件名</param>
//         public void DeleteFile(string fileUrl)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             try
//             {
//                 ftpUrl = new FTPUrl(fileUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 string strDirName = ftpUrl.SubUrl;
//                 if (strDirName.IndexOf("/") >= 0)
//                     strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirName);
//                 ftpConnect.SendCommand("DELE " + Path.GetFileName(ftpUrl.SubUrl));
//                 if (ftpConnect.ReplyCode != 250)
//                     throw (new Exception(ftpConnect.ReplyString.Substring(4)));
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (ftpConnect != null)
//                     ftpConnect.DisConnect();
//             }
//         }
//         #endregion
//         #region 重命名
//         /// <summary>
//         /// 重命名(如果新文件名与已有文件重名,将覆盖已有文件)
//         /// </summary>
//         /// <param name="strOldFileName">旧文件名</param>
//         /// <param name="strNewFileName">新文件名</param>
//         public void Rename(string originalUrl, string newName)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             try
//             {
//                 ftpUrl = new FTPUrl(originalUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 string strDirName = ftpUrl.SubUrl;
//                 if (strDirName.IndexOf("/") >= 0)
//                     strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirName);
//                 ftpConnect.SendCommand("RNFR " + Path.GetFileName(ftpUrl.SubUrl));
//                 if (ftpConnect.ReplyCode != 350)
//                     throw (new Exception(ftpConnect.ReplyString.Substring(4)));
//                 ftpConnect.SendCommand("RNTO " + newName);
//                 if (ftpConnect.ReplyCode != 250)
//                     throw (new Exception(ftpConnect.ReplyString.Substring(4)));
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (ftpConnect != null)
//                     ftpConnect.DisConnect();
//             }
//         }
//         #endregion
//         #region 创建文件夹
//         public void MakeDirectory(string dirUrl)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             int iCharIndex = 0;
//             string strDirUrl, strDirUrlTemp;
//             try
//             {
//                 ftpUrl = new FTPUrl(dirUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 strDirUrl = ftpUrl.SubUrl;
//                 while (true && strDirUrl.Trim() != "")
//                 {
//                     iCharIndex = strDirUrl.IndexOf("/", iCharIndex) + 1;
//                     if (iCharIndex == 0)
//                         strDirUrlTemp = strDirUrl;
//                     else
//                         strDirUrlTemp = strDirUrl.Substring(0, iCharIndex);
//                     if (strDirUrlTemp == "")
//                         continue;
//                     ftpConnect.SendCommand("MKD " + strDirUrlTemp);
//                     if (ftpConnect.ReplyCode != 257 && ftpConnect.ReplyCode != 550)
//                         throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     if (strDirUrlTemp == strDirUrl)
//                         break;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (ftpConnect != null)
//                     ftpConnect.DisConnect();
//             }
//         }
//         #endregion
//         #region 删除目录
//         /// <summary>
//         /// 删除目录
//         /// </summary>
//         /// <param name="dirUrl">目录名</param>
//         public void RemoveDirectory(string dirUrl)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             try
//             {
//                 ftpUrl = new FTPUrl(dirUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 string strDirPath = ftpUrl.SubUrl;
//                 string strDirName = ftpUrl.SubUrl;
//                 if (strDirPath.IndexOf("/") >= 0)
//                 {
//                     strDirPath = strDirPath.Substring(0, strDirPath.LastIndexOf("/"));
//                     strDirName = strDirPath.Substring(strDirPath.LastIndexOf("/") + 1);
//                 }
//                 ftpConnect.ChangeDir(strDirPath);
//                 ftpConnect.SendCommand("RMD " + strDirName);
//                 if (ftpConnect.ReplyCode != 250)
//                     throw (new Exception(ftpConnect.ReplyString.Substring(4)));
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (ftpConnect != null)
//                     ftpConnect.DisConnect();
//             }
//         }
//         #endregion
//         #region 得到简单的文件列表
//         public string[] ListDirectory(string listUrl)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             Socket socketData = null;
//             try
//             {
//                 ftpUrl = new FTPUrl(listUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 string strDirPath = ftpUrl.SubUrl;
//                 if (strDirPath.IndexOf("/") >= 0)
//                     strDirPath = strDirPath.Substring(0, strDirPath.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirPath);
//                 socketData = ftpConnect.CreateDataSocket();
//                 ftpConnect.SendCommand("NLST");
//                 if (ftpConnect.ReplyCode != 150)
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 //获得结果
//                 ftpConnect.Message = "";
//                 while (true)
//                 {
//                     int iBytes = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
//                     ftpConnect.Message += ftpConnect.EncodeType.GetString(ftpConnect.Buffer, 0, iBytes);
//                     if (iBytes < ftpConnect.Buffer.Length)
//                         break;
//                 }
//                 ftpConnect.Message = ftpConnect.Message.Replace('\r', ' ');
//                 char[] seperator = { '\n' };
//                 string[] strsFileList = ftpConnect.Message.Split(seperator);
//                 socketData.Close();//数据socket关闭时也会有返回码
//                 socketData = null;
//                 if (ftpConnect.ReplyCode != 226)
//                 {
//                     ftpConnect.ReadReply();
//                     if (ftpConnect.ReplyCode != 226)
//                         throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 }
//                 return strsFileList;
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (socketData != null && socketData.Connected)
//                     socketData.Close();
//                 socketData = null;
//                 if (ftpConnect != null)
//                     ftpConnect.DisConnect();
//             }
//         }
//         #endregion
//         #region 得到详细的文件列表
//         public List<FileStruct> ListDirectoryDetails(string listUrl)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             Socket socketData = null;
//             try
//             {
//                 ftpUrl = new FTPUrl(listUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 string strDirPath = ftpUrl.SubUrl;
//                 if (strDirPath.IndexOf("/") >= 0)
//                     strDirPath = strDirPath.Substring(0, strDirPath.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirPath);
//                 socketData = ftpConnect.CreateDataSocket();
//                 ftpConnect.SendCommand("LIST");
//                 if (ftpConnect.ReplyCode != 150)
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 //获得结果
//                 ftpConnect.Message = "";
//                 while (true)
//                 {
//                     int iBytes = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
//                     ftpConnect.Message += ftpConnect.EncodeType.GetString(ftpConnect.Buffer, 0, iBytes);
//                     if (iBytes < ftpConnect.Buffer.Length)
//                         break;
//                 }
//                 if (ftpConnect.Message.StartsWith("t"))
//                     ftpConnect.Message = ftpConnect.Message.Substring(ftpConnect.Message.IndexOf("/r/n") + 2);
//                 return this.GetList(ftpConnect.Message);
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (socketData != null && socketData.Connected)
//                     socketData.Close();
//                 socketData = null;
//                 if (ftpConnect != null)
//                     ftpConnect.DisConnect();
//             }
//         }
//         #region 用于得到文件列表的方法
//         /// <summary>
//         /// 文件列表类型枚举
//         /// </summary>
//         private enum FileListStyle
//         {
//             /// <summary>
//             /// Unix系统类型
//             /// </summary>
//             UnixStyle,
//             /// <summary>
//             /// Windows系统类型
//             /// </summary>
//             WindowsStyle,
//             /// <summary>
//             /// 未知类型
//             /// </summary>
//             Unknown
//         }
//         /// <summary>
//         /// 列出FTP服务器上面当前目录的所有文件
//         /// </summary>
//         /// <param name="listUrl">查看目标目录</param>
//         /// <returns>返回文件信息列表</returns>
//         public List<FileStruct> ListFiles(string listUrl)
//         {
//             List<FileStruct> listFile = null;
//             try
//             {
//                 List<FileStruct> listAll = this.ListDirectoryDetails(listUrl);
//                 listFile = new List<FileStruct>();
//                 foreach (FileStruct file in listAll)
//                 {
//                     if (!file.IsDirectory)
//                     {
//                         listFile.Add(file);
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }
//             return listFile;
//         }
//         /// <summary>
//         /// 列出FTP服务器上面当前目录的所有的目录
//         /// </summary>
//         /// <param name="listUrl">查看目标目录</param>
//         /// <returns>返回目录信息列表</returns>
//         public List<FileStruct> ListDirectories(string listUrl)
//         {
//             List<FileStruct> listAll = this.ListDirectoryDetails(listUrl);
//             List<FileStruct> listDirectory = new List<FileStruct>();
//             foreach (FileStruct file in listAll)
//             {
//                 if (file.IsDirectory)
//                 {
//                     listDirectory.Add(file);
//                 }
//             }
//             return listDirectory;
//         }
//         /// <summary>
//         /// 获得文件和目录列表
//         /// </summary>
//         /// <param name="datastring">FTP返回的列表字符信息</param>
//         private List<FileStruct> GetList(string datastring)
//         {
//             List<FileStruct> myListArray = new List<FileStruct>();
//             string[] dataRecords = datastring.Split('\n');
//             FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
//             foreach (string s in dataRecords)
//             {
//                 if (_directoryListStyle != FileListStyle.Unknown && s != "")
//                 {
//                     FileStruct f = new FileStruct();
//                     f.Name = "..";
//                     switch (_directoryListStyle)
//                     {
//                         case FileListStyle.UnixStyle:
//                             f = ParseFileStructFromUnixStyleRecord(s);
//                             break;
//                         case FileListStyle.WindowsStyle:
//                             f = ParseFileStructFromWindowsStyleRecord(s);
//                             break;
//                     }
//                     if (!(f.Name == "." || f.Name == ".."))
//                     {
//                         myListArray.Add(f);
//                     }
//                 }
//             }
//             return myListArray;
//         }
// 
//         /// <summary>
//         /// 从Windows格式中返回文件信息
//         /// </summary>
//         /// <param name="Record">文件信息</param>
//         private FileStruct ParseFileStructFromWindowsStyleRecord(string Record)
//         {
//             FileStruct f = new FileStruct();
//             string processstr = Record.Trim();
//             string dateStr = processstr.Substring(0, 8);
//             processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
//             string timeStr = processstr.Substring(0, 7);
//             processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
//             DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
//             myDTFI.ShortTimePattern = "t";
//             f.CreateTime = DateTime.Parse(dateStr + " " + timeStr, myDTFI);
//             if (processstr.Substring(0, 5) == "<DIR>")
//             {
//                 f.IsDirectory = true;
//                 processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
//             }
//             else
//             {
//                 //string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);   // true);
//                 //processstr = strs[1];
//                 processstr = processstr.Substring(processstr.IndexOf(' ') + 1);
//                 f.IsDirectory = false;
//             }
//             f.Name = processstr;
//             return f;
//         }
//         /// <summary>
//         /// 根据文件列表记录猜想文件列表类型
//         /// </summary>
//         /// <param name="recordList"></param>
//         /// <returns></returns>
//         private FileListStyle GuessFileListStyle(string[] recordList)
//         {
//             foreach (string s in recordList)
//             {
//                 if (s.Length > 10 && Regex.IsMatch(s.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
//                 {
//                     return FileListStyle.UnixStyle;
//                 }
//                 else if (s.Length > 8 && Regex.IsMatch(s.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
//                 {
//                     return FileListStyle.WindowsStyle;
//                 }
//             }
//             return FileListStyle.Unknown;
//         }
//         /// <summary>
//         /// 从Unix格式中返回文件信息
//         /// </summary>
//         /// <param name="Record">文件信息</param>
//         private FileStruct ParseFileStructFromUnixStyleRecord(string Record)
//         {
//             FileStruct f = new FileStruct();
//             string processstr = Record.Trim();
//             f.Flags = processstr.Substring(0, 10);
//             f.IsDirectory = (f.Flags[0] == 'd');
//             processstr = (processstr.Substring(11)).Trim();
//             _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
//             f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
//             f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
//             f.FileSize = Convert.ToInt32(_cutSubstringFromStringWithTrim(ref processstr, ' ', 0));
//             //_cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
//             string yearOrTime = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
//             if (yearOrTime.IndexOf(":") >= 0)  //time
//             {
//                 processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
//             }
//             f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
//             f.Name = processstr;   //最后就是名称
//             return f;
//         }
//         /// <summary>
//         /// 按照一定的规则进行字符串截取
//         /// </summary>
//         /// <param name="s">截取的字符串</param>
//         /// <param name="c">查找的字符</param>
//         /// <param name="startIndex">查找的位置</param>
//         private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
//         {
//             int pos1 = s.IndexOf(c, startIndex);
//             string retString = s.Substring(0, pos1);
//             s = (s.Substring(pos1)).Trim();
//             return retString;
//         }
//         #endregion
//         #endregion
//         #region 上传文件
//         /// <summary>
//         /// 直接上传文件
//         /// </summary>
//         /// <param name="uploadUrl">上传的目标全路径与文件名</param>
//         /// <param name="isContinueUpload">是否断点续传</param>
//         /// <returns>上传是否成功</returns>
//         public void UploadFileNow(byte[] fileBytes, string uploadUrl, bool isContinueUpload)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             Socket socketData = null;
//             int iCharIndex = 0;
//             int bytesRead;
//             long lOffset = 0, lTotalReaded = 0;
//             string strDirUrl, strDirUrlTemp;
//             try
//             {
//                 ftpUrl = new FTPUrl(uploadUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 this.m_FTPConnectList.Add(ftpConnect);//添加到控制器中
//                 #region 判断并创建文件夹
//                 strDirUrl = ftpUrl.SubUrl;
//                 if (strDirUrl.IndexOf("/") >= 0)
//                     strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/"));
//                 while (true && strDirUrl.Trim() != "")
//                 {
//                     iCharIndex = strDirUrl.IndexOf("/", iCharIndex) + 1;
//                     if (iCharIndex == 0)
//                         strDirUrlTemp = strDirUrl;
//                     else
//                         strDirUrlTemp = strDirUrl.Substring(0, iCharIndex);
//                     if (strDirUrlTemp == "")
//                         continue;
//                     ftpConnect.SendCommand("MKD " + strDirUrlTemp);
//                     if (ftpConnect.ReplyCode != 257 && ftpConnect.ReplyCode != 550)
//                         throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     if (strDirUrlTemp == strDirUrl)
//                         break;
//                 }
//                 #endregion
//                 ftpConnect.ChangeDir(strDirUrl);
//                 #region 得到服务器上的文件大小
//                 if (isContinueUpload)
//                 {
//                     string strDirName = ftpUrl.SubUrl;
//                     if (strDirName.IndexOf("/") >= 0)
//                         strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                     ftpConnect.ChangeDir(strDirName);
//                     ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
//                     if (ftpConnect.ReplyCode == 213)
//                         lOffset = Int64.Parse(ftpConnect.ReplyString.Substring(4));
//                 }
//                 #endregion
//                 #region 开始上传
//                 lTotalReaded = lOffset;
//                 socketData = ftpConnect.CreateDataSocket();
//                 if (lOffset > 0)
//                     ftpConnect.SendCommand("APPE " + Path.GetFileName(uploadUrl));
//                 else
//                     ftpConnect.SendCommand("STOR " + Path.GetFileName(uploadUrl));
//                 if (!(ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 150))
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 ftpConnect.DataTransmitting = true;
//                 while (true)
//                 {
//                     if (!ftpConnect.DataTransmitting)
//                     {
//                         this.OnFileUploadCanceled(ftpConnect);
//                         break;
//                     }
//                     this.OnFileUploading(ftpConnect, fileBytes.Length, lTotalReaded);
//                     //开始上传资料
//                     bytesRead = (int)((fileBytes.Length > lTotalReaded + ftpConnect.Buffer.Length) ? ftpConnect.Buffer.Length : (fileBytes.Length - lTotalReaded));
//                     if (bytesRead == 0)
//                         break;
//                     Array.Copy(fileBytes, lTotalReaded, ftpConnect.Buffer, 0, bytesRead);
//                     socketData.Send(ftpConnect.Buffer, bytesRead, 0);
//                     lTotalReaded += bytesRead;
//                 }
//                 if (socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect.DataTransmitting)
//                 {
//                     if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                     {
//                         ftpConnect.ReadReply();
//                         if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                             throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     }
//                     this.OnFileUploadCompleted(ftpConnect);
//                 }
//                 #endregion
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (socketData != null && socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect != null)
//                 {
//                     ftpConnect.DisConnect();
//                     this.m_FTPConnectList.Remove(ftpConnect);
//                 }
//             }
//         }
//         /// <summary>
//         /// 直接上传文件
//         /// </summary>
//         /// <param name="filePath">上传文件的全路径</param>
//         /// <param name="uploadUrl">上传的目标全路径与文件名</param>
//         /// <param name="isContinueUpload">是否断点续传</param>
//         /// <returns>上传是否成功</returns>
//         public void UploadFileNow(string filePath, string uploadUrl, bool isContinueUpload)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             Socket socketData = null;
//             FileStream fileStream = null;
//             int iCharIndex = 0;
//             int bytesRead;
//             long lOffset = 0, lTotalReaded = 0;
//             string strDirUrl, strDirUrlTemp;
//             try
//             {
//                 ftpUrl = new FTPUrl(uploadUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 this.m_FTPConnectList.Add(ftpConnect);//添加到控制器中
//                 #region 判断并创建文件夹
//                 strDirUrl = ftpUrl.SubUrl;
//                 if (strDirUrl.IndexOf("/") >= 0)
//                     strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/"));
//                 while (true && strDirUrl.Trim() != "")
//                 {
//                     iCharIndex = strDirUrl.IndexOf("/", iCharIndex) + 1;
//                     if (iCharIndex == 0)
//                         strDirUrlTemp = strDirUrl;
//                     else
//                         strDirUrlTemp = strDirUrl.Substring(0, iCharIndex);
//                     if (strDirUrlTemp == "")
//                         continue;
//                     ftpConnect.SendCommand("MKD " + strDirUrlTemp);
//                     if (ftpConnect.ReplyCode != 257 && ftpConnect.ReplyCode != 550)
//                         throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     if (strDirUrlTemp == strDirUrl)
//                         break;
//                 }
//                 #endregion
//                 ftpConnect.ChangeDir(strDirUrl);
//                 #region 得到服务器上的文件大小
//                 if (isContinueUpload)
//                 {
//                     string strDirName = ftpUrl.SubUrl;
//                     if (strDirName.IndexOf("/") >= 0)
//                         strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                     ftpConnect.ChangeDir(strDirName);
//                     ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
//                     if (ftpConnect.ReplyCode == 213)
//                         lOffset = Int64.Parse(ftpConnect.ReplyString.Substring(4));
//                 }
//                 #endregion
//                 #region 开始上传
//                 fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
//                 lTotalReaded = lOffset;
//                 fileStream.Seek(lOffset, SeekOrigin.Begin);
//                 socketData = ftpConnect.CreateDataSocket();
//                 if (lOffset > 0)
//                     ftpConnect.SendCommand("APPE " + Path.GetFileName(uploadUrl));
//                 else
//                     ftpConnect.SendCommand("STOR " + Path.GetFileName(uploadUrl));
//                 if (!(ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 150))
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 ftpConnect.DataTransmitting = true;
//                 while (true)
//                 {
//                     if (!ftpConnect.DataTransmitting)
//                     {
//                         this.OnFileUploadCanceled(ftpConnect);
//                         break;
//                     }
//                     this.OnFileUploading(ftpConnect, fileStream.Length, lTotalReaded);
//                     //开始上传资料
//                     bytesRead = fileStream.Read(ftpConnect.Buffer, 0, ftpConnect.Buffer.Length);
//                     if (bytesRead == 0)
//                         break;
//                     socketData.Send(ftpConnect.Buffer, bytesRead, 0);
//                     lTotalReaded += bytesRead;
//                 }
//                 fileStream.Close();
//                 if (socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect.DataTransmitting)
//                 {
//                     if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                     {
//                         ftpConnect.ReadReply();
//                         if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                             throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     }
//                     this.OnFileUploadCompleted(ftpConnect);
//                 }
//                 #endregion
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (fileStream != null)
//                     fileStream.Close();
//                 if (socketData != null && socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect != null)
//                 {
//                     ftpConnect.DisConnect();
//                     this.m_FTPConnectList.Remove(ftpConnect);
//                 }
//             }
//         }
//         /// <summary>
//         /// 上传文件
//         /// </summary>
//         /// <param name="filePath"></param>
//         /// <param name="uploadUrl"></param>
//         public string UploadFile(string filePath, string uploadUrl)
//         {
//             return this.UploadFile(filePath, uploadUrl, false);
//         }
//         /// <summary>
//         /// 上传文件
//         /// </summary>
//         /// <param name="filePath">上传文件的全路径</param>
//         /// <param name="uploadUrl">上传的目标全路径 包含了用户名用户密码与文件名</param>
//         /// <param name="isContinueUpload">是否断点续传</param>
//         /// <returns>返回控制上传下载的ID</returns>
//         public string UploadFile(string filePath, string uploadUrl, bool isContinueUpload)
//         {
//             String strFTPId = System.Guid.NewGuid().ToString();
//             IList<object> objList = new List<object> { filePath, uploadUrl, isContinueUpload, strFTPId };
//             System.Threading.Thread threadUpload = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadUploadFile));
//             threadUpload.Start(objList);  //开始采用线程方式下载
//             return strFTPId;
//         }
//         /// <summary>
//         /// 线程接收上传
//         /// </summary>
//         /// <param name="obj"></param>
//         private void ThreadUploadFile(object obj)
//         {
//             string filePath;
//             string uploadUrl;
//             bool isContinueUpload;
//             string strFTPId;
//             IList<object> objList = obj as IList<object>;
//             if (objList != null && objList.Count == 4)
//             {
//                 filePath = objList[0] as string;
//                 uploadUrl = objList[1] as string;
//                 isContinueUpload = (bool)objList[2];
//                 strFTPId = objList[3] as string;
//                 this.ThreadUploadFile(filePath, uploadUrl, isContinueUpload, strFTPId);
//             }
//         }
//         /// <summary>
//         /// 线程上传文件
//         /// </summary>
//         /// <param name="filePath">上传文件的全路径</param>
//         /// <param name="uploadUrl">上传的目标全路径与文件名</param>
//         /// <param name="isContinueUpload">是否断点续传</param>
//         /// <returns>上传是否成功</returns>
//         private void ThreadUploadFile(string filePath, string uploadUrl, bool isContinueUpload, string strFTPId)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             Socket socketData = null;
//             FileStream fileStream = null;
//             int iCharIndex = 0;
//             int bytesRead;
//             long lOffset = 0, lTotalReaded = 0;
//             string strDirUrl, strDirUrlTemp;
//             try
//             {
//                 ftpUrl = new FTPUrl(uploadUrl);
//                 ftpConnect = new FTPConnect(ftpUrl, strFTPId);
//                 ftpConnect.Connect();
//                 this.m_FTPConnectList.Add(ftpConnect);//添加到控制器中
//                 #region 判断并创建文件夹
//                 strDirUrl = ftpUrl.SubUrl;
//                 if (strDirUrl.IndexOf("/") >= 0)
//                     strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/"));
//                 while (true && strDirUrl.Trim() != "")
//                 {
//                     iCharIndex = strDirUrl.IndexOf("/", iCharIndex) + 1;
//                     if (iCharIndex == 0)
//                         strDirUrlTemp = strDirUrl;
//                     else
//                         strDirUrlTemp = strDirUrl.Substring(0, iCharIndex);
//                     if (strDirUrlTemp == "")
//                         continue;
//                     ftpConnect.SendCommand("MKD " + strDirUrlTemp);
//                     if (ftpConnect.ReplyCode != 257 && ftpConnect.ReplyCode != 550)
//                         throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     if (strDirUrlTemp == strDirUrl)
//                         break;
//                 }
//                 #endregion
//                 ftpConnect.ChangeDir(strDirUrl);
//                 #region 得到服务器上的文件大小
//                 if (isContinueUpload)
//                 {
//                     string strDirName = ftpUrl.SubUrl;
//                     if (strDirName.IndexOf("/") >= 0)
//                         strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                     ftpConnect.ChangeDir(strDirName);
//                     ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
//                     if (ftpConnect.ReplyCode == 213)
//                         lOffset = Int64.Parse(ftpConnect.ReplyString.Substring(4));
//                 }
//                 #endregion
//                 #region 开始上传
//                 fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
//                 lTotalReaded = lOffset;
//                 fileStream.Seek(lOffset, SeekOrigin.Begin);
//                 socketData = ftpConnect.CreateDataSocket();
//                 if (lOffset > 0)
//                     ftpConnect.SendCommand("APPE " + Path.GetFileName(uploadUrl));
//                 else
//                     ftpConnect.SendCommand("STOR " + Path.GetFileName(uploadUrl));
//                 if (!(ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 150))
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 ftpConnect.DataTransmitting = true;
//                 while (true)
//                 {
//                     if (!ftpConnect.DataTransmitting)
//                     {
//                         this.OnFileUploadCanceled(ftpConnect);
//                         break;
//                     }
//                     this.OnFileUploading(ftpConnect, fileStream.Length, lTotalReaded);
//                     //开始上传资料
//                     bytesRead = fileStream.Read(ftpConnect.Buffer, 0, ftpConnect.Buffer.Length);
//                     if (bytesRead == 0)
//                         break;
//                     socketData.Send(ftpConnect.Buffer, bytesRead, 0);
//                     lTotalReaded += bytesRead;
//                 }
//                 fileStream.Close();
//                 if (socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect.DataTransmitting)
//                 {
//                     if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                     {
//                         ftpConnect.ReadReply();
//                         if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                             throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     }
//                     this.OnFileUploadCompleted(ftpConnect);
//                 }
//                 #endregion
//             }
//             catch (Exception ex)
//             {
//                 this.OnFtpError(ftpConnect, ex);
//             }
//             finally
//             {
//                 if (fileStream != null)
//                     fileStream.Close();
//                 if (socketData != null && socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect != null)
//                 {
//                     ftpConnect.DisConnect();
//                     this.m_FTPConnectList.Remove(ftpConnect);
//                 }
//             }
//         }
//         /// <summary>
//         /// 取消正在上传的文件
//         /// </summary>
//         /// <returns></returns>
//         public void CancelUploadFile(FTPConnect ftpConnect)
//         {
//             if (ftpConnect != null)
//                 ftpConnect.DataTransmitting = false;
//         }
//         /// <summary>
//         /// 取消正在上传的文件
//         /// </summary>
//         /// <param name="strID"></param>
//         public void CancelUploadFile(string strID)
//         {
//             foreach (FTPConnect ftp in this.m_FTPConnectList)
//             {
//                 if (ftp != null && ftp.ID == strID)
//                 {
//                     ftp.DataTransmitting = false;
//                     break;
//                 }
//             }
//         }
//         #endregion
//         #region 下载文件
//         /// <summary>
//         /// 直接下载文件
//         /// </summary>
//         /// <param name="downloadUrl">要下载文件的路径</param>
//         /// <param name="fileBytes">存贮的内容</param>
//         /// <returns>下载是否成功</returns>
//         public void DownloadFileNow(string downloadUrl, out byte[] fileBytes)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             Socket socketData = null;
//             fileBytes = new byte[] { };
//             int bytesRead;
//             long lTotalReaded = 0, lFileSize;
//             try
//             {
//                 ftpUrl = new FTPUrl(downloadUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 this.m_FTPConnectList.Add(ftpConnect);//添加到控制器中
//                 string strDirUrl = ftpUrl.SubUrl;
//                 if (strDirUrl.IndexOf("/") >= 0)
//                     strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirUrl);
//                 #region 得到服务器上的文件大小
//                 string strDirName = ftpUrl.SubUrl;
//                 if (strDirName.IndexOf("/") >= 0)
//                     strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirName);
//                 ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
//                 if (ftpConnect.ReplyCode == 213)
//                     lFileSize = Int64.Parse(ftpConnect.ReplyString.Substring(4));
//                 else
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 #endregion
// 
//                 socketData = ftpConnect.CreateDataSocket();
//                 fileBytes = new byte[lFileSize];
//                 ftpConnect.SendCommand("RETR " + Path.GetFileName(ftpUrl.SubUrl));
//                 if (!(ftpConnect.ReplyCode == 150 || ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 #region 开始下载
//                 ftpConnect.DataTransmitting = true;
//                 while (true)
//                 {
//                     if (!ftpConnect.DataTransmitting)    //判断取消是否取消了下载
//                     {
//                         this.OnFileDownloadCanceled(ftpConnect);
//                         break;
//                     }
//                     this.OnFileDownloading(ftpConnect, lFileSize, lTotalReaded);
//                     //开始将文件流写入本地
//                     bytesRead = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
//                     if (bytesRead <= 0)
//                         break;
//                     Array.Copy(ftpConnect.Buffer, 0, fileBytes, lTotalReaded, bytesRead);
//                     lTotalReaded += bytesRead;
//                 }
//                 if (socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect.DataTransmitting)
//                 {
//                     if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                     {
//                         ftpConnect.ReadReply();
//                         if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                             throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     }
//                     this.OnFileDownloadCompleted(ftpConnect);
//                 }
//                 #endregion
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (socketData != null && socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect != null)
//                 {
//                     ftpConnect.DisConnect();
//                     this.m_FTPConnectList.Remove(ftpConnect);
//                 }
//             }
//         }
//         /// <summary>
//         /// 直接下载文件
//         /// </summary>
//         /// <param name="downloadUrl">要下载文件的路径</param>
//         /// <param name="targetFile">目标存在全路径</param>
//         /// <param name="isContinueDownload">是否断点续传</param>
//         /// <returns>下载是否成功</returns>
//         public void DownloadFileNow(string downloadUrl, string targetFile, bool isContinueDownload)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             Socket socketData = null;
//             FileStream fileStream = null;
//             int bytesRead;
//             long lTotalReaded = 0, lFileSize;
//             try
//             {
//                 ftpUrl = new FTPUrl(downloadUrl);
//                 ftpConnect = new FTPConnect(ftpUrl);
//                 ftpConnect.Connect();
//                 this.m_FTPConnectList.Add(ftpConnect);//添加到控制器中
//                 string strDirUrl = ftpUrl.SubUrl;
//                 if (strDirUrl.IndexOf("/") >= 0)
//                     strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirUrl);
//                 #region 得到服务器上的文件大小
//                 string strDirName = ftpUrl.SubUrl;
//                 if (strDirName.IndexOf("/") >= 0)
//                     strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirName);
//                 ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
//                 if (ftpConnect.ReplyCode == 213)
//                     lFileSize = Int64.Parse(ftpConnect.ReplyString.Substring(4));
//                 else
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 #endregion
// 
//                 socketData = ftpConnect.CreateDataSocket();
//                 //断点续传长度的偏移量
//                 if (System.IO.File.Exists(targetFile) && isContinueDownload)
//                 {
//                     System.IO.FileInfo fiInfo = new FileInfo(targetFile);
//                     lTotalReaded = fiInfo.Length;
//                     ftpConnect.SendCommand("REST " + fiInfo.Length.ToString());
//                     if (ftpConnect.ReplyCode != 350)
//                         throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 }
//                 ftpConnect.SendCommand("RETR " + Path.GetFileName(ftpUrl.SubUrl));
//                 if (!(ftpConnect.ReplyCode == 150 || ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 #region 开始下载
//                 string strTargetPath = targetFile;
//                 strTargetPath = strTargetPath.Substring(0, strTargetPath.LastIndexOf("//"));
//                 if (!System.IO.Directory.Exists(strTargetPath)) //判断目标路径是否存在，如果不存在就创建
//                     System.IO.Directory.CreateDirectory(strTargetPath);
//                 if (System.IO.File.Exists(targetFile) && isContinueDownload)  //目标文件已经是全路径了 断点续传
//                     fileStream = new System.IO.FileStream(targetFile, System.IO.FileMode.Append, System.IO.FileAccess.Write);
//                 else
//                     fileStream = new System.IO.FileStream(targetFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
//                 ftpConnect.DataTransmitting = true;
//                 while (true)
//                 {
//                     if (!ftpConnect.DataTransmitting)    //判断取消是否取消了下载
//                     {
//                         this.OnFileDownloadCanceled(ftpConnect);
//                         break;
//                     }
//                     this.OnFileDownloading(ftpConnect, lFileSize, lTotalReaded);
//                     //开始将文件流写入本地
//                     bytesRead = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
//                     if (bytesRead <= 0)
//                         break;
//                     fileStream.Write(ftpConnect.Buffer, 0, bytesRead);
//                     lTotalReaded += bytesRead;
//                 }
//                 fileStream.Close();
//                 if (socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect.DataTransmitting)
//                 {
//                     if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                     {
//                         ftpConnect.ReadReply();
//                         if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                             throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     }
//                     this.OnFileDownloadCompleted(ftpConnect);
//                 }
//                 #endregion
//             }
//             catch (Exception ex)
//             {
//                 throw (ex);
//             }
//             finally
//             {
//                 if (fileStream != null)
//                     fileStream.Close();
//                 if (socketData != null && socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect != null)
//                 {
//                     ftpConnect.DisConnect();
//                     this.m_FTPConnectList.Remove(ftpConnect);
//                 }
//             }
//         }
//         /// <summary>
//         /// 下载文件
//         /// </summary>
//         /// <param name="downloadUrl"></param>
//         /// <param name="targetFile"></param>
//         public string DownloadFile(string downloadUrl, string targetFile)
//         {
//             return this.DownloadFile(downloadUrl, targetFile, false);
//         }
//         /// <summary>
//         /// 下载文件
//         /// </summary>
//         /// <param name="downloadUrl">要下载文件的路径 包含了登录名与登录密码</param>
//         /// <param name="TargetFile">目标存在路径包含文件名</param>
//         /// <param name="isContinueDownload">是否断点续传</param>
//         /// <returns>返回下载控制ID</returns>
//         public string DownloadFile(string downloadUrl, string targetFile, bool isContinueDownload)
//         {
//             String strFTPId = System.Guid.NewGuid().ToString();
//             IList<object> objList = new List<object> { downloadUrl, targetFile, isContinueDownload, strFTPId };
//             System.Threading.Thread threadDownload = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadDownloadFile));
//             threadDownload.Start(objList);  //开始采用线程方式下载
//             return strFTPId;
//         }
//         /// <summary>
//         /// 线程接收下载
//         /// </summary>
//         /// <param name="obj"></param>
//         private void ThreadDownloadFile(object obj)
//         {
//             string downloadUrl;
//             string targetFile;
//             bool isContinueDownload;
//             string strFTPId;
//             IList<object> objList = obj as IList<object>;
//             if (objList != null && objList.Count == 4)
//             {
//                 downloadUrl = objList[0] as string;
//                 targetFile = objList[1] as string;
//                 isContinueDownload = (bool)objList[2];
//                 strFTPId = objList[3] as String;
//                 this.ThreadDownloadFile(downloadUrl, targetFile, isContinueDownload, strFTPId);
//             }
//         }
//         /// <summary>
//         /// 线程下载文件
//         /// </summary>
//         /// <param name="downloadUrl">要下载文件的路径</param>
//         /// <param name="targetFile">目标存在全路径</param>
//         /// <param name="isContinueDownload">是否断点续传</param>
//         /// <returns>下载是否成功</returns>
//         private void ThreadDownloadFile(string downloadUrl, string targetFile, bool isContinueDownload, string strFTPId)
//         {
//             FTPUrl ftpUrl = null;
//             FTPConnect ftpConnect = null;
//             Socket socketData = null;
//             FileStream fileStream = null;
//             int bytesRead;
//             long lTotalReaded = 0, lFileSize;
//             try
//             {
//                 ftpUrl = new FTPUrl(downloadUrl);
//                 ftpConnect = new FTPConnect(ftpUrl, strFTPId);
//                 ftpConnect.Connect();
//                 this.m_FTPConnectList.Add(ftpConnect);//添加到控制器中
//                 string strDirUrl = ftpUrl.SubUrl;
//                 if (strDirUrl.IndexOf("/") >= 0)
//                     strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirUrl);
//                 #region 得到服务器上的文件大小
//                 string strDirName = ftpUrl.SubUrl;
//                 if (strDirName.IndexOf("/") >= 0)
//                     strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/"));
//                 ftpConnect.ChangeDir(strDirName);
//                 ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
//                 if (ftpConnect.ReplyCode == 213)
//                     lFileSize = Int64.Parse(ftpConnect.ReplyString.Substring(4));
//                 else
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 #endregion
// 
//                 socketData = ftpConnect.CreateDataSocket();
//                 //断点续传长度的偏移量
//                 if (System.IO.File.Exists(targetFile) && isContinueDownload)
//                 {
//                     System.IO.FileInfo fiInfo = new FileInfo(targetFile);
//                     lTotalReaded = fiInfo.Length;
//                     ftpConnect.SendCommand("REST " + fiInfo.Length.ToString());
//                     if (ftpConnect.ReplyCode != 350)
//                         throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 }
//                 ftpConnect.SendCommand("RETR " + Path.GetFileName(ftpUrl.SubUrl));
//                 if (!(ftpConnect.ReplyCode == 150 || ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                     throw new IOException(ftpConnect.ReplyString.Substring(4));
//                 #region 开始下载
//                 string strTargetPath = targetFile;
//                 strTargetPath = strTargetPath.Substring(0, strTargetPath.LastIndexOf("//"));
//                 if (!System.IO.Directory.Exists(strTargetPath)) //判断目标路径是否存在，如果不存在就创建
//                     System.IO.Directory.CreateDirectory(strTargetPath);
//                 if (System.IO.File.Exists(targetFile) && isContinueDownload)  //目标文件已经是全路径了 断点续传
//                     fileStream = new System.IO.FileStream(targetFile, System.IO.FileMode.Append, System.IO.FileAccess.Write);
//                 else
//                     fileStream = new System.IO.FileStream(targetFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
//                 ftpConnect.DataTransmitting = true;
//                 while (true)
//                 {
//                     if (!ftpConnect.DataTransmitting)    //判断取消是否取消了下载
//                     {
//                         this.OnFileDownloadCanceled(ftpConnect);
//                         break;
//                     }
//                     this.OnFileDownloading(ftpConnect, lFileSize, lTotalReaded);
//                     //开始将文件流写入本地
//                     bytesRead = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
//                     if (bytesRead <= 0)
//                         break;
//                     fileStream.Write(ftpConnect.Buffer, 0, bytesRead);
//                     lTotalReaded += bytesRead;
//                 }
//                 fileStream.Close();
//                 if (socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect.DataTransmitting)
//                 {
//                     if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                     {
//                         ftpConnect.ReadReply();
//                         if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
//                             throw new IOException(ftpConnect.ReplyString.Substring(4));
//                     }
//                     this.OnFileDownloadCompleted(ftpConnect);
//                 }
//                 #endregion
//             }
//             catch (Exception ex)
//             {
//                 this.OnFtpError(ftpConnect, ex);
//             }
//             finally
//             {
//                 if (fileStream != null)
//                     fileStream.Close();
//                 if (socketData != null && socketData.Connected)
//                     socketData.Close();
//                 if (ftpConnect != null)
//                 {
//                     ftpConnect.DisConnect();
//                     this.m_FTPConnectList.Remove(ftpConnect);
//                 }
//             }
//         }
//         /// <summary>
//         /// 取消正在下载的文件
//         /// </summary>
//         /// <returns></returns>
//         public void CancelDownloadFile(FTPConnect ftpConnect)
//         {
//             if (ftpConnect != null)
//                 ftpConnect.DataTransmitting = false;
//         }
//         /// <summary>
//         /// 取消正在下载的文件
//         /// </summary>
//         /// <param name="strID"></param>
//         public void CancelDownloadFile(string strID)
//         {
//             foreach (FTPConnect ftp in this.m_FTPConnectList)
//             {
//                 if (ftp != null && ftp.ID == strID)
//                 {
//                     ftp.DataTransmitting = false;
//                     break;
//                 }
//             }
//         }
//         #endregion
//         #region 根据指定的ID查找FTPConnect
//         /// <summary>
//         /// 根据指定的ID查找FTPConnect
//         /// </summary>
//         /// <param name="id"></param>
//         /// <returns></returns>
//         public FTPConnect FindFTPConnectByID(string id)
//         {
//             foreach (FTPConnect ftpConnect in this.m_FTPConnectList)
//             {
//                 if (ftpConnect != null && ftpConnect.ID == id)
//                     return ftpConnect;
//             }
//             return null;
//         }
//         #endregion
// 
//         #region 传输类型
//         public enum FTPType
//         {
//             /// <summary>
//             /// 无状态
//             /// </summary>
//             None,
//             /// <summary>
//             /// 上传
//             /// </summary>
//             Upload,
//             /// <summary>
//             /// 下载
//             /// </summary>
//             Download
//         }
//         #endregion
//     }
//     #region 文件信息结构
//     public struct FileStruct
//     {
//         public string Flags;
//         public string Owner;
//         public string Group;
//         public int FileSize;
//         public bool IsDirectory;
//         public DateTime CreateTime;
//         public string Name;
//     }
//     #endregion
// 
//     #region 文件传输进度控制事件
//     public delegate void FTPSendEventHandler(object sender, FTPSendEventArgs e);
//     public class FTPSendEventArgs : System.EventArgs
//     {
//         private long m_totalbytes;			// Total Bytes
//         private long m_bytestransfered;
// 
//         public FTPSendEventArgs()
//         {
//             m_totalbytes = 0;
//             m_bytestransfered = 0;
//         }
//         public FTPSendEventArgs(long lTotalBytes, long lBytesTransfered)
//         {
//             m_totalbytes = lTotalBytes;
//             m_bytestransfered = lBytesTransfered;
//         }
//         /// <summary>
//         /// 总字节数
//         /// </summary>
//         public long TotalBytes
//         {
//             get { return m_totalbytes; }
//             set { m_totalbytes = value; }
//         }
//         /// <summary>
//         /// 已传输字节数
//         /// </summary>
//         public long BytesTransfered
//         {
//             get { return m_bytestransfered; }
//             set { m_bytestransfered = value; }
//         }
//     }
//     public delegate void FTPErrorEventHandler(object sender, FTPErrorEventArgs e);
//     public class FTPErrorEventArgs : System.EventArgs
//     {
//         private Exception m_Error = null;
//         public FTPErrorEventArgs() { }
//         public FTPErrorEventArgs(Exception error)
//         {
//             this.m_Error = error;
//         }
//         /// <summary>
//         /// 错误消息
//         /// </summary>
//         public Exception Error
//         {
//             get { return this.m_Error; }
//             set { this.m_Error = value; }
//         }
//     }
//     #endregion
// }