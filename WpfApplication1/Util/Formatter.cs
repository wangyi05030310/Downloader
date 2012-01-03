using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1.Util
{
    class Formatter
    {
        /// <summary>
        /// 得到一个表示大小的string
        /// </summary>
        /// <param name="b">以b为单位时的大小</param>
        /// <returns></returns>
        public static string formatSize(long b)
        {
            if (b < 1024)
            {
                return b + " B";
            }

            double kb = b / 1024;
            if (kb < 1024)
            {
                return kb + " KB";
            }

            double mb = kb / 1024;
            if (mb < 1024)
            {
                return mb + " MB";
            }

            double gb = mb / 1024;
            return gb + " GB";
        }
    }
}
