using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using VWFSCN.IT.Log;
using VWFSCN.IT.Log.Common;
using VWFSCN.IT.Log.Configuration;
using VWFSCN.IT.Log.Entity;
using VWFSCN.IT.Log.Framework;

namespace CF.VRent.Log
{
    /// <summary>
    /// The logic is the same as internal MulticastLogWriter of VWFSCN.IT.Log
    /// </summary>
    public class PublicMulticastLogWriter : ILogWriter
    {
        public LoggingConfiguration Config { get {
            return _config;
        } }

        private LoggingConfiguration _config = null;

		private List<ILog> _logWirters = null;

		private LogInfo _defaultLogInfo = null;

		private Thread _mainThread = null;

		private static Queue<LogInfo> _logQueue = new Queue<LogInfo>();

		private static int _sleepMillisecondsForWriteLog = 1000;

		private static Thread _writeLogThread = null;

        public PublicMulticastLogWriter(LoggingConfiguration config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			this._config = config;
			this._logWirters = new List<ILog>();
			foreach (LogListener current in this._config.Listeners)
			{
				this._logWirters.Add(current.ConvertToLogWriter());
			}
			Process currentProcess = Process.GetCurrentProcess();
			Thread currentThread = Thread.CurrentThread;
			this._defaultLogInfo = new LogInfo();
			this._defaultLogInfo.MachineIpAddress = (Utility.GetIPAddress() ?? string.Empty);
			this._defaultLogInfo.MachineName = (currentProcess.MachineName ?? string.Empty);
			this._defaultLogInfo.MachinePhysicalAddress = (Utility.GetMacAddress() ?? string.Empty);
			this._defaultLogInfo.ManagedThreadName = (currentThread.Name ?? string.Empty);
			this._defaultLogInfo.ProcessId = currentProcess.Id;
			this._defaultLogInfo.ProcessName = (currentProcess.ProcessName ?? string.Empty);
			this._defaultLogInfo.Win32ThreadId = currentThread.ManagedThreadId;
			this.StartWriteLogThread();
		}

		private void StartWriteLogThread()
		{
            if (PublicMulticastLogWriter._writeLogThread == null || (PublicMulticastLogWriter._writeLogThread.ThreadState & System.Threading.ThreadState.Stopped) == System.Threading.ThreadState.Stopped || (PublicMulticastLogWriter._writeLogThread.ThreadState & System.Threading.ThreadState.Aborted) == System.Threading.ThreadState.Aborted)
			{
				if (this._config.EnabledOwnTraceLog)
				{
					this.WriteOwnTraceLog(string.Format("write log thread create, main thread id:{0}", Thread.CurrentThread.ManagedThreadId));
				}
				this._mainThread = Thread.CurrentThread;
				ThreadStart start = new ThreadStart(this.ActualWriteLog);
                PublicMulticastLogWriter._writeLogThread = new Thread(start);
                PublicMulticastLogWriter._writeLogThread.IsBackground = false;
                PublicMulticastLogWriter._writeLogThread.Start();
			}
		}

        public void WriteLog(VWFSCN.IT.Log.LogType logType, string logTitle, LogStatus logStatus, string logMessage, LogPriority logPriority, LogSeverity logSeverity, string applicationName, string appModuleName, string currentDomainAccount, string currentUserName)
		{
			this.WriteLog(logType, logTitle, logStatus, logMessage, logPriority, logSeverity, applicationName, appModuleName, currentDomainAccount, currentUserName, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		}

        public void WriteLog(VWFSCN.IT.Log.LogType logType, string logTitle, LogStatus logStatus, string logMessage, LogPriority logPriority, LogSeverity logSeverity, string applicationName, string appModuleName, string currentDomainAccount, string currentUserName, string sourceAddress, string destinationAddress, string logicalAccessType, string resourceChangeType, string resourceChangeMessage, string resourceChangeRequest, string ruleType, string ruleMessage)
		{
			if (logSeverity >= this._config.MinSeverity)
			{
				LogInfo logInfo = this.NewLogInfo();
				logInfo.LogType = logType.ToString();
				logInfo.LogTitle = logTitle;
				logInfo.LogStatus = logStatus.ToString();
				logInfo.LogMessage = logMessage;
				logInfo.LogPriority = logPriority.ToString();
				logInfo.LogSeverity = logSeverity.ToString();
				logInfo.ApplicationName = applicationName;
				logInfo.AppModuleName = appModuleName;
				logInfo.CurrentDomainAccount = currentDomainAccount;
				logInfo.CurrentUserName = currentUserName;
				logInfo.SourceAddress = sourceAddress;
				logInfo.DestinationAddress = destinationAddress;
				logInfo.LogicalAccessType = logicalAccessType;
				logInfo.ResourceChangeType = resourceChangeType;
				logInfo.ResourceChangeMessage = resourceChangeMessage;
				logInfo.ResourceChangeRequest = resourceChangeRequest;
				logInfo.RuleType = ruleType;
				logInfo.RuleMessage = ruleMessage;
				if (this._config.EnabledSync)
				{
					this.WriteLog(logInfo);
				}
				else
				{
                    lock (PublicMulticastLogWriter._logQueue)
					{
                        if (PublicMulticastLogWriter._logQueue.Count < this._config.MaxLengthOfLogQueue)
						{
                            PublicMulticastLogWriter._logQueue.Enqueue(logInfo);
						}
					}
					this.StartWriteLogThread();
				}
			}
		}

		private LogInfo NewLogInfo()
		{
			return new LogInfo
			{
				MachineIpAddress = this._defaultLogInfo.MachineIpAddress,
				MachineName = this._defaultLogInfo.MachineName,
				MachinePhysicalAddress = this._defaultLogInfo.MachinePhysicalAddress,
				ManagedThreadName = this._defaultLogInfo.ManagedThreadName,
				ProcessId = this._defaultLogInfo.ProcessId,
				ProcessName = this._defaultLogInfo.ProcessName,
				Win32ThreadId = this._defaultLogInfo.Win32ThreadId
			};
		}

		private void ActualWriteLog()
		{
			try
			{
				this.WriteOwnTraceLog(string.Format("write log thread begin, thread id:{0}", Thread.CurrentThread.ManagedThreadId));
				while (true)
				{
					bool flag = false;
					if ((this._mainThread.ThreadState & System.Threading.ThreadState.Stopped) == System.Threading.ThreadState.Stopped || (this._mainThread.ThreadState & System.Threading.ThreadState.Aborted) == System.Threading.ThreadState.Aborted)
					{
						flag = true;
					}
					try
					{
						LogInfo[] array = null;
                        lock (PublicMulticastLogWriter._logQueue)
						{
                            array = new LogInfo[PublicMulticastLogWriter._logQueue.Count];
                            PublicMulticastLogWriter._logQueue.CopyTo(array, 0);
                            PublicMulticastLogWriter._logQueue.Clear();
						}
						this.WriteOwnTraceLog(string.Format("write log thread is running, there are {0} logs to write, main thread state:{1}, id:{2}", array.Length, this._mainThread.ThreadState, this._mainThread.ManagedThreadId));
						LogInfo[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							LogInfo logInfo = array2[i];
							try
							{
								this.WriteLog(logInfo);
							}
							catch (Exception ex)
							{
								this.WriteOwnExceptionLog(ex);
							}
						}
					}
					catch (Exception ex)
					{
						this.WriteOwnExceptionLog(ex);
					}
					if (flag)
					{
						break;
					}
                    Thread.Sleep(PublicMulticastLogWriter._sleepMillisecondsForWriteLog);
				}
				if (this._config.EnabledOwnTraceLog)
				{
					this.WriteOwnTraceLog(string.Format("write log thread end, thread id:{0}", Thread.CurrentThread.ManagedThreadId));
				}
			}
			catch (Exception ex)
			{
				this.WriteOwnTraceLog(string.Format("write log thread error, thread id:{0}", Thread.CurrentThread.ManagedThreadId));
				this.WriteOwnTraceLog(string.Format("ErrorMsg:{0}, StackTrace:{1}", ex.Message, ex.StackTrace));
				this.WriteOwnExceptionLog(ex);
			}
		}

		private void WriteLog(LogInfo logInfo)
		{
			foreach (ILog current in this._logWirters)
			{
				try
				{
					current.WriteLog(logInfo);
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Error:", new object[]
					{
						ex.Message,
						ex.StackTrace
					});
					this.WriteOwnExceptionLog(ex);
				}
			}
		}

		private void WriteOwnExceptionLog(Exception e)
		{
			if (this._config.EnabledOwnExceptionLog)
			{
				try
				{
					string ownExceptionLogFileName = this._config.OwnExceptionLogFileName;
					File.AppendAllText(ownExceptionLogFileName, string.Format("\r\nDateTime:{0}\r\nMessage:{1}\r\nStackTrace:{2}\r\n", DateTime.Now, e.Message, e.StackTrace));
				}
				catch
				{
				}
			}
		}

		private void WriteOwnTraceLog(string msg)
		{
			if (this._config.EnabledOwnTraceLog)
			{
				try
				{
					string ownTraceLogFileName = this._config.OwnTraceLogFileName;
					File.AppendAllText(ownTraceLogFileName, string.Format("\r\nDateTime:{0}\r\nMessage:{1}\r\n", DateTime.Now, msg));
				}
				catch
				{
				}
			}
		}
    }
}
