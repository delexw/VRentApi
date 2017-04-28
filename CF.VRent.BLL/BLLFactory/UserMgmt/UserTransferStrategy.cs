using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Log;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.UserStatus.Interfaces;
using CF.VRent.UserRole;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public class UserTransferStrategy : IUserTransferStrategy
    {
        private UserExtension _originalUser;
        private ProxyUserSetting _sessionUser;

        private List<UserTransferStrategyHeaderEntity> _strategy;


        public UserTransferStrategy(UserExtension originalUser, ProxyUserSetting sessionUser, string inputStatus)
        {
            _originalUser = originalUser;
            _sessionUser = ServiceUtility.ConvertFromUserExtention(new UserFactory().CreateEntity(sessionUser));

            _strategy = new List<UserTransferStrategyHeaderEntity>();

            #region Current user is end user && input user is end user
            //Current user is end user && input user is end user && VRent Manager
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = true,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = true,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.VRentManagerKey,
                        ProxyMethod = null
                    }
                }
            });
            //Current user is end user && input user is end user && Service Center
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = true,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = true,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.ServiceCenterKey,
                        ProxyMethod = null
                    }
                }
            });
            //Current user is end user && input user is end user && Operation Manager
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = true,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = true,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.OperationManagerKey,
                        ProxyMethod = null
                    }
                }
            });
            #endregion

            #region Current user is end user && input user is corporate user
            //Current user is end user && input user is not end user && Vrent Manager && Navigation is Transfer
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = false,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = true,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.VRentManagerKey,
                        Navigation = UserMgmtNavigation.Operation.TransferUser,
                        ProxyMethod = (inputUser) =>
                        {
                            //Company approved
                            if (inputStatus == "7")
                            {
                                return _toCorporateByVM(inputUser);
                            }
                            else if (inputStatus == "8")
                            {
                                return _toCorporateByVMReject(inputUser);
                            }
                            return inputUser;
                        }
                    }
                }
            });

            //Current user is end user && input user is not end user && Service Center && Navigation is UpdateUser
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = false,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = true,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.ServiceCenterKey,
                        ProxyMethod = (inputUser) =>
                        {
                            return _updateNewCorprtateBySCOrSCL(inputUser);
                        }
                    }
                }
            });

            //Current user is end user && input user is not end user && Operation Manager && Navigation is UpdateUser
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = false,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = true,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.OperationManagerKey,
                        ProxyMethod = (inputUser) =>
                        {
                            return _updateNewCorprtateBySCOrSCL(inputUser);
                        }
                    }
                }
            });
            #endregion

            #region Current user is corporate user && input user is corporate user
            //Current user is corporate user && input usr is corporate user && VRent Manager && Navigation is Transfer
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = false,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = false,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.VRentManagerKey,
                        Navigation = UserMgmtNavigation.Operation.TransferUser,
                        ProxyMethod = (inputUser) =>
                        {
                            //Company approved
                            if (inputStatus == "7")
                            {
                                return _toCorporateByVM(inputUser);
                            }
                            else if (inputStatus == "8")
                            {
                                return _toCorporateByVMReject(inputUser);
                            }
                            else
                            {
                                return _resetCompanyName(inputUser);
                            }
                        }
                    }
                }
            });

            //Current user is corporate user && input usr is corporate user && Service Center && Navigation is UpdateUser
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = false,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = false,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.ServiceCenterKey,
                        ProxyMethod = (inputUser) =>
                        {
                            return this._updateNewCorprtateBySCOrSCL(inputUser);
                        }
                    }
                }
            });

            //Current user is corporate user && input usr is corporate user && Operation Manager && Navigation is UpdateUser
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = false,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = false,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.OperationManagerKey,
                        ProxyMethod = (inputUser) =>
                        {
                            return this._updateNewCorprtateBySCOrSCL(inputUser);
                        }
                    }
                }
            });

            #endregion

            #region Current user is corporate user && input user is end user
            //Current user is corporate user && input user is end user && VRent Manager
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = true,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = false,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.VRentManagerKey,
                        ProxyMethod = null
                    }
                }
            });

            //Current user is corporate user && input user is end user && Service Center
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = true,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = false,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.ServiceCenterKey,
                        ProxyMethod = (inputUser) =>
                        {
                            return _corporate2EndBySCOrSCL(inputUser);
                        }
                    }
                }
            });

            //Current user is corporate user && input user is end user && Operation Manager
            _strategy.Add(new UserTransferStrategyHeaderEntity()
            {
                InputUserIsEndUser = true,
                InnerEntity = new UserTransferStrategyEntity()
                {
                    CurrentUserIsEndUser = false,
                    InnerEntity = new UserTransferStrategyInnerEntity()
                    {
                        RoleKey = UserRoleConstants.OperationManagerKey,
                        ProxyMethod = (inputUser) =>
                        {
                            return _corporate2EndBySCOrSCL(inputUser);
                        }
                    }
                }
            });
            #endregion
        }

        /// <summary>
        /// Reset the company name
        /// </summary>
        /// <param name="inputUser"></param>
        /// <returns></returns>
        private UserExtension _resetCompanyName(UserExtension inputUser)
        {
            //inputUser.Company = KemasAccessWrapper.CreateKemasExtensionAPIInstance().GetCompanyName(_originalUser.Clients[0].ID, _sessionUser.SessionID);
            return inputUser;
        }

        /// <summary>
        /// Run the user transfer strategy
        /// </summary>
        /// <param name="user">Input user info from api parameter</param>
        /// <param name="currentUserRoleKey">current user roles</param>
        /// <returns></returns>
        public UserExtension Run(UserExtension user, UserRoleEntityCollection currentUserRoleKey, ref IUserStatusManager statusManager)
        {
            if (_originalUser.Clients != null && _originalUser.Clients.Length > 0 && !String.IsNullOrWhiteSpace(user.ClientID))
            {
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

                user.IsPrivateUserBefore = true;
                user.IsPrivateUser = true;
                var endUserValidator = ServiceImpInstanceFactory.CreateEndUserValidatorInstance();
                var validatedUser = endUserValidator.Validate((_originalUser.Clients != null && _originalUser.Clients.Length > 0) ?
                    _originalUser.Clients[0].ID : null, _sessionUser.SessionID);
                if (validatedUser.HasValue)
                {
                    user.IsPrivateUserBefore = validatedUser.Value;
                }
                var validateRet = endUserValidator.Validate(user.ClientID, _sessionUser.SessionID);
                if (validateRet.HasValue)
                {
                    user.IsPrivateUser = validateRet.Value;
                }

                //Get the method
                var method = _strategy.FirstOrDefault(r => r.InputUserIsEndUser == user.IsPrivateUser
                    && r.InnerEntity.CurrentUserIsEndUser == user.IsPrivateUserBefore &&
                    r.InnerEntity.InnerEntity.RoleKey == key);

                if (method != null &&
                    method.InnerEntity != null &&
                    method.InnerEntity.InnerEntity != null &&
                    method.InnerEntity.InnerEntity.ProxyMethod != null)
                {
                    return method.InnerEntity.InnerEntity.ProxyMethod(user);
                }
            }

            return user;
        }

        /// <summary>
        /// End user/Corporate user to corportate user (VRent Manager Approved)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private UserExtension _toCorporateByVM(UserExtension input)
        {
            var userTransferOperator = ServiceImpInstanceFactory.CreateUserTransferOperatorInstance(_sessionUser);
            var userTransfer = userTransferOperator.GetUserTransferByUserID(input.ID.ToGuid());
            if (userTransfer != null && userTransfer.ClientIDTo.HasValue)
            {
                input.ClientID = userTransfer.ClientIDTo.ToStr();
                input.Company = KemasAccessWrapper.CreateKemasExtensionAPIInstance().GetCompanyName(userTransfer.ClientIDTo.ToStr(), _sessionUser.SessionID);
            }

            try
            {
                //Approve the old transfer pending
                userTransferOperator.UpdateUserTransfer(new UserTransferRequest()
                {
                    UserID = input.ID.ToGuid(),
                    TransferResult = UserTransferResult.Approve
                }, _sessionUser);
            }
            catch (VrentApplicationException ve)
            {
                //Just log
                LogInfor.WriteInfo(ve.ErrorCode.ToStr(), ve.InnerException.ToStr(), _sessionUser.ID);
            }

            return input;
        }

        /// <summary>
        /// End user/Corporate user to corportate user (VRent Manager Rejected)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private UserExtension _toCorporateByVMReject(UserExtension input)
        {
            try
            {
                var userTransferOperator = ServiceImpInstanceFactory.CreateUserTransferOperatorInstance(_sessionUser);
                //Reject the old transfer pending
                userTransferOperator.UpdateUserTransfer(new UserTransferRequest()
                {
                    UserID = input.ID.ToGuid(),
                    TransferResult = UserTransferResult.Reject
                }, _sessionUser);
            }
            catch (VrentApplicationException ve)
            {
                //Just log
                LogInfor.WriteInfo(ve.ErrorCode.ToStr(), ve.InnerException.ToStr(), _sessionUser.ID);
            }
            return input;
        }

        /// <summary>
        /// End user/Corporate user to corportate user (service center/Operation Manager)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private UserExtension _updateNewCorprtateBySCOrSCL(UserExtension input)
        {
            if (_originalUser.Clients[0].ID != input.ClientID)
            {
                //Company pending
                var userTransferOperator = ServiceImpInstanceFactory.CreateUserTransferOperatorInstance(_sessionUser);

                try
                {
                    //Reject the old transfer pending
                    userTransferOperator.UpdateUserTransfer(new UserTransferRequest()
                    {
                        UserID = input.ID.ToGuid(),
                        TransferResult = UserTransferResult.Reject
                    }, _sessionUser);
                }
                catch (VrentApplicationException ve)
                {
                    //Just log
                    LogInfor.WriteInfo(ve.ErrorCode.ToStr(), ve.InnerException.ToStr(), _sessionUser.ID);
                }

                //Add the new transfer pending
                userTransferOperator.AddUserTransfer(new UserTransferRequest()
                {
                    CreatedBy = _sessionUser.ID.ToGuidNull(),
                    CreatedOn = DateTime.Now,
                    TransferResult = UserTransferResult.Pending,
                    UserID = input.ID.ToGuid(),
                    FirstName = _originalUser.Name,
                    LastName = _originalUser.VName,
                    ClientIDFrom = _originalUser.Clients[0].ID.ToGuidNull(),
                    ClientIDTo = input.ClientID.ToGuidNull(),
                    State = UserTransferState.Active,
                    Mail = _originalUser.Mail
                }, _sessionUser);

                //Reset company name according the first company id
                input.Company = _originalUser.Clients[0].Name;
            }

            return input;
        }

        /// <summary>
        /// Corporate user to end user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private UserExtension _corporate2EndBySCOrSCL(UserExtension input)
        {
            var userTransferOperator = ServiceImpInstanceFactory.CreateUserTransferOperatorInstance(_sessionUser);
            try
            {
                //Reject the old transfer pending
                userTransferOperator.UpdateUserTransfer(new UserTransferRequest()
                {
                    UserID = input.ID.ToGuid(),
                    TransferResult = UserTransferResult.Reject
                }, _sessionUser);
            }
            catch (VrentApplicationException ve)
            {
                //Just log
                LogInfor.WriteInfo(ve.ErrorCode.ToStr(), ve.InnerException.ToStr(), _sessionUser.ID);
            }

            //set the end user client
            input.ClientID = input.ClientID;
            //Reset company name according the first company id
            input.Company = KemasAccessWrapper.CreateKemasExtensionAPIInstance().GetCompanyName(input.ClientID, _sessionUser.SessionID);

            return input;
        }
    }


    internal class UserTransferStrategyInnerEntity
    {
        /// <summary>
        /// SC, SCL, VM, ADMIN
        /// </summary>
        public string RoleKey { get; set; }
        /// <summary>
        /// Current navigation in portal 
        /// </summary>
        public UserMgmtNavigation.Operation Navigation { get; set; }
        public Func<UserExtension, UserExtension> ProxyMethod { get; set; }

        public UserTransferStrategyInnerEntity()
        {
            //Default value
            Navigation = UserMgmtNavigation.Operation.UpdateUser;
        }
    }

    internal class UserTransferStrategyEntity
    {
        public bool CurrentUserIsEndUser { get; set; }
        public UserTransferStrategyInnerEntity InnerEntity { get; set; }
    }

    internal class UserTransferStrategyHeaderEntity
    {
        public bool InputUserIsEndUser { get; set; }
        public UserTransferStrategyEntity InnerEntity { get; set; }
    }
}
