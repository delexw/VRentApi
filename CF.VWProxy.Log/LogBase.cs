using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Log
{
    public class LogBase
    {
        public LogBase()
        { 
        
        }

        public virtual  void WriteLog(LogType type, string message)
        {

        }
        public virtual  void WriteException(Exception ex)
        {

        }
    }
}
