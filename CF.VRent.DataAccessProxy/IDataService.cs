
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender.Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Serialization;


namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IDataService
    {

        #region Requesting Fapiao
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ReturnResultExt UpdateFapiaoRequest(ProxyFapiaoRequest request,UserProfile opera);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ReturnResultExtRetrieve RetrieveFapiaoRequestDetailByFapiaoSource(int proxyBookingID, int[] fapiaoSource, UserProfile opera);

        
        #endregion

        #region Reservation related API

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ReturnResultBulkSink BulkSyncProxyReservations(ProxyReservation[] reservations);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyReservation[] RetrieveReservations(Guid userID, string[] states);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyReservationsWithPaging RetrieveReservationsWithPaging(ProxyReservationsWithPaging pagedBookings);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyReservationsWithPaging RetrieveReservationsWithPagingByRole(ProxyReservationsWithPaging pagedBookings,ProxyUserSetting userInfo);
        
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyReservation RetrieveReservationByBookingID(int proxyBookingID);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyReservation CreateProxyReservation(ProxyReservation reservation, ProxyBookingPayment upPayment, ProxyBookingPrice pbp);


        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyReservation CancelProxyReservation(ProxyReservation reservation, ProxyBookingPrice pbp);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyReservation UpdateProxyReservation(ProxyReservation reservation, ProxyBookingPrice pbp);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyBookingPrice LoadPrincingItems(int proxyBookingID);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyBookingPrice SavingPrincingItems(ProxyBookingPrice pbp);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        bool AddBookingIndirectFeePayment(IEnumerable<BookingIndirectFeePayment> records);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int UpdateBookingIndirectFeePayment(IEnumerable<BookingIndirectFeePayment> records);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        IEnumerable<BookingIndirectFeePayment> GetBookingIndirectFeePaymentByBookingID(int bookingId);

        #endregion

        #region Payment related APIs
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        IEnumerable<ProxyReservationPayment> GetWaitingPayDUBBookings();

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int UpdateBookingPaymentState(ProxyBookingPayment bookp, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int AddBookingPayment(ProxyBookingPayment bookp, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int AddOrderAfterPayment(ProxyOrder order, string userId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        void SendPaymentEmail(EmailParameterEntity paras, string emailType, string[] emailAddress);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        IEnumerable<ProxyBookingPayment> GetBookingPaymentByPaymentId(int paymentId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        IEnumerable<ProxyBookingPayment> GetBookingPaymentByBookingId(int bookingId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        IEnumerable<BookingIndirectFee> GetTotalIndirectFeeForAll();

        #endregion

        #region FapiaoData

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyFapiao[] RetrieveMyFapiaoData (Guid userID);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ProxyFapiao RetrieveFapiaoDataDetail(int FapiaoDataID);

        #endregion

        #region Indirect Fee
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        IndirectFeeType[] RetrieveIndirectFeeTypes();

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int AddIndirectFeeTypes(IndirectFeeType[] addedTypes);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ReturnResultRetrieveOrderItems FindBookingOrders(int bookingId, string[] groups, ProxyUserSetting operationInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ReturnResultAddIndirectFeeItems AddOrderItemsByProxyBookingID(int proxyBookingID, ProxyOrderItem[] orderItems, ProxyUserSetting operationInfo);

        #endregion

        #region User Transfer
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        UserTransferCUDResult AddTransferRequest(UserTransferRequest transferRequest, ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        UserTransferCUDResult UpdateTransferRequest(UserTransferRequest transferRequest, ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        UserTransferRResult RetrieveTransferRequests(ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        UserTransferRResult RetrievePendingTransferRequests(Guid userID);        
        #endregion

        #region Common
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DBEntityAggregation<Country, DBConditionObject> GetAllCountries(DBConditionObject condition);
        #endregion

        #region Statistic
        /// <summary>
        /// Get statistic of ccb
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DBEntityAggregation<GeneralLedgerStatisticCCB, DBConditionObject> GetCCBStatistic(DBConditionObject condition);
        /// <summary>
        /// Get statistic of dub
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DBEntityAggregation<GeneralLedgerStatisticDUB, DBConditionObject> GetDUBStatistic(DBConditionObject condition);
        /// <summary>
        /// Add GL header
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        long AddGeneralLedgerHeader(GeneralLedgerHeader entity);
        /// <summary>
        /// Add GL item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        long AddGeneralLedgerItem(GeneralLedgerItem entity);
        /// <summary>
        /// Add GL details
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        long AddGeneralLedgerItemDetails(GeneralLedgerItemDetail entity); 
        #endregion
    }


}
