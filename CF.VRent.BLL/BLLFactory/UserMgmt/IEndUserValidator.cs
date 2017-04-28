using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public interface IEndUserValidator
    {
        bool? Validate(string currentClientID, string kemasSessionID);
    }
}
