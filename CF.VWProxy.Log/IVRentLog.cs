using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VWFSCN.IT.Log;
using VWFSCN.IT.Log.Configuration;

namespace CF.VRent.Log
{
    public interface IVRentLog
    {
        ILogWriter Logger { get; }
        LoggingConfiguration Configuration { get; }

        void WriteInfo(string title, string message, string userName);
        void WriteError(string title, string message, string userName);
        void WriteAudit(string title, string message, string userName);
        void WriteDebug(string title, string message, string userName);
    }
}
