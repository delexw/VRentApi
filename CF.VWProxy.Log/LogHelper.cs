using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel.Web;
using System.Reflection;

namespace CF.VRent.Log
{
    public enum LogType
    {
        EXCEPTION,
        INFORMATION,
        ERROR,
        ALERT
    }
    /// <summary>
    /// Log helper
    /// </summary>
    public class LogHelper:LogBase
    {

        private string logDir = string.Empty;
        private string logFile = string.Empty;

        private static object _locker = new object();

        public LogHelper()
        {
            //logDir = System.Web.HttpContext.Current.Server.MapPath("\\bin\\logs");
            logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"logs");
            
            logFile = string.Format("log_{0}_{1}_{2}.log",
                DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
        }

        //public static void Write(LogType type, string message)
        //{
        //   string logDir = System.Web.HttpContext.Current.Server.MapPath("\\bin\\logs");

        //   string  logFile = string.Format("log_{0}_{1}_{2}.log",
        //        DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        //   string curTime = DateTime.Now.ToLongTimeString();

        //   string logFilePath = logDir + "\\" + logFile;

        //   StreamWriter sw = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write));

           

        //   try
        //   {
        //       sw.WriteLine("{0}       {1}       {2}", curTime, type.ToString(), message);
        //   }
        //   catch (Exception e)
        //   {
        //       Console.WriteLine("Writting log file failed!{0}", e.Message);
        //       throw e;
        //   }
        //   finally
        //   {
        //       sw.Flush();
        //       sw.Close();
        //   }
        //}

        /// <summary>
        /// Write a web fault exception to the log file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        public void WriteWebFaultException<T>(WebFaultException<T> ex)
        {
            lock (_locker)
            {
                CheckDirAndFilePath();

                string curTime = DateTime.Now.ToLongTimeString();

                string logFilePath = logDir + "\\" + logFile;
                StreamWriter sw = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write));
                try
                {
                    sw.WriteLine("{0}       EXCEPTION       {1}", curTime, ex.Message);

                    sw.WriteLine("Detail: {0}   {1}", ex.StatusCode, ex.Detail);
                    sw.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Writting log file failed!{0}", e.Message);
                    throw e;
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// Write a exception to the log file
        /// </summary>
        /// <param name="ex"></param>
        public override void WriteException(Exception ex)
        {
            lock (_locker)
            {
                //base.WriteException(ex);

                CheckDirAndFilePath();

                string curTime = DateTime.Now.ToLongTimeString();

                string logFilePath = logDir + "\\" + logFile;

                StreamWriter sw = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write));
                try
                {
                    sw.WriteLine("{0}       EXCEPTION       {1}", curTime, ex.Message);
                    if (ex.TargetSite != null)
                    {
                        sw.WriteLine("Detail: {0}", ex.StackTrace);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Writting log file failed!{0}", e.Message);
                    throw e;
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        public override void WriteLog(LogType type, string message)
        {
            lock (_locker)
            {
                CheckDirAndFilePath();

                string curTime = DateTime.Now.ToLongTimeString();

                string logFilePath = logDir + "\\" + logFile;

                StreamWriter sw = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write));
                try
                {
                    sw.WriteLine("{0}       {1}       {2}", curTime, type.ToString(), message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Writting log file failed!{0}", e.Message);
                    throw e;
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                }
            }
            //base.WriteLog(type, message);
        }

        private void CheckDirAndFilePath()
        {
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
        }


    }
}
