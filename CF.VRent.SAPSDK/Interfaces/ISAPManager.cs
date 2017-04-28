using CF.VRent.SAPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.SAPSDK.Interfaces
{
    public interface ISAPManager
    {
        PostEntityCollection Common { get; }
        PostEntityCollection Header { get; }
        PostEntityCollection Item { get; }
        PostEntityCollection FileName { get; }
    }
}
