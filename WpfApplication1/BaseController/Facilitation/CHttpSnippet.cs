using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Windows;

using WpfApplication1.BaseController;

namespace WpfApplication1.BaseController.Facilitation
{
    public delegate void HttpEventHandler(object sender, EventArgs e);

    class CHttpSnippet
    {
        private CHttpManage threadManage;
        private int threadNumber;
        private string fileName;
        private string theurl;
        private FileStream fs;
        private HttpWebRequest request;
        private Stream iostream;
        private byte[] nbytes;
        private int nreadsize;
        public bool downloading = false;
        public bool downloaded = false;
        public int downloadbytes;

        public event HttpEventHandler StartEvent;
        public event HttpEventHandler EndEvent;
        public event HttpEventHandler StopSnippetEvent;

        public CHttpSnippet(CHttpManage controller, int thread)
        {
            threadManage = controller;
            threadNumber = thread;
        }

        public void initiate()
        {
            fileName = threadManage.snippet[threadNumber];
            theurl = threadManage.theUrl;
            iostream = null;
            nbytes = new byte[512];
            nreadsize = 0;
            fs = new FileStream(fileName, FileMode.Create);

            downloading = true;
            downloaded = false;
            downloadbytes = threadManage.DevidedSize - 1 - threadManage.fileSize[threadNumber];

            //TODO: send an event to UI where a snippet start downloading
        }

        public void receive()
        {
            initiate();

            try
            {
                StartEvent(this, null);
                request = (HttpWebRequest)HttpWebRequest.Create(theurl);

                request.AddRange(threadManage.fileStart[threadNumber],
                    threadManage.fileStart[threadNumber] + threadManage.fileSize[threadNumber]);
                iostream = request.GetResponse().GetResponseStream();

                do
                {
                    if (downloading)
                    {
                        nreadsize = iostream.Read(nbytes, 0, 512);
                        fs.Write(nbytes, 0, nreadsize);
                        downloadbytes += nreadsize;
                    }
                    else
                    {
                        threadManage.threadEnd[threadNumber] = true;
                        return;
                    }
                } while (nreadsize > 0);

                fs.Close();
                iostream.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error in snippet receiving:" + ex.Message);
                fs.Close();
            }

            endreceive();
            threadManage.threadEnd[threadNumber] = true;
        }

        private void endreceive()
        {
            downloading = false;
            downloaded = true;
            EndEvent(this, null);

        }

        public void StopSnippet()
        {
            StopSnippetEvent(this, null);
        }

        public void StopSnippetProc(object sender, EventArgs e)
        {
            if (!downloaded)
            {
                downloading = false;
                downloaded = true;
                Thread.Sleep(200);

                lock (this)
                {
                    FileStream fileStore = new FileStream(threadManage.location + threadManage.FileName + ".tempinfo", FileMode.Append);

                    StreamWriter sw = new StreamWriter(fileStore);

                    sw.WriteLine(threadManage + ":" + downloadbytes);

                    sw.Close();

                    fileStore.Close();
                }
            }
            else
            {
                MessageBox.Show("Snippet downloading thread has stopped!");
            }
        }
    }
}
