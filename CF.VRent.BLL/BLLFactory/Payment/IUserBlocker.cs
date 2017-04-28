using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public interface IUserBlocker
    {
        UserExtension admin { get; }
        UserExtension currentUser { get; }

        bool DeactiveDUB();
    }
}
