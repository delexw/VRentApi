using CF.VRent.Entities;
using CF.VRent.Entities.KemasWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Contract;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Common.Entities;
using System.ServiceModel.Web;
using System.Net;
using CF.VRent.Common.Entities.UserExt;
using Microsoft.Practices.Unity;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common.UserContracts;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.Log;
using CF.VRent.UserRole;
using CF.VRent.UserCompany;
using CF.VRent.UserStatus;

namespace CF.VRent.BLL
{
    /// <summary>
    /// Implementation
    /// </summary>
    public class UserManagementBLL :AbstractBLL, IUserMgmt
    {

        public UserManagementBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        {
        }
        public UserManagementBLL()
            : this(null)
        {
        }

        /// <summary>
        /// Get user list by page
        /// </summary>
        /// <param name="itemsPerPage"></param>
        /// <param name="pageNumber"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public EntityPager<UserExtensionHeader> GetUserList(
            int itemsPerPage,
            int pageNumber,
            UserExtension where, UserRoleEntityCollection currentUserRoleKey = null)
        {
            //Permission filter
            var permissionWhere = new UserPermissionFactory().CreateUserConditionEntity(where, currentUserRoleKey);

            IKemasUserAPI userApi = KemasAccessWrapper.CreateKemasUserAPI2Instance();

            var request = new getUsers2Request()
            {
                SessionID = this.UserInfo.SessionID,
                ItemsPerPage = itemsPerPage,
                ItemsPerPageSpecified = true,
                Page = pageNumber,
                PageSpecified = true,
                SearchCondition = new getUsers2RequestSearchCondition()
            };

            //VM
            if (currentUserRoleKey.IsVRentManagerUser())
            {
                var currentUser = userApi.findUser2(this.UserInfo.ID, this.UserInfo.SessionID);
                if (currentUser.UserData != null && 
                    currentUser.UserData.Clients != null && 
                    currentUser.UserData.Clients.Length > 0)
                {
                    request.SearchCondition.ClientID = currentUser.UserData.Clients[0].ID;
                }
                ////Company pending status
                //if (String.IsNullOrWhiteSpace(where.Status))
                //{
                //    //Default value
                //    request.SearchCondition.Status = "6";
                //}
            }

            //SC,SCL or ADMIN
            if (currentUserRoleKey.IsServiceCenterUser() || currentUserRoleKey.IsAdministrationUser() || currentUserRoleKey.IsOperationManagerUser())
            {
                request.SearchCondition = new getUsers2RequestSearchCondition();
                //License pending status
                //if (String.IsNullOrWhiteSpace(where.Status))
                //{
                //    //Default value
                //    request.SearchCondition.Status = "3";
                //}
            }

            //Show all employee users
            var kemasExtApi = KemasAccessWrapper.CreateKemasExtensionAPIInstance();
            var roleManager = UserRoleContext.CreateRoleManager();
            var defaultKemasRole = roleManager.Roles[UserRoleConstants.BookingUserKey].GetDefaultKemasRole();
            request.SearchCondition.RoleID = kemasExtApi.GetRoleID(defaultKemasRole.Name, this.UserInfo.SessionID);

            #region Search Condition
            //if the condition is null or String.Empty, ignore it
            if (!String.IsNullOrWhiteSpace(permissionWhere.Status))
            {
                request.SearchCondition.Status = permissionWhere.Status;
            }
            if (!String.IsNullOrWhiteSpace(permissionWhere.ClientID))
            {
                request.SearchCondition.ClientID = permissionWhere.ClientID;
            }
            if (!String.IsNullOrWhiteSpace(permissionWhere.Name))
            {
                request.SearchCondition.Name = permissionWhere.Name;
            }
            if (!String.IsNullOrWhiteSpace(permissionWhere.Mail))
            {
                request.SearchCondition.Username = permissionWhere.Mail;
            }
            if (!String.IsNullOrWhiteSpace(permissionWhere.Company))
            {
                request.SearchCondition.Client = permissionWhere.Company;
            }
            if (!String.IsNullOrWhiteSpace(permissionWhere.Phone))
            {
                request.SearchCondition.Phone = permissionWhere.Phone;
            }
            #endregion

            var kUser = userApi.getUsers2(request);

            var extension = new UserFactory().CreateHeaderEntity(kUser.Users);

            //Pager
            return new PagerFactory<UserExtensionHeader>().CreateEntity(extension, new Pager() { 
                ItemsPerPage = itemsPerPage,
                PageNumber = pageNumber,
                TotalCount = kUser.rows
            });
        }

        /// <summary>
        /// Get users with company pending status
        /// </summary>
        /// <param name="where"></param>
        /// <param name="currentUserRoleKey"></param>
        /// <returns></returns>
        public IEnumerable<UserExtensionHeader> GetCompanyUserList(UserExtension where, UserRoleEntityCollection currentUserRoleKey = null)
        {
            //Cache
            //var userListCache = ServiceImpInstanceFactory.CreateUserCacheOperatorInstance();

            var userTransferOperator = ServiceImpInstanceFactory.CreateUserTransferOperatorInstance(this.UserInfo);

            //Current session user
            var currentUser = this.UserInfo;

            //Company pending user
            var pendingUser = userTransferOperator.GetUserTransfersByUserClientID(currentUser.ClientID.ToGuid(), currentUser).Where(r => r.TransferResult == UserTransferResult.Pending);

            return new UserFactory().CreateHeaderEntity(pendingUser);
        }

        /// <summary>
        /// Get user detail
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserExtension GetUserDetail(string userId, UserRoleEntityCollection currentUserRoleKey = null)
        {
            IKemasUserAPI userApi = KemasAccessWrapper.CreateKemasUserAPI2Instance();

            var kUser = userApi.findUser2(userId, UserInfo.SessionID);

            var extension = new UserFactory().CreateEntity(kUser.UserData);

            extension.TypeOfJourney = ServiceImpInstanceFactory.CreateTypeofJourneyStrategyInstance().GetValueFromKemasValue(extension.TypeOfJourney);

            //Permission filter
            return new UserPermissionFactory().CreateEntity(extension, currentUserRoleKey);
        }

        /// <summary>
        /// User setting
        /// </summary>

        /// <summary>
        /// Create user with radom pwd and email sending
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public UserExtension CreateCorpUser(UserExtension user, string roleKey = null, UserRoleEntityCollection currentUserRoleKey = null)
        {
            var userPermission = new UserPermissionFactory();
            var typeofJourneyStrategy = ServiceImpInstanceFactory.CreateTypeofJourneyStrategyInstance();

            //Permission filter
            user = userPermission.CreateNewCorpUserEntity(user);

            IKemasUserAPI userApi = KemasAccessWrapper.CreateKemasUserAPI2Instance();

            var pwd = UserUtility.RadomPassword(6);
            var userStatusMg = UserStatusContext.CreateStatusManager();

            //Create corporate user strategy
            if (roleKey == null)
            {
                //Invoke by User Mgmt
                var createCorporateUserStrategy = ServiceImpInstanceFactory.CreateCreateCorporateUserStrategyInstance(this.UserInfo);
                user = createCorporateUserStrategy.Run(user, currentUserRoleKey, ref userStatusMg);
            }

            //Booking user Role(default value)
            var defaultKemasRole = new KemasRoleEntity();
            var kemasExtApi = KemasAccessWrapper.CreateKemasExtensionAPIInstance();
            if (roleKey == null)
            {
                var roleManager = UserRoleContext.CreateRoleManager();
                defaultKemasRole = roleManager.Roles[UserRoleConstants.BookingUserKey].GetDefaultKemasRole();
            }
            else
            {
                var roleManager = UserRoleContext.CreateRoleManager();
                defaultKemasRole = roleManager.Roles[roleKey].GetDefaultKemasRole();
            }

            var originalClientID = user.ClientID;
            //End user client
            //Get end user company id(default value)
            if (!String.IsNullOrWhiteSpace(user.ClientID) && roleKey == null)
            {
                if (currentUserRoleKey.IsServiceCenterUser() || currentUserRoleKey.IsOperationManagerUser() || currentUserRoleKey.IsAdministrationUser())
                {
                    var defaultEndUserCompany = UserCompanyContext.CreateCompanyManager().Companies[UserCompanyConstants.EndUserCompanyKey].GetDefaultKemasCompany();
                    var kemasCompay = kemasExtApi.GetCompanyID(defaultEndUserCompany.Name, this.UserInfo.SessionID);
                    user.ClientID = kemasCompay;
                    user.Company = defaultEndUserCompany.Name;
                }
                else if (currentUserRoleKey.IsVRentManagerUser())
                {
                    var vm = userApi.findUser2(this.UserInfo.ID, this.UserInfo.SessionID);
                    //user.ClientID = user.ClientID;
                    var client = vm.UserData.Clients.FirstOrDefault(r => r.ID == user.ClientID);
                    if (client != null)
                    {
                        user.Company = client.Name;
                    }
                    else
                    {
                        throw new VrentApplicationException(ErrorConstants.NoCompanyPermissionCode,
                            string.Format(ErrorConstants.NoCompanyPermissionMessage, user.ClientID), ResultType.VRENT);
                    }
                }
            }

            var newCorpUser = new UserFactory().CreateEntity(user);
            newCorpUser.AllowChangePwd = 1;
            newCorpUser.Enabled = 1;
            newCorpUser.Blocked = 0;
            newCorpUser.TypeOfJourney = typeofJourneyStrategy.GetValueFromApiInputValue(user.TypeOfJourney);
            newCorpUser.Roles = new string[] { kemasExtApi.GetRoleID(defaultKemasRole.Name, this.UserInfo.SessionID) };
            newCorpUser.Password = Encrypt.GetPasswordFormat(pwd);
            newCorpUser.Clients = new string[] { user.ClientID };
            newCorpUser.Company = user.Company;

            var newUser = userApi.updateUser2(new updateUser2Request()
            {
                SessionID = this.UserInfo.SessionID,
                Language = "english",
                UserData = newCorpUser
            });

            var extension = new UserFactory().CreateEntity(newUser.UserData);

            //Permission filter
            extension = userPermission.CreateEntity(extension, currentUserRoleKey);

            extension.Password = pwd;

            extension.TypeOfJourney = typeofJourneyStrategy.GetValueFromKemasValue(extension.TypeOfJourney);

            //Invoke this method from User Mgmt not Client Mgmt
            if (roleKey == null)
            {
                //Add to local db
                if (currentUserRoleKey.IsServiceCenterUser() || currentUserRoleKey.IsOperationManagerUser())
                {
                    var transferOp = ServiceImpInstanceFactory.CreateUserTransferOperatorInstance(this.UserInfo);
                    var transferRequest = new UserTransferRequest()
                    {
                        CreatedBy = this.UserInfo.ID.ToGuidNull(),
                        CreatedOn = DateTime.Now,
                        TransferResult = UserTransferResult.Pending,
                        UserID = extension.ID.ToGuid(),
                        FirstName = user.Name,
                        LastName = user.VName,
                        ClientIDFrom = null,
                        ClientIDTo = originalClientID.ToGuidNull(),
                        State = UserTransferState.Active,
                        Mail = user.Mail
                    };
                    var addBack = transferOp.AddUserTransfer(transferRequest, this.UserInfo);
                    if (addBack.ID <= 0)
                    {
                        LogInfor.WriteError("Add UserTransfer Failed", String.Format("Transfer Entity:{0}", transferRequest.ObjectToJson()), this.UserInfo.ID);
                    }
                }
            }


            return extension;
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public UserExtension UpdateUser(UserExtension user, UserRoleEntityCollection currentUserRoleKey = null)
        {
            var userDataPermission = new UserPermissionFactory();
            var userExtensionFactory = new UserFactory();
            var typeofJourneyStrategy = ServiceImpInstanceFactory.CreateTypeofJourneyStrategyInstance();
            var userTransferOperator = ServiceImpInstanceFactory.CreateUserTransferOperatorInstance(this.UserInfo);
            var inputStatus = user.Status;

            IKemasUserAPI userApi = KemasAccessWrapper.CreateKemasUserAPI2Instance();

            //Current user data in kemas
            var userInKemas = userApi.findUser2(user.ID, this.UserInfo.SessionID);
            var userExtensionInKemas = userExtensionFactory.CreateEntity(userInKemas.UserData);

            //Reset kemas status
            user.Enabled = userInKemas.UserData.Enabled;
            user.Blocked = userInKemas.UserData.Blocked;

            //Status
            var userStatusMg = UserStatusContext.CreateStatusManager(userInKemas.UserData.Status);

            //Status entities copy
            user.OriginalStatusEntities = UserStatusContext.CreateStatusManager(userInKemas.UserData.Status).Status;

            //Get transfer only for VRent Manager
            var transfer = userTransferOperator.GetUserTransferByUserID(user.ID.ToGuid(), currentUserRoleKey);
            if (transfer != null && transfer.ClientIDTo.HasValue)
            {
                user.ClientID = transfer.ClientIDTo.Value.ToStr();
            }

            #region Status flow strategy -> Update strategy -> Transfer strategy
            //Status flow strategy -> Transfer strategy -> Update strategy
            //Status flow strategy
            var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(this.UserInfo);
            user = userStatusFlowStratege.Run(user, currentUserRoleKey, ref userStatusMg);

            //Transfer stategy
            var transferStrategy = ServiceImpInstanceFactory.CreateUserTransferStrategyInstance(userExtensionInKemas, this.UserInfo, inputStatus);
            user = transferStrategy.Run(user, currentUserRoleKey, ref userStatusMg);

            //Update strategy
            var updateStrategy = ServiceImpInstanceFactory.CreateUpdateCorporateUserStrategyInstance(this.UserInfo, userExtensionInKemas);
            user = updateStrategy.Run(user, currentUserRoleKey, ref userStatusMg);

            //License
            user.License = new UserLicenseFactory().CreateEntity(user.ProxyLicense);
            user.License.State = user.StatusEntities["4"].Value == 1 ? 0 : 2;

            //User data
            var updateUserData = userExtensionFactory.CreateEntity(user);
            if (!String.IsNullOrWhiteSpace(user.ClientID))
            {
                updateUserData.Clients = new string[] { user.ClientID };
            }
            #endregion

            //Disable ccb permission
            if (user.StatusEntities["9"].Value == 1)
            {
                var disableCCBOp = ServiceImpInstanceFactory.CreateDisableCCBAccoutInstance(this.UserInfo);
                disableCCBOp.DisableCCBPermission(user, currentUserRoleKey);
            }

            //TypeofJourney strategy
            updateUserData.TypeOfJourney = typeofJourneyStrategy.GetValueFromApiInputValue(user.TypeOfJourney);

            //Current user name
            if (String.IsNullOrWhiteSpace(user.Mail))
            {
                updateUserData.Mail = userInKemas.UserData.Mail;
            }
            else
            {
                updateUserData.Mail = user.Mail;
            }

            //Update
            var newUser = userApi.updateUser2(new updateUser2Request()
            {
                SessionID = this.UserInfo.SessionID,
                Language = "english",
                UserData = updateUserData
            });

            //Convert it to user extension
            var extension = userExtensionFactory.CreateEntity(newUser.UserData);

            extension.IsPrivateUser = user.IsPrivateUser;
            extension.IsPrivateUserBefore = user.IsPrivateUserBefore;
            extension.LoginName = user.Mail;

            //Permission filter
            extension = userDataPermission.CreateEntity(extension, currentUserRoleKey);

            extension.TypeOfJourney = typeofJourneyStrategy.GetValueFromKemasValue(extension.TypeOfJourney);

            return extension;
        }
    }
}
