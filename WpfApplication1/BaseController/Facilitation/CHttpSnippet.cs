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

        public event HttpEventHandler EndEvent;

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
            nreadsize = -1;
            fs = new FileStream(fileName, FileMode.Create);

            //TODO: send an event to UI where a snippet start downloading
        }

        public void receive()
        {
            initiate();

            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(theurl);

                request.AddRange(threadManage.fileStart[threadNumber],
                    threadManage.fileStart[threadNumber] + threadManage.fileSize[threadNumber]);
                iostream = request.GetResponse().GetResponseStream();

                nreadsize = iostream.Read(nbytes, 0, 512);
                while (nreadsize > 0)
                {
                    fs.Write(nbytes, 0, nreadsize);
                    nreadsize = iostream.Read(nbytes, 0, 512);
                    //TODO: send an event to UI where something'll change

                }
                fs.Close();
                iostream.Close();

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error in snippet receiving:" + ex.Message);
                fs.Close();
            }


            EndReceive();
            threadManage.threadEnd[threadNumber] = true;
        }

        private void EndReceive()
        {
            //TODO: send a event to UI where a snippet have reach an end
            EndEvent(this, null);
        }
    }
}
