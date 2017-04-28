using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Common;

namespace CF.VRent.Contract
{
    public interface IJourneyType
    {
        //Modified by Liu for CR0001, add parameter userID
        List<ProxyJourneyType> GetAllJourneyTypes(string userID,Lang lang);
    }
}
