using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities
{
    public class UserInfoConverter
    {
        protected virtual UserProfile ConvertToProfile(UserExtension ue)
        {
            return new UserProfile()
            {
                BillingOption = ue.TypeOfJourney,
                UserID = Guid.Parse(ue.ID),
                CorporateID = Guid.Parse(ue.ClientID)
            };
        }

        protected virtual UserProfile ConvertToProfile(ProxyUserSetting pus)
        {
            return new UserProfile()
            {
                UserID = Guid.Parse(pus.ID)
            };
        }
    }
}
