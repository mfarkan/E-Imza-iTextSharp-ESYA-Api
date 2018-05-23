using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Windows.Forms;
using System.Web;
using System.Collections.Specialized;
using System.Threading;

namespace DesktopPdfSigner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //bool openProg;
            //Mutex mtx = new Mutex(true, "Form1", out openProg);

            //if (!openProg)
            //{
            //    MessageBox.Show("Çalıştırmak istediğiniz program zaten açık durumda !!");
            //    return;
            //}
            //else
            //{

            //    
            //}

            //GC.KeepAlive(mtx);

        }
    }
}
