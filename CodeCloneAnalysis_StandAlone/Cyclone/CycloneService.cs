using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Cyclone
{
    partial class CycloneService : ServiceBase
    {
        private static System.Timers.Timer timer;
        private bool isProcessing { get; set; }
     
        public CycloneService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new System.Timers.Timer(30000);  // 30000 milliseconds = 30 seconds
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer = null;
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (CheckForInternetConnection())
            {
               // if (isProcessing!=true)
                //{
                 //   isProcessing = true;

                    CycloneHelper ch = new CycloneHelper();
                    ch.RetriveFromHook();
                    ch.AutomatedAnalyse();
                    isProcessing = false;

               // }
            }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
