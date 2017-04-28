using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.WCFExtension.Behavior
{
    public interface IGlobalBehavior
    {
        bool LogMessageInfoEnabled { get; set; }  
    }
}
