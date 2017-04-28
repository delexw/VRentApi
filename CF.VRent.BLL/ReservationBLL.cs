﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Common;
using System.ServiceModel.Web;
using System.Net;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Contract;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Log;
using System.Reflection;
using System.ServiceModel;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Common.Entities;
using System.Configuration;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Common.UserContracts;
using CF.VRent.UserRole;
using System.Threading.Tasks;
using CF.VRent.Entities.AccountingService;

namespace CF.VRent.BLL
{
    public class ProxyReservationUtility
    {
        public static void SyncBooking(findReservation2_Response kemasResponse, ProxyReservation existing, ProxyUserSetting UserInfo)
        {

            ProxyReservation finalRet = null;
            #region pricing Handling
            ProxyBookingPrice pbp = null;
            int isDev = Convert.ToInt32(ConfigurationManager.AppSettings[ErrorConstants.ThrowPricingExceptionConfig]);
            bool isConsistent = ReservationConstantRepo.IsPricingConsistent(kemasResponse.Reservation, ref pbp, isDev);
            #endregion

            finalRet = KemasEntityFactory.ConvertFromReservationToProxyBooking(kemasResponse.Reservation, UserInfo);
            //do not change state
            finalRet.State = existing.State;

            //do not refresh client info
            finalRet.CorporateID = existing.CorporateID;//do not refresh client info
            finalRet.CorporateName = existing.CorporateName;

            finalRet.ProxyBookingID = existing.ProxyBookingID;
            finalRet.CreatedOn = existing.CreatedOn;
            finalRet.CreatedBy = existing.CreatedBy;
            finalRet.ModifiedBy = Guid.Parse(UserInfo.ID);
            finalRet.ModifiedOn = DateTime.Now;

            ReservationConstantRepo.SyncPricingFromBooking(finalRet, pbp);

            IDataService dsc = new DataAccessProxyManager();
            finalRet = dsc.UpdateProxyReservation(finalRet, pbp);
        }

        #region Bulk sync booking State


        public static ReturnResultBulkSink SyncBookingsWhenOpenList(ProxyUserSetting userInfo, CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation[] kemasBookings, string[] states, string language)
        {
            ReturnResultBulkSink bulkSyncRet = null;

            DateTime start = DateTime.Now;
            LogInfor.WriteInfo(string.Format("Start Sync Data {0}", start), string.Empty, userInfo.ID);

            ProxyReservation[] bookings = kemasBookings.Select(m => KemasEntityFactory.ConvertFromReservationToProxyBooking(m, userInfo)).ToArray();
            foreach (var booking in bookings)
            {
                booking.CreatedOn = DateTime.Now;
                booking.CreatedBy = ErrorConstants.SystemID;
            }

            DateTime end = DateTime.Now;
            LogInfor.WriteInfo(string.Format("End Sync Data", end), string.Empty, userInfo.ID);
            double seconds = (end - start).TotalSeconds;

            IDataService dsc = new DataAccessProxyManager();
            bulkSyncRet = dsc.BulkSyncProxyReservations(bookings);


            return bulkSyncRet;
        }

        public static string[] DoNotSyncStates()
        {
            return new string[3] { "swBookingModel/completed", "swBookingModel/canceled", "swBookingModel/autocanceled" };
        }
        #endregion

    }

    public class ProxyReservationImpl : AbstractBLL, IProxyReservation
    {
        public ProxyReservationImpl(ProxyUserSetting userInfo)
            : base(userInfo)
        {
        }

        #region Retrieve Pricing
        public string GetPrice(int BookingID)
        {
            string priceSimple = null;
            ProxyReservation proxyDB = FindReservationByBookingID(BookingID);

            if (proxyDB != null)
            {

                IKemasReservation kemas = new KemasReservationAPI();
                priceSimple = kemas.getPrice(proxyDB.KemasBookingID.ToString());
            }

            return priceSimple;

        }

        public BookingPriceInfo GetPriceDetail(int BookingID)
        {
            BookingPriceInfo price = null;
            ProxyReservation proxyDB = FindReservationByBookingID(BookingID);

            if (proxyDB != null)
            {
                IBookingPrice ibp = new BookingPriceImpl(UserInfo);
                price = BookingPriceImpl.ConvertFromDBPriceInfo(ibp.LoadPrincingInfo(proxyDB.ProxyBookingID));
            }

            return price;
        }

        public BookingPriceInfo GetCancelReservationFees(int bookingID, string sessionID)
        {
            BookingPriceInfo price = null;
            ProxyReservation proxyDB = FindReservationByBookingID(bookingID);

            if (proxyDB != null)
            {
                IKemasReservation kemas = new KemasReservationAPI();
                getCancelReservationFees_Response response = kemas.getCancelReservationFees(proxyDB.KemasBookingID.ToString(), sessionID);
                if (!string.IsNullOrEmpty(response.PriceDetails))
                {
                    IPricingFactory factory = new PricingProcessor(response.PriceDetails);
                    factory.Process();
                    price = factory.Price;
                }
            }

            return price;
        }

        #endregion

        #region Find Operations

        public static findReservation2_Response FindReservation(string kemasBookingID, string sessionID, string lang)
        {

            IKemasReservation kemasReserve = new KemasReservationAPI();
            return kemasReserve.findReservation2Kemas(kemasBookingID, sessionID, lang);
        }

        //public List<FindMyReservationRes> FindMyReservation2(string userID, string[] status, string lang)
        //{
        //    List<FindMyReservationRes> list = null;

        //    IKemasReservation kemas = new KemasReservationAPI();
        //    BookingDetail[] kemasbookings = kemas.findMyReservations2Kemas(userID, status, lang);

        //    if (kemasbookings != null)
        //    {
        //        list = kemasbookings.Select(m => KemasHelper.ConvertFromBookingDetail(m)).ToList();
        //    }

        //    return list;
        //}

        #endregion

        public KemasReservationEntity ProxyReservationDetail(string sessionID, int bookingId, string language)
        {
            KemasReservationEntity kre = null;

            if (!ReservationConstantRepo.IsValidProxyBookingID(bookingId))
            {
                throw new VrentApplicationException(ErrorConstants.BadInputFromFECode, ErrorConstants.BadInputFromFEMessage, ResultType.VRENTFE);
            }

            ProxyReservation proxyBooking = FindReservationByBookingID(bookingId);

            if (proxyBooking != null)
            {
                findReservation2_Response kemasFind = ProxyReservationImpl.FindReservation(proxyBooking.KemasBookingID.ToString(), UserInfo.SessionID, language);

                if (kemasFind.Reservation != null)
                {
                    string[] specialStates = ProxyReservationUtility.DoNotSyncStates();

                    if (!specialStates.Contains(proxyBooking.State)
                        && !specialStates.Contains(kemasFind.Reservation.State)
                        && ReservationConstantRepo.IsDataOutOfDate(kemasFind.Reservation, proxyBooking, UserInfo))
                    {
                        ProxyReservationUtility.SyncBooking(kemasFind, proxyBooking, UserInfo);
                    }

                    int throwPricingException = int.Parse(ConfigurationManager.AppSettings[ErrorConstants.ThrowPricingExceptionConfig]);
                    //prevent pricing bugs
                    bool isValidPricing = ReservationConstantRepo.CheckValidPricingFields(kemasFind.Reservation, throwPricingException);
                    //Get indirect fee/booking created fee/booking current fee
                    //Added by Adam
                    IDataService dsc = new DataAccessProxyManager();
                    proxyBooking.TotalAmount = kemasFind.Reservation.Price.ToDecimalNull();
                    var bookingPrice = dsc.BulkSyncProxyReservations(new ProxyReservation[]{
                        proxyBooking
                    });

                    kre = KemasEntityFactory.ConvertFromReservation(kemasFind.Reservation, UserInfo);

                    //Format current price of booking, FE will use this as price details. It is correct logically
                    //Added by Adam
                    IPricingFactory factory = new PricingProcessor(kemasFind.Reservation.PriceDetail);
                    factory.Process();
                    kre.PriceDetailEntity = factory.Price;

                    if (bookingPrice.SyncData.Length > 0)
                    {
                        // = CurrentTotalAmount + IndirectFeeAmount
                        if (specialStates.Contains(proxyBooking.State))
                        {
                            kre.Price = bookingPrice.SyncData[0].CurrentTotalAmount + bookingPrice.SyncData[0].IndirectFeeAmount;
                        }
                        // = TotalAmount
                        else
                        {
                            kre.Price = bookingPrice.SyncData[0].TotalAmount.Value;
                        }
                    }

                }
                else
                {
                    Error kemasError = kemasFind.Error;
                    throw new VrentApplicationException(kemasError.ErrorCode, kemasError.ErrorMessage, ResultType.KEMAS);
                }

                KemasEntityFactory.AppendProxyReservationInfo(kre, proxyBooking);
            }
            else
            {
                throw new VrentApplicationException(ErrorConstants.BookingNodeExistCode, ErrorConstants.BookingNodeExistMessage, ResultType.VRENT);
            }
            return kre;
        }

        public KemasReservationEntity CreateReservation(string sessionID, KemasReservationEntity bookingData, string lang)
        {
            KemasReservationEntity proxyResponse = null;

            if (ReservationConstantRepo.IsValidBillingOption(bookingData, UserInfo))
            {
                IKemasReservation kemasReserve = new KemasReservationAPI();
                updateReservation2_Response kemasResponse = kemasReserve.UpdateReservationKemas2(sessionID, KemasEntityFactory.ConvertUniformBookingDataToReservationData(bookingData), lang);

                //add debuging information
                if (kemasResponse != null && kemasResponse.Reservation != null)
                {
                    //defensive programming
                    #region pricing Handling

                    ProxyBookingPrice pbp = null;
                    int isDev = Convert.ToInt32(ConfigurationManager.AppSettings[ErrorConstants.ThrowPricingExceptionConfig]);

                    bool isConsistent = ReservationConstantRepo.IsPricingConsistent(kemasResponse.Reservation, ref pbp, isDev);
                    ProxyReservation beginCreateProxyBooking = KemasEntityFactory.ConvertFromReservationToProxyBooking(kemasResponse.Reservation, UserInfo);


                    #endregion

                    beginCreateProxyBooking.CreatedOn = DateTime.Now;
                    beginCreateProxyBooking.CreatedBy = beginCreateProxyBooking.UserID;

                    ReservationConstantRepo.SyncPricingFromBooking(beginCreateProxyBooking, pbp);

                    ProxyBookingPayment upPayment = new ProxyBookingPayment()
                    {
                        UPPaymentID = bookingData.UPPaymentID,
                        State = 1, // 1: active, o: delted
                        CreatedOn = beginCreateProxyBooking.CreatedOn,
                        CreatedBy = beginCreateProxyBooking.CreatedBy,
                        ModifiedOn = null,
                        ModifiedBy = null
                    };

                    IDataService proxyReplicate = new DataAccessProxyManager();

                    ProxyReservation proxyResponseDB = proxyReplicate.CreateProxyReservation(beginCreateProxyBooking, upPayment, pbp);

                    if (proxyResponseDB == null)
                    {
                        throw new VrentApplicationException(ErrorConstants.ProxyBookingFailedCode, ErrorConstants.ProxyBookingFailedCodeMsg, ResultType.VRENT);
                    }
                    else
                    {
                        proxyResponse = KemasEntityFactory.ConvertFromReservation(kemasResponse.Reservation, UserInfo);
                        KemasEntityFactory.AppendProxyReservationInfo(proxyResponse, proxyResponseDB);

                        //append UPPayment Info
                        KemasEntityFactory.AppendUPPaymentInfo(proxyResponse, bookingData);
                    }
                }
                else
                {
                    Error error = kemasResponse.Error;
                    throw new VrentApplicationException(error.ErrorCode, error.ErrorMessage, ResultType.KEMAS);
                }

            }
            else
            {
                throw new VrentApplicationException(ErrorConstants.BadInputFromFECode, ErrorConstants.BadInputFromFEMessage, ResultType.VRENTFE);
            }

            return proxyResponse;
        }


        public const string BookingWithPagingParameter = "Proxy Paging Parameters: Where-{0}|Order-{1}|ItemsPerPage-{2}|PageNumber-{3}";
        public const string BookingWithPagingTitle = "Proxy RetrieveBookings WithPaging Parameters";

        public ProxyReservationsWithPagingResponse FindReservationsWithPaging(ProxyReservationsWithPaging pagingConditions)
        {
            //check access right first
            LogInfor.WriteInfo(BookingWithPagingTitle,
                string.Format(BookingWithPagingParameter,
                    string.Join(",", pagingConditions.RawWhereConditions),
                    string.Join(",", pagingConditions.RawOrderByConditions),
                    pagingConditions.ItemsPerPage,
                    pagingConditions.PageNumber), UserInfo == null ? string.Empty : UserInfo.ID);

            //necessary check prevent sql injection
            ReservationPagingUtility.ParseWhereConditions(pagingConditions.RawWhereConditions);
            Dictionary<string, string> orderByConditions = ReservationPagingUtility.ParseOrderByConditions(pagingConditions.RawOrderByConditions);


            ProxyReservationsWithPaging paging = new ProxyReservationsWithPaging();
            if (pagingConditions.ItemsPerPage <= 0)
            {
                paging.ItemsPerPage = ReservationPagingUtility.DefaultItemsPerPage;
            }
            else
            {
                if (pagingConditions.ItemsPerPage > ReservationPagingUtility.DefaultMaxItemsPerPage)
                {
                    paging.ItemsPerPage = ReservationPagingUtility.DefaultMaxItemsPerPage;
                }
                else
                {
                    paging.ItemsPerPage = pagingConditions.ItemsPerPage;
                }
            }

            if (pagingConditions.PageNumber <= 0)
            {
                paging.PageNumber = ReservationPagingUtility.DefaultPageNumber;
            }
            else
            {
                paging.PageNumber = pagingConditions.PageNumber;
            }

            paging.RawWhereConditions = pagingConditions.RawWhereConditions;


            if (orderByConditions.Keys.Count == 0)
            {
                paging.RawOrderByConditions = new string[1] { ReservationPagingUtility.DefaultOrderBy };
            }
            else
            {
                List<string> orderByTemp = new List<string>();
                orderByTemp.Add(ReservationPagingUtility.DefaultOrderBy);
                orderByTemp.AddRange(pagingConditions.RawOrderByConditions);

                paging.RawOrderByConditions = orderByTemp.ToArray();
            }


            IDataService dsc = new DataAccessProxyManager();
            ProxyReservationsWithPaging dbPaging = dsc.RetrieveReservationsWithPaging(paging);

            ProxyReservationsWithPagingResponse response = new ProxyReservationsWithPagingResponse()
            {
                ItemsPerPage = dbPaging.ItemsPerPage,
                PageNumber = dbPaging.PageNumber,
                RawWhereConditions = dbPaging.RawWhereConditions,
                RawOrderByConditions = dbPaging.RawOrderByConditions,
                TotalPage = dbPaging.TotalPage
            };

            if (dbPaging != null && dbPaging.Reservations != null)
            {
                response.Reservations = dbPaging.Reservations.Select(m => KemasEntityFactory.ConvertFromProxyReservation(m)).ToArray();
            }

            return response;
        }

        private const string ServiceCenterRole = "SERVICE CENTER";
        private const string VrentManagerRole = "VRENT MANAGER";
        private const string EmployeeRole = "EMPLOYEE";


        public ProxyReservationsWithPagingResponse FindReservationsWithPaging(ProxyReservationsWithPaging pagingConditions, ProxyUserSetting userInfo)
        {
            //check access right first
            LogInfor.WriteInfo(BookingWithPagingTitle,
                string.Format(BookingWithPagingParameter,
                    string.Join(",", pagingConditions.RawWhereConditions),
                    string.Join(",", pagingConditions.RawOrderByConditions),
                    pagingConditions.ItemsPerPage,
                    pagingConditions.PageNumber), UserInfo == null ? string.Empty : UserInfo.ID);

            //necessary check prevent sql injection
            ReservationPagingUtility.ParseWhereConditions(pagingConditions.RawWhereConditions);
            Dictionary<string, string> orderByConditions = ReservationPagingUtility.ParseOrderByConditions(pagingConditions.RawOrderByConditions);


            ProxyReservationsWithPaging paging = new ProxyReservationsWithPaging();
            if (pagingConditions.ItemsPerPage <= 0)
            {
                paging.ItemsPerPage = ReservationPagingUtility.DefaultItemsPerPage;
            }
            else
            {
                if (pagingConditions.ItemsPerPage > ReservationPagingUtility.DefaultMaxItemsPerPage)
                {
                    paging.ItemsPerPage = ReservationPagingUtility.DefaultMaxItemsPerPage;
                }
                else
                {
                    paging.ItemsPerPage = pagingConditions.ItemsPerPage;
                }
            }

            if (pagingConditions.PageNumber <= 0)
            {
                paging.PageNumber = ReservationPagingUtility.DefaultPageNumber;
            }
            else
            {
                paging.PageNumber = pagingConditions.PageNumber;
            }

            paging.RawWhereConditions = pagingConditions.RawWhereConditions;


            if (orderByConditions.Keys.Count == 0)
            {
                paging.RawOrderByConditions = new string[1] { ReservationPagingUtility.DefaultOrderBy };
            }
            else
            {
                List<string> orderByTemp = new List<string>();
                orderByTemp.Add(ReservationPagingUtility.DefaultOrderBy);
                orderByTemp.AddRange(pagingConditions.RawOrderByConditions);

                paging.RawOrderByConditions = orderByTemp.ToArray();
            }


            //filter roles
            if (
                !(RoleUtility.IsAdministrator(userInfo)
                || RoleUtility.IsOperationManager(userInfo)
                || RoleUtility.IsServiceCenter(userInfo)
                || RoleUtility.IsVRentManager(userInfo))
               )
            {
                throw new VrentApplicationException(
                    ErrorConstants.NoPrivilegeCode,
                    string.Format(ErrorConstants.NoPrivilegeMessage, userInfo.ID),
                    ResultType.VRENT
                    );
            }

            IDataService dsc = new DataAccessProxyManager();
            ProxyReservationsWithPaging dbPaging = dsc.RetrieveReservationsWithPagingByRole(paging, userInfo);

            ProxyReservationsWithPagingResponse response = new ProxyReservationsWithPagingResponse()
            {
                ItemsPerPage = dbPaging.ItemsPerPage,
                PageNumber = dbPaging.PageNumber,
                RawWhereConditions = dbPaging.RawWhereConditions,
                RawOrderByConditions = dbPaging.RawOrderByConditions,
                TotalPage = dbPaging.TotalPage
            };

            if (dbPaging != null && dbPaging.Reservations != null)
            {
                response.Reservations = dbPaging.Reservations.Select(m => KemasEntityFactory.ConvertFromProxyReservation(m)).ToArray();
            }

            return response;
        }

        public KemasReservationEntity[] FindReservations(Guid userID, string[] states, string language)
        {
            List<KemasReservationEntity> bookings = new List<KemasReservationEntity>();

            IKemasReservation kemasReserve = KemasAccessWrapper.CreateKemasReservationAPIInstance();
            findReservations2_Request fr2Req = new findReservations2_Request();
            fr2Req.Driver = userID.ToString();
            fr2Req.States = states;
            fr2Req.Language = language;
            fr2Req.SessionID = UserInfo.SessionID;

            string[] specialStates = ProxyReservationUtility.DoNotSyncStates();

            //Call KemasReservationAPIProxy.findReservations2Kemas which is a method overrode its base method throws exception automatically
            //Modified by Adam
            findReservations2_Response fr2Res = kemasReserve.findReservations2Kemas(fr2Req);


            //if (fr2Res.Error.ErrorCode.Equals(ErrorConstants.KemasNoError))
            //{
            if (fr2Res.Reservations != null && fr2Res.Reservations.Length > 0)
            {
                ReturnResultBulkSink syncReulst = ProxyReservationUtility.SyncBookingsWhenOpenList(UserInfo, fr2Res.Reservations, states, language);

                if (syncReulst.SyncData != null)
                {
                    Parallel.ForEach(syncReulst.SyncData, proxybooking =>
                    {
                        //Don't need get booking from kemas again
                        //Modified by Adam
                        //CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation kemasBooking = fr2Res.Reservations.FirstOrDefault(m => m.ID.Equals(proxybooking.KemasBookingID.ToString()));
                        //if (kemasBooking != null)
                        //{
                        KemasReservationEntity feBooking = new KemasReservationEntity()
                        {
                            ProxyBookingID = proxybooking.ProxyBookingID,
                            DateBegin = proxybooking.DateBegin.ToStr(),
                            Price = proxybooking.TotalAmount.Value
                        };
                        //if (specialStates.Contains(proxybooking.State))
                        //{
                        //    feBooking.Price = proxybooking.TotalAmount.Value;
                        //}
                        //else
                        //{
                        //    feBooking.Price = decimal.Parse( kemasBooking.Price);
                        //}

                        //CurrentTotalAmount is the amount of money got from kemas
                        //TotalAmount is the amount of money when the booking was created
                        //Modified by Adam
                        if (specialStates.Contains(proxybooking.State))
                        {
                            feBooking.Price = proxybooking.IndirectFeeAmount + proxybooking.CurrentTotalAmount;
                        }
                        bookings.Add(feBooking);
                        //}
                    });

                    //foreach (var proxybooking in syncReulst.SyncData)
                    //{

                    //}
                }
            }
            //}
            //}
            //else
            //{
            //    throw new VrentApplicationException(fr2Res.Error.ErrorCode, fr2Res.Error.ErrorMessage, ResultType.KEMAS);
            //}

            return bookings.OrderByDescending(r => r.DateBegin.ToDate()).ToArray();
            //IDataService dsc = new DataAccessProxyManager();
            //return dsc.RetrieveReservations(userID, states)
            //.Select(m => KemasEntityFactory.ConvertFromProxyReservation(m)).ToArray();
        }


        public static ProxyReservation FindReservationByBookingID(int proxyBookingID)
        {
            //DataServiceClient dsc = new DataServiceClient();
            IDataService dsc = new DataAccessProxyManager();
            return dsc.RetrieveReservationByBookingID(proxyBookingID);
        }

        #region Cancel booking

        public const int VrentCancelSuccess = 0;

        public int CancelReservation(string sessionID, int ProxyBookingID, string lang)
        {

            int cancelRet = -1;
            KemasReservationEntity kre = null;
            ProxyReservation finalRet = null;

            ProxyReservation proxyBooking = FindReservationByBookingID(ProxyBookingID);

            Error retrieveCancelFeeError = null;
            if (proxyBooking != null)
            {
                findReservation2_Response res = ProxyReservationImpl.FindReservation(proxyBooking.KemasBookingID.ToString(), UserInfo.SessionID, lang);

                if (res.Reservation != null)
                {
                    kre = KemasEntityFactory.ConvertFromReservation(res.Reservation, UserInfo);

                    IKemasReservation kemasReserve = new KemasReservationAPI();
                    Error kemasError = kemasReserve.CancelReservation2Kemas(proxyBooking.KemasBookingID.ToString(), sessionID);

                    if (kemasError.ErrorCode.Equals("E0000"))
                    {
                        CheckAndUpdateCancelFee(proxyBooking, sessionID, ref retrieveCancelFeeError);

                        #region pricing Handling

                        ProxyBookingPrice pbp = null;
                        int isDev = Convert.ToInt32(ConfigurationManager.AppSettings[ErrorConstants.ThrowPricingExceptionConfig]);

                        bool isConsistent = ReservationConstantRepo.IsPricingConsistent(proxyBooking, ref pbp, isDev);

                        #endregion


                        //prepare update vrent DB
                        proxyBooking.State = BookingUtility.TransformToProxyBookingState("canceled");
                        proxyBooking.ModifiedBy = proxyBooking.UserID;
                        proxyBooking.ModifiedOn = DateTime.Now;

                        ReservationConstantRepo.SyncPricingFromBooking(proxyBooking, pbp);

                        //DataServiceClient dsc = new DataServiceClient();
                        IDataService dsc = new DataAccessProxyManager();
                        finalRet = dsc.CancelProxyReservation(proxyBooking, pbp);

                    }
                    else
                    {
                        //cancel the booking via kemas failed
                        throw new VrentApplicationException(kemasError.ErrorCode, kemasError.ErrorMessage, ResultType.KEMAS);
                    }
                }
                else
                {
                    Error locateKemasBookingError = res.Error;
                    //cancel the booking via kemas failed
                    throw new VrentApplicationException(locateKemasBookingError.ErrorCode, locateKemasBookingError.ErrorMessage, ResultType.KEMAS);
                }
            }
            else
            {
                throw new VrentApplicationException(ErrorConstants.BookingNodeExistCode, ErrorConstants.BookingNodeExistCode, ResultType.VRENT);
            }

            //in case retrieve cancelfee failed, synchronize state first, finally throw out error
            if (finalRet != null)
            {
                cancelRet = VrentCancelSuccess;
            }

            return cancelRet;
        }

        public const string GetCancelFeeTitle = "Kemas-Pricing Return";
        public const string GetCancelFeeMessage = "Kemas-Pricing: KemasBookingID-{0}, Price-{1}, Price Detail:{2}";

        private void CheckAndUpdateCancelFee(ProxyReservation proxyBooking, string sessionID, ref Error retrieveCancelFeeError)
        {
            IBookingPrice bfi = new BookingPriceImpl(UserInfo);
            getCancelReservationFees_Response cancelationFee = bfi.getCancelReservationFees(proxyBooking.KemasBookingID.ToString(), sessionID);

            //prevent kemas pricing-bug
            LogInfor.WriteInfo(GetCancelFeeTitle, string.Format(GetCancelFeeMessage, proxyBooking.KemasBookingID, cancelationFee.Price, cancelationFee.PriceDetails), string.Empty);

            if (!string.IsNullOrEmpty(cancelationFee.PriceDetails))
            {

                proxyBooking.TotalAmount = decimal.Parse(cancelationFee.Price);
                proxyBooking.PricingDetail = cancelationFee.PriceDetails;
            }
            else
            {
                retrieveCancelFeeError = cancelationFee.Error;

                proxyBooking.TotalAmount = decimal.Parse(cancelationFee.Price);
                proxyBooking.PricingDetail = null;
            }
        }


        public KemasReservationEntity UpdateReservation(string sessionID, KemasReservationEntity bookingData, string lang)
        {
            KemasReservationEntity uniformRet = null;
            ProxyReservation finalRet = null;
            ProxyReservation syncResult = null;

            if (!ReservationConstantRepo.IsValidProxyBookingID(bookingData.ProxyBookingID))
            {
                throw new VrentApplicationException(ErrorConstants.BadInputFromFECode, ErrorConstants.BadInputFromFEMessage, ResultType.VRENTFE);
            }

            if (ReservationConstantRepo.IsValidBillingOption(bookingData, UserInfo))
            {
                ProxyReservation proxyBooking = FindReservationByBookingID(bookingData.ProxyBookingID);
                if (proxyBooking != null)
                {
                    //findReservation2_Response KemasFind = FindReservationAndSync(proxyBooking.KemasBookingID.ToString(), sessionID, lang, ref proxyBooking);
                    findReservation2_Response KemasFind = ProxyReservationImpl.FindReservation(proxyBooking.KemasBookingID.ToString(), UserInfo.SessionID, lang);

                    if (KemasFind.Reservation != null)
                    {
                        uniformRet = KemasEntityFactory.ConvertFromReservation(KemasFind.Reservation, UserInfo);

                        IKemasReservation kemasReserve = new KemasReservationAPI();
                        CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.ReservationData kemasUpdateData = KemasEntityFactory.ConvertUniformBookingDataToReservationData(bookingData);
                        kemasUpdateData.ID = proxyBooking.KemasBookingID.ToString();

                        updateReservation2_Response kemasResponse = kemasReserve.UpdateReservationKemas2(sessionID, kemasUpdateData, lang);

                        if (kemasResponse.Reservation != null)
                        {
                            #region pricing Handling
                            ProxyBookingPrice pbp = null;
                            int isDev = Convert.ToInt32(ConfigurationManager.AppSettings[ErrorConstants.ThrowPricingExceptionConfig]);
                            bool isConsistent = ReservationConstantRepo.IsPricingConsistent(kemasResponse.Reservation, ref pbp, isDev);
                            #endregion


                            finalRet = KemasEntityFactory.ConvertFromReservationToProxyBooking(kemasResponse.Reservation, UserInfo);

                            finalRet.ProxyBookingID = bookingData.ProxyBookingID;
                            finalRet.CreatedOn = proxyBooking.CreatedOn;
                            finalRet.CreatedBy = proxyBooking.CreatedBy;
                            finalRet.ModifiedBy = proxyBooking.UserID;
                            finalRet.ModifiedOn = DateTime.Now;

                            ReservationConstantRepo.SyncPricingFromBooking(finalRet, pbp);

                            IDataService dsc = new DataAccessProxyManager();
                            finalRet = dsc.UpdateProxyReservation(finalRet, pbp);
                            syncResult = finalRet;

                            uniformRet = KemasEntityFactory.ConvertFromReservation(kemasResponse.Reservation, UserInfo);
                            KemasEntityFactory.AppendProxyReservationInfo(uniformRet, syncResult);
                            KemasEntityFactory.AppendUPPaymentInfo(uniformRet, bookingData);
                        }
                        else
                        {
                            Error kemasUpdateError = kemasResponse.Error;

                            //update the booking via kemas failed
                            throw new VrentApplicationException(kemasUpdateError.ErrorCode, kemasUpdateError.ErrorMessage, ResultType.KEMAS);
                        }
                    }
                    else
                    {
                        Error locateKemasBookingError = KemasFind.Error;
                        //cancel the booking via kemas failed
                        throw new VrentApplicationException(locateKemasBookingError.ErrorCode, locateKemasBookingError.ErrorMessage, ResultType.KEMAS);
                    }
                }
                else
                {
                    throw new VrentApplicationException(ErrorConstants.BookingNodeExistCode, ErrorConstants.BookingNodeExistCode, ResultType.VRENT);
                }
            }
            return uniformRet;

        }
        #endregion
    }

    public class BLLPrincingHelper
    {

        #region Convert a FE Princing Info to a DB princing info
        public static ProxyBookingPrice ConvertFromFEPriceInfo(BookingPriceInfo priceInfo)
        {
            ProxyBookingPrice price = null;

            try
            {
                price = new ProxyBookingPrice();


                price.Timestamp = priceInfo.TimeStamp;
                price.Total = priceInfo.Total;
                price.TagID = priceInfo.ID;

                List<ProxyPrincingItem> items = new List<ProxyPrincingItem>();

                //old version
                //items.Add(ConvertFromRental(priceInfo.Rental));

                //new version
                items.AddRange(ConvertFromRentalItems(priceInfo.Rental));

                items.Add(ConvertFromInsuranceFee(priceInfo.InsuranceFee));
                items.Add(ConvertFromFuel(priceInfo.Fuel));
                items.AddRange(ConvertFromFine(priceInfo.Fine));

                price.PrincingItems = items.ToArray();
            }
            catch
            {
                price = null;
            }


            return price;
        }

        private static ProxyPrincingItem ConvertFromRental(RentalFee rental)
        {
            return new ProxyPrincingItem()
            {
                Category = PrincingInfoFactory.RentalNode,
                Group = PrincingInfoFactory.RentalFeeCategory,
                Total = rental.Total,
            };
        }

        private static ProxyPrincingItem[] ConvertFromRentalItems(RentalFee rental)
        {
            List<ProxyPrincingItem> rentalItems = new List<ProxyPrincingItem>();
            if (rental.Items != null && rental.Items.Length > 0)
            {
                foreach (var item in rental.Items)
                {
                    ProxyPrincingItem ppi = new ProxyPrincingItem()
                    {
                        Type = item.Type,
                        Category = PrincingInfoFactory.RentalNode,
                        Group = PrincingInfoFactory.RentalFeeCategory,
                        Description = item.RawDescription,
                        Total = item.Total
                    };
                    rentalItems.Add(ppi);
                }
            }
            return rentalItems.ToArray();
        }

        private static ProxyPrincingItem ConvertFromInsuranceFee(Insurance insurance)
        {
            return new ProxyPrincingItem()
            {
                Category = PrincingInfoFactory.InsuranceFeeNode,
                Group = PrincingInfoFactory.RentalFeeCategory,
                Total = insurance.Total,
            };
        }

        private static ProxyPrincingItem ConvertFromFuel(FuelFee fuel)
        {
            return new ProxyPrincingItem()
            {
                Category = PrincingInfoFactory.FuelNode,
                Group = PrincingInfoFactory.RentalFeeCategory,
                Quantity = fuel.Kilometer,
                Total = fuel.Total,
            };
        }

        private const string FineCategor = "FINE";

        private static ProxyPrincingItem[] ConvertFromFine(FineFee fine)
        {
            List<ProxyPrincingItem> fines = new List<ProxyPrincingItem>();

            if (fine.Items != null)
            {
                foreach (var item in fine.Items)
                {
                    ProxyPrincingItem ppi = new ProxyPrincingItem()
                    {
                        Category = FineCategor,
                        Type = item.Type,
                        Group = PrincingInfoFactory.RentalFeeCategory,
                        Description = item.Description,
                        Total = (decimal)item.Total,
                    };

                    fines.Add(ppi);
                }
            }

            return fines.ToArray();
        }

        #endregion


    }

}
