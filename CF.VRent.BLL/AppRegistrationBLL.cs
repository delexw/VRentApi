using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web;
using System.ServiceModel.Web;
using System.Net;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using System.Configuration;
using CF.VRent.Common.UserContracts;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Contract;
using CF.VRent.UserStatus;
using CF.VRent.UserCompany;
using CF.VRent.UserRole;

namespace CF.VRent.BLL
{
    public class KemasAdmin 
    {
        private const string AdminKey = "admin";
        private const string PwdKey = "password";

        public static WS_Auth_Response SignOn()
        {
            WS_Auth_Response adminSession = null;
            string userName = ConfigurationManager.AppSettings[AdminKey];
            string password = ConfigurationManager.AppSettings[PwdKey];
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                IKemasAuthencation kemas = new KemasAuthencationAPI();
                adminSession = kemas.authByLogin(userName,Encrypt.GetPasswordFormat(password));
            }
            return adminSession;
        }

        public static void AssignClients(string userID,string clientID, updateUserData kemasUserData) 
        {
            WS_Auth_Response adminSession = KemasAdmin.SignOn();
            if (adminSession != null && !string.IsNullOrEmpty(adminSession.ID) && !string.IsNullOrEmpty(adminSession.SessionID))
            {
                //must use admin session
                KemasConfigsAPI config = new KemasConfigsAPI();
                getClientsResponse configRes = config.getClients(adminSession.SessionID);

                if (configRes.Clients == null || configRes.Clients.Length == 0)
                {
                    throw new VrentApplicationException(
                        ErrorConstants.AdminRetrieveClientsFailCode,
                        string.Format(ErrorConstants.AdminRetrieveClientsFailMessage, adminSession.ID)
                        , ResultType.VRENT
                        );
                }
                else
                {
                    Dictionary<string, string> companyDic = configRes.Clients.ToDictionary(m => m.ID, m => m.Name);
                    if (companyDic.ContainsKey(clientID))
                    {
                        //kemasUserData.Clients = new string[] { clientID };
                        //kemasUserData.Company = companyDic[clientID];
                    }
                    else
                    {
                        throw new VrentApplicationException(
                            ErrorConstants.JoiningCompanyCode,
                            string.Format(ErrorConstants.JoiningCompanyMessage, clientID, userID), ResultType.VRENT
                        );
                    }
                }
            }
            else
            {
                ReturnResult ret = new ReturnResult();
                string msg = string.Format(ErrorConstants.AdminAccountWrongMessage, adminSession.ID);
                ret.Code = ErrorConstants.AdminAccountWrongCode;
                ret.Message = msg;
                ret.Type = ResultType.VRENTFE;

                throw new VrentApplicationException(ret);
            }
        }

        public static bool IsPrivateUser(UserData2 kemasBase) 
        {
            bool IsPUser = false;

            var admin = KemasAdmin.SignOn();
            var companyManager = UserCompanyContext.CreateCompanyManager();
            var defaultCompany = companyManager.Companies[UserCompanyConstants.EndUserCompanyKey].GetDefaultKemasCompany();

            var kemasExtApi = KemasAccessWrapper.CreateKemasExtensionAPIInstance();
            var defaultCompanyID = kemasExtApi.GetCompanyID(defaultCompany.Name, admin.SessionID);

            if (kemasBase.Clients[0].ID.Equals(defaultCompanyID))
            {
                IsPUser = true;
            }

            return IsPUser;
        }


        public static void AssignRoleByUserType(UserExtension vMUe,string userRoleKey,string sessionID)
        {
            string adminSession = null;
            if (string.IsNullOrEmpty(sessionID))
            {
                WS_Auth_Response auth = KemasAdmin.SignOn();
                adminSession = auth.SessionID;
            }
            else
            {
                adminSession = sessionID;
            }
            var kemasExtApi = KemasAccessWrapper.CreateKemasExtensionAPIInstance();
            var roleManager = UserRoleContext.CreateRoleManager();
            var defaultKemasRole = roleManager.Roles[userRoleKey].GetDefaultKemasRole();
            Role vmrole = new Role() { ID = kemasExtApi.GetRoleID(defaultKemasRole.Name, adminSession) };
            vMUe.Roles = new Role[] { vmrole };

            //WS_Auth_Response adminSession = KemasAdmin.SignOn();

            //if (adminSession != null && !string.IsNullOrEmpty(adminSession.ID) && !string.IsNullOrEmpty(adminSession.SessionID))
            //{
            //    //must use admin session to assign vm role
            //    KemasUserAPI roleClient = new KemasUserAPI();
            //    getRolesResponse rolesRes = roleClient.getRoles(adminSession.SessionID);

            //    if (rolesRes != null && rolesRes.Roles != null)
            //    {
            //        vMUe.Roles = rolesRes.Roles.Where(m => m.Name.ToUpper().Equals("VRENT MANAGER")).ToArray();
            //    }
            //    else
            //    {
            //        throw new VrentApplicationException(
            //            rolesRes.Error.ErrorCode,
            //            rolesRes.Error.ErrorMessage, ResultType.VRENT);
            //    }
            //}
            //else
            //{
            //    ReturnResult ret = new ReturnResult();
            //    string msg = string.Format(ErrorConstants.AdminAccountWrongMessage, adminSession.ID);
            //    ret.Code = ErrorConstants.AdminAccountWrongCode;
            //    ret.Message = msg;
            //    ret.Type = ResultType.VRENTFE;

            //    throw new VrentApplicationException(ret);
            //}
        }

        public static void SetVMStatus(updateUserData vmUe)
        {
            UserStatusManager usm = new UserStatusManager(null);

            //joining_company_approve
            usm.Status["7"].Value = 1;

            //usm.Status["2"].Value = 1;

            //since the password for vm is auto-generated, 
            //a vm need to change password first, and then update his/her own profile.
            usm.Status["1"].Value = 1;

            vmUe.Status = usm.Status.BinaryPattern;
        }

        public static void AssignPrivateUserClient(updateUserData toUpdateUser, WS_Auth_Response auth)
        {
            var companyManager = UserCompanyContext.CreateCompanyManager();
            var defaultCompany = companyManager.Companies[UserCompanyConstants.EndUserCompanyKey].GetDefaultKemasCompany();

            var kemasExtApi = KemasAccessWrapper.CreateKemasExtensionAPIInstance();

            var defaultCompanyID = kemasExtApi.GetCompanyID(defaultCompany.Name, auth.SessionID);
            toUpdateUser.Clients = new string[] { defaultCompanyID };
            toUpdateUser.Company = defaultCompany.Name;
        }

    }

    public class UserRegistrationConst 
    {


        public static updateUserData ConvertUserEntityToUpdateUserData(UserExtension root, params object[] otherValues)
        {
            updateUserData uud = new updateUserData();

            uud.ID = root.ID;
            uud.PNo = root.PNo;
            uud.Enabled = root.Enabled;
            uud.Blocked = root.Blocked;
            uud.Status = root.Status;
            uud.AllowChangePwd = root.AllowChangePwd;
            uud.Name = root.Name;
            uud.VName = root.VName;
            uud.Department = root.Department;
            uud.Phone = root.Phone;
            uud.Mail = root.Mail;
            uud.Company = root.Company;
            uud.PersonInCharge = root.PersonInCharge;
            uud.PrivateMobileNumber = root.PrivateMobileNumber;
            uud.PrivateBankAccount = root.PrivateBankAccount;
            uud.PrivateEmail = root.PrivateEmail;
            uud.PrivateAddress = root.PrivateAddress;
            uud.BusinessAddress  =root.BusinessAddress;
            uud.Valid_to  = root.Valid_to;

            uud.Clients = new string[1] { root.ClientID };
            uud.License = root.License;


            if (root.Roles != null)
            {
                uud.Roles = root.Roles.Select(m => m.ID).ToArray();
            }


            return uud;
        }

        public static bool ContainsChangePWDFromFE(UserExtension root)
        {
            bool contains = false;
            if (!string.IsNullOrEmpty(root.Password) 
                //&& !string.IsNullOrEmpty(root.CurrentPassword)
                && string.IsNullOrEmpty(root.ID) && !root.ID.Equals(Guid.Empty))
            {
                contains = true;
            }

            return contains;
        }

        public static bool IsInitialRegistration(UserExtension ueFE) 
        {
            bool IsInitReg = false;
            if (!string.IsNullOrEmpty(ueFE.Password) && !string.IsNullOrEmpty(ueFE.Mail) && string.IsNullOrEmpty(ueFE.ID) &&  Guid.Empty.ToString() != ueFE.ID)
            {
                IsInitReg = true;
            }
            return IsInitReg;
        }

        public static bool IsValidBookingUser(string userIDFromFE)
        {
            bool IsUpdate = false;
            if (!string.IsNullOrEmpty(userIDFromFE) && userIDFromFE != Guid.Empty.ToString())
            {
                IsUpdate = true;
            }
            return IsUpdate;
        }

        public static UserData2 UpdateKemasUser(updateUserData kemasUserData, string sessionID, string lang) 
        {
            UserData2 ud2 = null;

            KemasUserAPI kemasUser = new KemasUserAPI();
            updateUser2Request createUserRequest = new updateUser2Request();
            createUserRequest.SessionID = sessionID;
            createUserRequest.Language = lang;


            createUserRequest.UserData = kemasUserData;
            updateUser2Response createRet = kemasUser.updateUser2(createUserRequest);

            if (createRet.UserData != null)
            {
                ud2 = createRet.UserData;
            }
            else
            {
                throw new VrentApplicationException(createRet.Error.ErrorCode, createRet.Error.ErrorMessage, ResultType.KEMAS);
            }
            return ud2;
        }

        public static UserData2 RetrieveKemasUserByID(string kemasUserID, string sessionID)
        {
            UserData2 ud2 = null;
            KemasUserAPI kemasUser = new KemasUserAPI();
            findUser2Response kemasFind = kemasUser.findUser2(kemasUserID, sessionID);

            if (kemasFind.UserData != null)
            {
                ud2 = kemasFind.UserData;
            }
            else
            {
                throw new VrentApplicationException(kemasFind.Error.ErrorCode, kemasFind.Error.ErrorMessage, ResultType.KEMAS);
            }
            return ud2;
        }

        public static updateUserData ConvertUserData2ToUpdateUserData(UserData2 kemasOri) 
        {
            updateUserData uud = new updateUserData();

            uud.AllowChangePwd = kemasOri.AllowChangePwd;
            
            uud.BirthDay = kemasOri.BirthDay;
            uud.Blocked = kemasOri.Blocked;
            uud.BusinessAddress = string.IsNullOrEmpty(kemasOri.BusinessAddress) ? null : kemasOri.BusinessAddress;

            uud.Clients = kemasOri.Clients == null ? null : kemasOri.Clients.Select(m => m.ID).ToArray();
            uud.Company = (kemasOri.Clients != null && kemasOri.Clients.Length > 0) ? kemasOri.Clients[0].Name : null;
            uud.Costcenter = string.IsNullOrEmpty(kemasOri.Costcenter) ? null : kemasOri.Costcenter;

            uud.Department = string.IsNullOrEmpty(kemasOri.Department) ? null : kemasOri.Department;
            uud.Description = kemasOri.Description;

            uud.Enabled = kemasOri.Enabled;
            uud.Gender = kemasOri.Gender;//integer (0 = not specied, 1 = mal, 2 = female)
            uud.ID = kemasOri.ID;
            uud.License = kemasOri.License;
            uud.Mail = kemasOri.Mail;

            uud.Name = kemasOri.Name;
            uud.Nationality = kemasOri.Nationality;

            uud.PersonInCharge = string.IsNullOrEmpty(kemasOri.PersonInCharge) ? null : kemasOri.PersonInCharge;
            uud.Phone = string.IsNullOrEmpty(kemasOri.Phone) ? null : kemasOri.Phone;
            uud.PNo = string.IsNullOrEmpty(kemasOri.PNo) ? null : kemasOri.PNo;
            uud.PrivateAddress = kemasOri.PrivateAddress;
            uud.PrivateBankAccount = string.IsNullOrEmpty(kemasOri.PrivateBankAccount) ? null : kemasOri.PrivateBankAccount;
            uud.PrivateEmail = string.IsNullOrEmpty(kemasOri.PrivateEmail) ? null : kemasOri.PrivateEmail;
            uud.PrivateMobileNumber = string.IsNullOrEmpty(kemasOri.PrivateMobileNumber) ? null : kemasOri.PrivateMobileNumber;

            uud.Roles = kemasOri.Roles == null ? null : kemasOri.Roles.Select(m => m.ID).ToArray();
            uud.Status = kemasOri.Status;
            uud.TypeOfJourney = kemasOri.TypeOfJourney;
            uud.TypeOfJourneySpecified = true;

            uud.Valid_to = kemasOri.Valid_to;
            uud.VName = kemasOri.VName;
            return uud;
        }

        public static List<ProxyJourneyType> AllBillingOptions = new List<ProxyJourneyType>()
        {
            new ProxyJourneyType() { Key = "3", Value = "pay by myself" },
            new ProxyJourneyType() { Key = "2", Value = "pay by company" }
        };

        public static void EvaluateBillingOptions(UserData2 kemasUser,UserExtension ue) 
        {
            List<ProxyJourneyType> billingOptions = new List<ProxyJourneyType>();
            switch (kemasUser.TypeOfJourney)
            {
                case 0:
                    break;
                case 1:
                    billingOptions.Add(AllBillingOptions.FirstOrDefault(m=> m.Key.Equals("2")));
                    billingOptions.Add(AllBillingOptions.FirstOrDefault(m=> m.Key.Equals("3")));
                    break;
                case 2:
                    billingOptions.Add(AllBillingOptions.FirstOrDefault(m=> m.Key.Equals("2")));
                    break;
                case 3:
                    billingOptions.Add(AllBillingOptions.FirstOrDefault(m=> m.Key.Equals("3")));
                    break;

                default:
                    break;
            }

            ue.BillingOptions = billingOptions;

        }


        public static UserExtension AssembleUserExtention(UserData2 kemasUser, WS_Auth_Response auth)
        {
            UserExtension ue = null;
            UserFactory uf = new UserFactory();
            ue = uf.CreateEntity(kemasUser);

            if (string.IsNullOrEmpty(ue.Company) &&  kemasUser.Clients!= null && kemasUser.Clients.Length > 0)
            {
                ue.Company = kemasUser.Clients[0].Name;
            }

            ue.IsPrivateUser = KemasAdmin.IsPrivateUser(kemasUser);

            UserRegistrationConst.ExtractAddress(kemasUser, ue);

            UserSettingBLL.AppendUserIdentity(auth, ue);

            if (kemasUser.TypeOfJourney < 0 || kemasUser.TypeOfJourney > 3)
            {
                throw new VrentApplicationException(ErrorConstants.BadBillingOptionCode, string.Format(ErrorConstants.BadBillingOptionMessage, kemasUser.TypeOfJourney, kemasUser.ID), ResultType.VRENT);
            }
            else
            {
                EvaluateBillingOptions(kemasUser, ue); 
            }

            //remove it unnecessary stuff for APP
            ue.License = null;
            ue.ComanyEntites = null;
            ue.Clients = null;
            //ue.RoleEntities = null;
            ue.StatusExtensionEntities = null;
            //ue.Roles = null;
            ue.SessionID = auth.SessionID;
            return ue;

        }
        #region handling kemas Private Address
        //Postcode,Province,City,Street,Street2 in this order
        public static string privateAddressPattern = "{0},{1},{2},{3},{4}";

        public static void ExtractAddress(UserData2 kemasFind, UserExtension ue)
        {
            if (!string.IsNullOrEmpty(kemasFind.PrivateAddress))
            {
                string[] addressParts = kemasFind.PrivateAddress.Split(',');

                if (addressParts.Length == 5)
                {
                    ue.Postcode = addressParts[0];
                    ue.Province = addressParts[1];
                    ue.City = addressParts[2];
                    ue.Street = addressParts[3];
                    ue.Street2 = addressParts[4]; 
                }
            }
        }

        public static void PrepareAddress(updateUserData kemasUpdate, UserExtension ue,List<string> missingFields)
        {
            if (ue.Postcode == null || ue.Postcode.Trim().Equals(string.Empty))
            {
                missingFields.Add(PostcodeField);
            }

            if (ue.Province == null || ue.Province.Trim().Equals(string.Empty))
            {
                missingFields.Add(ProvinceField);
            }
            if (ue.City == null || ue.City.Trim().Equals(string.Empty))
            {
                missingFields.Add(CityField);
            }

            if (ue.Street == null || ue.Street.Trim().Equals(string.Empty))
            {
                missingFields.Add(StreetField);
            }

            kemasUpdate.PrivateAddress = string.Format(privateAddressPattern, ue.Postcode.Trim(), ue.Province.Trim(), ue.City.Trim(), ue.Street.Trim(), ue.Street2.Trim());
        }
        #endregion

        private const string UsernameWithMailFormatField = "UserName";
        private const string FirstnameField = "FirstName";
        private const string LastnameField = "LastName";
        private const string BirthdayField = "Birthday";
        private const string GenderField = "Gender";
        private const string PrivateMobileNumberField = "PrivateMobileNumber";
        private const string PhoneField = "Phone";
        private const string PostcodeField = "Postcode";
        private const string ProvinceField = "Province";
        private const string CityField = "City";
        private const string StreetField = "Street";

        private const string PinField = "Pin";
        private const string Pin2Field = "Pin2";
        private const string LicenseNumberField = "LicenseNumber";
        private const string DateOfIssueField = "DateOfIssue";
        private const string ExpireDateField = "ExpireDate";



        public static string[] SyncUpdateDataWithKemas(updateUserData kemasBase, UserExtension profileDataFromFE, bool firstTimeInput) 
        {
            List<string> missingFields = new List<string>();

            //should not update mail
            if (profileDataFromFE.Mail == null || profileDataFromFE.Mail.Trim().Equals(string.Empty))
            {
                missingFields.Add(UsernameWithMailFormatField);
            }
            else
            {
                kemasBase.Mail = profileDataFromFE.Mail.Trim();
            }

            //kemasBase.PNo = profileDataFromFE.PNo; //readonly

            if (profileDataFromFE.Name == null || profileDataFromFE.Name.Trim().Equals(string.Empty))
            {
                missingFields.Add(LastnameField);
            }
            else
            {
                kemasBase.Name = profileDataFromFE.Name.Trim();
            }

            if (profileDataFromFE.VName == null || profileDataFromFE.VName.Trim().Equals(string.Empty))
            {
                missingFields.Add(FirstnameField);
            }
            else
            {
                kemasBase.VName = profileDataFromFE.VName.Trim();
            }

            if (profileDataFromFE.BirthDay == null || profileDataFromFE.BirthDay.Trim().Equals(string.Empty))
            {
                missingFields.Add(BirthdayField);
            }
            else
            {
                kemasBase.BirthDay = profileDataFromFE.BirthDay.Trim();
            }

            if (profileDataFromFE.Gender == 0)
            {
                missingFields.Add(GenderField);
            }
            else
            {
                kemasBase.Gender = profileDataFromFE.Gender;//missing
                kemasBase.GenderSpecified = true;
            }

            kemasBase.Nationality = profileDataFromFE.Nationality.Trim();//optional,missing

            kemasBase.Costcenter = profileDataFromFE.Costcenter.Trim();

            if (profileDataFromFE.PrivateMobileNumber == null || profileDataFromFE.PrivateMobileNumber.Trim().Equals(string.Empty))
            {
                missingFields.Add(PrivateMobileNumberField);
            }
            else
            {
                kemasBase.PrivateMobileNumber = profileDataFromFE.PrivateMobileNumber.Trim();
            }

            kemasBase.Description = profileDataFromFE.Description.Trim();

            //
            PrepareAddress(kemasBase, profileDataFromFE, missingFields);

            //if (string.IsNullOrEmpty(profileDataFromFE.Phone))
            //{
            //    missingFields.Add(PhoneField);
            //}
            //else
            //{
            //    kemasBase.Phone = profileDataFromFE.Phone;
            //}

            //kemasBase.PersonInCharge = profileDataFromFE.PersonInCharge;
            //kemasBase.PrivateBankAccount = profileDataFromFE.PrivateBankAccount;

            //kemasBase.PrivateEmail = profileDataFromFE.PrivateEmail;
            //kemasBase.BusinessAddress = profileDataFromFE.BusinessAddress;//not changable in kemas
            //kemasBase.Valid_to = profileDataFromFE.Valid_to;

            //update license info
            if (firstTimeInput)
            {
                ExtractDrieverLicenseFromFE(kemasBase, profileDataFromFE,firstTimeInput,missingFields); 
            }

            return missingFields.ToArray();
        }

        public static void ConvertLicenseExtentionToKemasLicense(updateUserData kemasBase, UserLicenseExtension ule) 
        {
            License kemasLicense = null;
            if (ule != null)
            {
                kemasLicense = new License();
                kemasLicense.ChangePIN = ule.ChangePIN;
                kemasLicense.ChangePINWithIdentification = ule.ChangePINWithIdentification;

                kemasLicense.DateOfIssue = ule.DateOfIssue;
                kemasLicense.Description = ule.Description;
                kemasLicense.ExpireDate = ule.ExpireDate;
                kemasLicense.ID = ule.ID;
                kemasLicense.LastCheck = ule.LastCheck;
                kemasLicense.Lic_Expired = ule.Lic_Expired;
                kemasLicense.LicenseNumber = ule.LicenseNumber;

                kemasLicense.UsePIN = 1; // use Pin 1. not use pin 0
                kemasLicense.UsePINSpecified = true;
                kemasLicense.PIN = ule.PIN;
                kemasLicense.PINSpecified = (ule.PIN != 0) ? true : false;
                kemasLicense.PIN2 = ule.PIN2;
                kemasLicense.PIN2Specified = (ule.PIN2 != 0) ? true : false;

                kemasLicense.RFID = ule.RFID;
                kemasLicense.State = ule.State;
                //                kemasLicense.UsePIN = ule.UsePIN;
            }

            kemasBase.License = kemasLicense;
        }


        private static void ExtractDrieverLicenseFromFE(updateUserData kemasBase, UserExtension profileDataFromFE, bool firstTimeInput, List<string> missingFields)
        {
            if (profileDataFromFE.ProxyLicense != null)
            {
                UserLicenseExtension proxyLicense = profileDataFromFE.ProxyLicense;
                if (proxyLicense.PIN == 0)
                {
                    missingFields.Add(PinField);
                }
                if (proxyLicense.PIN2 == 0)
                {
                    missingFields.Add(Pin2Field);
                }

                if (firstTimeInput)
                {
                    if (string.IsNullOrEmpty(proxyLicense.LicenseNumber))
                    {
                        missingFields.Add(LicenseNumberField);
                    }

                    if (string.IsNullOrEmpty(proxyLicense.DateOfIssue))
                    {
                        missingFields.Add(DateOfIssueField);
                    }

                    if (string.IsNullOrEmpty(proxyLicense.ExpireDate))
                    {
                        missingFields.Add(ExpireDateField);
                    }
                }
            }
        }

        public static void RemoveUncessaryFileds(UserExtension ue) 
        {
            ue.SessionID = null;
        }
    }

    public interface IStatusHelper
    {
        bool NeedChangePWD();
        bool IsProfileFilled();

        bool SetRegistrationDone();
        bool SetChangePWDDone();
        bool SetFillProfileDone();

        void TriggerVMApprove();

        bool IsSCRejected();
        bool CleanUpBasicReject();

        bool IsUserTransferRejected();
        bool CleanUpTransferRequest();

        int IsValidToDoBooking();

        string Status { get; }

        // Flag: 1, 2, 3, 5, 9, B, C, D, F都不能做任何Booking
        // Flag: A 只是不能做DUB booking
    }

    public class UserStatusHelper : IStatusHelper
    {
        private UserStatusManager _mgr;
        public UserStatusHelper(UserStatusManager usm)
        {
            _mgr = usm;
        }

        public bool NeedChangePWD()
        {
            return _mgr.Status["1"].Value == 1 ? true : false;
        }

        public bool IsProfileFilled()
        {
            return _mgr.Status["2"].Value == 1 ? false : true;
        }

        public bool SetChangePWDDone()
        {
            bool set = false;

            if (_mgr.Status["1"].Value == 1)
            {
                _mgr.Status["1"].Value = 0;
                _mgr.Status["2"].Value = 1;
                set = true;
            }

            return set;
        }

        public void TriggerVMApprove()
        {
            _mgr.Status["6"].Value = 1;
            if (IsUserTransferRejected())
            {
                CleanUpTransferRequest();
            }
        }

        public bool SetFillProfileDone()
        {
            bool set = false;
            if (_mgr.Status["2"].Value == 1)
            {
                _mgr.Status["2"].Value = 0;
                _mgr.Status["3"].Value = 1;
                set = true;

                if (IsSCRejected())
                {
                    CleanUpBasicReject();
                }
            }

            return set;
        }

        public string Status
        {
            get
            {
                return _mgr.Status.BinaryPattern;
            }
        }

        public bool SetRegistrationDone()
        {
            bool set = false;
            if (_mgr.Status["2"].Value == 0)
            {
                _mgr.Status["2"].Value = 1;
                set = true;
            }
            return set;
        }

        public bool IsSCRejected()
        {
            return _mgr.Status["F"].Value == 1 ? true : false;
        }

        public bool CleanUpBasicReject()
        {
            bool set = false;
            if (_mgr.Status["F"].Value == 1)
            {
                _mgr.Status["F"].Value = 0;
                set = true;
            }
            return set;            
        }

        public bool IsUserTransferRejected()
        {
            return _mgr.Status["8"].Value == 1 ? true : false;
        }

        public bool CleanUpTransferRequest() 
        {
            bool set = false;
            if (_mgr.Status["8"].Value == 1)
            {
                _mgr.Status["8"].Value = 0;
                set = true;
            }
            return set; 
        }

        public int IsValidToDoBooking()
        {
            int typeOfJourney = -1;
            if (_mgr.Status["1"].Value == 1
                || _mgr.Status["2"].Value == 1
                || _mgr.Status["3"].Value == 1
                || _mgr.Status["5"].Value == 1
                || _mgr.Status["9"].Value == 1
                || _mgr.Status["B"].Value == 1
                || _mgr.Status["C"].Value == 1
                || _mgr.Status["D"].Value == 1
                || _mgr.Status["F"].Value == 1)
            {
                typeOfJourney = 0; // no billing
            }
            else if (_mgr.Status["A"].Value == 1)
            {
                typeOfJourney = 2;//no dub
            }

            else if (_mgr.Status["4"].Value == 1)
            {
                typeOfJourney = 3;
                if (_mgr.Status["A"].Value == 0)
                {
                    typeOfJourney = 1;
                }
    }


            return typeOfJourney;

            // Flag: 1, 2, 3, 5, 9, B, C, D, F都不能做任何Booking
            // Flag: A 只是不能做DUB booking

        }
    }

    public class AppRegistrationBLL : AbstractBLL, IAppRegistrationBLL
    {
        public AppRegistrationBLL(ProxyUserSetting profile):base(profile)
        {
        }

        public AppRegistrationBLL():this(null)
        {

        }

        public UserExtension UserRegistration(UserExtension feReg, string lang)
        {
            UserExtension createdUser = null;

            if (UserRegistrationConst.IsInitialRegistration(feReg) && Enum.IsDefined(typeof(Lang), lang))
            {
                WS_Auth_Response auth = KemasAdmin.SignOn();
                if (auth != null && !string.IsNullOrEmpty(auth.ID) && !string.IsNullOrEmpty(auth.SessionID))
                {
                    updateUserData kemasUserData = new updateUserData();
                    kemasUserData.Mail = feReg.Mail;
                    kemasUserData.Password = Encrypt.GetPasswordFormat(feReg.Password);


                    kemasUserData.Enabled = (int)VRentDataDictionay.UserStatusFlagValue.Enable;
                    kemasUserData.EnabledSpecified = true;
                    kemasUserData.AllowChangePwd = 1;
                    kemasUserData.AllowChangePwdSpecified = true;
                    kemasUserData.TypeOfJourney = 0; //private user
                    kemasUserData.TypeOfJourneySpecified = true;

                    //set status
                    IStatusHelper statusHelper = new UserStatusHelper(new UserStatusManager(null));

                    statusHelper.SetRegistrationDone();


                    KemasAdmin.AssignPrivateUserClient(kemasUserData, auth);
                    //assign default role employee
                    KemasAdmin.AssignRoleByUserType(feReg, UserRoleConstants.BookingUserKey, auth.SessionID);
                    kemasUserData.Roles = feReg.Roles.Select(m => m.ID).ToArray();

                    kemasUserData.Status = statusHelper.Status;
                    UserData2 ud2 = UserRegistrationConst.UpdateKemasUser(kemasUserData, auth.SessionID, lang);

                    //assembly
                    createdUser = UserRegistrationConst.AssembleUserExtention(ud2, auth);
                }
                else
                {
                    ReturnResult ret = new ReturnResult();
                    string msg = string.Format(ErrorConstants.AdminAccountWrongMessage, auth.ID);
                    ret.Code = ErrorConstants.AdminAccountWrongCode;
                    ret.Message = msg;
                    ret.Type = ResultType.VRENTFE;

                    throw new VrentApplicationException(ret);
                }
            }
            else
            {
                ReturnResult ret = new ReturnResult();
                string msg = string.Format(ErrorConstants.BadRegistrationDataMessage, feReg.Password, feReg.Mail, feReg.ID, lang);
                ret.Code = ErrorConstants.BadRegistrationDataCode;
                ret.Message = msg;
                ret.Type = ResultType.VRENTFE;

                throw new VrentApplicationException(ret);
            }

            createdUser.RoleEntities = null;
            return createdUser;
        }


        public UserExtension ChangePassword(UserExtension feUpdate, string lang)
        {
            UserExtension updateUser = null;

            if (UserRegistrationConst.IsValidBookingUser(feUpdate.ID) && Enum.IsDefined(typeof(Lang), lang) && !string.IsNullOrEmpty(UserInfo.SessionID))
            {
                if (string.IsNullOrEmpty(feUpdate.Password) || string.IsNullOrEmpty(feUpdate.CurrentPassword))
                {
                    throw new VrentApplicationException(ErrorConstants.ChangePWDCode,
                        string.Format(ErrorConstants.ChangePWSMessage, feUpdate.Password, feUpdate.CurrentPassword),
                        ResultType.VRENTFE);
                }
                else
                {
                    UserData2 beforeUpdate = UserRegistrationConst.RetrieveKemasUserByID(feUpdate.ID, UserInfo.SessionID);

                    if (!string.IsNullOrEmpty(beforeUpdate.ID))
                    {
                        WS_Auth_Response auth = new WS_Auth_Response();
                        auth.ID = UserInfo.ID;
                        auth.SessionID = UserInfo.SessionID;

                        
                        KemasUserAPI kemasUser = new KemasUserAPI();
                        UserData ud = new UserData();
                        ud.ID = UserInfo.ID;
                        ud.Password = Encrypt.GetPasswordFormat(feUpdate.Password);
                        ud.CurrentPassword = feUpdate.CurrentPassword;

                        string result = kemasUser.updateUser(beforeUpdate.ID, ud);

                        if (!string.IsNullOrEmpty(result) && Int32.Parse(result) == 0)
                        {
                            updateUserData kemasBase = UserRegistrationConst.ConvertUserData2ToUpdateUserData(beforeUpdate);

                            //set status
                            IStatusHelper statusHelper = new UserStatusHelper(new UserStatusManager(kemasBase.Status));
                            if (statusHelper.NeedChangePWD())
                            {
                                statusHelper.SetChangePWDDone();
                                kemasBase.Status = statusHelper.Status;

                                UserData2 ud2 = UserRegistrationConst.UpdateKemasUser(kemasBase, UserInfo.SessionID, lang);

                                //assembly
                                updateUser = UserRegistrationConst.AssembleUserExtention(ud2, auth);
                            }
                            else
                            {
                                //assembly
                                updateUser = UserRegistrationConst.AssembleUserExtention(beforeUpdate, auth);
                            }
                        }
                        else 
                        {
                            throw new VrentApplicationException(
                                ErrorConstants.ChangePWDFailCode,
                                string.Format(ErrorConstants.ChangePWDFailMessage,auth.ID,feUpdate.CurrentPassword)
                                ,ResultType.VRENTFE
                                );
                        }
                    }
                    else
                    {
                        ReturnResult ret = new ReturnResult();
                        ret.Success = -1;
                        ret.Code = MessageCode.CVCE000005.ToString();
                        ret.Message = MessageCode.CVCE000005.GetDescription();
                        ret.Type = ResultType.VRENT;
                        throw new VrentApplicationException(ret);
                    }
                }
            }
            else
            {
                ReturnResult ret = new ReturnResult();
                string msg = string.Format(ErrorConstants.BadProfileDataMessage, feUpdate.ID, lang);
                ret.Code = ErrorConstants.BadProfileDataCode;
                ret.Message = msg;
                ret.Type = ResultType.VRENTFE;

                throw new VrentApplicationException(ret);
            }

            updateUser.RoleEntities = null;
            return updateUser;

        }

        public UserExtension UpdateProfile(UserExtension feUpdate, string lang)
        {
            UserExtension updateUser = null;

            if (UserRegistrationConst.IsValidBookingUser(feUpdate.ID) && Enum.IsDefined(typeof(Lang), lang)
                && !string.IsNullOrEmpty(UserInfo.SessionID))
            {
                UserData2 beforeUpdate = UserRegistrationConst.RetrieveKemasUserByID(feUpdate.ID, UserInfo.SessionID);
                updateUserData kemasBase = UserRegistrationConst.ConvertUserData2ToUpdateUserData(beforeUpdate);

                //set status
                IStatusHelper statusHelper = new UserStatusHelper(new UserStatusManager(kemasBase.Status));
                if (statusHelper.NeedChangePWD())
                {
                    throw new VrentApplicationException(ErrorConstants.ChangePWDFirstCode
                        , string.Format(ErrorConstants.ChangePWDFirstMessage, feUpdate.ID), ResultType.VRENTFE);
                }
                else
                {
                    bool firstTime = !statusHelper.IsProfileFilled();
                    //this action should be change profile,must based on kemasfind
                    string[] missingFields = UserRegistrationConst.SyncUpdateDataWithKemas(kemasBase, feUpdate, firstTime);

                    if (missingFields.Length > 0)
                    {
                        throw new VrentApplicationException(
                            ErrorConstants.MandatoryFieldsMissingCode,
                            string.Format(ErrorConstants.MandatoryFieldsMissingMessage, string.Join(",", missingFields)),
                            ResultType.VRENTFE);
                    }
                    else
                    {
                        if (firstTime == true)
                        {
                            statusHelper.SetFillProfileDone();
                        }

                        #region
                        //handling user tranafer
                        if (!string.IsNullOrEmpty(feUpdate.ClientID) && !feUpdate.ClientID.Equals( beforeUpdate.Clients[0].ID))
                        {
                            KemasAdmin.AssignClients(feUpdate.ID, feUpdate.ClientID, kemasBase);

                            //need more error handling
                            UserTransferBLL utb = new UserTransferBLL(UserInfo);
                            UserInfo.Name = feUpdate.Name;
                            UserInfo.VName = feUpdate.VName;
                            UserInfo.ClientID = kemasBase.Clients[0];
                            UserInfo.Mail = kemasBase.Mail;
                            UserTransferRequest utr = utb.RequestUserTransfer(feUpdate.ClientID);
                            statusHelper.TriggerVMApprove();
                        }
                        #endregion

                        WS_Auth_Response auth = new WS_Auth_Response();
                        auth.ID = UserInfo.ID;
                        auth.SessionID = UserInfo.SessionID;

                        //convert proxy license to kemas License
                        UserRegistrationConst.ConvertLicenseExtentionToKemasLicense(kemasBase, feUpdate.ProxyLicense);

                        kemasBase.Status = statusHelper.Status;
                        UserData2 ud2 = UserRegistrationConst.UpdateKemasUser(kemasBase, UserInfo.SessionID, lang); 

                        //prepare final output
                        updateUser = UserRegistrationConst.AssembleUserExtention(ud2, auth);

                        ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(updateUser);
                        setting.SessionID = auth.SessionID;

                        RightBLL rb = new RightBLL(setting);
                        List<ProxyRole> roleList = rb.GetAllRoles(setting.ID, setting.SessionID);
                        if (roleList != null)
                        {
                            setting.AllRoles = roleList.ToArray();
                        }

                        if (HttpContext.Current != null)
                        {

                            HttpContext.Current.Session.Add(ServiceUtility.UserSettings, setting);
                        }
                    }
                }
            }
            else
            {
                ReturnResult ret = new ReturnResult();
                string msg = string.Format(ErrorConstants.BadProfileDataMessage, feUpdate.ID, lang);
                ret.Code = ErrorConstants.BadProfileDataCode;
                ret.Message = msg;
                ret.Type = ResultType.VRENTFE;

                throw new VrentApplicationException(ret);
            }

            updateUser.RoleEntities = null;
            return updateUser;

        }

        public UserExtension RetrieveProfile()
        {
            UserExtension existingUser = null;

            if (UserRegistrationConst.IsValidBookingUser(UserInfo.ID) && !string.IsNullOrEmpty(UserInfo.SessionID))
            {
                UserData2 kemasRepo = UserRegistrationConst.RetrieveKemasUserByID(UserInfo.ID, UserInfo.SessionID);

                WS_Auth_Response auth = new WS_Auth_Response()
                {
                    ID = UserInfo.ID,
                    SessionID = UserInfo.SessionID
                };
                existingUser = UserRegistrationConst.AssembleUserExtention(kemasRepo, auth);
            }
            else
            {
                ReturnResult ret = new ReturnResult();
                string msg = string.Format(ErrorConstants.BadProfileDataMessage, UserInfo.ID);
                ret.Code = ErrorConstants.BadProfileDataCode;
                ret.Message = msg;
                ret.Type = ResultType.VRENTFE;

                throw new VrentApplicationException(ret);
            }

            existingUser.RoleEntities = null;
            return existingUser;


        }
    }
}
