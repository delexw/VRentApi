using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CF.VRent.Entities;
using System.ServiceModel.Web;
using CF.VRent.Log;
using System.Net;
using CF.VRent.BLL;
using System.Configuration;
using System.Runtime.Serialization;
using CF.VRent.Common;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Contract;
using System.ServiceModel;
using System.Diagnostics;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.WCFExtension;
using CF.VRent.Entities.FapiaoPreferenceProxy;
using Microsoft.Practices.Unity;
using CF.VRent.Common.Entities;
using System.Security.Permissions;
using CF.VRent.Common.UserContracts;
using CF.VRent.BLL.BLLFactory;

namespace Proxy
{
    /// <summary>
    /// Methods related to Reservation Operation Service
    /// </summary>

    public partial class DataService : IVrentService
    {

        #region Requesting Fapiao
        [WebInvoke(UriTemplate = "FapiaoRequests?lang={lang}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public ProxyFapiaoRequest UpdateFapiaoRequest(string lang, ProxyFapiaoRequest fapiaoRequest)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            RequestFapiaoBLL rfb = new RequestFapiaoBLL(setting);
            return rfb.UpdateFapiaoRequest(fapiaoRequest, lang);
        }

        [WebGet(UriTemplate = "FapiaoRequests?bookingId={bookingId}&fapiaoSource={fapiaoSource}&lang={lang}", ResponseFormat = WebMessageFormat.Json)]
        public ProxyFapiaoRequest[] RetrieveFapiaoRequestBySource(string bookingID, string fapiaoSource, string lang)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            int[] sources = ReservationConstantRepo.SplitFapiaoSourceToArray(fapiaoSource);

            RequestFapiaoBLL rfb = new RequestFapiaoBLL(setting);
            return rfb.RetrieveFapiaoRequestBySource(Convert.ToInt32(bookingID), sources, lang);
        }


        #endregion


        #region Indirect Fee
        [WebGet(UriTemplate = "BookingOrders?bookingId={bookingId}&group={group}", ResponseFormat = WebMessageFormat.Json)]
        public ProxyOrderItem[] FindOrderItems(string bookingId, string group)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            IndirectFeeBLL ifl = new IndirectFeeBLL(setting);
            string[] groups = IndirectFeeBLL.SplitGroupStrToArray(group);

            return ifl.RetrieveOrderItems(int.Parse(bookingId), groups);
        }

        [WebInvoke(UriTemplate = "Reservations/{bookingID}/AddOrderItems", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public int AddOrderItemsByBookingID(string bookingID, ProxyOrderItem[] orderItems)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            //IndirectFeeBLL ifl = new IndirectFeeBLL(setting);

            IIndirectFeeOperation ifl = ServiceImpInstanceFactory.CreateIndirectFeeOperationInstance(setting);

            return ifl.AddOrderItems(Convert.ToInt32(bookingID), orderItems);
        }
        #endregion

        #region All BookingOptions
        [WebGet(UriTemplate = "User/{uid}/BookingOptions?Lang={lang}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public BookingOptions GetUserBookingOptions(string uid, string lang)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);
            OptionsBLL ob = new OptionsBLL(setting);
            return ob.GetAllBookingOptions(setting.SessionID, lang);
        }

        #endregion

        #region

        [WebInvoke(UriTemplate = "Bookings", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public ProxyReservationsWithPagingResponse FindReservationsWithPaging(ProxyReservationsWithPaging pagedBookings)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            IProxyReservation ri = new ProxyReservationImpl(setting);
            return ri.FindReservationsWithPaging(pagedBookings,setting);
        }

        [WebGet(UriTemplate = "Reservations?uid={uid}&state={state}&lang={lang}", ResponseFormat = WebMessageFormat.Json)]
        public KemasReservationEntity[] FindReservations(string uid, string state, string lang)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);
            IProxyReservation ri = new ProxyReservationImpl(setting);
            string[] kemasStates = ReservationConstantRepo.SplitStatusToArray(state);
            return ri.FindReservations(Guid.Parse(uid), kemasStates, lang);
        }

        /// <summary>
        /// Update an existing Reservation 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="postData"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "Reservations/{bookingId}?Lang={lang}", Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public KemasReservationEntity UpdateReservation(string bookingId, KemasReservationEntity bookingData, string lang)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            IProxyReservation impl = UnityHelper.GetUnityContainer("ReservationContainer").Resolve<IProxyReservation>(new ResolverOverride[] { new ParameterOverride("userInfo", setting) });

            return impl.UpdateReservation(setting.SessionID, bookingData, lang);
        }

        /// <summary>
        /// Cancel a Reservation
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "Reservations/{bookingId}?Lang={lang}", Method = "DELETE", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public int CancelReservation(string bookingId, string lang)
        {
            //logTrace.StepIn();
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            IProxyReservation impl = new ProxyReservationImpl(setting);
            return impl.CancelReservation(setting.SessionID, Convert.ToInt32(bookingId), lang);
        }

        [WebInvoke(UriTemplate = "Reservations?Lang={lang}", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public KemasReservationEntity CreateReservationNew(KemasReservationEntity bookingData, string lang)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            IProxyReservation impl = UnityHelper.GetUnityContainer("ReservationContainer").Resolve<IProxyReservation>(new ResolverOverride[] { new ParameterOverride("userInfo", setting) });
            return impl.CreateReservation(setting.SessionID, bookingData, lang);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "Reservations/{bookingId}?Lang={lang}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public KemasReservationEntity FindReservationNew(string bookingId, string lang)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            IProxyReservation impl = UnityHelper.GetUnityContainer("ReservationContainer").Resolve<IProxyReservation>(new ResolverOverride[] { new ParameterOverride("userInfo", setting) });
            return impl.ProxyReservationDetail(setting.SessionID, Convert.ToInt32(bookingId), lang);
        }


        #endregion

        #region Kemas Princing Detail

        [WebInvoke(UriTemplate = "CheckPrice?Lang={lang}", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public checkPrice_Response CheckPrice(ProxyCheckPriceInfo info, string lang)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            checkPrice_Response price = null;

            Condition[] arr = new Condition[]
            { 
                new Condition(){ key="CSCarModel.vehicle_category", value=info.Category},
                new Condition(){ key="CarGroupModel.TypeOfJourney", value=info.BillingOption}
            };

            BookingSample sample = new BookingSample();

            sample.DateBegin = info.DateBegin;
            sample.DateEnd = info.DateEnd;
            sample.Driver = info.DriverID;
            sample.Conditions = arr;

            IBookingPrice ibp = new BookingPriceImpl(setting);
            price = ibp.CheckPrice(setting.ID, sample);

            return price;
        }

        [WebInvoke(UriTemplate = "PriceDetail", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public BookingPriceInfo CheckPriceDetail(ProxyCheckPriceInfo info)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            BookingPriceInfo bpi = null;

            IBookingPrice ibp = new BookingPriceImpl(setting);

            checkPrice2_RequestBookingData checkPriceInfo = new checkPrice2_RequestBookingData()
            {
                ID = info.KemasBookingID,
                DateBegin = info.DateBegin,
                DateEnd = info.DateEnd,
                Driver = info.DriverID,
                StartLocation = info.StartLocationID,
                BillingOption = Convert.ToInt32(info.BillingOption),
                Category = info.Category
            };

            checkPrice2_Response price = ibp.CheckPriceDetailed(setting.SessionID, checkPriceInfo);

            if (!string.IsNullOrEmpty(price.PriceDetails))
            {
                IPricingFactory factory = new PricingProcessor(price.PriceDetails);
                factory.Process();

                bpi = factory.Price;
            }
            else
            {
                Error kemasReturn = price.Error;
                throw new VrentApplicationException(kemasReturn.ErrorCode, kemasReturn.ErrorMessage, ResultType.KEMAS);
            }

            return bpi;

        }

        [WebGet(UriTemplate = "BookingPrice?bookingID={bookingID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string GetPrice(string bookingID)
        {

            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            IProxyReservation proxyBooking = new ProxyReservationImpl(setting);
            return proxyBooking.GetPrice(Convert.ToInt32(bookingID));
        }

        [WebGet(UriTemplate = "PriceDetail?bookingID={bookingID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public BookingPriceInfo GetPriceDetail(string bookingID)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            IProxyReservation proxyBooking = new ProxyReservationImpl(setting);
            return proxyBooking.GetPriceDetail(Convert.ToInt32(bookingID));
        }

        [WebGet(UriTemplate = "CancelFee?bookingID={bookingID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public BookingPriceInfo GetCancelFees(string bookingID)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            IProxyReservation proxyBooking = new ProxyReservationImpl(setting);
            return proxyBooking.GetCancelReservationFees(Convert.ToInt32(bookingID), setting.SessionID);
        }
        #endregion

        #region Fapiao

        [WebGet(UriTemplate = "FaPiaos?userId={userId}&bookingId={bookingId}&fapiaoPreferenceId={fapiaoPreferenceId}&type={type}&state={state}&startDate={startDate}&endDate={endDate}&offset={offset}&limit={limit}", ResponseFormat = WebMessageFormat.Json)]
        public ProxyFapiao[] RetrieveFaPiaos(string userId, string bookingId, string fapiaoPreferenceId, string type, string state, string startDate, string endDate, string offset, string limit)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(userId);

            ProxyFapiao[] myFapiaos = null;
            IBookingFapiao ibf = new BookingFapiaoImpl(setting);
            Guid currentUser = Guid.Parse(setting.ID);

            myFapiaos = ibf.RetrieveFapiaos(currentUser);

            return myFapiaos;
        }

        [WebGet(UriTemplate = "FaPiaos/{fapiaoID}", ResponseFormat = WebMessageFormat.Json)]
        public ProxyFapiao RetrieveFapiaoDetail(string fapiaoID)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            ProxyFapiao myFapiao = null;

            int fID = Convert.ToInt32(fapiaoID);

            IBookingFapiao ibf = new BookingFapiaoImpl(setting);
            myFapiao = ibf.RetrieveFapiaoDetail(fID);

            return myFapiao;
        }
        #endregion

    }

}