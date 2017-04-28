using CF.VRent.BLL.BLLFactory.Payment;
using CF.VRent.Common;
using CF.VRent.Contract;
using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using CF.VRent.BLL.BLLFactory.UserMgmt;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Common.UserContracts;
using CF.VRent.BLL.BLLFactory.GeneralLedger;
using CF.VRent.Entities.DataAccessProxy;

namespace CF.VRent.BLL.BLLFactory
{
    public class ServiceImpInstanceFactory
    {
        /// <summary>
        /// Create instance of user blocker
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static IUserBlocker CreateUserBlockInstance(UserExtension admin, UserExtension currentUser)
        {
            return new UserBlocker(admin, currentUser);
        }

        /// <summary>
        /// Create instance of payment message stream
        /// </summary>
        /// <returns></returns>
        public static IPaymentMessageStreamSerializer CreatePaymentMessageStreamSerializerInstance()
        {
            return new PaymentMessageStreamSerializer();
        }

        /// <summary>
        /// Create instance of TypeofJourneyStrategy
        /// </summary>
        /// <returns></returns>
        public static ITypeofJourneyStrategy CreateTypeofJourneyStrategyInstance()
        {
            return new TypeofJourneyStrategy();
        }

        /// <summary>
        /// Create instance of terms condition
        /// </summary>
        /// <returns></returns>
        public static ITermsCondition CreateTermsConditionInstance()
        {
            ITermsCondition tc = UnityHelper.GetUnityContainer("TermsConditionContainer").Resolve<ITermsCondition>();
            return tc;
        }

        /// <summary>
        /// Create instance of user mgmt
        /// </summary>
        /// <returns></returns>
        public static IUserMgmt CreateUserMgmtInstance()
        {
            return UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
        }

        /// <summary>
        /// Create instance of company
        /// </summary>
        /// <returns></returns>
        public static ICompany CreateCompanyInstance()
        {
            return UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<ICompany>("CompanyConfiguration");
        }

        /// <summary>
        /// Create instance of system configuration
        /// </summary>
        /// <returns></returns>
        public static ISystemConfiguration CreateSystemConfigurationInstance()
        {
            return UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<ISystemConfiguration>("UserStatusConfiguration");
        }

        /// <summary>
        /// Create instance of Portal login
        /// </summary>
        /// <returns></returns>
        public static IPortalLoginBLL CreatePortalLoginInstance()
        {
            return UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IPortalLoginBLL>();
        }

        /// <summary>
        /// Create User transfer operator
        /// </summary>
        /// <typeparam name="TUserTransfer"></typeparam>
        /// <returns></returns>
        public static IUserTransferDataAccessChannel CreateUserTransferOperatorInstance(ProxyUserSetting sessionUser)
        {
            return new UserTransferOperator(sessionUser);
        }

        ///// <summary>
        ///// Create instance of user mgmt cache
        ///// </summary>
        ///// <returns></returns>
        //public static IUserMgmtCacheChannel CreateUserCacheOperatorInstance()
        //{
        //    return new UserMgmtCacheOperator();
        //}

        /// <summary>
        /// Create instance of end user validation
        /// </summary>
        /// <returns></returns>
        public static IEndUserValidator CreateEndUserValidatorInstance()
        {
            return new EndUserValidator();
        }

        /// <summary>
        /// Create instance of user transfer strategy
        /// </summary>
        /// <param name="originalUser"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public static IUserTransferStrategy CreateUserTransferStrategyInstance(UserExtension originalUser,ProxyUserSetting sessionUser, string inputStatus)
        {
            return new UserTransferStrategy(originalUser, sessionUser, inputStatus);
        }

        /// <summary>
        /// Create instance of user transfer flow strategy instance
        /// </summary>
        /// <returns></returns>
        public static IUserStatusFlowStrategy CreateUserTransferFlowStrategyInstance(ProxyUserSetting sessionUser)
        {
            return new UserStatusFlowStrategy(sessionUser);
        }

        /// <summary>
        /// Create instance of CreateCorporateUserStrategy instance
        /// </summary>
        /// <param name="sessionUser"></param>
        /// <returns></returns>
        public static ICreateCorporateUserStrategy CreateCreateCorporateUserStrategyInstance(ProxyUserSetting sessionUser)
        {
            return new CreateCorporateUserStrategy(sessionUser);
        }

        /// <summary>
        /// Create instance of UpdateCorporateUserStrategy instance
        /// </summary>
        /// <param name="sessionUser"></param>
        /// <returns></returns>
        public static IUpdateCorporateUserStrategy CreateUpdateCorporateUserStrategyInstance(ProxyUserSetting sessionUser, UserExtension originalUser)
        {
            return new UpdateCorporateUserStrategy(sessionUser, originalUser);
        }

        /// <summary>
        /// Create instance of AppRegistrationBLL
        /// </summary>
        /// <param name="sessionUser"></param>
        /// <returns></returns>
        public static IAppRegistrationBLL CreateAppRegistrationInstance(ProxyUserSetting sessionUser =null)
        {
            if (sessionUser != null)
            {
                return UnityHelper.GetUnityContainer("UserRegistrationContainer").Resolve<IAppRegistrationBLL>(new ParameterOverride("profile", sessionUser));
            }
            else
            {
                return UnityHelper.GetUnityContainer("UserRegistrationContainer").Resolve<IAppRegistrationBLL>();
            }
        }

        /// <summary>
        /// Create instance of BookingStatusSync 
        /// </summary>
        /// <returns></returns>
        public static IBookingStatusSync CreateBookingStatusSyncInstance(ProxyUserSetting loginUser)
        {
            return new BookingStatusSync(loginUser);
        }

        /// <summary>
        /// Create instance of PriceDetails 
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static IGetPriceDetails CreatePriceDetailsInstance(ProxyUserSetting loginUser)
        {
            return new GetPriceDetails(loginUser);
        }

        /// <summary>
        /// Create instance of VRentManagerInfo
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static IGetVRentManagerInfo CreateGetVRentManagerInfoInstance(ProxyUserSetting loginUser)
        {
            return new GetVRentManagerInfo(loginUser);
        }

        /// <summary>
        /// Create instance of IndirectFeeOperation
        /// </summary>
        /// <returns></returns>
        public static IIndirectFeeOperation CreateIndirectFeeOperationInstance(ProxyUserSetting loginUser)
        {
            return UnityHelper.GetUnityContainer("IndirectFeeContainer").Resolve<IIndirectFeeOperation>(new ParameterOverride("userInfo", loginUser));
        }

        /// <summary>
        /// Create instance of DisableCCBAccout
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static IDisableCCBAccount CreateDisableCCBAccoutInstance(ProxyUserSetting loginUser)
        {
            return new DisableCCBAccount(loginUser);
        }

        /// <summary>
        /// Create instance of SendUserTransferEmail instance
        /// </summary>
        /// <param name="adminSessionId"></param>
        /// <param name="inputUser"></param>
        /// <returns></returns>
        public static ISendUserTransferEmail CreateSendUserTransferEmailInstance(string adminSessionId, UserExtension inputUser)
        {
            return new SendUserTransferEmail(adminSessionId, inputUser);
        }

        /// <summary>
        /// Create instance of ClientMgmt
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static ICompanyBLL CreateClientMgmtInstance(ProxyUserSetting loginUser)
        {
            return UnityHelper.GetUnityContainer("ClientsContainer").Resolve<ICompanyBLL>(new ParameterOverride("userInfo", loginUser));
        }

        /// <summary>
        /// Create instance of UserMgmt permission
        /// </summary>
        /// <returns></returns>
        public static IUserMgmtPermissionStrategy CreateUserMgmtPermissionInstance()
        {
            return new UserMgmtPermissionStrategy();
        }

        /// <summary>
        /// Create instance of payment
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static IPayment CreatePaymentInstance(ProxyUserSetting loginUser)
        {
            return UnityHelper.GetUnityContainer("PaymentContainer").Resolve<IPayment>(new ParameterOverride("userInfo", loginUser));
        }

        /// <summary>
        /// Create instance of CreateOrder in payment
        /// </summary>
        /// <returns></returns>
        public static ICreateOrder CreateCreateOrderInstance()
        {
            return new CreateOrder();
        }

        /// <summary>
        /// Create instance of transaction
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static ITransaction CreateTransactionInstance(ProxyUserSetting loginUser)
        {
            return UnityHelper.GetUnityContainer("TransactionContainer").Resolve<ITransaction>(new ParameterOverride("userInfo", loginUser));
        }

        /// <summary>
        /// Create instance of reservation
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static IProxyReservation CreateReservationInstance(ProxyUserSetting loginUser)
        {
            return UnityHelper.GetUnityContainer("ReservationContainer").Resolve<IProxyReservation>(new ResolverOverride[] { new ParameterOverride("userInfo", loginUser) });
        }

        /// <summary>
        /// Create instance of options
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static IOptionsBLL CreateOptionInstance(ProxyUserSetting loginUser)
        {
            return UnityHelper.GetUnityContainer("DataDicContainer").Resolve<IOptionsBLL>(new ResolverOverride[] { new ParameterOverride("userInfo", loginUser) });
        }

        /// <summary>
        /// Create instance of retry strategy
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static IRetryStrategy CreateRetryInstance(ProxyUserSetting loginUser)
        {
            return new RetryStrategy(loginUser);
        }

        /// <summary>
        /// Create instance of general ledger
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static IGeneralLedgerBLL CreateGeneralLedgerInstance(ProxyUserSetting loginUser)
        {
            return UnityHelper.GetUnityContainer("GeneralLedgerContainer").Resolve<IGeneralLedgerBLL>(new ResolverOverride[] { new ParameterOverride("userInfo", loginUser) });
        }

        /// <summary>
        /// Create instance of the catalog operaion of general ledger
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static ICatalogWithClient<GeneralLedgerStatisticDUB, string> CreateGeneralLedgerCatalogInstance(ProxyUserSetting loginUser)
        {
            return new CatalogWithClient(loginUser);
        }

        /// <summary>
        /// Create instance of the catalog operaion of ccb general ledger
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static ICatalogWithClient<GeneralLedgerStatisticCCB, string> CreateCCBGeneralLedgerCatalogInstance(ProxyUserSetting loginUser)
        {
            return new CatalogWithClientCCB(loginUser);
        }

        /// <summary>
        /// Create instance of GenerateLedger with generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public static T CreateGenerateLedgerInstance<T>(ProxyUserSetting loginUser) where T: IGenerateLedger
        {
            return (T)Activator.CreateInstance(typeof(T), loginUser);
        }
    }
}
