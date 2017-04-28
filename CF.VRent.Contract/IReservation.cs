using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Common;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.Contract
{
    public interface IProxyReservation
    {
        #region Retrieve via ProxyBookingId
        
        string GetPrice(int BookingID);
        BookingPriceInfo GetPriceDetail(int bookingID);
        BookingPriceInfo GetCancelReservationFees(int bookingID, string sessionID);
        #endregion

        #region New API
        KemasReservationEntity ProxyReservationDetail(string sessionID, int proxyBookingID, string lang);

        KemasReservationEntity CreateReservation(string sessionID, KemasReservationEntity bookingData, string lang);

        KemasReservationEntity UpdateReservation(string sessionID,KemasReservationEntity bookingData, string lang);

        int CancelReservation(string sessionID, int proxyBookingID,string lang);

        ProxyReservationsWithPagingResponse FindReservationsWithPaging(ProxyReservationsWithPaging pagingConditions);
        ProxyReservationsWithPagingResponse FindReservationsWithPaging(ProxyReservationsWithPaging pagingConditions,ProxyUserSetting userInfo);
        KemasReservationEntity[] FindReservations(Guid uid, string[] state, string lang);

        //findReservation2_Response FindReservation(string kemasBookingID, string sessionID, string lang);

        #endregion

        #region
        //FindReservationRes FindReservationKemas(string bookingId, string userId);

        //[Obsolete("OLD Kemas API, please switch to FindReservation2(string userID, string[] status, string lang)")]
        //List<FindMyReservationRes> FindMyReservation(string uid);

        //List<FindMyReservationRes> FindMyReservation2(string userID, string[] status, string lang);

        #endregion
    }
}
