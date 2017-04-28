using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public class GetPriceDetails:IGetPriceDetails
    {
        private ProxyUserSetting _loginUser;

        public string PriceDetails { get; private set; }
        public string PriceTotal { get; private set; }

        public GetPriceDetails(ProxyUserSetting loginUser)
        {
            _loginUser = loginUser;
        }

        public ProxyBookingPrice Get(string kemasBookingId, int proxyBookingId)
        {
            IBookingPrice bmil = new BookingPriceImpl(_loginUser);

            var priced = bmil.GetPriceDetailed(kemasBookingId);

            PriceDetails = priced;

            PrincingInfoFactory factory = new PrincingInfoFactory(priced);
            factory.Process();

            PriceTotal = factory.Price.Total.ToStr();

            var pi = BookingPriceImpl.ConvertFromFEPriceInfo(factory.Price);

            if (pi != null)
            {
                pi.CreatedOn = DateTime.Now;
                pi.CreatedBy = _loginUser.ID.ToGuidNull();
                pi.ProxyBookingID = proxyBookingId;
                foreach (ProxyPrincingItem item in pi.PrincingItems)
                {
                    item.CreatedOn = pi.CreatedOn;
                    item.CreatedBy = pi.CreatedBy;
                };
            }
            else
            {
                LogInfor.WriteError(MessageCode.CVB000052.ToStr(),
                                        "BookingNo:" + kemasBookingId + "Price" + PriceDetails, _loginUser.ID);
            }

            return pi;
        }
    }
}
