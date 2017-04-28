using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface IBookingPrice
    {
        checkPrice_Response CheckPrice(string uid, BookingSample bookingInfo);
        checkPrice2_Response CheckPriceDetailed(string sessionID, checkPrice2_RequestBookingData bookingInfo);

        string GetPrice(string BookingID);

        string GetPriceDetailed(string BookingID);

        getCancelReservationFees_Response getCancelReservationFees(string bookingID, string sessionID);

        //ProxyBookingPrice SavePrincingInfo(ProxyBookingPrice input);

        ProxyBookingPrice LoadPrincingInfo(int bookingID);
    }


}
