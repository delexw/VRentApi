using CF.VRent.BLL;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.Entities.PaymentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using CF.VRent.Common;
using CF.VRent.Common.Entities;

namespace Proxy
{
    public partial class DataService
    {
        [WebInvoke(UriTemplate = "Booking/PreRetry/{bookingId}?retry={retry}",
            Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public ReturnResult BookingPreRetry(string bookingId, string retry)
        {
            var user = ServiceUtility.RetrieveUserInfoFromSession();

            var bll = ServiceImpInstanceFactory.CreateTransactionInstance(user);

            if (retry.ToInt() == 1)
            {
                return bll.UpdateExchangeMessageEnableRetryByBooking(bookingId.ToInt());
            }
            else
            {
                return bll.UpdateExchangeMessageDisableRetryByBooking(bookingId.ToInt());
            }
        }
    }
}