using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.AccountingService;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities.FapiaoPreferenceProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Entities.PaymentService;
using CF.VRent.Entities.TermsConditionService;
using CF.VRent.UPSDK.Entities;
using CF.VRent.UserStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using ConfigClient = CF.VRent.Entities.KEMASWSIF_CONFIGRef;

namespace CF.VRent.Contract
{

    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IVrentService
    {
        #region DUB
        [OperationContract]
        DUBDetailSearchConditions RetrieveDUBDetailsWithPaging(string bookingNumber, string userID,string userName, string beginDate, string endDate, string status, string itemsPerPage, string pageNumber);
        #endregion

        #region Debit-Note and CCB List

        [OperationContract]
        DebitNoteDetailSearchConditions RetrieveDebitNotesDetailsByID(string debitNoteID, string bookingNumber, string userID, string userName, string beginDate, string endDate, string itemsPerPage, string pageNumber);

        [OperationContract]
        DebitNotePeriod[] RetrieveDebitNotesPeriods();

        [OperationContract]
        DebitNotesSearchConditions RetrieveDebitNotesWithPaging(string clientID,string state,string beginDate,string endDate,string itemsPerPage,string pageNumber);

        [OperationContract]
        DebitNote RetrieveDebitNotesByID(string debitNoteID);

        [OperationContract]
        DebitNote UpdateDebitNoteByID(string debitNoteID, DebitNotePaymentState state);

        #endregion

        #region Client Managemnt
        [OperationContract]
        UserCompanyExtenstion CreateClient(CompanyProfileRequest companyProfile);

        [OperationContract]
        UserCompanyExtenstion UpdateClient(string ClientID, UserCompanyExtenstion companyProfile);

        [OperationContract]
        UserCompanyExtenstion[] RetrieveClients();

        [OperationContract]
        UserCompanyExtenstion RetrieveClientByID(string ClientID);

        [OperationContract]
        UserCompanyExtenstion EnableDisableClient(string ClientID, Status status);

        #endregion

        #region Requesting Fapiao
        [OperationContract]
        ProxyFapiaoRequest UpdateFapiaoRequest(string lang, ProxyFapiaoRequest request);

        [OperationContract]
        ProxyFapiaoRequest[] RetrieveFapiaoRequestBySource(string bookingId, string fapiaoSource, string lang);
        #endregion

        [OperationContract]
        ProxyReservationsWithPagingResponse FindReservationsWithPaging(ProxyReservationsWithPaging pagedBookings);

        #region Indirect Fee Types
        [OperationContract]
        IndirectFeeType[] GetIndirectFeeTypes();

        [OperationContract]
        int AddIndirectFeeTypes(IndirectFeeType[] newTypes);

        [OperationContract]
        ProxyOrderItem[] FindOrderItems(string bookingId, string group);

        [OperationContract]
        int AddOrderItemsByBookingID(string bookingID, ProxyOrderItem[] orderItems);

        #endregion

        [OperationContract]
        KemasReservationEntity UpdateReservation(string bookingId, KemasReservationEntity bookingData, string lang);

        [OperationContract]
        int CancelReservation(string bookingId, string lang);

        [OperationContract]
        KemasReservationEntity CreateReservationNew(KemasReservationEntity bookingDate, string lang);

        [OperationContract]
        KemasReservationEntity FindReservationNew(string bookingId, string lang);

        [OperationContract]
        KemasReservationEntity[] FindReservations(string uid, string state, string lang);
        #region Princing
        [OperationContract]
        checkPrice_Response CheckPrice(ProxyCheckPriceInfo info, string lang);

        [OperationContract]
        BookingPriceInfo CheckPriceDetail(ProxyCheckPriceInfo info);


        [OperationContract]
        string GetPrice(string bookingID);

        [OperationContract]
        BookingPriceInfo GetPriceDetail(string bookingID);

        [OperationContract]
        BookingPriceInfo GetCancelFees(string bookingID);
        #endregion

        [OperationContract]
        ProxyFapiao[] RetrieveFaPiaos(string userId, string bookingId, string fapiaoPreferenceId, string type, string state, string startDate, string endDate, string offset, string limit);

        [OperationContract]
        ProxyFapiao RetrieveFapiaoDetail(string fapiaoID);


        [OperationContract]
        List<ProxyCategory> GetAllCategories(string lang);

        [OperationContract]
        List<ProxyJourneyType> GetTypeOfJourney(string uid, string lang);

        [OperationContract]
        List<ProxyRight> GetAllRights();

        [OperationContract]
        int GetCountAvaliableCars(string uid, string startLocation, string bId, string dateBegin, string dateEnd, string category, string typeofJourney);

        [OperationContract]
        string[] GetAvailableCarCategories(string uid, string StartLocationID, string bId, string dateBegin, string dateEnd, string BillingOption, string creatorId, string driverId);

        [OperationContract]
        List<ProxyDriver> FindAllDrivers();

        [OperationContract]
        List<ProxyLocation> GetAllLocations();

        [OperationContract]
        SystemConfig FindSystemConfiguration();

        [OperationContract]
        CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference[] GetAllFapiaoPreference(string uid, int fapiaoTypeId, string searchName, int offset, int limit);

        [OperationContract]
        CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference CreateFapiaoPreference(CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference fp);

        [OperationContract]
        CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference GetFapiaoPreferenceDetail(string fpid);

        [OperationContract]
        void DeleteFapiaoPreference(string fpid);

        [OperationContract]
        CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference UpdateFapiaoPreference(string fpid, CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference fp);

        [OperationContract]
        string Ping();

        #region Payment
        /// <summary>
        /// Add new binding bank card
        /// </summary>
        /// <param name="upObject">unionPay param</param>
        /// <param name="uid">userId</param>
        /// <returns></returns>
        [OperationContract]
        UnionPayCustomInfo AddBindingBankCard(UnionPayCustomInfo cusObj, string userId);

        /// <summary>
        /// Get all binding band cards
        /// </summary>
        /// <param name="uid">userId</param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<PaymentCard> GetBindingBankCard(string uid);


        /// <summary>
        /// Get sms verfication code from UP
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Payment GetBindingSMSCode(UnionPayCustomInfo cardObject, string uid, string type, string price);


        [OperationContract]
        ReturnResult FeeDeduction(string bookingId, string userId);
        [OperationContract]
        Payment CancelPreauthorization(string bookingId,
            string userId, string price,
            string cardId, string smsCode,
            string tempOrderId, string tempOrderTime);

        /// <summary>
        /// Pre authorize
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="price"></param>
        /// <param name="smsCode"></param>
        /// <param name="tempOrderId"></param>
        /// <param name="tempOrderTime"></param>
        /// <returns></returns>
        [OperationContract]
        Payment PreAuthorize(string uid, string price, string cardId, string smsCode, string tempOrderId, string tempOrderTime, string bookingId);

        /// <summary>
        /// Check the payment status
        /// </summary>
        /// <param name="paymentID"></param>
        /// <returns></returns>
        [OperationContract]
        ReturnResult CheckPaymentStatus(string userId, string paymentID, string bookingId);

        /// <summary>
        /// Delete binding card and unbind card
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        ReturnResult DeleteBindingBankCard(string cardId, string userId);

        [OperationContract]
        ReturnResult CancelPreauth(string bookingId, string userId);

        /// <summary>
        /// The api used to set the retry flag of specified transaction
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        [OperationContract]
        ReturnResult BookingPreRetry(string bookingId, string retry);

        #endregion

        #region T&C
        [OperationContract]
        IEnumerable<TermsConditionExtension> GetLastestTC(string type, string isIncludeContent);

        [OperationContract]
        ReturnResult AddOrUpgradeTC(TermsCondition tc);

        [OperationContract]
        ReturnResult AcceptTC(string type);
        #endregion

        #region User Mgmt
        [OperationContract]
        UserExtension GetUserDetial(string userId);

        [OperationContract]
        EntityPager<UserExtensionHeader> GetUserList(int itemsPerPage,
            int pageNumber,
            string status, string companyName, string userName, string name, string phone);

        [OperationContract]
        IEnumerable<UserExtensionHeader> GetCompanyPendingUserList();

        [OperationContract]
        IEnumerable<UserCompanyExtenstion> GetCompanies();

        [OperationContract]
        IEnumerable<UserStatusEntity> GetStatus();

        [OperationContract]
        UserExtension CreateCorpUser(UserExtension user);

        [OperationContract]
        UserExtension UpdateUser(UserExtension user,string userId); 
        #endregion

        #region Common Data Dictionay
        [OperationContract]
        IEnumerable<Country> GetAllCountries();
        #endregion
    }
}
