using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Log;
using CF.VRent.UserRole;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.DAL
{
    public class ReservationDAL
    {
        private const string Sp_RetrieveMyBookings = "Sp_RetrieveBookingsByUserID"; //Sp_GetFapiaoPreferenceByBookingID

        private const string Sp_RetrieveBookingDetail = "Sp_RetrieveBookingByBookingID"; //Sp_GetFapiaoPreferenceByBookingID

        private const string CreateProxyReservationWithPrincingPaymentSP = "Sp_VrentBookings_CreateWithPricingPaymentInfo";

        private const string CancelProxyReservationSP = "Sp_VrentBookings_CancelWithPricing";

        private const string UpdateProxyReservationSP = "Sp_VrentBookings_Update";

        private const string RetrieveBookingsByRowCountSP = "Sp_RetrieveBookingsWithPaging";

        private const string BulkSyncVrentBookings_ = "Sp_VrentBookings_BulkSync";

        private const string UpdateUpPaymentFromCCBToDUB = "Sp_BookingPayment_UpdateViaBooking";

        #region Booking With Payment Info

        public static ProxyReservation CreateReservationWithPricingAndPaymentInfo(ProxyReservation reservation, ProxyBookingPayment UPPayment, ProxyBookingPrice pbp)
        {
            ProxyReservation finalReturn = null;

            List<SqlDataRecord> pricing = null;
            List<SqlDataRecord> pricingDetails = null;
            if (pbp != null)
            {
                pricing = new List<SqlDataRecord>();
                pricingDetails = new List<SqlDataRecord>();
                ProceesingProxyPricing(reservation, pbp, ref pricing, ref pricingDetails);
            }

            //payment
            List<SqlDataRecord> payments = new List<SqlDataRecord>();
            if (payments != null)
            {
                SqlDataRecord upRecord = CreatePaymentRecord(UPPayment);
                payments.Add(upRecord);
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@BookingType", reservation.BillingOption));
            parameters.Add(new SqlParameter("@KemasBookingID", reservation.KemasBookingID));
            parameters.Add(new SqlParameter("@KemasBookingNumber", reservation.KemasBookingNumber));
            parameters.Add(new SqlParameter("@DateBegin", reservation.DateBegin));
            parameters.Add(new SqlParameter("@DateEnd", reservation.DateEnd));
            parameters.Add(new SqlParameter("@TotalAmount", reservation.TotalAmount));


            parameters.Add(new SqlParameter("@UserID", reservation.UserID));
            parameters.Add(new SqlParameter("@UserFirstName", reservation.UserFirstName));
            parameters.Add(new SqlParameter("@UserLastName", reservation.UserLastName));


            parameters.Add(new SqlParameter("@CorporateID", reservation.CorporateID));
            parameters.Add(new SqlParameter("@CorporateName", reservation.CorporateName));

            parameters.Add(new SqlParameter("@StartLocationID", reservation.StartLocationID));
            parameters.Add(new SqlParameter("@StartLocationName", reservation.StartLocationName));

            parameters.Add(new SqlParameter("@CreatorID", reservation.CreatorID));
            parameters.Add(new SqlParameter("@CreatorFirstName", reservation.CreatorFirstName));
            parameters.Add(new SqlParameter("@CreatorLastName", reservation.CreatorLastName));


            parameters.Add(new SqlParameter("@State", reservation.State));
            parameters.Add(new SqlParameter("@CreatedOn", reservation.CreatedOn));
            parameters.Add(new SqlParameter("@CreatedBy", reservation.CreatedBy));

            //input
            SqlParameter priceParam = new SqlParameter("@PricingInfoInput", pricing);
            priceParam.SqlDbType = SqlDbType.Structured;
            priceParam.TypeName = "dbo.BookingPrice";
            parameters.Add(priceParam);
            //input
            SqlParameter priceDetailParam = new SqlParameter("@PricingDetailsInput", pricingDetails);
            priceDetailParam.SqlDbType = SqlDbType.Structured;
            priceDetailParam.TypeName = "dbo.BookingPriceItem";
            parameters.Add(priceDetailParam);

            //payment
            SqlParameter paymentParam = new SqlParameter("@UPPaymentInput", payments);
            paymentParam.SqlDbType = SqlDbType.Structured;
            paymentParam.TypeName = "dbo.BookingPayment";
            parameters.Add(paymentParam);

            finalReturn = DataAccessProxyConstantRepo.ExecuteSPReturnReader
                (
                    CreateProxyReservationWithPrincingPaymentSP,
                    parameters.ToArray(),
                    (sqlDataReader) => ReadSingleBookingFromDataReader(sqlDataReader)
                 );

            parameters.Clear();

            return finalReturn;
        }

        //public static ProxyReservation CreateProxyReservationWithPaymentInfo(ProxyReservation reservation, ProxyBookingPayment UPPayment)
        //{
        //    ProxyReservation finalReturn = null;

        //    //pricing
        //    List<SqlDataRecord> pricing = new List<SqlDataRecord>();
        //    List<SqlDataRecord> pricingDetails = new List<SqlDataRecord>();
        //    ProceesingProxyPricing(reservation, reservation.PricingDetail, ref pricing, ref pricingDetails);

        //    //payment
        //    List<SqlDataRecord> payments = new List<SqlDataRecord>();
        //    if (payments != null)
        //    {
        //        SqlDataRecord upRecord = CreatePaymentRecord(UPPayment);
        //        payments.Add(upRecord); 
        //    }

        //    List<SqlParameter> parameters = new List<SqlParameter>();

        //    parameters.Add(new SqlParameter("@BookingType", reservation.BillingOption));
        //    parameters.Add(new SqlParameter("@KemasBookingID", reservation.KemasBookingID));
        //    parameters.Add(new SqlParameter("@KemasBookingNumber", reservation.KemasBookingNumber));
        //    parameters.Add(new SqlParameter("@DateBegin", reservation.DateBegin));
        //    parameters.Add(new SqlParameter("@DateEnd", reservation.DateEnd));
        //    parameters.Add(new SqlParameter("@TotalAmount", reservation.TotalAmount));


        //    parameters.Add(new SqlParameter("@UserID", reservation.UserID));
        //    parameters.Add(new SqlParameter("@UserFirstName", reservation.UserFirstName));
        //    parameters.Add(new SqlParameter("@UserLastName", reservation.UserLastName));


        //    parameters.Add(new SqlParameter("@CorporateID", reservation.CorporateID));
        //    parameters.Add(new SqlParameter("@CorporateName", reservation.CorporateName));

        //    parameters.Add(new SqlParameter("@StartLocationID", reservation.StartLocationID));
        //    parameters.Add(new SqlParameter("@StartLocationName", reservation.StartLocationName));

        //    parameters.Add(new SqlParameter("@CreatorID", reservation.CreatorID));
        //    parameters.Add(new SqlParameter("@CreatorFirstName", reservation.CreatorFirstName));
        //    parameters.Add(new SqlParameter("@CreatorLastName", reservation.CreatorLastName));

            
        //    parameters.Add(new SqlParameter("@State", reservation.State));
        //    parameters.Add(new SqlParameter("@CreatedOn", reservation.CreatedOn));
        //    parameters.Add(new SqlParameter("@CreatedBy", reservation.CreatedBy));

        //    //input
        //    SqlParameter priceParam = new SqlParameter("@PricingInfoInput", pricing);
        //    priceParam.SqlDbType = SqlDbType.Structured;
        //    priceParam.TypeName = "dbo.BookingPrice";
        //    parameters.Add(priceParam);
        //    //input
        //    SqlParameter priceDetailParam = new SqlParameter("@PricingDetailsInput", pricingDetails);
        //    priceDetailParam.SqlDbType = SqlDbType.Structured;
        //    priceDetailParam.TypeName = "dbo.BookingPriceItem";
        //    parameters.Add(priceDetailParam);

        //    //payment
        //    SqlParameter paymentParam = new SqlParameter("@UPPaymentInput", payments);
        //    paymentParam.SqlDbType = SqlDbType.Structured;
        //    paymentParam.TypeName = "dbo.BookingPayment";
        //    parameters.Add(paymentParam);
            
        //    finalReturn = DataAccessProxyConstantRepo.ExecuteSPReturnReader
        //        (
        //            CreateProxyReservationWithPrincingPaymentSP, 
        //            parameters.ToArray(), 
        //            (sqlDataReader) => ReadSingleBookingFromDataReader(sqlDataReader)
        //         );

        //    parameters.Clear();

        //    return finalReturn;
        //}

        #endregion

        #region Create child payment record
        //parent records
        private static SqlDataRecord CreatePaymentRecord(ProxyBookingPayment upPayment)
        {
            //[BookingID] [int] NOT NULL,
            //[UPPaymentID] [int] NOT NULL,
            //[state] [tinyint] NOT NULL,
            //[CreatedOn] [datetime] NOT NULL,
            //[CreatedBy] [uniqueidentifier] NOT NULL,
            //[ModifiedOn] [datetime] NULL,
            //[ModifiedBy] [uniqueidentifier] NULL
            SqlMetaData[] metaData = new SqlMetaData[7]
            {
                new SqlMetaData("BookingID",SqlDbType.Int),
                new SqlMetaData("UPPaymentID",SqlDbType.Int),
                new SqlMetaData("state",SqlDbType.TinyInt),
                new SqlMetaData("CreatedOn",SqlDbType.DateTime),
                new SqlMetaData("CreatedBy",SqlDbType.UniqueIdentifier),
                new SqlMetaData("ModifiedOn",SqlDbType.DateTime),
                new SqlMetaData("ModifiedBy",SqlDbType.UniqueIdentifier),
            };



            return SetPaymentValues(metaData, upPayment);
        }
        private static SqlDataRecord SetPaymentValues(SqlMetaData[] metaData, ProxyBookingPayment upPayment)
        {
            SqlDataRecord payment = new SqlDataRecord(metaData);

            //[BookingID] // will not known when insert a booking
            //,[UPPaymentID]
            //,[state]
            //,[CreatedOn]
            //,[CreatedBy]
            //,[ModifiedOn]
            //,[ModifiedBy]

            payment.SetSqlInt32(0, upPayment.ProxyBookingID);
            payment.SetSqlInt32(1, upPayment.UPPaymentID);

            payment.SetByte(2, (byte)upPayment.State);

            payment.SetDateTime(3, upPayment.CreatedOn.Value);
            payment.SetSqlGuid(4, upPayment.CreatedBy.Value);

            if (upPayment.ModifiedOn == null)
            {
                payment.SetDBNull(5);
            }
            else
            {
                payment.SetDateTime(5, upPayment.ModifiedOn.Value);
            }

            if (upPayment.ModifiedBy == null)
            {
                payment.SetDBNull(6);
            }
            else
            {
                payment.SetSqlGuid(6, upPayment.ModifiedBy.Value);
            }

            return payment;
        }

        #endregion

        #region cancel booking
        public static ProxyReservation CancelProxyReservationWithPricing(ProxyReservation reservation, ProxyBookingPrice pbp)
        {
            ProxyReservation cancelbooking = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            List<SqlDataRecord> pricing = null;
            List<SqlDataRecord> pricingDetails = null;
            if (pbp != null)
            {
                pricing = new List<SqlDataRecord>();
                pricingDetails = new List<SqlDataRecord>();
                ProceesingProxyPricing(reservation, pbp, ref pricing, ref pricingDetails); 
            }


            parameters.Add(new SqlParameter("@ProxyBookingID", reservation.ProxyBookingID));

            parameters.Add(new SqlParameter("@TotalAmount", reservation.TotalAmount));
            parameters.Add(new SqlParameter("@State", reservation.State));
            parameters.Add(new SqlParameter("@ModifiedOn", reservation.ModifiedOn));
            parameters.Add(new SqlParameter("@ModifiedBy", reservation.ModifiedBy));

            //
            //input
            SqlParameter priceParam = new SqlParameter("@PricingInfoInput", pricing);
            priceParam.SqlDbType = SqlDbType.Structured;
            priceParam.TypeName = "dbo.BookingPrice";
            parameters.Add(priceParam);

            //input
            SqlParameter priceDetailParam = new SqlParameter("@PricingDetailsInput", pricingDetails);
            priceDetailParam.SqlDbType = SqlDbType.Structured;
            priceDetailParam.TypeName = "dbo.BookingPriceItem";
            parameters.Add(priceDetailParam);


            cancelbooking = DataAccessProxyConstantRepo.ExecuteSPReturnReader
            (
                CancelProxyReservationSP,
                parameters.ToArray(),
                (sqlDataReader) => ReadSingleBookingFromDataReader(sqlDataReader)
             );

            parameters.Clear();


            return cancelbooking;
        }

        //public static ProxyReservation CancelProxyReservation(ProxyReservation reservation)
        //{
        //    ProxyReservation cancelbooking = null;

        //    List<SqlParameter> parameters = new List<SqlParameter>();

        //    List<SqlDataRecord> pricing = new List<SqlDataRecord>();
        //    List<SqlDataRecord> pricingDetails = new List<SqlDataRecord>();
        //    ProceesingProxyPricing(reservation, reservation.PricingDetail, ref pricing, ref pricingDetails);


        //    parameters.Add(new SqlParameter("@ProxyBookingID", reservation.ProxyBookingID));

        //    parameters.Add(new SqlParameter("@TotalAmount", reservation.TotalAmount));
        //    parameters.Add(new SqlParameter("@State", reservation.State));
        //    parameters.Add(new SqlParameter("@ModifiedOn", reservation.ModifiedOn));
        //    parameters.Add(new SqlParameter("@ModifiedBy", reservation.ModifiedBy));

        //    //
        //    //input
        //    SqlParameter priceParam =  new SqlParameter("@PricingInfoInput", pricing);
        //    priceParam.SqlDbType = SqlDbType.Structured;
        //    priceParam.TypeName = "dbo.BookingPrice";
        //    parameters.Add(priceParam);

        //    //input
        //    SqlParameter priceDetailParam = new SqlParameter("@PricingDetailsInput", pricingDetails.Count == 0 ? null : pricingDetails);
        //    priceDetailParam.SqlDbType = SqlDbType.Structured;
        //    priceDetailParam.TypeName = "dbo.BookingPriceItem";
        //    parameters.Add(priceDetailParam);


        //    cancelbooking = DataAccessProxyConstantRepo.ExecuteSPReturnReader
        //    (
        //        CancelProxyReservationSP,
        //        parameters.ToArray(),
        //        (sqlDataReader) => ReadSingleBookingFromDataReader(sqlDataReader)
        //     );

        //    parameters.Clear();


        //    return cancelbooking;
        //}

        #endregion

        #region update booking
        public static ProxyReservation UpdateReservationWithPricingInfo(ProxyReservation reservation, ProxyBookingPrice pbp)
        {
            ProxyReservation finalRet = null;

            List<SqlDataRecord> pricing = null;
            List<SqlDataRecord> pricingDetails = null;
            if (pbp != null)
            {
                pricing = new List<SqlDataRecord>();
                pricingDetails = new List<SqlDataRecord>();
                ProceesingProxyPricing(reservation, pbp, ref pricing, ref pricingDetails);
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@ProxyBookingID", reservation.ProxyBookingID));
            parameters.Add(new SqlParameter("@BookingType", reservation.BillingOption));
            parameters.Add(new SqlParameter("@KemasBookingID", reservation.KemasBookingID));
            parameters.Add(new SqlParameter("@KemasBookingNumber", reservation.KemasBookingNumber));
            parameters.Add(new SqlParameter("@DateBegin", reservation.DateBegin));
            parameters.Add(new SqlParameter("@DateEnd", reservation.DateEnd));
            parameters.Add(new SqlParameter("@TotalAmount", reservation.TotalAmount));


            parameters.Add(new SqlParameter("@UserID", reservation.UserID));
            parameters.Add(new SqlParameter("@UserFirstName", reservation.UserFirstName));
            parameters.Add(new SqlParameter("@UserLastName", reservation.UserLastName));


            parameters.Add(new SqlParameter("@CorporateID", reservation.CorporateID));
            parameters.Add(new SqlParameter("@CorporateName", reservation.CorporateName));

            parameters.Add(new SqlParameter("@StartLocationID", reservation.StartLocationID));
            parameters.Add(new SqlParameter("@StartLocationName", reservation.StartLocationName));

            parameters.Add(new SqlParameter("@CreatorID", reservation.CreatorID));
            parameters.Add(new SqlParameter("@CreatorFirstName", reservation.CreatorFirstName));
            parameters.Add(new SqlParameter("@CreatorLastName", reservation.CreatorLastName));

            parameters.Add(new SqlParameter("@State", reservation.State));
            parameters.Add(new SqlParameter("@ModifiedOn", reservation.ModifiedOn));
            parameters.Add(new SqlParameter("@ModifiedBy", reservation.ModifiedBy));

            SqlParameter priceParam = new SqlParameter("@PricingInfoInput", pricing);
            priceParam.SqlDbType = SqlDbType.Structured;
            priceParam.TypeName = "dbo.BookingPrice";

            parameters.Add(priceParam);
            //input
            SqlParameter priceDetailParam = new SqlParameter("@PricingDetailsInput", pricingDetails);
            priceDetailParam.SqlDbType = SqlDbType.Structured;
            priceDetailParam.TypeName = "dbo.BookingPriceItem";
            parameters.Add(priceDetailParam);

            finalRet = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyReservation>(UpdateProxyReservationSP, parameters.ToArray(), (sqldataReader) => ReadSingleBookingFromDataReader(sqldataReader));

            parameters.Clear();

            return finalRet;
        }

        //public static ProxyReservation UpdateProxyBookingWithPricingInfo(ProxyReservation reservation)
        //{
        //    ProxyReservation finalRet = null;

        //    //pricing
        //    List<SqlDataRecord> pricing = new List<SqlDataRecord>();
        //    List<SqlDataRecord> pricingDetails = new List<SqlDataRecord>();
        //    ProceesingProxyPricing(reservation, reservation.PricingDetail, ref pricing, ref pricingDetails);

        //    List<SqlParameter> parameters = new List<SqlParameter>();

        //    parameters.Add(new SqlParameter("@ProxyBookingID", reservation.ProxyBookingID));
        //    parameters.Add(new SqlParameter("@BookingType", reservation.BillingOption));
        //    parameters.Add(new SqlParameter("@KemasBookingID", reservation.KemasBookingID));
        //    parameters.Add(new SqlParameter("@KemasBookingNumber", reservation.KemasBookingNumber));
        //    parameters.Add(new SqlParameter("@DateBegin", reservation.DateBegin));
        //    parameters.Add(new SqlParameter("@DateEnd", reservation.DateEnd));
        //    parameters.Add(new SqlParameter("@TotalAmount", reservation.TotalAmount));


        //    parameters.Add(new SqlParameter("@UserID", reservation.UserID));
        //    parameters.Add(new SqlParameter("@UserFirstName", reservation.UserFirstName));
        //    parameters.Add(new SqlParameter("@UserLastName", reservation.UserLastName));


        //    parameters.Add(new SqlParameter("@CorporateID", reservation.CorporateID));
        //    parameters.Add(new SqlParameter("@CorporateName", reservation.CorporateName));

        //    parameters.Add(new SqlParameter("@StartLocationID", reservation.StartLocationID));
        //    parameters.Add(new SqlParameter("@StartLocationName", reservation.StartLocationName));

        //    parameters.Add(new SqlParameter("@CreatorID", reservation.CreatorID));
        //    parameters.Add(new SqlParameter("@CreatorFirstName", reservation.CreatorFirstName));
        //    parameters.Add(new SqlParameter("@CreatorLastName", reservation.CreatorLastName));

        //    parameters.Add(new SqlParameter("@State", reservation.State));
        //    parameters.Add(new SqlParameter("@ModifiedOn", reservation.ModifiedOn));
        //    parameters.Add(new SqlParameter("@ModifiedBy", reservation.ModifiedBy));

        //    SqlParameter priceParam = new SqlParameter("@PricingInfoInput", pricing);
        //    priceParam.SqlDbType = SqlDbType.Structured;
        //    priceParam.TypeName = "dbo.BookingPrice";

        //    parameters.Add(priceParam);
        //    //input
        //    SqlParameter priceDetailParam = new SqlParameter("@PricingDetailsInput", pricingDetails);
        //    priceDetailParam.SqlDbType = SqlDbType.Structured;
        //    priceDetailParam.TypeName = "dbo.BookingPriceItem";
        //    parameters.Add(priceDetailParam);

        //    finalRet = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyReservation>(UpdateProxyReservationSP, parameters.ToArray(), (sqldataReader) => ReadSingleBookingFromDataReader(sqldataReader));

        //    parameters.Clear();

        //    return finalRet;

        //}

        #endregion

        #region Retrieve My Reservations

        #region Retrieve My Reservations With Paging

        public const string BadInputFromFECode = "CVD000004";
        public const string BadInputFromFEMessage = "Paging Bad input {0} {1}";
        public const string BookingWithPagingParameter = "DataAccess Proxy Paging Parameters: Where-{0}|Order-{1}|ItemsPerPage-{2}|PageNumber-{3}";
        public const string BookingWithPagingTitle = "DataAccess Proxy RetrieveBookings WithPaging Parameters";

        public static ProxyReservationsWithPaging RetrieveReservationsWithPaging(ProxyReservationsWithPaging pagedBookings)
        {
            ProxyReservationsWithPaging output = null;

            //check access right first
            LogInfor.WriteInfo(BookingWithPagingTitle,
                string.Format(BookingWithPagingParameter,
                    string.Join(",", pagedBookings.RawWhereConditions),
                    string.Join(",", pagedBookings.RawOrderByConditions),
                    pagedBookings.ItemsPerPage,
                    pagedBookings.PageNumber), string.Empty);

            //necessary check prevent sql injection
            ReservationPagingUtility.ParseWhereConditions(pagedBookings.RawWhereConditions);
            Dictionary<string, string> orderByConditions = ReservationPagingUtility.ParseOrderByConditions(pagedBookings.RawOrderByConditions);


            if (pagedBookings != null)
            {
                string[] rawWhereConditions = pagedBookings.RawWhereConditions;
                string[] rawOrderByConditions = pagedBookings.RawOrderByConditions;
                int itemsPerPage = pagedBookings.ItemsPerPage;
                int pageNumber = pagedBookings.PageNumber;

                if (rawOrderByConditions.Length == 0 || itemsPerPage <= 0 || pageNumber <= 0)
                {
                    ReturnResult badInput = new ReturnResult()
                    {
                        Code = BadInputFromFECode,
                        Message = BadInputFromFEMessage,
                        Type = ResultType.VRENTFE,
                        Success = -1
                    };
                    throw new FaultException<ReturnResult>(badInput,badInput.Message);
                }
                else
                {
                    IPaging reservationPaging = PagingFactory.CreatePaging(pagedBookings.RawWhereConditions, pagedBookings.RawOrderByConditions, pagedBookings.ItemsPerPage, pagedBookings.PageNumber);
                    output = RetrieveReservations(reservationPaging as ReservationsWithPaging);

                    output.RawWhereConditions = pagedBookings.RawWhereConditions;
                    output.RawOrderByConditions = pagedBookings.RawOrderByConditions;
                }
            }

            return output;
        }

        public static ProxyReservationsWithPaging RetrieveReservationsWithPaging(ProxyReservationsWithPaging pagedBookings,ProxyUserSetting userInfo)
        {
            ProxyReservationsWithPaging output = null;

            //check access right first
            LogInfor.WriteInfo(BookingWithPagingTitle,
                string.Format(BookingWithPagingParameter,
                    string.Join(",", pagedBookings.RawWhereConditions),
                    string.Join(",", pagedBookings.RawOrderByConditions),
                    pagedBookings.ItemsPerPage,
                    pagedBookings.PageNumber), string.Empty);

            //necessary check prevent sql injection
            ReservationPagingUtility.ParseWhereConditions(pagedBookings.RawWhereConditions);
            Dictionary<string, string> orderByConditions = ReservationPagingUtility.ParseOrderByConditions(pagedBookings.RawOrderByConditions);


            if (pagedBookings != null)
            {
                string[] rawWhereConditions = pagedBookings.RawWhereConditions;
                string[] rawOrderByConditions = pagedBookings.RawOrderByConditions;
                int itemsPerPage = pagedBookings.ItemsPerPage;
                int pageNumber = pagedBookings.PageNumber;

                if (rawOrderByConditions.Length == 0 || itemsPerPage <= 0 || pageNumber <= 0)
                {
                    ReturnResult badInput = new ReturnResult()
                    {
                        Code = BadInputFromFECode,
                        Message = BadInputFromFEMessage,
                        Type = ResultType.VRENTFE,
                        Success = -1
                    };
                    throw new FaultException<ReturnResult>(badInput, badInput.Message);
                }
                else
                {
                    IPaging reservationPaging = PagingFactory.CreatePaging(pagedBookings.RawWhereConditions, pagedBookings.RawOrderByConditions, pagedBookings.ItemsPerPage, pagedBookings.PageNumber);
                    output = RetrieveReservations(reservationPaging as ReservationsWithPaging,userInfo);

                    output.RawWhereConditions = pagedBookings.RawWhereConditions;
                    output.RawOrderByConditions = pagedBookings.RawOrderByConditions;
                }
            }

            return output;
        }

        private static ProxyReservationsWithPaging RetrieveReservations(ReservationsWithPaging pagedBookings,ProxyUserSetting userInfo)
        {
            ProxyReservationsWithPaging output = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

        //public static string ServiceCenterKey { get { return "SC"; } }
        //public static string VRentManagerKey { get { return "VM"; } }
        //public static string OperationManagerKey { get { return "SCL"; } }
        //public static string AdministratorKey { get { return "ADMIN"; } }
            if(RoleUtility.IsAdministrator(userInfo))
            {
                //role parameters 
                SqlParameter rolePara = new SqlParameter("@Role", UserRoleConstants.AdministratorKey);
                parameters.Add(rolePara);
                SqlParameter companyPara = new SqlParameter("@CorporateID", userInfo.ClientID);
                parameters.Add(companyPara);
            }
            else if(RoleUtility.IsOperationManager(userInfo))
            {
                //role parameters 
                SqlParameter rolePara = new SqlParameter("@Role", UserRoleConstants.OperationManagerKey);
                parameters.Add(rolePara);
                SqlParameter companyPara = new SqlParameter("@CorporateID", userInfo.ClientID);
                parameters.Add(companyPara);

            }
            else if(RoleUtility.IsServiceCenter(userInfo))
            {
                //role parameters 
                SqlParameter rolePara = new SqlParameter("@Role", UserRoleConstants.ServiceCenterKey);
                parameters.Add(rolePara);
                SqlParameter companyPara = new SqlParameter("@CorporateID", userInfo.ClientID);
                parameters.Add(companyPara);
            }
            else if(RoleUtility.IsVRentManager(userInfo))
            {
                //role parameters 
                SqlParameter rolePara = new SqlParameter("@Role", UserRoleConstants.VRentManagerKey);
                parameters.Add(rolePara);
                SqlParameter companyPara = new SqlParameter("@CorporateID", userInfo.ClientID);
                parameters.Add(companyPara);
            }


            SqlParameter wherePara = new SqlParameter("@WhereCondition", pagedBookings.WhereClause);
            parameters.Add(wherePara);

            SqlParameter orderPara = new SqlParameter("@OrderByCondition", pagedBookings.OrderByClause);
            parameters.Add(orderPara);

            SqlParameter itemsPerPagePara = new SqlParameter("@ItemsPerPage", pagedBookings.ItemsPerPage);
            parameters.Add(itemsPerPagePara);

            SqlParameter pageNumberPara = new SqlParameter("@PageNumber", pagedBookings.PageNumber);
            parameters.Add(pageNumberPara);

            SqlParameter totalPagePara = new SqlParameter("@TotalPage", pagedBookings.TotalPage);
            totalPagePara.Direction = ParameterDirection.Output;
            parameters.Add(totalPagePara);


            ProxyReservation[] bookings = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyReservation[]>(RetrieveBookingsByRowCountSP, parameters.ToArray(), (sqldatareader) => ReadMultipleBookingFromDataReader(sqldatareader));

            parameters.Clear();

            output = new ProxyReservationsWithPaging();
            output.ItemsPerPage = pagedBookings.ItemsPerPage;
            output.PageNumber = pagedBookings.PageNumber;
            output.Reservations = bookings;
            output.TotalPage = Convert.ToInt32(totalPagePara.Value);


            return output;
        }




        private static ProxyReservationsWithPaging RetrieveReservations(ReservationsWithPaging pagedBookings)
        {
            ProxyReservationsWithPaging output = null;

            List<SqlParameter> parameters = new List<SqlParameter>();


            SqlParameter wherePara = new SqlParameter("@WhereCondition", pagedBookings.WhereClause);
            parameters.Add(wherePara);

            SqlParameter orderPara = new SqlParameter("@OrderByCondition", pagedBookings.OrderByClause);
            parameters.Add(orderPara);

            SqlParameter itemsPerPagePara = new SqlParameter("@ItemsPerPage", pagedBookings.ItemsPerPage);
            parameters.Add(itemsPerPagePara);

            SqlParameter pageNumberPara = new SqlParameter("@PageNumber", pagedBookings.PageNumber);
            parameters.Add(pageNumberPara);

            SqlParameter totalPagePara = new SqlParameter("@TotalPage", pagedBookings.TotalPage);
            totalPagePara.Direction = ParameterDirection.Output;
            parameters.Add(totalPagePara);


            ProxyReservation[] bookings = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyReservation[]>(RetrieveBookingsByRowCountSP, parameters.ToArray(), (sqldatareader) => ReadMultipleBookingFromDataReader(sqldatareader));

            parameters.Clear();

            output = new ProxyReservationsWithPaging();
            output.ItemsPerPage = pagedBookings.ItemsPerPage;
            output.PageNumber = pagedBookings.PageNumber;
            output.Reservations = bookings;
            output.TotalPage = Convert.ToInt32(totalPagePara.Value);


            return output;
        }

        #endregion


        public static ProxyReservation[] RetrieveReservations(Guid userID, string[] states)
        {
            ProxyReservation[] bookings = null;

            List<SqlParameter> parameters = new List<SqlParameter>();


            SqlParameter userPara = new SqlParameter("@UserID", userID);
            parameters.Add(userPara);

            SqlParameter priceParam = new SqlParameter("@KemasBookingState", CreateStateRecord(states));
            priceParam.SqlDbType = SqlDbType.Structured;
            priceParam.TypeName = "dbo.BookingState";
            parameters.Add(priceParam);


            //SqlParameter retPara = new SqlParameter("@ReturnValue", -1);

            bookings = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyReservation[]>(Sp_RetrieveMyBookings, parameters.ToArray(), (sqldatareader) => ReadMultipleBookingFromDataReader(sqldatareader));

            parameters.Clear();

            return bookings;
        }

        private static IEnumerable<SqlDataRecord> CreateStateRecord(string[] states)
        {
            //price.BookingID,
            //price.[Total],
            //price.[TagID],
            //price.[TimeStamp],
            //price.[state],
            //price.[CreatedOn],
            //price.[CreatedBy],
            //price.[ModifiedOn],
            //price.[ModifiedBy]
            SqlMetaData[] metaData = new SqlMetaData[1]
            {
                new SqlMetaData("KemasBookingState",SqlDbType.NVarChar,50)
            };

            return states.Select(m => SetStateValue(metaData, m));
        }

        private static SqlDataRecord SetStateValue(SqlMetaData[] metaData, string state)
        {
            SqlDataRecord kemasState = new SqlDataRecord(metaData);
            kemasState.SetString(0, state);

            return kemasState;
        }

        public static ProxyReservation RetrieveReservationDetailByID(int ProxyBookingID)
        {
            ProxyReservation booking = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter proxyBookingID = new SqlParameter("@ProxyBookingID", ProxyBookingID);
            parameters.Add(proxyBookingID);

            booking = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyReservation>(Sp_RetrieveBookingDetail, parameters.ToArray(), (sqldatareader) => ReadSingleBookingFromDataReader(sqldatareader));

            parameters.Clear();

            return booking;
        }

        #endregion
        
        #region Helper Methods

        private static ProxyReservation ReadSingleBookingFromDataReader(SqlDataReader sqlReader)
        {
            ProxyReservation booking = new ProxyReservation();

            while (sqlReader.Read())
            {
                //  [ID]
                //  ,[BookingType]
                //,[KemasBookingID]
                //,[KemasBookingNumber]
                booking.ProxyBookingID = Convert.ToInt32(sqlReader[0].ToString());
                booking.BillingOption = Convert.ToInt32(sqlReader[1].ToString());
                booking.KemasBookingID = Guid.Parse(sqlReader[2].ToString());
                booking.KemasBookingNumber = sqlReader[3].ToString();

                //,[DateBegin]
                //,[DateEnd]
                //,[TotalAmount]
                booking.DateBegin = Convert.ToDateTime(sqlReader[4].ToString());
                booking.DateEnd = Convert.ToDateTime(sqlReader[5].ToString());
                booking.TotalAmount = Convert.ToDecimal(sqlReader[6].ToString());

                //,[UserID]
                //,[UserFirstName]
                //,[UserLastName]
                booking.UserID = Guid.Parse(sqlReader[7].ToString());
                booking.UserFirstName = sqlReader[8].ToString();
                booking.UserLastName = sqlReader[9].ToString();

                //,[CorporateID]
                //,[CorporateName]
                booking.CorporateID = sqlReader[10].ToString();
                booking.CorporateName = sqlReader[11].ToString();

                //,[CreatorID]
                //,[CreatorFirstName]
                //,[CreatorLastName]
                booking.CreatorID = sqlReader[12].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[12].ToString());
                booking.CreatorFirstName = sqlReader[13].ToString();
                booking.CreatorLastName = sqlReader[14].ToString();

                //,[StartLocationID]
                //,[StartLocationName]
                booking.StartLocationID = sqlReader[15].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[15].ToString());
                booking.StartLocationName = sqlReader[16].ToString();

                //,[State]
                //,[CreatedOn]
                //,[CreatedBy]
                //,[ModifiedOn]
                //,[ModifiedBy]
                booking.State = sqlReader[17].ToString();
                booking.CreatedOn = Convert.ToDateTime(sqlReader[18].ToString());
                booking.CreatedBy = Guid.Parse(sqlReader[19].ToString());

                booking.ModifiedOn = sqlReader[20].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[20].ToString());
                booking.ModifiedBy = sqlReader[21].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[21].ToString());
            }

            return booking;
        }

        private static ProxyReservation[] ReadMultipleBookingFromDataReader(SqlDataReader sqlReader)
        {
            List<ProxyReservation> bookings = new List<ProxyReservation>();

            while (sqlReader.Read())
            {
                ProxyReservation booking = new ProxyReservation();

                booking.ProxyBookingID = Convert.ToInt32(sqlReader[0].ToString());

                booking.BillingOption = Convert.ToInt32(sqlReader[1].ToString());
                booking.KemasBookingID = Guid.Parse(sqlReader[2].ToString());
                booking.KemasBookingNumber = sqlReader[3].ToString();

                //,[DateBegin]
                //,[DateEnd]
                //,[TotalAmount]
                booking.DateBegin = Convert.ToDateTime(sqlReader[4].ToString());
                booking.DateEnd = Convert.ToDateTime(sqlReader[5].ToString());
                booking.TotalAmount = Convert.ToDecimal(sqlReader[6].ToString());

                //,[UserID]
                //,[UserFirstName]
                //,[UserLastName]
                booking.UserID = Guid.Parse(sqlReader[7].ToString());
                booking.UserFirstName = sqlReader[8].ToString();
                booking.UserLastName = sqlReader[9].ToString();

                //,[CorporateID]
                //,[CorporateName]
                booking.CorporateID = sqlReader[10].ToString();
                booking.CorporateName = sqlReader[11].ToString();

                //,[CreatorID]
                //,[CreatorFirstName]
                //,[CreatorLastName]
                booking.CreatorID = sqlReader[12].Equals(DBNull.Value)? new Nullable<Guid>(): Guid.Parse(sqlReader[12].ToString());
                booking.CreatorFirstName = sqlReader[13].ToString();
                booking.CreatorLastName = sqlReader[14].ToString();

                //,[StartLocationID]
                //,[StartLocationName]
                booking.StartLocationID = sqlReader[15].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[15].ToString());
                booking.StartLocationName = sqlReader[16].ToString();

                //,[State]
                //,[CreatedOn]
                //,[CreatedBy]
                //,[ModifiedOn]
                //,[ModifiedBy]
                booking.State = sqlReader[17].ToString();
                booking.CreatedOn = Convert.ToDateTime(sqlReader[18].ToString());
                booking.CreatedBy = Guid.Parse(sqlReader[19].ToString());

                booking.ModifiedOn = sqlReader[20].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[20].ToString());
                booking.ModifiedBy = sqlReader[21].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[21].ToString());

                if (sqlReader.FieldCount >= 24)
                {
                    booking.IndirectFeeAmount = sqlReader[22].Equals(DBNull.Value) ? 0 : sqlReader[22].ToDecimal();

                    booking.CurrentTotalAmount = sqlReader[23].Equals(DBNull.Value) ? 0 : sqlReader[23].ToDecimal();
                }
                bookings.Add(booking);
            }

            return bookings.ToArray();
        }

        private static void ProceesingProxyPricing(ProxyReservation booking, ProxyBookingPrice pbp, ref List<SqlDataRecord> pricing, ref List<SqlDataRecord> pricingItems)
        {
            List<ProxyBookingPrice> prices = new List<ProxyBookingPrice>();
            prices.Add(pbp);
            pricing.AddRange(PricingDAL.CreatePrincingRecord(prices));
            pricingItems.AddRange(PricingDAL.CreatePrincingDetailRecords(pbp));

            if (pricing.Count == 0)
            {
                pricing = null;
            }

            if (pricingItems.Count == 0)
            {
                pricingItems = null;
            }
        }

        //private static void ProceesingProxyPricing(ProxyReservation booking, string priceDetail, ref List<SqlDataRecord> pricing, ref List<SqlDataRecord> pricingItems)
        //{
        //    ProxyBookingPrice pbp = null;
        //    List<ProxyBookingPrice> prices = new List<ProxyBookingPrice>();

        //    if (!string.IsNullOrEmpty(priceDetail))
        //    {
        //        PrincingInfoFactory factory = new PrincingInfoFactory(priceDetail);
        //        factory.Process();
        //        pbp = PrincingHelper.ConvertFromFEPriceInfo(factory.Price);
        //    }
        //    else
        //    {
        //        pbp = new ProxyBookingPrice();
        //        pbp.Total = booking.TotalAmount.Value;
        //        pbp.PrincingItems = new List<ProxyPrincingItem>().ToArray();
        //    }

        //    //set system columns to proper values
        //    pbp.CreatedOn = booking.CreatedOn;
        //    pbp.CreatedBy = booking.CreatedBy;

        //    if (booking.ProxyBookingID > 0)
        //    {
        //        // update booking
        //        pbp.ProxyBookingID = booking.ProxyBookingID;
        //        pbp.ModifiedOn = booking.ModifiedOn;
        //        pbp.ModifiedBy = booking.ModifiedBy;
        //    }
        //    else
        //    {
        //        //create booking
        //        pbp.ProxyBookingID = -1;
        //    }


        //    foreach (ProxyPrincingItem item in pbp.PrincingItems)
        //    {
        //        item.CreatedOn = booking.CreatedOn;
        //        item.CreatedBy = booking.CreatedBy;
        //    } 

        //    prices.Add(pbp);
        //    pricing.AddRange(PricingDAL.CreatePrincingRecord(prices));
        //    pricingItems.AddRange(PricingDAL.CreatePrincingDetailRecords(pbp));



        //}

        #endregion

        #region Retrieve Bookings with Paging



        public class ReservationsWithPaging : IPaging
        {
            //DB columns
            public const string ID = "ID";
            public const string KemasBookingID = "KEMASBOOKINGID";
            public const string KemasBookingNumber = "KEMASBOOKINGNUMBER";
            public const string DateBegin = "DATEBEGIN";
            public const string DateEnd = "DATEEND";

            public const string Driver = "USERID";
            public const string DriverFirstName = "USERFIRSTNAME";
            public const string DriverLastName = "USERLASTNAME";

            public const string CorporateID = "CORPORATEID";
            public const string CorporateName = "CORPORATENAME";

            public const string Creator = "CREATORID";
            public const string CreatorFirstName = "CREATORFIRSTNAME";
            public const string CreatorLastName = "CREATORLASTNAME";

            public const string StartLocationID = "STARTLOCATIONID";
            public const string StartLocationName = "STARTLOCATIONNAME";
            
            public const string State = "STATE";
            public const string BillingOption = "BOOKINGTYPE";
            public const string Price = "TOTALAMOUNT";

            public const string CreatedOn = "CREATEDON";
            public const string CreatedBy = "CREATEDBY";
            public const string ModifiedOn = "MODIFIEDON";
            public const string ModifiedBy = "MODIFIEDBY";


            private Dictionary<string, string> _whereConditions = null;
            private Dictionary<string, string> _orderByConditions = null;
            private int _itemsPerPage = -1;
            private int _pageNumber = -1;
            private int _totalPage = -1;

            private string _where;
            private string _orderBy;

            private const string _pagingSampleSqlPattern = "Select @StartRowID = vb.ID from VrentBookings as vb  {0} {1}";
            private string _pagingSampleSql = null;

            public ReservationsWithPaging(Dictionary<string, string> where, Dictionary<string, string> orderBy, int itemsPerPage, int pageNumber)
            {
                _whereConditions = where;
                _orderByConditions = orderBy;
                _itemsPerPage = itemsPerPage;
                _pageNumber = pageNumber;
            }

            #region Input from FE
            public Dictionary<string, string> WhereCondition
            {
                get
                {
                    return _whereConditions;
                }
            }

            public Dictionary<string, string> OrderByCondition
            {
                get
                {
                    return _orderByConditions;
                }
            }

            public string WhereClause
            {
                get
                {
                    return _where;
                }
            }

            public string OrderByClause
            {
                get
                {
                    return _orderBy;
                }
            }

            public int ItemsPerPage
            {
                get
                {
                    return _itemsPerPage;
                }
            }

            public int PageNumber
            {
                get
                {
                    return _pageNumber;
                }
            }

            public int TotalPage
            {
                get
                {
                    return _totalPage;
                }
            }

            #endregion
            private const string DBwherePattern = "{0} {1} {2}";
            private const string DBFuzzyNamePattern = "(UserFirstName Like '%{0}%' or UserLastName Like '%{0}%' or UserFirstName+UserLastName like '%{0}%')";
            private const string DBFuzzyStartLocationName = "StartlocationName like '%{0}%'";
            private const string DBFuzzyBookingNumber = "KemasBookingNumber like '%{0}%'";
            private const string DBOrderByPattern = " {0} {1} ";
            private void CombineWhereConditionsToDBValues()
            {
                int count = 0;

                StringBuilder sb = new StringBuilder();
                foreach (string key in _whereConditions.Keys)
                {
                    //columns.Add("ProxyBookingID != 1");
                    //columns.Add("KemasBookingID != " + Guid.NewGuid().ToString());
                    //columns.Add("DateBegin >= " + "2015-07-02 12:00:45");
                    //columns.Add("DateEnd <= " + "2015-07-02 12:00:45");
                    //columns.Add("Number != 0000000123");
                    //columns.Add("DriverID != " + Guid.NewGuid().ToString());
                    //columns.Add("CORPORATEID != " + "ABC");
                    //columns.Add("State = Created");
                    //columns.Add("BillingOption = 2");
                    //columns.Add("Price > 3.45");
                    //columns.Add("CreatedOn > " + "2015-07-02 12:00:45");
                    //columns.Add("CreatedBy != " + Guid.NewGuid().ToString());
                    //columns.Add("ModifiedOn > " + "2015-07-02 12:00:45");
                    //columns.Add("ModifiedBy != " + Guid.NewGuid().ToString());
                    //columns.Add("FAPIAOPREFERENCEID != null");
                    string temp = _whereConditions[key];
                    string tempStr = null;

                    string[] threeParts = temp.Split(' ');
                    switch (key)
                    {
                        //[ID]
                        //,[BookingType]
                        //,[KemasBookingID]
                        //,[KemasBookingNumber]
                        //,[DateBegin]
                        //,[DateEnd]
                        //,[TotalAmount]
                        //,[UserID]
                        //,[CorporateID]
                        //,[FapiaoPreferenceID]
                        //,[IsFapiaoRequested]
                        //,[State]
                        //,[CreatedOn]
                        //,[CreatedBy]
                        //,[ModifiedOn]
                        //,[ModifiedBy]

                        case ReservationPagingUtility.ProxyBookingIDField:
                            tempStr = string.Format(DBwherePattern, ID, threeParts[1], Convert.ToInt32(threeParts[2]));
                            break;
                        case ReservationPagingUtility.BillingOptionField:
                            tempStr = (string.Format(DBwherePattern, BillingOption, threeParts[1], threeParts[2]));
                            break;

                        case ReservationPagingUtility.KemasBookingIDField:
                            tempStr = (string.Format(DBwherePattern, KemasBookingID, threeParts[1], "'" + threeParts[2] + "'"));
                            break;
                        case ReservationPagingUtility.KemasBookingNumberField:
                            int fuzzyNumberIndex = temp.IndexOf("=");
                            string fuzzNumber = temp.Substring(fuzzyNumberIndex + 1).Trim();
                            tempStr = string.Format(DBFuzzyBookingNumber, fuzzNumber);

                            break;
                        case ReservationPagingUtility.DateBeginField:
                            tempStr = string.Format(DBwherePattern, DateBegin, threeParts[1], "'" + threeParts[2] + " " + threeParts[3] + "'");
                            break;
                        case ReservationPagingUtility.DateEndField:
                            tempStr = string.Format(DBwherePattern, DateEnd, threeParts[1], "'" + threeParts[2] + " " + threeParts[3] + "'");
                            break;
                        case ReservationPagingUtility.PriceField:
                            tempStr = string.Format(DBwherePattern, Price, threeParts[1], threeParts[2]);
                            break;

                        //case ReservationPagingUtility.DriverIDField:
                        //    tempStr = string.Format(DBwherePattern, Driver, threeParts[1], "'" + threeParts[2] + "'");
                        //    break;
                        //case ReservationPagingUtility.DriverFirstNameField:
                        //    tempStr = string.Format(DBwherePattern, DriverFirstName, threeParts[1], "'" + threeParts[2] + "'");
                        //    break;
                        //case ReservationPagingUtility.DriverLastNameField:
                        //    tempStr = string.Format(DBwherePattern, DriverLastName, threeParts[1], "'" + threeParts[2] + "'");
                        //    break;
                        //FE
                        case ReservationPagingUtility.FuzzyDriverName:
                            int fuzzyNameIndex = temp.IndexOf("=");
                            string fuzzName = temp.Substring(fuzzyNameIndex + 1).Trim();
                            tempStr = string.Format(DBFuzzyNamePattern, fuzzName);
                            break;
                        //

                        case ReservationPagingUtility.CreatorIDField:
                            tempStr = string.Format(DBwherePattern, Creator, threeParts[1], "'" + threeParts[2] + "'");
                            break;
                        case ReservationPagingUtility.CreatorFirstNameField:
                            tempStr = string.Format(DBwherePattern, CreatorFirstName, threeParts[1], "'" + threeParts[2] + "'");
                            break;
                        case ReservationPagingUtility.CreatorLastNameField:
                            tempStr = string.Format(DBwherePattern, CreatorLastName, threeParts[1], "'" + threeParts[2] + "'");
                            break;

                        case ReservationPagingUtility.CorporateIDField:
                            string nullStr = threeParts[2];
                            if (nullStr.Equals(ReservationPagingUtility.NULLValueFromFE))
                            {
                                tempStr = string.Format(DBwherePattern, CorporateID, threeParts[1].Equals("=") ? "IS" : "IS NOT", "NULL");
                            }
                            else
                            {
                                tempStr = string.Format(DBwherePattern, CorporateID, threeParts[1], "'" + threeParts[2] + "'");
                            }
                            break;
                        case ReservationPagingUtility.CorporateNameField:
                            string fpNullStr = threeParts[2];
                            if (fpNullStr.Equals(ReservationPagingUtility.NULLValueFromFE))
                            {
                                tempStr = string.Format(DBwherePattern, CorporateName, threeParts[1].Equals("=") ? "IS" : "IS NOT", "NULL");
                            }
                            else
                            {
                                tempStr = string.Format(DBwherePattern, CorporateName, threeParts[1], "'" + threeParts[2] + "'");
                            }
                            break;

                        case ReservationPagingUtility.StartLocationIDField:
                            string nullStr1 = threeParts[2];
                            if (nullStr1.Equals(ReservationPagingUtility.NULLValueFromFE))
                            {
                                tempStr = string.Format(DBwherePattern, StartLocationID, threeParts[1].Equals("=") ? "IS" : "IS NOT", "NULL");
                            }
                            else
                            {
                                tempStr = string.Format(DBwherePattern, StartLocationID, threeParts[1], "'" + threeParts[2] + "'");
                            }
                            break;

                        case ReservationPagingUtility.StartLocationNameField:
                            string fpNullStr2 = threeParts[2];
                            if (fpNullStr2.Equals(ReservationPagingUtility.NULLValueFromFE))
                            {
                                tempStr = string.Format(DBwherePattern, StartLocationName, threeParts[1].Equals("=") ? "IS" : "IS NOT", "NULL");
                            }
                            else
                            {
                                int fuzzyStartLocationNameIndex = temp.IndexOf("=");
                                string fuzzyStartLocationName = temp.Substring(fuzzyStartLocationNameIndex + 1).Trim();
                                tempStr = string.Format(DBFuzzyStartLocationName, fuzzyStartLocationName);
                            }
                            break;


                        case ReservationPagingUtility.StateField:
                            tempStr = string.Format(DBwherePattern, State, threeParts[1], "'" + BookingUtility.TransformToProxyBookingState( threeParts[2] ) + "'");
                            break;
                        case ReservationPagingUtility.CreatedOnField:
                            tempStr = string.Format(DBwherePattern, CreatedOn, threeParts[1], "'" + threeParts[2] + " " + threeParts[3] + "'");
                            break;
                        case ReservationPagingUtility.CreatedByField:
                            tempStr = string.Format(DBwherePattern, CreatedBy, threeParts[1], "'" + threeParts[2] + "'");
                            break;
                        case ReservationPagingUtility.ModifiedOnField:
                            tempStr = string.Format(DBwherePattern, ModifiedOn, threeParts[1], "'" + threeParts[2] + " " + threeParts[3] + "'");
                            break;
                        case ReservationPagingUtility.ModifiedByField:
                            tempStr = string.Format(DBwherePattern, ModifiedBy, threeParts[1], "'" + threeParts[2] + "'");
                            break;
                        default:
                            break;
                    }

                    if (count == _whereConditions.Keys.Count - 1)
                    {
                        sb.Append(tempStr);
                    }
                    else
                    {
                        sb.Append(tempStr + " AND ");
                    }
                    count++;
                }

                string whereTemp = sb.ToString();

                _where = whereTemp == string.Empty ? "WHERE 1=1" : "WHERE " + whereTemp;
            }

            private void CombineOrderByConditionsToDBValues()
            {
                int count = 0;

                StringBuilder sb = new StringBuilder();
                foreach (string key in _orderByConditions.Keys)
                {
                    string temp = _orderByConditions[key];
                    string tempStr = null;
                    switch (key)
                    {
                        case ReservationPagingUtility.ProxyBookingIDField:
                            tempStr = string.Format(DBOrderByPattern, ID, temp);
                            break;
                        case ReservationPagingUtility.BillingOptionField:
                            tempStr = string.Format(DBOrderByPattern, BillingOption, temp);
                            break;
                        case ReservationPagingUtility.KemasBookingIDField:
                            tempStr = string.Format(DBOrderByPattern, KemasBookingID, temp);
                            break;
                        case ReservationPagingUtility.KemasBookingNumberField:
                            tempStr = string.Format(DBOrderByPattern, KemasBookingNumber, temp);
                            break;
                        case ReservationPagingUtility.DateBeginField:
                            tempStr = string.Format(DBOrderByPattern, DateBegin, temp);
                            break;
                        case ReservationPagingUtility.DateEndField:
                            tempStr = string.Format(DBOrderByPattern, DateEnd, temp);
                            break;
                        case ReservationPagingUtility.PriceField:
                            tempStr = string.Format(DBOrderByPattern, Price, temp);
                            break;

                        case ReservationPagingUtility.DriverIDField:
                            tempStr = string.Format(DBOrderByPattern, Driver, temp);
                            break;
                        case ReservationPagingUtility.DriverFirstNameField:
                            tempStr = string.Format(DBOrderByPattern, DriverFirstName, temp);
                            break;
                        case ReservationPagingUtility.DriverLastNameField:
                            tempStr = string.Format(DBOrderByPattern, DriverLastName, temp);
                            break;

                        case ReservationPagingUtility.CorporateIDField:
                            tempStr = string.Format(DBOrderByPattern, CorporateID, temp);
                            break;
                        case ReservationPagingUtility.CorporateNameField:
                            tempStr = string.Format(DBOrderByPattern, CorporateName, temp);
                            break;

                        case ReservationPagingUtility.CreatorIDField:
                            tempStr = string.Format(DBOrderByPattern, Creator, temp);
                            break;
                        case ReservationPagingUtility.CreatorFirstNameField:
                            tempStr = string.Format(DBOrderByPattern, CreatorFirstName, temp);
                            break;
                        case ReservationPagingUtility.CreatorLastNameField:
                            tempStr = string.Format(DBOrderByPattern, CreatorLastName, temp);
                            break;

                        case ReservationPagingUtility.StartLocationIDField:
                            tempStr = string.Format(DBOrderByPattern, StartLocationID, temp);
                            break;
                        case ReservationPagingUtility.StartLocationNameField:
                            tempStr = string.Format(DBOrderByPattern, StartLocationName, temp);
                            break;

                        case ReservationPagingUtility.StateField:
                            tempStr = string.Format(DBOrderByPattern, State, temp);
                            break;
                        case ReservationPagingUtility.CreatedOnField:
                            tempStr = string.Format(DBOrderByPattern, CreatedOn, temp);
                            break;
                        case ReservationPagingUtility.CreatedByField:
                            tempStr = string.Format(DBOrderByPattern, CreatedBy, temp);
                            break;
                        case ReservationPagingUtility.ModifiedOnField:
                            tempStr = string.Format(DBOrderByPattern, ModifiedOn, temp);
                            break;
                        case ReservationPagingUtility.ModifiedByField:
                            tempStr = string.Format(DBOrderByPattern, ModifiedBy, temp);
                            break;
                        default:
                            break;
                    }

                    if (count == _orderByConditions.Keys.Count - 1)
                    {
                        sb.Append(tempStr);
                    }
                    else
                    {
                        sb.Append(tempStr + ",");
                    }
                    count++;
                }
                _orderBy = " ORDER BY " + sb.ToString();
            }

            public void Process()
            {

                CombineWhereConditionsToDBValues();

                CombineOrderByConditionsToDBValues();

                _pagingSampleSql = string.Format(_pagingSampleSqlPattern, _where, _orderBy);
            }
        }

        public interface IPaging
        {
            Dictionary<string, string> WhereCondition { get; }
            Dictionary<string, string> OrderByCondition { get; }
            string WhereClause { get; }
            string OrderByClause { get; }
            int ItemsPerPage { get; }
            int PageNumber { get; }

            void Process();
        }

        public class PagingFactory
        {

            public const int DefaultItemsPerPage = 10;
            public const int DefaultPageNumer = 1;

            public static IPaging CreatePaging(string[] where, string[] order, int ItemsPerPage, int PageNumber)
            {
                ReservationsWithPaging bookingPaging = null;

                try
                {
                    Dictionary<string, string> wherePairs = ReservationPagingUtility.ParseWhereConditions(where);

                    Dictionary<string, string> orderByPairs = ReservationPagingUtility.ParseOrderByConditions(order);

                    int itemsPerPage = ItemsPerPage <= 0 ? DefaultItemsPerPage : ItemsPerPage;
                    int pageNumber = PageNumber <= 0 ? DefaultPageNumer : PageNumber;

                    bookingPaging = new ReservationsWithPaging(wherePairs, orderByPairs, itemsPerPage, pageNumber);

                    bookingPaging.Process();

                }
                catch (Exception ex)
                {
                    if (ex is FaultException<ReturnResult>)
                    {
                        throw ex;
                    }
                    else
                    {
                        throw new VrentApplicationException(ReservationPagingUtility.UnknownPagingExceptionCode, ReservationPagingUtility.UnknownPagingExceptionMessage, ResultType.VRENT);
                    }
                }


                return bookingPaging;
            }

        }

        #endregion

        #region Bulk Sync
        public static ReturnResultBulkSink BulkSyncProxyReservations(ProxyReservation[] reservations)
        {
            List<SqlDataRecord> bookings = new List<SqlDataRecord>(CreateProxyReservationRecord(reservations));

            List<SqlParameter> parameters = new List<SqlParameter>();

            //input
            SqlParameter priceParam = new SqlParameter("@BulkSinkInput", bookings.Count > 0 ? bookings : null);
            priceParam.SqlDbType = SqlDbType.Structured;
            priceParam.TypeName = "dbo.Booking";
            parameters.Add(priceParam);

            SqlParameter insertedParam = new SqlParameter("@InsertCnt", -1);
            insertedParam.Direction = ParameterDirection.InputOutput;
            parameters.Add(insertedParam);

            SqlParameter updatedParam = new SqlParameter("@UpdatedCnt", -1);
            updatedParam.Direction = ParameterDirection.InputOutput;
            parameters.Add(updatedParam);

            ProxyReservation[] dbSync = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyReservation[]>(BulkSyncVrentBookings_, parameters.ToArray()
                , (sqldatareader) => ReadMultipleBookingFromDataReader(sqldatareader));

            parameters.Clear();

            ReturnResultBulkSink bulkSink = new ReturnResultBulkSink();
            bulkSink.SyncData = dbSync;
            bulkSink.InsertedCnt = Convert.ToInt32(insertedParam.Value);
            bulkSink.UpdatedCnt = Convert.ToInt32(updatedParam.Value);

            return bulkSink;
        }

        //parent records
        private static IEnumerable<SqlDataRecord> CreateProxyReservationRecord(ProxyReservation[] bookings)
        {
            SqlMetaData[] metaData = new SqlMetaData[22]
            {
                //[ID] [int] IDENTITY(1,1) NOT NULL,
                //[BookingType] [tinyint] NOT NULL,
                //[KemasBookingID] [uniqueidentifier] NOT NULL,
                //[KemasBookingNumber] [nvarchar](20) NOT NULL,
                //[DateBegin] [datetime] NOT NULL,

                new SqlMetaData("ID",SqlDbType.Int),
                new SqlMetaData("BookingType",SqlDbType.Int),
                new SqlMetaData("KemasBookingID",SqlDbType.UniqueIdentifier),
                new SqlMetaData("KemasBookingNumber",SqlDbType.NVarChar,20),
                new SqlMetaData("DateBegin",SqlDbType.DateTime),

                //[DateEnd] [datetime] NOT NULL,
                //[TotalAmount] [decimal](10, 3) NOT NULL,
                //[UserID] [uniqueidentifier] NOT NULL,
                //[UserFirstName] [nvarchar](50) NULL,
                //[UserLastName] [nvarchar](50) NULL,

                new SqlMetaData("DateEnd",SqlDbType.DateTime),
                new SqlMetaData("TotalAmount",SqlDbType.Decimal,10,3),
                new SqlMetaData("UserID",SqlDbType.UniqueIdentifier),
                new SqlMetaData("UserFirstName",SqlDbType.NVarChar,50),
                new SqlMetaData("UserLastName",SqlDbType.NVarChar,50),

                //[CorporateID] [nvarchar](50) NULL,
                //[CorporateName] [nvarchar](50) NULL,
                //[CreatorID] [uniqueidentifier] NULL,
                //[CreatorFirstName] [nvarchar](50) NULL,
                //[CreatorLastName] [nvarchar](50) NULL,
                new SqlMetaData("CorporateID",SqlDbType.NVarChar,50),
                new SqlMetaData("CorporateName",SqlDbType.NVarChar,50),
                new SqlMetaData("CreatorID",SqlDbType.UniqueIdentifier),
                new SqlMetaData("CreatorFirstName",SqlDbType.NVarChar,50),
                new SqlMetaData("CreatorLastName",SqlDbType.NVarChar,50),

                //[StartLocationID] [uniqueidentifier] NULL,
                //[StartLocationName] [nvarchar](50) NULL,
                new SqlMetaData("StartLocationID",SqlDbType.UniqueIdentifier),
                new SqlMetaData("StartLocationName",SqlDbType.NVarChar,50),

                //[State] [nvarchar](50) NOT NULL,
                //[CreatedOn] [datetime] NOT NULL,
                //[CreatedBy] [uniqueidentifier] NOT NULL,
                //[ModifiedOn] [datetime] NULL,
                //[ModifiedBy] [uniqueidentifier] NULL,

                new SqlMetaData("state",SqlDbType.NVarChar,50),
                new SqlMetaData("CreatedOn",SqlDbType.DateTime),
                new SqlMetaData("CreatedBy",SqlDbType.UniqueIdentifier),
                new SqlMetaData("ModifiedOn",SqlDbType.DateTime),
                new SqlMetaData("ModifiedBy",SqlDbType.UniqueIdentifier),
            };
            return bookings.Select(m => SetProxyReservationValues(metaData, m));
        }

        private static SqlDataRecord SetProxyReservationValues(SqlMetaData[] metaData, ProxyReservation booking)
        {
            SqlDataRecord reservation = new SqlDataRecord(metaData);

            //[ID] [int] IDENTITY(1,1) NOT NULL,
            //[BookingType] [tinyint] NOT NULL,
            //[KemasBookingID] [uniqueidentifier] NOT NULL,
            //[KemasBookingNumber] [nvarchar](20) NOT NULL,
            //[DateBegin] [datetime] NOT NULL,

            reservation.SetSqlInt32(0, booking.ProxyBookingID);
            reservation.SetSqlInt32(1, booking.BillingOption);
            reservation.SetGuid(2, booking.KemasBookingID);
            reservation.SetSqlString(3, booking.KemasBookingNumber);
            reservation.SetDateTime(4, booking.DateBegin.Value);

            //[DateEnd] [datetime] NOT NULL,
            //[TotalAmount] [decimal](10, 3) NOT NULL,
            //[UserID] [uniqueidentifier] NOT NULL,
            //[UserFirstName] [nvarchar](50) NULL,
            //[UserLastName] [nvarchar](50) NULL,

            reservation.SetDateTime(5, booking.DateEnd.Value);
            reservation.SetDecimal(6, booking.TotalAmount.Value);
            reservation.SetGuid(7, booking.UserID);
            reservation.SetSqlString(8, booking.UserFirstName);
            reservation.SetSqlString(9, booking.UserLastName);

            //[CorporateID] [nvarchar](50) NULL,
            //[CorporateName] [nvarchar](50) NULL,
            //[CreatorID] [uniqueidentifier] NULL,
            //[CreatorFirstName] [nvarchar](50) NULL,
            //[CreatorLastName] [nvarchar](50) NULL,

            reservation.SetSqlString(10, booking.CorporateID);
            reservation.SetSqlString(11, booking.CorporateName);
            reservation.SetGuid(12, booking.CreatorID.Value);
            reservation.SetSqlString(13, booking.CreatorFirstName);
            reservation.SetSqlString(14, booking.CreatorLastName);

            //[StartLocationID] [uniqueidentifier] NULL,
            //[StartLocationName] [nvarchar](50) NULL,
            reservation.SetGuid(15, booking.StartLocationID.Value);
            reservation.SetSqlString(16, booking.StartLocationName);

            //[State] [nvarchar](50) NOT NULL,
            //[CreatedOn] [datetime] NOT NULL,
            //[CreatedBy] [uniqueidentifier] NOT NULL,
            //[ModifiedOn] [datetime] NULL,
            //[ModifiedBy] [uniqueidentifier] NULL,

            reservation.SetSqlString(17, booking.State);

            if (booking.CreatedOn.HasValue)
            {
                reservation.SetDateTime(18, booking.CreatedOn.Value);
            }
            else
            {
                reservation.SetDBNull(18);
            }

            if (booking.CreatedBy.HasValue)
            {
                reservation.SetGuid(19, booking.CreatedBy.Value);
            }
            else
            {
                reservation.SetDBNull(19);
            }


            if (booking.ModifiedOn.HasValue)
            {
                reservation.SetDateTime(20, booking.ModifiedOn.Value);
            }
            else
            {
                reservation.SetDBNull(20);
            }

            if (booking.ModifiedBy.HasValue)
            {
                reservation.SetGuid(21, booking.ModifiedBy.Value);
            }
            else
            {
                reservation.SetDBNull(21);
            }

            return reservation;
        }

        #endregion

        #region UpdateUpPaymentFromCCBToDUB
        public const string WrongBookingTypeCode = "CVD000201";
        public const string WrongBookingTypeMessage = "Bad Booking Type:{0} or booking:{1} does not exist when associate the booking with paymentID:{2}, ";
        public static void UpdateUpPaymentForBooking(ProxyBookingPayment payment)
        {

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter bookingIDPara = new SqlParameter("@bookingID", payment.ProxyBookingID);
            parameters.Add(bookingIDPara);

            SqlParameter upIDPara = new SqlParameter("@upPaymentID", payment.UPPaymentID);
            parameters.Add(upIDPara);

            SqlParameter statePara = new SqlParameter("@state", payment.State);
            parameters.Add(statePara);

            SqlParameter createdOnPara = new SqlParameter("@createdOn", payment.CreatedOn);
            parameters.Add(createdOnPara);

            SqlParameter createdByPara = new SqlParameter("@createdBy", payment.CreatedBy);
            parameters.Add(createdByPara);

            SqlParameter retPara = new SqlParameter("@returnValue", -1);
            retPara.Direction = ParameterDirection.InputOutput;
            parameters.Add(retPara);

            int ret = -1;

            DataAccessProxyConstantRepo.ExecuteSPNonQuery(UpdateUpPaymentFromCCBToDUB, parameters.ToArray());
            parameters.Clear();
            ret = Convert.ToInt32(retPara.Value);

            if (ret == 1) //booking Type is 2
            {
                ReturnResult output = new ReturnResult();
                output.Code = WrongBookingTypeCode;
                output.Message = string.Format(WrongBookingTypeMessage, 2, payment.ProxyBookingID, payment.UPPaymentID);
                output.Success = ret;
                output.Type = ResultType.DATAACCESSProxy;

                throw new VrentApplicationException(output);
            }
        }
        #endregion
    }
}
