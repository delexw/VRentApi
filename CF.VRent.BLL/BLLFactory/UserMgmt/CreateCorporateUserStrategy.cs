using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.UserRole;
using CF.VRent.UserStatus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public class CreateCorporateUserStrategy : ICreateCorporateUserStrategy
    {
        private List<CreateCorporateUserStrategyEntity> _strategy;
        private ProxyUserSetting _sessionUser;

        public CreateCorporateUserStrategy(ProxyUserSetting sessionUser)
        {
            _strategy = new List<CreateCorporateUserStrategyEntity>();
            _sessionUser = sessionUser;

            //Input user clientId is not end user company id && VRent Manager
            _strategy.Add(new CreateCorporateUserStrategyEntity()
            {
                IsEndUser = false,
                InnerEntity =
                new CreateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.VRentManagerKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        //Change pwd
                        um.Status["1"].Value = 1;
                        var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(_sessionUser);
                        //Default company approved
                        uext.Status = "7";
                        return userStatusFlowStratege.Run(uext, curRole, ref um);
                    }
                }
            });

            //Input user clientId is not end user company id && Service Center
            _strategy.Add(new CreateCorporateUserStrategyEntity()
            {
                IsEndUser = false,
                InnerEntity =
                new CreateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.ServiceCenterKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        //Change pwd
                        um.Status["1"].Value = 1;
                        var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(_sessionUser);
                        //Default company pending
                        uext.Status = "6";
                        return userStatusFlowStratege.Run(uext, curRole, ref um);
                    }
                }
            });

            //Input user clientId is not end user company id &&  Operation manager
            _strategy.Add(new CreateCorporateUserStrategyEntity()
            {
                IsEndUser = false,
                InnerEntity =
                new CreateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.OperationManagerKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        //Change pwd
                        um.Status["1"].Value = 1;
                        var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(_sessionUser);
                        //Default company pending
                        uext.Status = "6";
                        return userStatusFlowStratege.Run(uext, curRole, ref um);
                    }
                }
            });
            //Input user clientId is not end user company id &&  Administrator
            _strategy.Add(new CreateCorporateUserStrategyEntity()
            {
                IsEndUser = false,
                InnerEntity =
                new CreateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.AdministratorKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        //Change pwd
                        um.Status["1"].Value = 1;
                        var userStatusFlowStratege = ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(_sessionUser);
                        //Default company pending
                        uext.Status = "6";
                        return userStatusFlowStratege.Run(uext, curRole, ref um);
                    }
                }
            });

            //Input user clientId is end user company id && VRent Manager
            _strategy.Add(new CreateCorporateUserStrategyEntity()
            {
                IsEndUser = true,
                InnerEntity =
                new CreateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.VRentManagerKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        throw new VrentApplicationException(new Common.Entities.ReturnResult()
                        {
                            Code = MessageCode.CVCE000007.ToStr(),
                            Message = MessageCode.CVCE000007.GetDescription(),
                            Type = MessageCode.CVCE000007.GetMessageType()
                        });
                    }
                }
            });

            //Input user clientId is end user company id && Service Center
            _strategy.Add(new CreateCorporateUserStrategyEntity()
            {
                IsEndUser = true,
                InnerEntity =
                new CreateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.ServiceCenterKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        throw new VrentApplicationException(new Common.Entities.ReturnResult()
                        {
                            Code = MessageCode.CVCE000007.ToStr(),
                            Message = MessageCode.CVCE000007.GetDescription(),
                            Type = MessageCode.CVCE000007.GetMessageType()
                        });
                    }
                }
            });

            //Input user clientId is end user company id && Operation manager
            _strategy.Add(new CreateCorporateUserStrategyEntity()
            {
                IsEndUser = true,
                InnerEntity =
                new CreateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.OperationManagerKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        throw new VrentApplicationException(new Common.Entities.ReturnResult()
                        {
                            Code = MessageCode.CVCE000007.ToStr(),
                            Message = MessageCode.CVCE000007.GetDescription(),
                            Type = MessageCode.CVCE000007.GetMessageType()
                        });
                    }
                }
            });

            //Input user clientId is end user company id && Operation manager
            _strategy.Add(new CreateCorporateUserStrategyEntity()
            {
                IsEndUser = true,
                InnerEntity =
                new CreateCorporateUserStrategyInnerEntity()
                {
                    RoleKey = UserRoleConstants.AdministratorKey,
                    ProxyMethod = (uext, curRole, um) =>
                    {
                        throw new VrentApplicationException(new Common.Entities.ReturnResult()
                        {
                            Code = MessageCode.CVCE000007.ToStr(),
                            Message = MessageCode.CVCE000007.GetDescription(),
                            Type = MessageCode.CVCE000007.GetMessageType()
                        });
                    }
                }
            });
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
            if (currentUserRoleKey.IsAdministrationUser())
            {
                key = UserRoleConstants.AdministratorKey;
            }

            var validateRet = endUserValidator.Validate(inputUser.ClientID, _sessionUser.SessionID);
            if (validateRet.HasValue)
            {
                inputUser.IsPrivateUser = validateRet.Value;

                var method = _strategy.FirstOrDefault(r => r.IsEndUser == inputUser.IsPrivateUser && r.InnerEntity.RoleKey == key);

                if (method != null && method.InnerEntity != null)
                {
                    return method.InnerEntity.ProxyMethod(inputUser, currentUserRoleKey, statusManager);
                }
            }

            return inputUser;
        }


    }

    internal class CreateCorporateUserStrategyInnerEntity
    {
        public string RoleKey { get; set; }
        public Func<UserExtension,UserRoleEntityCollection,IUserStatusManager, UserExtension> ProxyMethod { get; set; }
    }

    internal class CreateCorporateUserStrategyEntity
    {
        public bool IsEndUser { get; set; }

        public CreateCorporateUserStrategyInnerEntity InnerEntity { get; set; }
    }
}
