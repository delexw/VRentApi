using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using VWFSCN.IT.Log;
using VWFSCN.IT.Log.Configuration;

namespace CF.VRent.Log
{
    public abstract class VRentLog : IVRentLog
    {
        public virtual VWFSCN.IT.Log.ILogWriter Logger { get; private set; }
        public virtual LoggingConfiguration Configuration { get; private set; }

        public VRentLog(string sectionName)
        {
            LoggingConfiguration loggingConfiguration = ConfigurationManager.GetSection(sectionName) as LoggingConfiguration;
            
            if (loggingConfiguration != null)
            {
                //ignore the value from configuration file
                //write log sync always
                loggingConfiguration.EnabledSync = true;
                Configuration = loggingConfiguration;
                Logger = new PublicMulticastLogWriter(loggingConfiguration);
            }
            else
            {
                Logger = VWFSCN.IT.Log.LogFactory.GetLogWriter();
            }
        }

        public virtual void WriteInfo(string title, string message, string userName)
        {
            LogInfor.WriteLog(Logger, title, message, userName, VWFSCN.IT.Log.LogType.Info,
                                                LogStatus.Success,
                                                LogPriority.P4,
                                                LogSeverity.Low);
        }

        public virtual void WriteError(string title, string message, string userName)
        {
            LogInfor.WriteLog(Logger, title, message, userName, VWFSCN.IT.Log.LogType.Error,
                                                LogStatus.Fail,
                                                LogPriority.P2,
                                                LogSeverity.Middle);
        }

        public virtual void WriteAudit(string title, string message, string userName)
        {
            LogInfor.WriteLog(Logger, title, message, userName, VWFSCN.IT.Log.LogType.Audit,
                                                LogStatus.Success,
                                                LogPriority.P4,
                                                LogSeverity.Low);
        }

        public virtual void WriteDebug(string title, string message, string userName)
        {
            LogInfor.WriteLog(Logger, title, message, userName, VWFSCN.IT.Log.LogType.Debug,
                                                LogStatus.Success,
                                                LogPriority.P4,
                                                LogSeverity.Low);
        }
    }
}
