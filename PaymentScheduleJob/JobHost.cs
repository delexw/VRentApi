using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using System.ServiceModel;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using CF.VRent.Job;
using CF.VRent.Job.Interface;
using CF.VRent.Job.Debug;
using VRentDJ.Units;
using VRentDJ.Debug;
using VRentDJ.Job;
using CF.VRent.Job.Common;
using CF.VRent.Log;

namespace VRentDJ
{
    public partial class JobHost : ServiceBase
    {
        JobManager manager = new JobManager();

        public JobHost()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //var job = manager.GetJob(typeof(TimerJobContainer));
            //if (job != null)
            //{
            //    job.Run();
            //}

            manager.LogError = (JobException ex, IJob job) =>
            {
                LogInfor.WriteError(ex.Name + " exception", ex.Message, "Schedule Job");
            };

            manager.RunAll();
        }
        
        /// <summary>
        /// For test
        /// </summary>
        public void Test()
        {
            OnStart(null);
        }

        protected override void OnStop()
        {
            manager.Dispose();
            manager = null;
        }
    }
}
