using System;
using System.Net;
using System.Windows;
using System.Threading;
using System.IO;

using WpfApplication1.BaseController.Facilitation;

namespace WpfApplication1.BaseController
{
    class CHttpManage
    {
        public bool[] threadEnd;
        public string[] snippet;
        public int[] fileStart;
        public int[] fileSize;
        public string theUrl;
        private string FileName;
        private string location;
        private bool combination;
        public int threadTotality;

        private Thread[] threadRunning;
        private CHttpSnippet[] model;

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
        }

        public void startDownload()
        {
            DateTime dt = DateTime.Now;
            //TODO: send an message to UI of start time

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
                MessageBox.Show("Error in stardDownload:" + ex.Message);
            }

            threadEnd = new bool[threadTotality];
            snippet = new string[threadTotality];
            fileStart = new int[threadTotality];
            fileSize = new int[threadTotality];

            int filethread = (int)filesize / threadTotality;
            int filethreadrest = filethread + (int)filesize % threadTotality;

            for (int i = 0; i < threadTotality;++i )
            {
                threadEnd[i] = false;
                snippet[i] = i.ToString() + ".temp";
                if (i < threadTotality - 1)
                {
                    fileStart[i] = filethread * i;
                    fileSize[i] = filethread - 1;
                }
                else
                {
                    fileStart[i] = filethread * i;
                    fileSize[i] = filethread - 1;
                }
            }

            threadRunning = new Thread[threadTotality];
            model = new CHttpSnippet[threadTotality];

            for (int i = 0; i < threadTotality;++i )
            {
                model[i] = new CHttpSnippet(this, i);
                model[i].EndEvent += new HttpEventHandler(OnEndReceive);
                threadRunning[i] = new Thread(new ThreadStart(model[i].receive));
                threadRunning[i].Start();
            }

            Thread combineThread = new Thread(new ThreadStart(combineFile));
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
        }

        private void OnEndReceive(object sender, EventArgs e)
        {
            //TODO: Reply the snippet of the download finished event
        }
    }
}
