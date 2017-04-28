using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CF.VRent.BLL
{
    public class AbstractBLL : IBLL
    {
        private ProxyUserSetting _userInfo = null;
        public ProxyUserSetting UserInfo
        {
            get
            {
                return _userInfo;
            }
            set
            {
                _userInfo = value;
            }
        }

        public AbstractBLL(ProxyUserSetting userInfo)
        {
            _userInfo = userInfo;
        }
    }

    public class ErrorConstants 
    {
        public static bool IsNumber(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }
            else
            {
                return s.ToCharArray().Where(m => !Char.IsDigit(m)).Count() == 0 ? true : false;
            }
        }

        public static Guid SystemID = Guid.Parse("99999999-9999-9999-9999-999999999999");

        public const string ThrowPricingExceptionConfig = "ThrowPricingException";
        public const string KemasNoError = "E0000";


        public const string UpdateProxyBookingFailedCode = "CVB000101";
        public const string UpdateProxyBookingFailedCodeMsg = "update booking failed at Vrent";

        public const string ProxyBookingFailedCode = "CVB000102";
        public const string ProxyBookingFailedCodeMsg = "booking a car failed at Vrent";

        public const string KemasBookingFailedCode = "CVB000103";
        public const string KemasBookingFailedCodeMsg = "booking a car via Kemas failed";

        public const string BookingNodeExistCode = "CVB000104";
        public const string BookingNodeExistMessage = "The booking does not exist";

        public const string ForbidenCCBBookingFapiaoCode = "CVB000105";
        public const string ForbidenCCBBookingFapiaoMessage = "The Booking:{0}-{1} is not eligible for requesting a fapiao by User:{2}.";

        public const string BadInputFromFECode = "CVB000106";
        public const string BadInputFromFEMessage = "Input parameter is not correct";

        public const string BookingOutOfSyncCode = "CVB000107";
        public const string BookingOutOfSyncMessage = "Critical properties are out of sync between Kemas booking and Vrent with ID:{0}";

        public const string CancelReservationFailCode = "CVB000108";
        public const string CancelReservationFailMessage = "Cancellation Failed";

        //public const string InvalidFEBookingStateCode = "CVB000109";
        //public const string InvalidFEBookingStateMessage = "there is/are invalid booking state/states from FE";

        public const string FEMissingKeyFieldsCode = "CVB000110";
        public const string FEMissingKeyFieldsMessage = "Required fields are missing";

        public const string OrderIDNotConsistentErrorCode = "CVB000111";
        public const string OrderIDNotConsistentErrorMessage = "OrderID is not consistent from FE";
        
        public const string NotOrderItemsCode = "CVB000112";
        public const string NotOrderItemsMessage = "No order items to append";

        public const string BadPricingfieldsCode = "CVB000113";
        public const string BadPricingfieldsMessage = "KemasBookingID:{0},State:{1}, Price:{2}, Price Detail:{3}";


        public const string BadBookingStateCode = "CVB000201";
        public const string BadBookingStateMessage = "State:{0}";

        public const string BadFapiaoSourceCode = "CVB000203";
        public const string BadFapiaoSourceMessage = "Fapiao Source {0} is not valid.";

        public const string InvalidBillingOptionCode = "CVB000204";
        public const string InvalidBillingOptionMessage = "Billing Option {0} is not valid at this moment for user {1},please contact service center";

        public const string UpdateProfileFirstCode = "CVB000205";
        public const string UpdateProfileFirstMessage = "please fill in your profile first {0}";

        public const string PendingRequestNotExistCode = "CVB000206";
        public const string PendingRequestNotExistMessage = "Pending user transfer request for user {0} no longer exists.";


        public const string BadRegistrationDataCode = "CVB000300";
        public const string BadRegistrationDataMessage = "Bad Registration Data. PWD:{0}, LoginName:{1}, ID:{2},language:{3}";

        public const string AdminAccountWrongCode = "CVB000301";
        public const string AdminAccountWrongMessage = "Admin session is not available. ID :{0}";

        public const string BadProfileDataCode = "CVB000302";
        public const string BadProfileDataMessage = "Bad Profile Data. ID:{0}";

        public const string BadUserStatusCode = "CVB000303";
        public const string BadUserStatusMessage = "Bad User Status. ID:{0},Status:{1},language:{2}";

        public const string ChangePWDFirstCode = "CVB000304";
        public const string ChangePWDFirstMessage = "Please Change you password ID:{0}";

        public const string NonExistingUserCode = "CVB000305";
        public const string NonExistingUserMessage = "User {0} does not exist in language {1}. ";

        public const string UpdatePwdFailedCode = "CVB000306";
        public const string UpdatePwdFailedMessage = "User {0} update password failed.";

        public const string MandatoryFieldsMissingCode = "CVB000307";
        public const string MandatoryFieldsMissingMessage = "the following mandatory fields are missing {0}";

        public const string ChangePWDCode = "CVB000308";
        public const string ChangePWSMessage = "invalid password data {0}-{1}";

        public const string ChangePWDFailCode = "CVB000309";
        public const string ChangePWDFailMessage = "if user {0} has inputed a wrong current password {1}";

        public const string JoiningCompanyCode = "CVB000310";
        public const string JoiningCompanyMessage = "Joining company {0} does not exists for user {1}";
        public const string AdminRetrieveClientsFailCode = "CVB000311";
        public const string AdminRetrieveClientsFailMessage = "Fail to retrieve available clients by admin {0}";

        public const string BadBillingOptionCode = "CVB000312";
        public const string BadBillingOptionMessage = "Bad BillingOption {0} for user {1}";

        public const string InvalidSesssionCode = "CVB000313";
        public const string InvalidSesssionMessage = "Please Login first.";

        public const string NotOwnerCode = "CVB000314";
        public const string NotOwnerCodeMessage = "Permission denied! User {0} Can't do this operation on other users {1}.";

        public const string NoPrivilegeCode = "CVB000315";
        public const string NoPrivilegeMessage = "Permission denied! User {0} does not allow to do this operation.";

        public const string NoCompanyPermissionCode = "CVB000316";
        public const string NoCompanyPermissionMessage = "User {0} does not have permission to view client {1}";

        public const string WrongCompanyStateCode = "CVB000317";
        public const string WrongCompanyStateMessage = "Company {0} have already been {1}";

        public const string RoleNotExistOrNoPrivilegeCode = "CVB000318";
        public const string RoleNotExistOrNoPrivilegeMessage = "Role {0}-{1} does not exist or user {2}-{3} does not have privileges.";

        public const string BadLoginDataCode = "CVB000319";
        public const string BadLoginDataMessage = "Bad Login Data {0},{1}";

        public const string LoginFailedCode = "CVB000320";
        public const string LoginFailedMessage = "Login failed. {0},{1}";

        public const string AppendNewClientToRoleTitle = "AppendNewClientToRole";
        public const string AppendNewClientToRoleCode = "CVB000321";
        public const string AppendNewClientToRoleCodeMessage = "Append new Client({0}-{1}) to User({2}-{3},Role({4}-{5}) Failed by User({6}-{7}). Exception Info: {8}";

        public const string RetrieveRoleIDFailureCode = "CVB000322";
        public const string RetrieveRoleIDFailureMessage = "Append new Client {0}-{1} to Role {2}, Fail to retrieve kemas role ID {3} by User {4}-{5}.";

        public const string RetrieveUsersWithRoleFailureCode = "CVB000323";
        public const string RetrieveUsersWithRoleFailureMessage = "Retrieve users with Role({0}-{1}) by User {2}-{3}. Exception Info {4}";

        public const string NoDebitNoteDataCode = "CVB000401";
        public const string NoDebitNoteDataMessage = "There is no debit-note data in DB.";

        public const string BadDebitNoteDataCode = "CVB000402";
        public const string BadDebitNoteDataMessage = "Bad Data from FE debit-note ID:{0}, debit-noteID:{1}";

        public const string DebitNoteNotExistCode = "CVB000403";
        public const string DebitNoteNotExistMessage = "Debit-note {0} does not exist in DB.";

    }

    public class ServiceUtility 
    {
        public const string UserSettings = "UserSetting";

        public static ProxyUserSetting RetrieveUserInfoFromSession()
        {
            ProxyUserSetting userInfo = HttpContext.Current.Session[UserSettings] as ProxyUserSetting;
            if (userInfo == null)
            {
                throw new VrentApplicationException(ErrorConstants.InvalidSesssionCode, ErrorConstants.InvalidSesssionMessage, ResultType.VRENT);
            }
            return userInfo;
        }

        public static ProxyUserSetting RetrieveUserInfoFromSession(string urlID)
        {
            ProxyUserSetting userInfo = HttpContext.Current.Session[UserSettings] as ProxyUserSetting;
            if (userInfo == null)
            {
                throw new VrentApplicationException(ErrorConstants.InvalidSesssionCode, ErrorConstants.InvalidSesssionMessage, ResultType.VRENT);
            }
            else
            {
                if (userInfo.ID != urlID)
                {
                    var webEx = new VrentApplicationException(
                        ErrorConstants.NotOwnerCode,
                        string.Format(ErrorConstants.NotOwnerCodeMessage, urlID, userInfo.ID),
                        ResultType.VRENTFE);
                    throw webEx;
                } 
            }

            return userInfo;
        }


        public static ProxyUserSetting ConvertFromUserExtention(UserExtension userInfo)
        {
            ProxyUserSetting pus = new ProxyUserSetting();
            //pus.UName = userInfo.Name; //not sure
            pus.ID = userInfo.ID;
            pus.Name = userInfo.Name;
            pus.VName = userInfo.VName;
            pus.Enabled = userInfo.Enabled;
            pus.Blocked = userInfo.Blocked;
            //pus.ChangePwd = userInfo.ch
            pus.AllowChangePwd = userInfo.AllowChangePwd;
            pus.Pwd = userInfo.Password;
            pus.SessionID = userInfo.SessionID;
            pus.Status = userInfo.Status;
            pus.Mail = userInfo.Mail;
            pus.SessionID = userInfo.SessionID;
            pus.ClientID = userInfo.ClientID;
            pus.Company = userInfo.Company;
            pus.IsPrivateUser = userInfo.IsPrivateUser;

            if (userInfo.RoleEntities != null)
            {
                pus.VrentRoles = userInfo.RoleEntities.Select(m => new ProxyRole() { RoleMember = m.Key }).ToList(); 
            }

            //pus.AllRights = userInfo.Rights;

            return pus;
        }

    }
}
