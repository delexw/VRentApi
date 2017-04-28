using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CF.VRent.Common;

namespace VRentDJ
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (!ConfigurationManager.AppSettings["localTest"].ToBool())
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
            { 
                new JobHost() 
            };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                new JobHost().Test();
                Thread.Sleep(10000000);
            }
        }
    }
}
