using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.UserRole;
using CF.VRent.UserStatus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public class UpdateCorporateUserStrategy:IUpdateCorporateUserStrategy
    {
        private List<UpdateCorporateUserStrategyEntity> _strategy;
        private UserExtension _originalUser;
        private ProxyUserSetting _sessionUser;


        public UpdateCorporateUserStrategy(ProxyUserSetting sessionUser, UserExtension originalUser)
        {
            _strategy = new List<UpdateCorporateUserStrategyEntity>();
            _sessionUser = sessionUser;
            _originalUser = originalUser;

            #region Input user clientId is corporate user company id && current clientId is corporate user company id
            //Input user clientId is not end user company id && VRent Manager
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = false,
                CurrentIsEndUser = false,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.VRentManagerKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(_sessionUser);
                        return userStatusFlowStratege.Run(uext, curRole, ref um);
                    }
                }
            });

            //Input user clientId is not end user company id && Service Center
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = false,
                CurrentIsEndUser = false,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.ServiceCenterKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        return _updateStatusFlowBySCOrSCL(uext, curRole, ref um);
                    }
                }
            });

            //Input user clientId is not end user company id && Operation Manager
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = false,
                CurrentIsEndUser = false,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.OperationManagerKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        return _updateStatusFlowBySCOrSCL(uext, curRole, ref um);
                    }
                }
            }); 
            #endregion

            #region Input user clientId is corporate user company id && current clientId is end user company id
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = false,
                CurrentIsEndUser = true,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.VRentManagerKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(_sessionUser);
                        return userStatusFlowStratege.Run(uext, curRole, ref um);
                    }
                }
            });

            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = false,
                CurrentIsEndUser = true,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.ServiceCenterKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        return _updateStatusFlowBySCOrSCL(uext, curRole, ref um);
                    }
                }
            });

            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = false,
                CurrentIsEndUser = true,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.OperationManagerKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        return _updateStatusFlowBySCOrSCL(uext, curRole, ref um);
                    }
                }
            }); 
            #endregion

            #region Input user clientId is end user company id && current clientId is end user company
            //Input user clientId is end user company id && VRent Manager
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = true,
                CurrentIsEndUser = true,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.VRentManagerKey,
                    ProxyMethod = null
                }
            });

            //Input user clientId is end user company id && Service Center
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = true,
                CurrentIsEndUser = true,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.ServiceCenterKey,
                    ProxyMethod = null
                }
            });

            //Input user clientId is end user company id && Operation manager
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = true,
                CurrentIsEndUser = true,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.OperationManagerKey,
                    ProxyMethod = null
                }
            }); 
            #endregion

            #region Input user clientId is end user company id && current clientId is corporate user company
            //Input user clientId is end user company id && VRent Manager
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = true,
                CurrentIsEndUser = false,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.VRentManagerKey,
                    ProxyMethod = null
                }
            });

            //Input user clientId is end user company id && Service Center
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = true,
                CurrentIsEndUser = false,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.ServiceCenterKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(_sessionUser);
                        return userStatusFlowStratege.DisableAllCompanyStatus(uext, ref um);
                    }
                }
            });

            //Input user clientId is end user company id && Operation manager
            _strategy.Add(new UpdateCorporateUserStrategyEntity()
            {
                InputIsEndUser = true,
                CurrentIsEndUser = false,
                InnerEntity =
                new UpdateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.OperationManagerKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(_sessionUser);
                        return userStatusFlowStratege.DisableAllCompanyStatus(uext, ref um);
                    }
                }
            }); 
            #endregion
        }

        /// <summary>
        /// do the strategy
        /// </summary>
        /// <param name="inputUser"></param>
        /// <param name="currentUserRoleKey"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        public UserExtension Run(UserExtension inputUser, UserRoleEntityCollection currentUserRoleKey, ref IUserStatusManager statusManager)
        {
            if (!String.IsNullOrWhiteSpace(inputUser.ClientID))
            {
                var endUserValidator = ServiceImpInstanceFactory.CreateEndUserValidatorInstance();

                var key = "";

                if (currentUserRoleKey.IsVRentManagerUser())
                {
                    key = UserRoleConstants.VRentManagerKey;
                }
                if (currentUserRoleKey.IsServiceCenterUser())
                {
                    key = UserRoleConstants.ServiceCenterKey;
                }
                if (currentUserRoleKey.IsOperationManagerUser())
                {
                    key = UserRoleConstants.OperationManagerKey;
                }

                var method = _strategy.FirstOrDefault(r => r.InputIsEndUser == inputUser.IsPrivateUser &&
                        r.CurrentIsEndUser == inputUser.IsPrivateUserBefore &&
                        r.InnerEntity.RoleKey == key);

                if (method != null && method.InnerEntity != null && method.InnerEntity.ProxyMethod != null)
                {
                    return method.InnerEntity.ProxyMethod(inputUser, currentUserRoleKey, statusManager);
                }
            }

            return inputUser;
        }

        /// <summary>
        /// Update user status flow by SC or Operation Manager
        /// </summary>
        /// <param name="inputUser"></param>
        /// <param name="LoginUserRole"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _updateStatusFlowBySCOrSCL(UserExtension inputUser, UserRoleEntityCollection LoginUserRole, ref IUserStatusManager statusManager)
        {
            var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(_sessionUser);
            //Default company pending if VM has not apporved/rejeceted yet
            //var api = KemasAccessWrapper.CreateKemasUserAPI2Instance();
            //var kemasInputUser = api.findUser2(inputUser.ID, _sessionUser.SessionID);
            //var tempStatusManager = UserContext.CreateStatusManager(kemasInputUser.UserData.Status);

            //Must know whehter the lastest clientid is approved in local db by VM
            var transferOp = ServiceImpInstanceFactory.CreateUserTransferOperatorInstance(_sessionUser);
            var userTransfer = transferOp.GetUserTransferByUserID(inputUser.ID.ToGuid());
            if (userTransfer != null)
            {
                inputUser.Status = "6";
                return userStatusFlowStratege.Run(inputUser, LoginUserRole, ref statusManager);
            }
            return inputUser;
        }
    }


    internal class UpdateCorporateUserStrategyInnerEntity
    {
        public string RoleKey { get; set; }
        public Func<UserExtension, UserRoleEntityCollection, IUserStatusManager, UserExtension> ProxyMethod { get; set; }
    }

    internal class UpdateCorporateUserStrategyEntity
    {
        public bool InputIsEndUser { get; set; }
        public bool CurrentIsEndUser { get; set; }

        public UpdateCorporateUserStrategyInnerEntity InnerEntity { get; set; }
    }
}
