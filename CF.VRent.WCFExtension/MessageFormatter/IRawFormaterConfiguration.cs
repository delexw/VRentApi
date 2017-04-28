using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.WCFExtension.MessageFormatter
{
    public interface IRawFormaterConfiguration
    {
        bool Enabled { get; set; }
        string JsonFormatter { get; set; }
        string OutGoingFormatter { get; set; }
    }
}
