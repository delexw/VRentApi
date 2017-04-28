using CF.VRent.BLL.BLLFactory;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender;
using CF.VRent.Email.EmailSender.Clients;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities.FapiaoPreferenceProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Log;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.VRent.BLL
{
    public interface ICompanyBLL 
    {
        UserCompanyExtenstion CreateCompany(CompanyProfileRequest feCompanyProfile);
        UserCompanyExtenstion UpdateCompany(UserCompanyExtenstion feInput);
        UserCompanyExtenstion[] RetrieveCompanys();
        UserCompanyExtenstion RetrieveCompanyByID(string clientID);
        UserCompanyExtenstion EnableDisableCompany(string clientID, int status);
    }

    public class ClientCompare : IEqualityComparer<CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client>
    {

        public bool Equals(Entities.KEMASWSIF_CONFIGRef.Client x, Entities.KEMASWSIF_CONFIGRef.Client y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            else
            {
                return x.ID.GetHashCode().Equals(y.ID.GetHashCode());
            }
        }

        public int GetHashCode(Entities.KEMASWSIF_CONFIGRef.Client obj)
        {
            return obj.ID.GetHashCode();
        }
    }

    public class ClientIDCompare : IEqualityComparer<string>
    {

        public bool Equals(string x, string y)
        {
            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
            {
                return false;
            }
            else
            {
                return x.GetHashCode().Equals(y.GetHashCode());
            }
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }


    public class CompanyUtility
    {
        public static CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client[] RetrieveClientByID(string sessionID) 
        {
            KemasConfigsAPI kemasConfig = new KemasConfigsAPI();
            getClientsResponse visableClients = kemasConfig.getClients(sessionID);

            if (visableClients.Clients == null)
            {
                throw new VrentApplicationException(visableClients.Error.ErrorCode, visableClients.Error.ErrorMessage, ResultType.KEMAS);
            }

            return visableClients.Clients;
        }

        public const string ClientNameField = "Name";
        public const string ClientRegisteredAddressField = "RegisteredAddress";
        public const string ClientOfficeAddressField = "OfficeAddress";


        public const string BankAccountNameField = "BankAccountName";
        public const string BankAccountNoField = "BankAccountNo";

        //public const string ContactPersonField = "ContactPerson";


        public const string VMNameField = "Name";
        public const string VMVNameField = "VName";
        public const string VMMailField = "Mail";
        public const string VMPhoneField = "Phone";

        public const string DefaultLanguage = "english";


        public static string[] SyncUpdateDataWithKemas(CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client kemasBase, CompanyProfileRequest cpr)
        {
            List<string> missingFields = new List<string>();

            UserCompanyExtenstion uce = cpr.CompanyProfile;
            UserExtension vmUe = cpr.VMProfile;

            if (uce == null)
            {
                uce = new UserCompanyExtenstion();
            }
            if (vmUe == null)
            {
                vmUe = new UserExtension();
            }

            #region VM section
            if (string.IsNullOrEmpty(vmUe.Name))
            {
                missingFields.Add(VMNameField);
            }

            if (string.IsNullOrEmpty(vmUe.VName))
            {
                missingFields.Add(VMVNameField);
            }

            if (string.IsNullOrEmpty(vmUe.Mail))
            {
                missingFields.Add(VMMailField);
            }

            if (string.IsNullOrEmpty(vmUe.Phone))
            {
                missingFields.Add(VMPhoneField);
            }
            
            #endregion

            #region Client section
                if (string.IsNullOrEmpty(uce.Name))
                {
                    missingFields.Add(ClientNameField);
                }
                else
                {
                    kemasBase.Name = uce.Name;
                }

                if (string.IsNullOrEmpty(uce.RegisteredAddress))
                {
                    missingFields.Add(ClientRegisteredAddressField);
                }

                if (string.IsNullOrEmpty(uce.OfficeAddress))
                {
                    missingFields.Add(ClientOfficeAddressField);
                }

                if (string.IsNullOrEmpty(uce.BankAccountName))
                {
                    missingFields.Add(BankAccountNameField);
                }

                if (string.IsNullOrEmpty(uce.BankAccountNo))
                {
                    missingFields.Add(BankAccountNoField);
                }

            #endregion

            if (missingFields.Count == 0)
            {
                UserCompanyFactory.CombineAddress(kemasBase, uce);
                UserCompanyFactory.CombineBankAccountInfo(kemasBase, uce);

                //optional
                kemasBase.BusinessLicenseID = uce.BusinessLicenseID;
                kemasBase.OrgCodeCertificate = uce.OrgCodeCertificate;
                kemasBase.Comment = uce.Comment;
                kemasBase.LegalRepresentativeID = uce.LegalRepresentativeID;
                kemasBase.Number = uce.Number;
                kemasBase.Mail = vmUe.Mail;//using vm's email box
                kemasBase.ContactInfo = uce.ContactInfo;

                //handling contact person 
                //string[] contactPerson = new string[]{vmUe.Name,vmUe.VName};
                kemasBase.ContactPerson = string.Format("{0} {1}", vmUe.Name, vmUe.VName);
            }

            return missingFields.ToArray();
        }

        public static string[] SyncUpdateDataWithKemas(CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client kemasBase, UserCompanyExtenstion uce)
        {
            List<string> missingFields = new List<string>();

            #region Client section
            if (string.IsNullOrEmpty(uce.Name))
            {
                missingFields.Add(ClientNameField);
            }
            else
            {
                kemasBase.Name = uce.Name;
            }

            if (string.IsNullOrEmpty(uce.RegisteredAddress))
            {
                missingFields.Add(ClientRegisteredAddressField);
            }

            if (string.IsNullOrEmpty(uce.OfficeAddress))
            {
                missingFields.Add(ClientOfficeAddressField);
            }

            if (string.IsNullOrEmpty(uce.BankAccountName))
            {
                missingFields.Add(BankAccountNameField);
            }

            if (string.IsNullOrEmpty(uce.BankAccountNo))
            {
                missingFields.Add(BankAccountNoField);
            }

            #endregion

            if (missingFields.Count == 0)
            {
                UserCompanyFactory.CombineAddress(kemasBase, uce);
                UserCompanyFactory.CombineBankAccountInfo(kemasBase, uce);

                //optional
                kemasBase.BusinessLicenseID = uce.BusinessLicenseID;
                kemasBase.OrgCodeCertificate = uce.OrgCodeCertificate;
                kemasBase.Comment = uce.Comment;
                kemasBase.LegalRepresentativeID = uce.LegalRepresentativeID;
                kemasBase.ContactInfo = uce.ContactInfo;
                kemasBase.Number = uce.Number;
            }

            return missingFields.ToArray();
        }

        public static ProxyFapiaoPreference GenerateCompanyFapiaoPreference(UserCompanyExtenstion uce,ProxyUserSetting userInfo) 
        {
            ProxyFapiaoPreference fp = new ProxyFapiaoPreference();

            fp.ID = Guid.NewGuid().ToString();
            fp.UserID = uce.ID;//in this case should be a company ID
            fp.CustomerName = uce.Name;
            fp.MailType = "DEBIT";

            string rawAdrress = uce.Address.Replace("&quot;", "\"");
            string[] addressParts = SerializedHelper.JsonDeserialize<string[]>(rawAdrress);

            fp.MailAddress = addressParts[1];
            fp.MailPhone = uce.ContactInfo;
            fp.AddresseeName = uce.Name;
            fp.FapiaoType = (int)FapiaoType.VAT;
            fp.CreatedOn = DateTime.Now;
            fp.CreatedBy = Guid.Parse(userInfo.ID);
            return fp;
        }

        public static CF.VRent.Entities.KEMASWSIF_USERRef.Client ConvertFromConfigToUser(CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client configClient) 
        {
            return new Entities.KEMASWSIF_USERRef.Client()
            {
                 ID = configClient.ID,
                 Enabled = configClient.Enabled,
                 Number = configClient.Number,
                 Name = configClient.Name,
                 Comment = configClient.Comment,
                 ContactPerson = configClient.ContactPerson,
                 Address = configClient.Address,
                 ContactInfo = configClient.ContactInfo,
                 CountEmployees = configClient.CountEmployees,
                 Deposit = configClient.Deposit,
                 BankAccountInfo = configClient.BankAccountInfo,
                 BusinessLicenseID = configClient.BusinessLicenseID,
                 OrgCodeCertificate = configClient.OrgCodeCertificate,
                 LegalRepresentativeID =configClient.LegalRepresentativeID,
                 Mail = configClient.Mail
            };
        }

        #region 
        public static UserData2[] FindAllSCUserByRole(string roleName, string roleID,ProxyUserSetting operatorInfo) 
        {
            UserData2[] scUsers = null;
            string kemasRoleID = roleID;

                    KemasUserAPI kemasUser = new KemasUserAPI();
                    getUsers2RequestSearchCondition allSCSearch = new getUsers2RequestSearchCondition();
                    allSCSearch.RoleID = kemasRoleID;

                    getUsers2Request gur = new getUsers2Request();
                    gur.SearchCondition = allSCSearch;
                    gur.SessionID = operatorInfo.SessionID;
                    gur.ItemsPerPageSpecified = false;
                    gur.PageSpecified = false;

                    getUsers2Response scusersRes = kemasUser.getUsers2(gur);
                    if (scusersRes.Users != null)
                    {
                        scUsers = scusersRes.Users.ToArray();
                    }
                    else
                    {
                        throw new VrentApplicationException(scusersRes.Error.ErrorCode, scusersRes.Error.ErrorMessage, ResultType.KEMAS);
                    }
            return scUsers;
        }

        public static UserData2[] AddNewClientToScUsers(
            string[] allClients,string clientName,
            string roleID, string roleName,
            UserData2[] scUsers, ProxyUserSetting userInfo, string lang) 
        {
            List<UserData2> updatedScs = new List<UserData2>();

            string userName = string.Format("{0} {1}", userInfo.Name, userInfo.VName);
            updateUserData[] scUpdate = scUsers.Select(m => UserRegistrationConst.ConvertUserData2ToUpdateUserData(m)).ToArray();

            string targetUserName = null;

            for (int i = 0; i < scUpdate.Length;  i++)
            {
                scUpdate[i].Clients = allClients;

                try
                {
                    targetUserName = string.Format("{0} {1}", scUpdate[i].Name, scUpdate[i].VName);
                    UserData2 updatedSC = UserRegistrationConst.UpdateKemasUser(scUpdate[i], userInfo.SessionID, lang);
                    updatedScs.Add(updatedSC);
                }
                catch(Exception ex)
                {
                    string errMsg = null;
                    if (ex is VrentApplicationException)
                    {
                        VrentApplicationException vae = ex as VrentApplicationException;

                        errMsg = string.Format("Msg:{0}-StackTrace:{1}", vae.ErrorCode, vae.ErrorMessage);
                    }
                    else
                    {
                        errMsg = string.Format("Msg:{0}-StackTrace:{1}", ex.Message, ex.StackTrace);
                    }
                    string appendMsg = string.Format(ErrorConstants.AppendNewClientToRoleCodeMessage
                        , clientName
                        , allClients[allClients.Length - 1]
                        , targetUserName
                        , scUpdate[i].ID
                        , roleName
                        , roleID
                        , userName
                        ,userInfo.ID 
                        , errMsg
                        );
                    LogInfor.WriteError(ErrorConstants.AppendNewClientToRoleTitle, appendMsg, userName);

                }
            }

            return updatedScs.ToArray();
        }

        private static void AppendNewClientToRole(string[] newClients,string clientName,string vrentRoleKey,ProxyUserSetting UserInfo) 
        {
            //find admin role ID
            var svRole = UserRoleContext.CreateRoleManager().Roles[vrentRoleKey].GetDefaultKemasRole();
            var scRoleId = KemasAccessWrapper.CreateKemasExtensionAPIInstance().GetRoleID(svRole.Name, UserInfo.SessionID);
            string userName = string.Format("{0} {1}",UserInfo.Name,UserInfo.VName);

            if (string.IsNullOrEmpty(scRoleId))
            {
                string appendMsg = string.Format(ErrorConstants.RetrieveRoleIDFailureMessage
                    , newClients[newClients.Length - 1]
                    , clientName
                    ,svRole.Name
                    , scRoleId
                    , UserInfo.ID
                    , userName
                    );
                LogInfor.WriteError(ErrorConstants.AppendNewClientToRoleTitle, appendMsg, userName);
            }
            else
            {
                UserData2[] scUsers = null;
                try
                {
                    scUsers = CompanyUtility.FindAllSCUserByRole(svRole.Name, scRoleId, UserInfo);
                }
                catch(Exception ex)
                {
                    string errMsg = null;
                    if (ex is VrentApplicationException)
                    {
                        VrentApplicationException vae = ex as VrentApplicationException;

                        errMsg = string.Format("Msg:{0}-StackTrace:{1}", vae.ErrorCode, vae.ErrorMessage);
                    }
                    else
                    {
                        errMsg = string.Format("Msg:{0}-StackTrace:{1}", ex.Message, ex.StackTrace);
                    }
                    LogInfor.WriteError(ErrorConstants.RetrieveUsersWithRoleFailureCode, string.Format(ErrorConstants.RetrieveUsersWithRoleFailureMessage, svRole.Name, scRoleId, userName, UserInfo.ID, errMsg), userName);
                }
                UserData2[] updateSCUsers = CompanyUtility.AddNewClientToScUsers(newClients, clientName, scRoleId, svRole.Name, scUsers, UserInfo, DefaultLanguage);
            }
        }

        public static void AppendNewClientToAdminUsers(string[] newclients, string clientName, ProxyUserSetting UserInfo)
        {
            ClientCompare compare = new ClientCompare();
            //Assign new clients to all SC users
            //admin
            AppendNewClientToRole(newclients, clientName, UserRoleConstants.ServiceCenterKey, UserInfo);
            AppendNewClientToRole(newclients, clientName,UserRoleConstants.AdministratorKey, UserInfo);
            AppendNewClientToRole(newclients, clientName,UserRoleConstants.OperationManagerKey, UserInfo);
        }

        #endregion
    }

    public class CompanyBLL : AbstractBLL, ICompanyBLL
    {
        public CompanyBLL(ProxyUserSetting userInfo)
            : base(userInfo) 
        {
        }

        public UserCompanyExtenstion CreateCompany(CompanyProfileRequest feCompanyProfile)
        {
            UserCompanyExtenstion uce = null;
            updateClientResponse ucrRes = null;

            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client configClient = new CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client();

            string[] missingFields = CompanyUtility.SyncUpdateDataWithKemas(configClient, feCompanyProfile);

            if (missingFields.Length > 0)
            {
                throw new VrentApplicationException(
                    ErrorConstants.MandatoryFieldsMissingCode,
                    string.Format(ErrorConstants.MandatoryFieldsMissingMessage, string.Join(",", missingFields)),
                    ResultType.VRENTFE);

            }
            else
            {
                configClient.Enabled = 1; //1: enable, 0: disable
                configClient.Status = "1".ToString(); // use enbale 

                updateClientRequest ucrReq = new updateClientRequest();
                ucrReq.SessionID = UserInfo.SessionID;
                ucrReq.Client = configClient;

                KemasCompanyAPI companyAPI = new KemasCompanyAPI();
                ucrRes = companyAPI.UpdateCompany(ucrReq);

                if (ucrRes.Client != null)
                {

                    #region Creating the client's VM

                    UserCompanyFactory ucf = new UserCompanyFactory();
                    uce = ucf.CreateEntity<CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client>(ucrRes.Client);

                    UserExtension vmRole = feCompanyProfile.VMProfile;
                    vmRole.ClientID = ucrRes.Client.ID;

                    //vrent bug https://jira.mcon-group.com/browse/VRENT-856
                    vmRole.Phone = vmRole.Phone;//use generated phone number

                    KemasAdmin.AssignRoleByUserType(vmRole, UserRoleConstants.VRentManagerKey, UserInfo.SessionID);
                    vmRole.Enabled = 1;

                    updateUserData createReq = UserRegistrationConst.ConvertUserEntityToUpdateUserData(vmRole);
                    var originalPwd = UserUtility.RadomPassword(6);
                    createReq.Password = Encrypt.GetPasswordFormat(originalPwd);

                    //set vm's status
                    KemasAdmin.SetVMStatus(createReq);

                    KemasUserAPI kua = new KemasUserAPI();
                    updateUser2Request uu2Req = new updateUser2Request();
                    uu2Req.Language = CompanyUtility.DefaultLanguage;
                    uu2Req.UserData = createReq;
                    uu2Req.SessionID = UserInfo.SessionID;

                    updateUser2Response uu2Res = kua.updateUser2(uu2Req);

                    if (uu2Res.UserData == null)
                    {
                        #region VM creation fails
                        //if it fails on crating the vm for the client, deactive the client
                        updateClientRequest deactiveClientReq = new updateClientRequest();
                        ucrReq.Client = ucrRes.Client;
                        ucrReq.SessionID = UserInfo.SessionID;
                        ucrReq.Client.Status = "0".ToString(); //1 : active client, 0: deactive client

                        KemasCompanyAPI deactiveClientAPI = new KemasCompanyAPI();
                        updateClientResponse deactiveClientRes = deactiveClientAPI.UpdateCompany(ucrReq);

                        if (deactiveClientRes.Client == null)
                        {
                            throw new VrentApplicationException(ucrRes.Error.ErrorCode, ucrRes.Error.ErrorMessage, ResultType.KEMAS);
                        }
                        else
                        {
                            UserCompanyFactory deactiveUcf = new UserCompanyFactory();
                            UserCompanyExtenstion deactiveUce = deactiveUcf.CreateEntity<CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client>(deactiveClientRes.Client);
                        }

                        #endregion

                        throw new VrentApplicationException(uu2Res.Error.ErrorCode, uu2Res.Error.ErrorMessage, ResultType.KEMAS);
                    }
                    else
                    {
                        //Send Email immedialtely after vm is created
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                IClientCreatedSender sender = EmailSenderFactory.CreateClientCreatedSender();
                                sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                                {
                                    DataAccessProxyManager manager = new DataAccessProxyManager();
                                    manager.SendPaymentEmail(arg1, arg2.ToStr(), arg3);
                                };
                                sender.Send(new EmailParameterEntity()
                                {
                                    FirstName = uu2Res.UserData.Name,
                                    LastName = uu2Res.UserData.VName,
                                    Mail = uu2Res.UserData.Mail,
                                    Password = originalPwd,
                                    VRentUrl = ConfigurationManager.AppSettings["VRentUrl"]
                                }, uu2Res.UserData.Mail);
                            }
                            catch (Exception ex)
                            {
                                //Email
                                LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                                    String.Format("Exception:{0}", ex.ToStr()), "System");
                            }
                        }, TaskCreationOptions.PreferFairness);

                        ClientIDCompare compare = new ClientIDCompare();
                        //assign to current user
                        UserData2 beforeUpdate = UserRegistrationConst.RetrieveKemasUserByID(UserInfo.ID, UserInfo.SessionID);
                        updateUserData kemasBase = UserRegistrationConst.ConvertUserData2ToUpdateUserData(beforeUpdate);

                        string[] existingClients = kemasBase.Clients.Distinct(compare).ToArray();
                        int existingCnt = existingClients.Length;
                        string[] newClients = new string[existingCnt + 1];

                        Array.Copy(existingClients, newClients, existingCnt);
                        newClients[newClients.Length - 1] = ucrRes.Client.ID;
                        kemasBase.Clients = newClients;

                        UserRegistrationConst.UpdateKemasUser(kemasBase, UserInfo.SessionID, CompanyUtility.DefaultLanguage);

                        //Assign the new client/clients to all sc users
                        Task.Factory.StartNew(() => {
                            //assign new client to SC users
                            CompanyUtility.AppendNewClientToAdminUsers(newClients, ucrRes.Client.Name, UserInfo);
                        }, TaskCreationOptions.LongRunning);
                        
                    }
                    #endregion

                }
                else
                {
                    throw new VrentApplicationException(ucrRes.Error.ErrorCode, ucrRes.Error.ErrorMessage, ResultType.KEMAS);
                }
            }

            return uce;
        }

        

        public UserCompanyExtenstion UpdateCompany(UserCompanyExtenstion feInput)
        {
            UserCompanyExtenstion uce = null;

            //retrieve first
            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client client = CompanyUtility.RetrieveClientByID(UserInfo.SessionID).FirstOrDefault(m => m.ID.Equals(feInput.ID));

            if (client == null)
            {
                throw new VrentApplicationException(ErrorConstants.NoCompanyPermissionCode,
                    string.Format(ErrorConstants.NoCompanyPermissionMessage, UserInfo.ID, feInput.ID), ResultType.VRENT);
            }
            string[] missingFields = CompanyUtility.SyncUpdateDataWithKemas(client,feInput);

            if (missingFields.Length > 0)
            {
                throw new VrentApplicationException(
                    ErrorConstants.MandatoryFieldsMissingCode,
                    string.Format(ErrorConstants.MandatoryFieldsMissingMessage, string.Join(",", missingFields)),
                    ResultType.VRENTFE);
            }
            else
            {
                UserCompanyFactory.CombineAddress(client, feInput);
                UserCompanyFactory.CombineBankAccountInfo(client, feInput);

                updateClientRequest ucrReq = new updateClientRequest();
                ucrReq.SessionID = UserInfo.SessionID;
                ucrReq.Client = client;

                KemasCompanyAPI companyAPI = new KemasCompanyAPI();
                updateClientResponse ucrRes = companyAPI.UpdateCompany(ucrReq);

                if (ucrRes.Client != null)
                {
                    UserCompanyFactory ucf = new UserCompanyFactory();
                    uce = ucf.CreateEntity<CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client>(ucrRes.Client);
                }
                else
                {
                    throw new VrentApplicationException(ucrRes.Error.ErrorCode, ucrRes.Error.ErrorMessage, ResultType.KEMAS);
                }
            }
            return uce;
        }

        public UserCompanyExtenstion[] RetrieveCompanys()
        {
            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client[] userClients = CompanyUtility.RetrieveClientByID(UserInfo.SessionID);

            UserCompanyFactory ucf = new UserCompanyFactory();
            return userClients.Select(m => ucf.CreateEntity<CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client>(m)).ToArray();
        }

        public UserCompanyExtenstion RetrieveCompanyByID(string clientID)
        {
            UserCompanyExtenstion uce = null;

            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client[] userClients = CompanyUtility.RetrieveClientByID(UserInfo.SessionID);

            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client userClient = userClients.FirstOrDefault(m => m.ID.Equals(clientID));

            if (userClient == null)
            {
                throw new VrentApplicationException(ErrorConstants.NoCompanyPermissionCode,
                    string.Format(ErrorConstants.NoCompanyPermissionMessage, UserInfo.ID,clientID), ResultType.VRENT);
            }
            else
            {
                UserCompanyFactory ucf = new UserCompanyFactory();
                uce = ucf.CreateEntity<CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client>(userClient); 
            }

            return uce;
        }

        public UserCompanyExtenstion EnableDisableCompany(string clientID,int status) 
        {

            UserCompanyExtenstion uce = null;

            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client[] userClients = CompanyUtility.RetrieveClientByID(UserInfo.SessionID);
            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client userClient = userClients.FirstOrDefault(m => m.ID.Equals(clientID));

            if (userClient == null)
            {
                throw new VrentApplicationException(ErrorConstants.NoCompanyPermissionCode,
                    string.Format(ErrorConstants.NoCompanyPermissionMessage, clientID), ResultType.VRENT);
            }
            else
            {
                //if (status == Convert.ToInt32(userClient.Status))
                //{
                //    throw new VrentApplicationException(ErrorConstants.WrongCompanyStateCode,
                //        string.Format(ErrorConstants.WrongCompanyStateCode, clientID, userClient.Enabled), ResultType.VRENT);
                //}
                //else
                //{
                    updateClientRequest ucrReq = new updateClientRequest();
                    ucrReq.Client = userClient;
                    ucrReq.SessionID = UserInfo.SessionID;
                    ucrReq.Client.Status = status.ToString();

                    KemasCompanyAPI companyAPI = new KemasCompanyAPI();
                    updateClientResponse ucrRes = companyAPI.UpdateCompany(ucrReq);

                    if (ucrRes.Client == null)
                    {
                        throw new VrentApplicationException(ucrRes.Error.ErrorCode, ucrRes.Error.ErrorMessage, ResultType.KEMAS);
                    }
                    else
                    {
                        UserCompanyFactory ucf = new UserCompanyFactory();
                        uce = ucf.CreateEntity<CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client>(ucrRes.Client);
                    }
                //}
            }
            return uce;
        }
    }
}
