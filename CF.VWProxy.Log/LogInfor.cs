using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VWFSCN.IT.Log;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using CF.VRent.Log.ConcreteLog;
using System.Threading.Tasks;

namespace CF.VRent.Log
{
    public static class LogInfor
    {
        /// <summary>
        /// wirte default log
        /// </summary>
        public static readonly IVRentLog DefaultLogWriter = new DefaultLogger();

        /// <summary>
        /// write email log
        /// </summary>
        public static readonly IVRentLog EmailLogWriter = new EmailLogger();

        /// <summary>
        /// write payment log
        /// </summary>
        public static readonly IVRentLog PayLogWriter = new PaymentLogger();


        /// <summary>
        /// write email log
        /// </summary>
        public static readonly IVRentLog DebitNoteLogWriter = new DebitNoteJobLogger();


        /// <summary>
        /// Get the particular logger
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetLogger<T>() where T : IVRentLog
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        public static void WriteInfo(string title,  string message, string userName)
        {
            _writeLog(DefaultLogWriter.Logger, title, message, userName, VWFSCN.IT.Log.LogType.Info,
                                                LogStatus.Success,
                                                LogPriority.P4,
                                                LogSeverity.Low);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        public static void WriteError(string title, string message, string userName)
        {
            _writeLog(DefaultLogWriter.Logger, title, message, userName, VWFSCN.IT.Log.LogType.Error,
                                                LogStatus.Fail,
                                                LogPriority.P2,
                                                LogSeverity.Crash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        public static void WriteAudit(string title, string message, string userName)
        {
            _writeLog(DefaultLogWriter.Logger, title, message, userName, VWFSCN.IT.Log.LogType.Audit,
                                                LogStatus.Success,
                                                LogPriority.P4,
                                                LogSeverity.High);
        }

        /// <summary>
        /// Write debug
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        public static void WriteDebug(string title, string message, string userName)
        {
            _writeLog(DefaultLogWriter.Logger, title, message, userName, VWFSCN.IT.Log.LogType.Debug,
                                                LogStatus.Success,
                                                LogPriority.P4,
                                                LogSeverity.Middle);
        }

        private static void _writeLog(ILogWriter logger,string title, string message,
            string userName, VWFSCN.IT.Log.LogType type, LogStatus status, LogPriority priority, LogSeverity level)
        {
            Task.Factory.StartNew(() => {
                string applicationName = AppDomain.CurrentDomain.FriendlyName;
                string domainUserName = string.Empty;
                try
                {

                    domainUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }
                catch
                {

                }
                logger.WriteLog(type, title, status, message,
                                                    priority,
                                                    level,
                                                    applicationName,
                                                    Assembly.GetExecutingAssembly().GetName().Name,
                                                    domainUserName,
                                                    userName);
            }, TaskCreationOptions.PreferFairness);
            
        }

        /// <summary>
        /// Write log
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <param name="level"></param>
        public static void WriteLog(ILogWriter logger,string title, string message,
            string userName, VWFSCN.IT.Log.LogType type, LogStatus status, LogPriority priority, LogSeverity level)
        {
            _writeLog(logger,title, message, userName, type, status, priority, level);
        }
    }
}
