using System;
using System.Net;
using System.Windows;
using System.Threading;
using System.IO;

using WpfApplication1.BaseController.Facilitation;

namespace WpfApplication1.BaseController
{
    class IntPair
    {
        public int threadNum;
        public int threadBytes;
    }

    class CHttpManage
    {
        public bool[] threadEnd;
        public string[] snippet;
        public int[] fileStart;
        public int[] fileSize;
        public string theUrl;
        public string FileName;
        public string location;
        private bool combination;
        public int threadTotality;
        public int DevidedSize;

        public bool suspend = false;

        private Thread[] threadRunning;
        private CHttpSnippet[] model;
        private Thread combineThread;

        public FileStream fileSwap;             

        public CHttpManage(string url, string loc, int num)
        {
            theUrl = url;
            location = loc;
            threadTotality = num;

            if (location.LastIndexOf("\\") != location.Length - 1)
            {
                location += "\\";
            }

            FileName = theUrl.Substring(theUrl.LastIndexOf('/'));

            DateTime dt = DateTime.Now;
            ///////////////////////////

            HttpWebRequest request;
            long filesize = 0;

            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(theUrl);
                filesize = request.GetResponse().ContentLength;
                request.Abort();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("ErrorEventArgs in stardDownload!" + ex.Message);
            }

            threadEnd = new bool[threadTotality];
            snippet = new string[threadTotality];
            fileStart = new int[threadTotality];
            fileSize = new int[threadTotality];

            DevidedSize = (int)filesize / threadTotality;

            string tempstr = "";
            IntPair ip = new IntPair();

            try
            {
                                //续传 则打开保存每个临时快线程读取进度的文件
                fileSwap = new FileStream(location + FileName + ".tempinfo", FileMode.Open);
                StreamReader sr = new StreamReader(fileSwap);

                threadTotality = Convert.ToInt32(sr.ReadLine());

                for (int i = 0; i < threadTotality; ++i )
                {
                    snippet[i] = location + FileName + "_" + i.ToString() + ".piece";
                    tempstr = sr.ReadLine();
                    ip = strTackle(tempstr);
                    fileStart[ip.threadNum] = ip.threadBytes + DevidedSize * ip.threadNum;
                    fileSize[ip.threadNum] = DevidedSize - 1 - ip.threadBytes;
                }

                sr.Close();

                //将读取好的临时文件清空
                fileSwap = new FileStream(location + FileName + ".tempinfo", FileMode.Create);
                fileSwap.Close();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                fileSwap = new FileStream(location + FileName + ".tempinfo", FileMode.Create);
                StreamWriter stw = new StreamWriter(fileSwap);
                stw.WriteLine(threadTotality);
                stw.Close();

                for (int i = 0; i < threadTotality; ++i)
                {
                    threadEnd[i] = false;
                    snippet[i] = location + FileName + "_" + i.ToString() + ".piece";
                    if (i < threadTotality - 1)
                    {
                        fileStart[i] = DevidedSize * i;
                        fileSize[i] = DevidedSize - 1;
                    }
                    else
                    {
                        fileStart[i] = DevidedSize * i;
                        fileSize[i] = DevidedSize - 1;
                    }
                }

                fileSwap.Close();
                stw.Close();
            }

            threadRunning = new Thread[threadTotality];
            model = new CHttpSnippet[threadTotality];

            for (int i = 0; i < threadTotality;++i )
            {
                model[i] = new CHttpSnippet(this, i);
                model[i].StartEvent += new HttpEventHandler(OnStartReceive);
                model[i].EndEvent += new HttpEventHandler(OnEndReceive);
                model[i].StopSnippetEvent += new HttpEventHandler(model[i].StopSnippetProc);

                threadRunning[i] = new Thread(new ThreadStart(model[i].receive));

                combineThread = new Thread(new ThreadStart(combineFile));
            }
        }

        public void startDownload()
        {
            for (int i = 0; i < threadTotality;++i )
            {
                threadRunning[i].Start();
            }
            combineThread.Start();
        }

        private void combineFile()
        {
            while (true)
            {
                combination = true;
                for (int i = 0; i < threadTotality;++i )
                {
                    if (threadEnd[i] == false)
                    {
                        combination = false;
                        Thread.Sleep(100);
                        break;
                    }
                }
                if (combination == true)
                {
                    break;
                }
                if (suspend)
                {
                    //
                    return;
                }
            }

            FileStream fs;
            FileStream fsTemp;
            int readfile;
            byte[] bytes = new byte[512];
            fs = new FileStream(location + FileName,FileMode.Create);
            for (int i = 0; i < threadTotality;++i )
            {
                fsTemp = new FileStream(snippet[i], FileMode.Open);
                while (true)
                {
                    readfile = fsTemp.Read(bytes, 0, 512);
                    if (readfile > 0)
                    {
                        fs.Write(bytes, 0, readfile);
                    }
                    else
                    {
                        break;
                    }
                }
                fsTemp.Close();
            }
            fs.Close();
            DateTime dt = DateTime.Now;
            //TODO: send an event to UI of the end time

            FileInfo file;

            for (int i = 0; i < threadTotality; ++i )
            {
                file = new FileInfo(snippet[i]);
                file.Delete();
            }

            file = new FileInfo(location + FileName + ".tempinfo");
            if (file.Exists)
            {
                file.Delete();
            }
        }

        void OnStartReceive(Object sender, EventArgs e)     //这个事件处理函数可以放在UI层里面用于通知UI作出相应的变化
        {
            //
        }

        void OnEndReceive(Object sender, EventArgs e)       //这个事件处理函数可以放在UI层里面用于通知UI作出相应的变化
        {
            //
        }

        public void StopDownload()
        {
            suspend = true; //如果中断 则设置中断量 在文件合并线程里面跳出

            fileSwap = new FileStream(location + FileName + ".tempinfo", FileMode.Create);
            StreamWriter stw = new StreamWriter(fileSwap);
            stw.WriteLine(threadTotality);
            stw.Close();
            fileSwap.Close();

            for (int i = 0; i < threadTotality; ++i)
            {
                if (model[i].downloading)
                {
                    model[i].StopSnippet();
                }
            }

            fileSwap.Close();
        }

        private IntPair strTackle(string strin)
        {
            IntPair ret = new IntPair();

            int index = strin.IndexOf(':');

            ret.threadNum = Convert.ToInt32(strin.Substring(0, index));
            ret.threadBytes = Convert.ToInt32(strin.Substring(index + 1));

            return ret;
        }
    }
}
