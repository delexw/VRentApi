
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.DBEntity.Respository;
using CF.VRent.Common.UserContracts;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.UPSDK;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class DataService : IDataService
    {
        private SQLServerRepository<GeneralLedgerHeader, GeneralLedgerHeaderDAL> glh;
        private SQLServerRepository<GeneralLedgerItem, GeneralLedgerItemDAL> gli;
        private SQLServerRepository<GeneralLedgerItemDetail, GeneralLedgerItemDetailDAL> glid;
        private SQLServerRepository<GeneralLedgerStatisticCCB, GeneralLedgerStatisticCCBDAL> glsc;
        private SQLServerRepository<GeneralLedgerStatisticDUB, GeneralLedgerStatisticDUBDAL> glsd;

        public DataService()
        {
            //Header
            glh = new SQLServerRepository<GeneralLedgerHeader, GeneralLedgerHeaderDAL>();
            //Item
            gli = new SQLServerRepository<GeneralLedgerItem, GeneralLedgerItemDAL>();
            //Item detail
            glid = new SQLServerRepository<GeneralLedgerItemDetail, GeneralLedgerItemDetailDAL>();
            //CCB statistic
            glsc = new SQLServerRepository<GeneralLedgerStatisticCCB, GeneralLedgerStatisticCCBDAL>();
            //DUB statistic
            glsd = new SQLServerRepository<GeneralLedgerStatisticDUB, GeneralLedgerStatisticDUBDAL>();
        }

        #region Fapiao Request
        public ReturnResultExt UpdateFapiaoRequest(ProxyFapiaoRequest request, UserProfile opera)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ReturnResultExt>
            (
                () => RequestFapiaoDAL.UpdateFapiaoRequest(request, opera)
            );
        }

        public ReturnResultExtRetrieve RetrieveFapiaoRequestDetailByFapiaoSource(int proxyBookingID, int[] fapiaoSource, UserProfile opera)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ReturnResultExtRetrieve>
            (
                () => RequestFapiaoDAL.RetrieveFapiaoRequestDetailFullByFapiaoSource(proxyBookingID, fapiaoSource,opera)
            );
        }

        #endregion


        #region Indirect Fee Types

        public IndirectFeeType[] RetrieveIndirectFeeTypes()
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<IndirectFeeType[]>
            (
                () => IndirectFeeDAL.RetrieveAllIndirectFeeTypes()
            );
        }

        public int AddIndirectFeeTypes(IndirectFeeType[] addedTypes)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>
            (
                () => IndirectFeeDAL.SaveIndirectFeeType(addedTypes)
            );
        }

        public ReturnResultRetrieveOrderItems FindBookingOrders(int bookingId, string[] groups, ProxyUserSetting operationInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ReturnResultRetrieveOrderItems>
           (
               () => IndirectFeeDAL.RetrieveBookingOrders(bookingId, groups,operationInfo)
           );
        }

        public int AddOrderItems(ProxyOrderItem[] orderItems)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>
           (
               () => IndirectFeeDAL.AddOrderItems(orderItems)
           );
        }

        public ReturnResultAddIndirectFeeItems AddOrderItemsByProxyBookingID(int proxyBookingID, ProxyOrderItem[] orderItems,ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ReturnResultAddIndirectFeeItems>
           (
               () => IndirectFeeDAL.AddOrderItemsByBookingID(proxyBookingID, orderItems, operatorInfo)
           );
        }

        #endregion

        #region APi
        public ReturnResultBulkSink BulkSyncProxyReservations(ProxyReservation[] reservations)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ReturnResultBulkSink>
                (
                    () => ReservationDAL.BulkSyncProxyReservations(reservations)
                );
        }

        public ProxyReservation CreateProxyReservation(ProxyReservation reservation, ProxyBookingPayment upPayment, ProxyBookingPrice pbp)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyReservation>
                (
                    () => ReservationDAL.CreateReservationWithPricingAndPaymentInfo(reservation, upPayment,pbp)
                );
        }

        public ProxyReservation CancelProxyReservation(ProxyReservation reservation, ProxyBookingPrice pbp)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyReservation>
                (
                    () => ReservationDAL.CancelProxyReservationWithPricing(reservation,pbp)
                );
        }

        public ProxyReservation UpdateProxyReservation(ProxyReservation reservation, ProxyBookingPrice pbp)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyReservation>
                (
                    () => ReservationDAL.UpdateReservationWithPricingInfo(reservation,pbp)
                );
        }

        public ProxyReservation[] RetrieveReservations(Guid userID,string[] states)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyReservation[]>
                (
                    () => ReservationDAL.RetrieveReservations(userID, states)
                ); 
        }

        public ProxyReservationsWithPaging RetrieveReservationsWithPaging(ProxyReservationsWithPaging pagedBookings)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyReservationsWithPaging>(
               () => ReservationDAL.RetrieveReservationsWithPaging(pagedBookings));
        }

        public ProxyReservationsWithPaging RetrieveReservationsWithPagingByRole(ProxyReservationsWithPaging pagedBookings, ProxyUserSetting userInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyReservationsWithPaging>(
               () => ReservationDAL.RetrieveReservationsWithPaging(pagedBookings,userInfo));
        }


        public ProxyReservation RetrieveReservationByBookingID(int proxyBookingID)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyReservation>
                (
                    () => ReservationDAL.RetrieveReservationDetailByID(proxyBookingID)
                ); 
        }

        #endregion


        #region 
        public ProxyFapiao[] RetrieveMyFapiaoData(Guid userID)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyFapiao[]>
                (
                    () => FapiaoDataDAL.RetrieveAllMyFapiaoData(userID)
                );     
        }

        public ProxyFapiao RetrieveFapiaoDataDetail(int FapiaoDataID)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyFapiao>
                (
                    () => FapiaoDataDAL.RetrieveFapiaoDataDetail(FapiaoDataID)
                );
        }

        #endregion

        

        public ProxyBookingPrice LoadPrincingItems(int proxyBookingID)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyBookingPrice>
                (
                    () => PricingDAL.LoadPricingItems(proxyBookingID)
                );     
        }

        public ProxyBookingPrice SavingPrincingItems(ProxyBookingPrice pbp)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyBookingPrice>
            (
                () => PricingDAL.SavePricingItems(pbp)
            );
        }

        public IEnumerable<ProxyReservationPayment> GetWaitingPayDUBBookings()
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<IEnumerable<ProxyReservationPayment>>(
                () => PaymentDAL.GetWaitingPayDUBBookings());
        }

        public int UpdateBookingPaymentState(ProxyBookingPayment bookp, string uid)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                 () => PaymentDAL.UpdateBookingPaymentState(bookp, uid)
                );
        }

        public int AddBookingPayment(ProxyBookingPayment bookp, string uid)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.AddBookingPayment(bookp, uid)
               );
        }

        public int AddOrderAfterPayment(ProxyOrder order, string userId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.AddOrderAfterPayment(order, userId)
               );
        }
        /// Send payment related email
        /// </summary>
        /// <param name="paras"></param>
        /// <param name="templateFileName"></param>
        /// <param name="emailAddress"></param>
        public void SendPaymentEmail(EmailParameterEntity paras, string emailType, string[] emailAddress)
        {
            EmailManager manager = EmailContext.CreateManager(emailType.ToEnum<EmailType>());
            manager.EmailTempParamsValue = paras;

            manager.SendEmail(emailAddress);

            //Test user
            if (manager.CurrentType.IsIncludeTester())
            {
                manager.SendEmail(manager.EmailAddressesGroups[EmailConstants.TestUserKey].GetAllAddresses());
            }
        }

        public IEnumerable<ProxyBookingPayment> GetBookingPaymentByPaymentId(int paymentId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<IEnumerable<ProxyBookingPayment>>(
                () => PaymentDAL.GetBookingPaymentByPaymentId(paymentId));
        }

        public IEnumerable<ProxyBookingPayment> GetBookingPaymentByBookingId(int bookingId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<IEnumerable<ProxyBookingPayment>>(
                () => PaymentDAL.GetBookingPaymentByBookingId(bookingId));
        }

        public bool AddBookingIndirectFeePayment(IEnumerable<BookingIndirectFeePayment> records)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<bool>(
                () => PaymentDAL.AddBookingIndirectFeePayment(records));
        }

        public int UpdateBookingIndirectFeePayment(IEnumerable<BookingIndirectFeePayment> records)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
               () => PaymentDAL.UpdateBookingIndirectFeePayment(records));
        }

        public IEnumerable<BookingIndirectFeePayment> GetBookingIndirectFeePaymentByBookingID(int bookingId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<IEnumerable<BookingIndirectFeePayment>>(
               () => PaymentDAL.GetBookingIndirectFeePaymentByBookingID(bookingId));
        }

        /// <summary>
        /// Get the bookings and total unpaid indirect fee
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BookingIndirectFee> GetTotalIndirectFeeForAll()
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<IEnumerable<BookingIndirectFee>>(
                           () => PaymentDAL.GetTotalIndirectFeeForAll());
        }

        #region user transfer

        public UserTransferCUDResult AddTransferRequest(UserTransferRequest transferRequest, ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<UserTransferCUDResult>(
                           () => UserTransferDAL.AddTransferRequest(transferRequest, operatorInfo));
        }

        public UserTransferCUDResult UpdateTransferRequest(UserTransferRequest transferRequest, ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<UserTransferCUDResult>(
                           () => UserTransferDAL.UpdateTransferRequest(transferRequest, operatorInfo));
        }

        public UserTransferRResult RetrieveTransferRequests(ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<UserTransferRResult>(
                           () => UserTransferDAL.RetrieveTransferRequests(operatorInfo));
        }

        #endregion


        public UserTransferRResult RetrievePendingTransferRequests(Guid userID)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<UserTransferRResult>(
                           () => UserTransferDAL.RetrievePendingTransferRequests(userID));

        }


        #region Common Data Dictionay
        /// <summary>
        /// Get all countries
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DBEntityAggregation<Country, DBConditionObject> GetAllCountries(DBConditionObject condition)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DBEntityAggregation<Country, DBConditionObject>>(
                           () =>
                           {
                               var respository = new SQLServerRepository<Country, DataDictionayDAL>();
                               return respository.Get(condition);
                           });

        }
        #endregion

        #region Statistic


        public DBEntityAggregation<GeneralLedgerStatisticCCB, DBConditionObject> GetCCBStatistic(DBConditionObject condition)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DBEntityAggregation<GeneralLedgerStatisticCCB, DBConditionObject>>(
                () =>
                {
                    return glsc.Get(condition);
                }); 
        }

        public DBEntityAggregation<GeneralLedgerStatisticDUB, DBConditionObject> GetDUBStatistic(DBConditionObject condition)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DBEntityAggregation<GeneralLedgerStatisticDUB, DBConditionObject>>(
                () =>
                {
                    return glsd.Get(condition);
                });
        }

        public long AddGeneralLedgerHeader(GeneralLedgerHeader entity)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<long>(
                           () =>
                           {
                               return glh.Add(entity);
                           }); 
        }

        public long AddGeneralLedgerItem(GeneralLedgerItem entity)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<long>(
                           () =>
                           {
                               return  gli.Add(entity);
                           });
        }

        public long AddGeneralLedgerItemDetails(GeneralLedgerItemDetail entity)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<long>(
                           () =>
                           {
                               return glid.Add(entity);
                           });
        }

        #endregion


        
    }
}
