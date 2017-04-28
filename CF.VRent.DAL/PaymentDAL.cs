using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.UPSDK;
using CF.VRent.UPSDK.Entities;
using System.Data.SqlClient;
using CF.VRent.Log;
using CF.VRent.Common;
using System.Configuration;
using System.Data;
using CF.VRent.UPSDK.SDK;
using CF.VRent.DataAccessProxy.Entities;
using Microsoft.SqlServer.Server;

namespace CF.VRent.DAL
{
    public class PaymentDAL
    {
        /// <summary>
        /// Get user's credit card no
        /// </summary>
        /// <param name="uid">UID</param>
        /// <returns>Card set</returns>
        public static IEnumerable<PaymentCard> GetUserCreditCards(string uid)
        {
            List<PaymentCard> cards = new List<PaymentCard>();

            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@UserID", uid)               
                };

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetUserCreditCards", CommandType.StoredProcedure, paras);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    string orignCardNo = SecurityUtil.DecryptDataByPrivateKey(r["Encrpty_Card_No"].ToStr());
                    string maskString = "";
                    PaymentCard card = new PaymentCard()
                    {
                        CardNo = orignCardNo.Replace(orignCardNo.Substring(0, orignCardNo.Length - 4), maskString.PadLeft(orignCardNo.Length - 4, '*')),
                        Id = r["ID"].ToStr(),
                        Bank = r["Bank_Code"].ToStr(),
                        CardId = r["Card_ID"].ToStr(),
                        PhoneNo = SecurityUtil.DecryptDataByPrivateKey(r["Card_User_Tel"].ToStr())
                    };
                    cards.Add(card);
                }

                return cards;
            }

            return null;
        }

        /// <summary>
        /// Add one card info
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static int AddCreditCardInfo(UnionPayCustomInfo card, string token, string uid)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@CardNo", SecurityUtil.EncryptDataByPublicKey(card.CardNo)),                
                   new SqlParameter("@PhoneNo",SecurityUtil.EncryptDataByPublicKey(card.PhoneNo)),
                   new SqlParameter("@UserID", uid),
                   new SqlParameter("@Token", UnionPayUtils.TokenSerialize(token)),
                   new SqlParameter("@BindingTime", DateTime.Now),
                   new SqlParameter("@CreatedOn", DateTime.Now),
                   new SqlParameter("@CreatedBy", uid),
                   new SqlParameter("@ModifiedOn", DBNull.Value),
                   new SqlParameter("@ModifiedBy", DBNull.Value),
                   new SqlParameter("@BankCode", card.Bank.ToStr()),
                   new SqlParameter("@CardID", card.CardId)

                };

            //TODO:no sp name
            int res = SQLHelper.ExecuteNonQuery(null, "Sp_CreditCardInfo_Create", CommandType.StoredProcedure, paras);

            return res;
        }

        /// <summary>
        /// Add payment log
        /// </summary>
        /// <param name="up"></param>
        /// <returns></returns>
        public static int AddPaymentLog(UnionPay up, string uid, string logType)
        {
            string strOId = null;
            Guid oid;
            if (Guid.TryParse(up.OrderId, out oid))
            {
                strOId = oid.ToString();
            }

            UnionPayEnum upe = (UnionPayEnum)Enum.Parse(typeof(UnionPayEnum), logType);

            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@CreatedOn", DateTime.Now),                
                    new SqlParameter("@TxnType", up.TxnType.ToStr()),
                    new SqlParameter("@TxnSubType", up.TxnSubType.ToStr()),
                    new SqlParameter("@BizType", up.BizType.ToStr()),
                    new SqlParameter("@ChannelType", up.ChannelType.ToStr()),
                    new SqlParameter("@UserID", uid.ToStr()),
                    new SqlParameter("@Message",up.ToKeyValueString()),
                    new SqlParameter("@OrderId", strOId.ToStr()),
                    new SqlParameter("@TxnTime", up.TxnTime.ToStr()),
                    new SqlParameter("@CurrencyCode", up.CurrencyCode.ToStr()),
                    new SqlParameter("@OperationType", logType.ToStr()),
                    new SqlParameter("@QueryId", up.QueryId.ToStr()),
                    new SqlParameter("@TraceNo", up.TraceNo.ToStr()),
                    new SqlParameter("@TraceTime", up.TraceTime.ToStr()),
                    new SqlParameter("@UniqueID", up.UniqueID.ToStr()),
                    new SqlParameter("@RespCode", up.RespCode.ToStr()),
                    new SqlParameter("@RespMsg", up.RespMsg.ToStr()),
                    new SqlParameter("@CreatedBy", uid.ToStr())
                };

            //TODO:no sp name
            int res = SQLHelper.ExecuteNonQuery(null, "Sp_PaymentLog_Create", CommandType.StoredProcedure, paras);

            return res;
        }

        /// <summary>
        /// Delete user's credit card info
        /// </summary>
        /// <param name="uid"></param>
        public static int DeleteUserCreditCard(string id, string uid)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    //TODO: no parameter
                    new SqlParameter("@CardID", id),
                    new SqlParameter("@UserID", uid)
                };

            //TODO:no sp name
            int res = SQLHelper.ExecuteNonQuery(null, "Sp_CreditCardInfo_Delete", CommandType.StoredProcedure, paras);

            return res;
        }

        /// <summary>
        /// Get binding card token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static string GetUserCardToken(string id, string uid)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@CardID", id),
                    new SqlParameter("@UserID",uid)
                };

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetUserCreditCardToken", CommandType.StoredProcedure, paras);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Encrypt_Token"].ToString();
            }
            return null;
        }

        /// <summary>
        /// Add the exchange message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int AddPaymentExchangeMessage(PaymentExchangeMessage message, string userId)
        {
            var outPut = new SqlParameter("@NewID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@UniqueID", message.UniqueID),                
                   new SqlParameter("@CreatedOn",message.CreatedOn),
                   new SqlParameter("@Operation",message.Operation.ToStr()),
                   new SqlParameter("@UserID",userId),
                   new SqlParameter("@PreAuthID",SecurityUtil.EncryptDataByPublicKey(message.PreAuthID.ToStr())),
                   new SqlParameter("@PreAuthQueryID",SecurityUtil.EncryptDataByPublicKey(message.PreAuthQueryID.ToStr())),
                   new SqlParameter("@PreAuthDateTime",message.PreAuthDateTime.ToStr()),
                   new SqlParameter("@PreAuthPrice",UnionPayUtils.YuanToFen(message.PreAuthPrice.ToStr())),
                   new SqlParameter("@PreAuthTempOrderID",message.PreAuthTempOrderID.ToStr()),
                   new SqlParameter("@SmsCode",SecurityUtil.EncryptDataByPublicKey(message.SmsCode.ToStr())),
                   new SqlParameter("@State",message.State),
                   new SqlParameter("@CardID",message.CardID.ToStr()),
                   new SqlParameter("@LastPaymentID",message.LastPaymentID),
                   outPut,
                   new SqlParameter("@DeductionPrice",UnionPayUtils.YuanToFen(message.DeductionPrice.ToStr())),
                   new SqlParameter("@RealPreAuthPrice",UnionPayUtils.YuanToFen(message.RealPreAuthPrice.ToStr()))
                };

            //TODO:no sp name
            SQLHelper.ExecuteNonQuery(null, "Sp_PaymentMessageExchange_Create", CommandType.StoredProcedure, paras);

            if (outPut.Value.ToInt() > 0)
            {
                return outPut.Value.ToInt();
            }
            return -1;
        }

        /// <summary>
        /// Get the exchange message
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetPaymentExchangeState(string id, string userId)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@ID", id)
                };

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetPaymentExchangeState", CommandType.StoredProcedure, paras);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["State"].ToInt();
            }
            return -2;
        }

        public static PaymentExchangeMessage GetPaymentExchangeInfo(int Id, string userId)
        {
            var ret = new PaymentExchangeMessage();

            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@ID", Id)
                };

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetPaymentExchangeInfo", CommandType.StoredProcedure, paras);

            if (dt.Rows.Count > 0)
            {
                ret.PreAuthID = SecurityUtil.DecryptDataByPrivateKey(dt.Rows[0]["PreAuthID"].ToStr());
                ret.PreAuthQueryID = SecurityUtil.DecryptDataByPrivateKey(dt.Rows[0]["PreAuthQueryID"].ToStr());
                ret.PreAuthDateTime = dt.Rows[0]["PreAuthDateTime"].ToStr();
                ret.PreAuthPrice = UnionPayUtils.FenToYuan(dt.Rows[0]["PreAuthPrice"].ToStr());
                ret.PreAuthTempOrderID = dt.Rows[0]["PreAuthTempOrderID"].ToStr();
                ret.RetryCount = dt.Rows[0]["Retry_Count"].ToInt();
                ret.UniqueID = dt.Rows[0]["Unique_ID"].ToStr();
                ret.PaymentID = dt.Rows[0]["ID"].ToInt();
                ret.Message = dt.Rows[0]["Message"].ToString();
                ret.SmsCode = SecurityUtil.DecryptDataByPrivateKey(dt.Rows[0]["SmsCode"].ToStr());
                ret.CardID = dt.Rows[0]["Card_ID"].ToStr();
                ret.LastPaymentID = dt.Rows[0]["LastPaymentID"].ToInt();
                ret.DeductionPrice = UnionPayUtils.FenToYuan(dt.Rows[0]["DeductionPrice"].ToStr());
                ret.UniqueID = dt.Rows[0]["Unique_ID"].ToStr();
                ret.Operation = dt.Rows[0]["Operation"].ToStr();
                ret.State = dt.Rows[0]["State"].ToInt();
                ret.UserID = dt.Rows[0]["User_ID"].ToStr();
                ret.RealPreAuthPrice = UnionPayUtils.FenToYuan(dt.Rows[0]["RealPreAuthPrice"].ToStr());
                ret.Retry = dt.Rows[0]["Retry"].ToInt();
                ret.CreatedOn = dt.Rows[0]["CreatedOn"].ToDate();
            }

            return ret;
        }

        public static int UpdatePaymentExchangeMessage(PaymentExchangeMessage message, string userId)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@ID", message.PaymentID),
                    new SqlParameter("@State", message.State),
                    new SqlParameter("@RCount", message.RetryCount),
                    new SqlParameter("@PreAuthID", SecurityUtil.EncryptDataByPublicKey(message.PreAuthID.ToStr())),
                    new SqlParameter("@PreAuthQueryID", SecurityUtil.EncryptDataByPublicKey(message.PreAuthQueryID.ToStr())),
                    new SqlParameter("@PreAuthDateTime", message.PreAuthDateTime.ToStr()),
                    new SqlParameter("@PreAuthPrice",UnionPayUtils.YuanToFen(message.PreAuthPrice.ToStr())),
                    new SqlParameter("@PreAuthTempOrderID", message.PreAuthTempOrderID.ToStr()),
                    new SqlParameter("@Operation",message.Operation.ToStr()),
                    new SqlParameter("@UserID",userId),
                    new SqlParameter("@DeductionPrice",UnionPayUtils.YuanToFen(message.DeductionPrice.ToStr())),
                    new SqlParameter("@Message",message.Message.ToStr()),
                    new SqlParameter("@RealPreAuthPrice",UnionPayUtils.YuanToFen(message.RealPreAuthPrice.ToStr()))
                };

            int res = SQLHelper.ExecuteNonQuery(null, "Sp_PaymentMessageExchange_Update", CommandType.StoredProcedure, paras);

            return res;
        }

        /// <summary>
        /// Update the state
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int UpdatePaymentExchangeMessageState(PaymentExchangeMessage message)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@ID", message.PaymentID),
                    new SqlParameter("@State", message.State),
                    new SqlParameter("@Operation",message.Operation),
                    new SqlParameter("@UserID",message.UserID)
                };

            int res = SQLHelper.ExecuteNonQuery(null, "Sp_PaymentMessageExchangeState_Update", CommandType.StoredProcedure, paras);

            return res;
        }


        /// <summary>
        /// Update the retry flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int UpdatePaymentExchangeMessageRetry(PaymentExchangeMessage message)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@ID",message.PaymentID),
                    new SqlParameter("@Flag",message.Retry),
                    new SqlParameter("@UserID", message.UserID)
                };

            int res = SQLHelper.ExecuteNonQuery(null, "Sp_PaymentMessageExchangeRetry_Update", CommandType.StoredProcedure, paras);

            return res;
        }

        public static int UpdateCreditCardState(string cardId, int state, string userId)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@CardId", cardId),
                    new SqlParameter("@State",state),
                    new SqlParameter("@UserID",userId)
                };

            int res = SQLHelper.ExecuteNonQuery(null, "Sp_CreditCardInfoState_Update", CommandType.StoredProcedure, paras);

            return res;
        }

        public static int AddOrderAfterPayment(ProxyOrder order, string userId)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@ProxyBId", order.ProxyBookingID),
                    new SqlParameter("@BookingUserID",order.BookingUserID),
                    new SqlParameter("@State",order.State),
                    new SqlParameter("@CreatedBy",userId)
                };

            int res = SQLHelper.ExecuteNonQuery(null, "Sp_VrentOrders_CreateAndItems", CommandType.StoredProcedure, paras);


            //            int res = SQLHelper.ExecuteNonQuery(null, "Sp_VrentOrders_Create", CommandType.StoredProcedure, paras);

            return res;
        }

        public static int UpdateBookingStatusAfterPayment(string kmId, string state, string userId)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@KBID", kmId),
                    new SqlParameter("@State",state),
                    new SqlParameter("@UserId",userId)
                };

            int res = SQLHelper.ExecuteNonQuery(null, "Sp_VrentBookingsState_Update", CommandType.StoredProcedure, paras);

            return res;
        }

        public static IEnumerable<ProxyReservationPayment> GetWaitingPayDUBBookings()
        {
            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetUncompletedDUBBookings", CommandType.StoredProcedure, null);

            List<ProxyReservationPayment> bookings = new List<ProxyReservationPayment>();
            foreach (DataRow r in dt.Rows)
            {
                bookings.Add(new ProxyReservationPayment()
                {
                    KemasBookingID = r["KemasBookingID"].ToString(),
                    UserID = r["UserID"].ToString(),
                    ProxyBookingID = r["BookingID"].ToInt(),
                    KemasBookingNumber = r["KemasBookingNumber"].ToStr(),
                    CardID = r["Card_ID"].ToStr(),
                    PaymentID = r["ID"].ToInt(),
                    BookingType = r["BookingType"].ToInt()
                });
            }

            return bookings;
        }

       
        public static int UpdateBookingPaymentState(ProxyBookingPayment bookp, string uid)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@State", bookp.State),
                    new SqlParameter("@UserId", uid),
                    new SqlParameter("@BookingId", bookp.ProxyBookingID),
                    new SqlParameter("@PaymentId", bookp.UPPaymentID)
                };

            int res = SQLHelper.ExecuteNonQuery(null, "Sp_BookingPaymentState_Update", CommandType.StoredProcedure, paras);

            return res;
        }

        public static int AddBookingPayment(ProxyBookingPayment bookp, string uid)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@BookingId", bookp.ProxyBookingID),
                    new SqlParameter("@PaymentId", bookp.UPPaymentID),
                    new SqlParameter("@State", bookp.State),
                    new SqlParameter("@UserId", uid)
                };

            int res = SQLHelper.ExecuteNonQuery(null, "Sp_BookingPayment_Create", CommandType.StoredProcedure, paras);

            return res;
        }

        public static int GetPaymentStatusByBookingId(int bookingId)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@BookingId", bookingId)
                };

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetPaymentExchangeStateByBookingID", CommandType.StoredProcedure, paras);

            //one transaction contain one or more payments
            //if one of them is failed, then the transaction is failed
            var failedState = dt.AsEnumerable().Where(r => !r["State"].ToStr().ToEnum<PaymentStatusEnum>().IsSuccessStatus());

            if (failedState.Count() > 0)
            {
                return failedState.First()["State"].ToInt();
            }
            else
            {
                if (dt.Rows.Count > 0)
                {
                    //the first record is latest one returned by store procedure
                    return dt.Rows[0]["State"].ToInt();
                }
            }

            return -2;
        }

        public static IEnumerable<ProxyBookingPayment> GetBookingPaymentByPaymentId(int paymentId)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@paymentId", paymentId)
                };

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetBookingPaymentByPaymentID", CommandType.StoredProcedure, paras);

            List<ProxyBookingPayment> list = new List<ProxyBookingPayment>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ProxyBookingPayment()
                {
                    ProxyBookingID = row["BookingID"].ToInt(),
                    State = row["state"].ToInt(),
                    CreatedOn = row["CreatedOn"].ToDateNull()
                });
            }
            return list;
        }

        public static IEnumerable<ProxyBookingPayment> GetBookingPaymentByBookingId(int bookingId)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@bookingId", bookingId)
                };

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetBookingPaymentByBookingID", CommandType.StoredProcedure, paras);

            List<ProxyBookingPayment> list = new List<ProxyBookingPayment>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ProxyBookingPayment()
                {
                    UPPaymentID = row["UPPaymentID"].ToInt(),
                    State = row["state"].ToInt(),
                    CreatedOn = row["CreatedOn"].ToDateNull()
                });
            }
            return list;
        }

        #region Indirect Fee
        /// <summary>
        /// Add booking indirect fee payment records
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static bool AddBookingIndirectFeePayment(IEnumerable<BookingIndirectFeePayment> records)
        {
            return SQLHelper.InsertNonQueryBatch<BookingIndirectFeePayment>(records);
        }

        /// <summary>
        /// Update booking indirect fee payment records state
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static int UpdateBookingIndirectFeePayment(IEnumerable<BookingIndirectFeePayment> records)
        {
            var table = records.ConvertToDataTable();
            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@BIFPObject", table){
                    SqlDbType = SqlDbType.Structured
                }
            };
            int res = SQLHelper.ExecuteNonQuery(null, "Sp_BookingIndirectFeePaymentState_Update", CommandType.StoredProcedure, paras);

            return res;
        }

        /// <summary>
        /// Get booking indirect fee payment records by bookingID
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        public static IEnumerable<BookingIndirectFeePayment> GetBookingIndirectFeePaymentByBookingID(int bookingId)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@BookingID", bookingId)
                };

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetBookingIndirectFeePaymentByBookingID", CommandType.StoredProcedure, paras);

            List<BookingIndirectFeePayment> list = new List<BookingIndirectFeePayment>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new BookingIndirectFeePayment()
                {
                    ID = row["ID"].ToInt(),
                    State = row["State"].ToInt(),
                    BookingID = bookingId,
                    UPPaymentID = row["UPPaymentID"].ToInt(),
                    OrderItemID = row["OrderItemID"].ToInt(),
                    CreatedBy = row["CreatedBy"].ToGuidNull(),
                    CreateOn = row["CreateOn"].ToDateNull(),
                    ModifiedBy = row["ModifiedBy"].ToGuidNull(),
                    ModifiedOn = row["ModifiedOn"].ToDateNull()
                });
            }
            return list;
        }

        public static IEnumerable<BookingIndirectFee> GetTotalIndirectFeeForAll()
        {
            List<BookingIndirectFee> ret = new List<BookingIndirectFee>();

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_GetTotalIndirectFeeForAll", CommandType.StoredProcedure, null);

            foreach (DataRow r in dt.Rows)
            {
                var ids = r["OrderItemIDs"].ToStr().Split(',');
                int[] intIds = new int[ids.Length];
                for (int i = 0; i < ids.Length; i++)
                {
                    intIds[i] = ids[i].ToInt();
                }

                ret.Add(new BookingIndirectFee()
                {
                    CardID = r["CardID"].ToStr(),
                    OrderID = r["OrderID"].ToInt(),
                    BookingID = r["BookingID"].ToInt(),
                    Fee = r["Fee"].ToDecimal(),
                    UserID = r["UserID"].ToStr(),
                    OrderItemIDs = intIds
                });
            }

            return ret;

        }
        #endregion

        /// <summary>
        /// Get all bookings have a retry flag
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RetryBooking> GetRetryBookings()
        {
            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_RetryBookings_Get", CommandType.StoredProcedure, null);

            List<RetryBooking> bookings = new List<RetryBooking>();

            foreach (DataRow row in dt.Rows)
            {
                var retryBooking = new RetryBooking() {
                    BookingId = row["BookingId"].ToInt(),
                    OrderItemId = row["OrderItemId"].ToStr(),
                    PaymentId = row["PaymentId"].ToInt(),
                    OldCard = row["OldCard"].ToGuidNull(),
                    PreAuthPrice = UnionPayUtils.FenToYuan(row["PreAuthPrice"].ToStr()),
                    RealPreAuthPrice = UnionPayUtils.FenToYuan(row["RealPreAuthPrice"].ToStr()),
                    DeductionPrice = UnionPayUtils.FenToYuan(row["DeductionPrice"].ToStr()),
                    NewCard = row["NewCard"].ToGuidNull(),
                    Operation = row["Operation"].ToStr().ToEnum<PayOperationEnum>(),
                    PreAuthQueryID = SecurityUtil.DecryptDataByPrivateKey(row["PreAuthQueryID"].ToStr()),
                    State = row["State"].ToInt(),
                    UserID = row["UserID"].ToGuid()
                };

                bookings.Add(retryBooking);
            }

            return bookings.AsEnumerable();
        }

        /// <summary>
        /// Get all failed transactions by booking
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        public static IEnumerable<PaymentExchangeMessage> GetFailedTransactionByBooking(int bookingId)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_BookingFaliedTransaction_Get", CommandType.StoredProcedure,
                new SqlParameter("BookingId", bookingId));

            List<PaymentExchangeMessage> payment = new List<PaymentExchangeMessage>();

            foreach (DataRow row in dt.Rows)
            {
                if (!row["State"].ToStr().ToEnum<PaymentStatusEnum>().IsSuccessStatus())
                {
                    var retryPayment = new PaymentExchangeMessage()
                    {
                        PaymentID = row["PaymentID"].ToInt(),
                        Retry = row["Retry"].ToInt()
                    };

                    payment.Add(retryPayment);
                }
            }

            return payment.AsEnumerable();
        }


        /// <summary>
        /// Add the exchange message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static void AddPaymentExchangeHistory(PaymentExchangeMessage message)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@UniqueID", message.UniqueID),                
                   new SqlParameter("@CreatedOn",message.CreatedOn),
                   new SqlParameter("@Operation",message.Operation.ToStr()),
                   new SqlParameter("@UserID",message.UserID),
                   new SqlParameter("@PreAuthID",SecurityUtil.EncryptDataByPublicKey(message.PreAuthID.ToStr())),
                   new SqlParameter("@PreAuthQueryID",SecurityUtil.EncryptDataByPublicKey(message.PreAuthQueryID.ToStr())),
                   new SqlParameter("@PreAuthDateTime",message.PreAuthDateTime.ToStr()),
                   new SqlParameter("@PreAuthPrice",UnionPayUtils.YuanToFen(message.PreAuthPrice.ToStr())),
                   new SqlParameter("@PreAuthTempOrderID",message.PreAuthTempOrderID.ToStr()),
                   new SqlParameter("@SmsCode",SecurityUtil.EncryptDataByPublicKey(message.SmsCode.ToStr())),
                   new SqlParameter("@State",message.State),
                   new SqlParameter("@CardID",message.CardID.ToStr()),
                   new SqlParameter("@LastPaymentID",message.LastPaymentID),
                   new SqlParameter("@DeductionPrice",UnionPayUtils.YuanToFen(message.DeductionPrice.ToStr())),
                   new SqlParameter("@RealPreAuthPrice",UnionPayUtils.YuanToFen(message.RealPreAuthPrice.ToStr())),
                   new SqlParameter("@PaymentID", message.PaymentID)
                };

            //TODO:no sp name
            SQLHelper.ExecuteNonQuery(null, "Sp_PaymentMessageExchangeHistory_Create", CommandType.StoredProcedure, paras);
        }
    }
}
