using CF.VRent.Common.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities
{
    public enum MessageCode
    {
        /// <summary>
        /// Unknow Exception
        /// </summary>
        [MessageCode(Description = "Unknow Exception", Type = ResultType.OTHER)]
        CVCE000000,
        #region Common
        /// <summary>
        /// Common Exception - Email recipient address is null
        /// </summary>
        [MessageCode(Description = "Email recipient address is null.", Type = ResultType.VRENT)]
        CVCE000001,
        /// <summary>
        /// Common Exception - Access Denied, Please login first.
        /// </summary>
        [MessageCode(Description = "Access Denied, Please login first.", Type = ResultType.VRENT)]
        CVCE000002,
        /// <summary>
        /// Common Exception - Email address exist.
        /// </summary>
        [MessageCode(Description = "Email address exist.", Type = ResultType.VRENT)]
        CVCE000003,
        /// <summary>
        /// Common Exception - Email address does not exists.
        /// </summary>
        [MessageCode(Description = "Email address does not exists.", Type = ResultType.VRENT)]
        CVCE000004,
        /// <summary>
        /// Common Exception - Change Password Failed.
        /// </summary>
        [MessageCode(Description = "Change Password Failed.", Type = ResultType.VRENT)]
        CVCE000005,
        /// <summary>
        /// Common Exception - Login Failed.
        /// </summary>
        [MessageCode(Description = "Login Failed.", Type = ResultType.VRENT)]
        CVCE000006,
        /// <summary>
        /// Dont't have the permission of Service Center or VRent Manager
        /// </summary>
        [MessageCode(Description = "Dont't have permission (Username:{0},UserId:{1})", Type = ResultType.VRENT)]
        CVCE000007,
        /// <summary>
        /// Email address is invalid.
        /// </summary>
        [MessageCode(Description = "Email address is invalid.(Email:{0})", Type = ResultType.VRENT)]
        CVCE000008,
        /// <summary>
        /// File doesn't exist
        /// </summary>
        [MessageCode(Description = "File doesn't exist. (File:{0})", Type = ResultType.VRENT)]
        CVCE000009,
        /// <summary>
        /// Email template doesn't exist.
        /// </summary>
        [MessageCode(Description = "Email template doesn't exist. (File:{0})", Type = ResultType.VRENT)]
        CVCE000010,
        /// <summary>
        /// Email sender event doesn't be defined
        /// </summary>
        [MessageCode(Description = "No email sender event is defined (onSendEvent is null) at {0}", Type = ResultType.VRENT)]
        CVCE000011,
        #endregion

        #region LoginService 1-9
        /// <summary>
        /// VRent Exception - Cannot reset contact cac.
        /// </summary>
        [MessageCode(Description = "Cannot reset contact cac.", Type = ResultType.VRENT)]
        CVB000001,
        /// <summary>
        /// VRent Exception - Lauguage code not identified.
        /// </summary>
        [MessageCode(Description = "Lauguage code not identified.", Type = ResultType.VRENT)]
        CVB000002,
        /// <summary>
        /// VRent Exception - User is blocked
        /// </summary>
        [MessageCode(Description = "User is blocked", Type = ResultType.VRENT)]
        CVB000003,
        #endregion

        #region UnionPayService 10-29
        /// <summary>
        /// Pre-authorizaion failed.
        /// </summary>
        [MessageCode(Description = "Pre-authorizaion failed.", Type = ResultType.VRENT)]
        CVB000010,
        /// <summary>
        /// Unionpay Exception - Sms code getting failed.
        /// </summary>
        [MessageCode(Description = "Sms code getting failed.", Type = ResultType.VRENT)]
        CVB000011,
        /// <summary>
        /// Unionpay Exception - Pre-authorizaion completion failed.
        /// </summary>
        [MessageCode(Description = "Pre-authorizaion completion failed.", Type = ResultType.VRENT)]
        CVB000012,
        /// <summary>
        /// Unionpay Exception - Deduction failed.
        /// </summary>
        [MessageCode(Description = "Deduction failed.", Type = ResultType.VRENT)]
        CVB000013,
        /// <summary>
        /// Unionpay Exception - Phone number isn't correct( sms code )
        /// </summary>
        [MessageCode(Description = "Phone number isn't correct", Type = ResultType.VRENT)]
        CVB000014,
        /// <summary>
        /// VRent Exception - Failed to bind card
        /// </summary>
        [MessageCode(Description = "Failed to bind card", Type = ResultType.VRENT)]
        CVB000015,
        /// <summary>
        /// VRent Exception - No avaliable credit card
        /// </summary>
        [MessageCode(Description = "No avaliable credit card", Type = ResultType.VRENT)]
        CVB000016,
        /// <summary>
        /// VRent Exception - Didn't pay the reservation
        /// </summary>
        [MessageCode(Description = "Didn't pay the reservation (Booking:{0} - Transaction:{1})", Type = ResultType.VRENT)]
        CVB000017,
        /// <summary>
        /// Preauth price must be greater than 0
        /// </summary>
        [MessageCode(Description = "Preauth price must be greater than 0 (CurrentPrice:{0})", Type = ResultType.VRENT)]
        CVB000018,
        /// <summary>
        /// Can't find the associated transaction
        /// </summary>
        [MessageCode(Description = "Can't find the associated transaction (Booking:{0} - Transaction:{1})", Type = ResultType.VRENT)]
        CVB000019,
        /// <summary>
        /// Failed to deactive user for DUB permission
        /// </summary>
        [MessageCode(Description = "Failed to deactive user for DUB permission (User:{0} - Status{1})", Type = ResultType.VRENT)]
        CVB000020,
        /// <summary>
        /// PaymentMessageStreamSerializer Error
        /// </summary>
        [MessageCode(Description = "PaymentMessageStreamSerializer Error", Type = ResultType.VRENT)]
        CVB000021,
        /// <summary>
        /// Preauth cancellation failed
        /// </summary>
        [MessageCode(Description = "Preauth cancellation failed", Type = ResultType.VRENT)]
        CVB000022,
        /// <summary>
        /// Failed to send request to Union pay
        /// </summary>
        [MessageCode(Description = "Failed to send request to Union pay", Type = ResultType.VRENT)]
        CVB000023,
        /// <summary>
        /// Didn't find the message in transction
        /// </summary>
        [MessageCode(Description = "Didn't find the message in transction {0}", Type = ResultType.VRENT)]
        CVB000024,
        /// <summary>
        /// Union Pay SDK Error
        /// </summary>
        [MessageCode(Description = "Union Pay SDK Error", Type = ResultType.VRENT)]
        CVB000025,
        #endregion

        #region User Mgmt 30-49
        /// <summary>
        /// This api is only for corporate user creation
        /// </summary>
        [MessageCode(Description = "This api is only for corporate user creation", Type = ResultType.VRENT)]
        CVB000030,
        /// <summary>
        /// ClientID must be corporate user company id
        /// </summary>
        [MessageCode(Description = "ClientID must be corporate user company id", Type = ResultType.VRENT)]
        CVB000031,
        /// <summary>
        /// Failed to send email
        /// </summary>
        [MessageCode(Description = "Failed to send email", Type = ResultType.VRENT)]
        CVB000032,
        /// <summary>
        /// Failed to deactive user after client has been deactived
        /// </summary>
        [MessageCode(Description = "Error occured during user deactiving after client has been deactived", Type = ResultType.VRENT)]
        CVB000033,
        #endregion

        #region Schedule job 50-59
        /// <summary>
        /// Booking Sync Failed
        /// </summary>
        [MessageCode(Description = "Booking Sync Failed", Type = ResultType.VRENT)]
        CVB000051,
        /// <summary>
        /// Error while getting booking price
        /// </summary>
        [MessageCode(Description = "Error while getting booking price", Type = ResultType.VRENT)]
        CVB000052,
        /// <summary>
        /// Add CCB booking price failed
        /// </summary>
        [MessageCode(Description = "Add CCB booking price failed", Type = ResultType.VRENT)]
        CVB000053,
        /// <summary>
        /// Error during fee deduction
        /// </summary>
        [MessageCode(Description = "Error during fee deduction", Type = ResultType.VRENT)]
        CVB000054,
        /// <summary>
        /// Error during indirect deduction
        /// </summary>
        [MessageCode(Description = "Error during indirect deduction", Type = ResultType.VRENT)]
        CVB000055,
        #endregion

        #region Transaction 60-69
        /// <summary>
        /// Can't modify the transaction status.
        /// </summary>
        [MessageCode(Description = "Can't modify the transaction status. (PaymentId:{0},CurrentStatus:{1})", Type = ResultType.VRENT)]
        CVB000060,
        /// <summary>
        /// Can't retry the transaction.
        /// </summary>
        [MessageCode(Description = "Can't retry the transaction. (PaymentId:{0},CurrentStatus:{1})", Type = ResultType.VRENT)]
        CVB000061,
        /// <summary>
        /// Can't find user's binding card. (UserId:{0},PaymentId:{1})
        /// </summary>
        [MessageCode(Description = "Can't find user's binding card. (UserId:{0},PaymentId:{1})", Type = ResultType.VRENT)]
        CVB000062,
        
        #endregion

        #region General Ledger
        /// <summary>
        /// Prefix: General Ledger Of DUB - 
        /// </summary>
        [MessageCode(Description = "General Ledger Of DUB - ", Type = ResultType.VRENT)]
        CVB000070,
        /// <summary>
        /// Prefix: General Ledger Of CCB - 
        /// </summary>
        [MessageCode(Description = "General Ledger Of CCB - ", Type = ResultType.VRENT)]
        CVB000071 
        #endregion
    }

    public class MessageCodeAttribute : Attribute, IEnumDescription
    {
        public string Description { get; set; }

        public ResultType Type { get; set; }

        public MessageCodeAttribute()
        {

        }
    }
}
