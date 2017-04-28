using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.EntityFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.UserStatus.Interfaces;
using CF.VRent.UserRole;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    /// <summary>
    /// User status flow strategy in portal 
    /// </summary>
    public class UserStatusFlowStrategy:IUserStatusFlowStrategy
    {
        private List<UserStatusStrategyHeaderEntity> _strategy;
        private ProxyUserSetting _sessionUser;

        public UserStatusFlowStrategy(ProxyUserSetting sessionUser)
        {
            _sessionUser = sessionUser;
            _strategy = new List<UserStatusStrategyHeaderEntity>();
            //Strategy group
            #region Service Center
            _strategy.Add(new UserStatusStrategyHeaderEntity()
                {
                    RoleKey = UserRoleConstants.ServiceCenterKey,
                    InnerEntity =
                    new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "4",
                        ProxyMethod = (user, statusManager) => { return this._approveLicense(user, ref statusManager); }
                    }
                });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "5",
                        ProxyMethod = (user, statusManager) => { return this._rejectLicense(user, ref statusManager); }
                    }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "D",
                        ProxyMethod = (user, statusManager) => { return this._preActiveUser(user, ref statusManager); }
                    }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "F",
                        ProxyMethod = (user, statusManager) => { return this._preDeactiveUser(user, ref statusManager); }
                    }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "9",
                        ProxyMethod = (user, statusManager) => { return this._deactiveUser(user, ref statusManager); }
                    }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "A",
                    ProxyMethod = (user, statusManager) => { return this._deactiveDUBUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "E",
                        ProxyMethod = (user, statusManager) => { return this._reactiveUser(user, ref statusManager); }
                    }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "B",
                        ProxyMethod = (user, statusManager) => { return this._blockUser(user, ref statusManager); }
                    }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "C",
                        ProxyMethod = (user, statusManager) => { return this._disableUser(user, ref statusManager); }
                    }
            });
            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "6",
                    ProxyMethod = (user, statusManager) => { return this._pendingCompanyUser(user, ref statusManager); }
                }
            });
            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.ServiceCenterKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = null,
                    ProxyMethod = (user, statusManager) => { return this._resetUserStatus(user, ref statusManager); }
                }
            });
            #endregion

            #region Operation Manager
            _strategy.Add(new UserStatusStrategyHeaderEntity()
                {
                    RoleKey = UserRoleConstants.OperationManagerKey,
                    InnerEntity =
                    new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "4",
                        ProxyMethod = (user, statusManager) => { return this._approveLicense(user, ref statusManager); }
                    }
                });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "5",
                    ProxyMethod = (user, statusManager) => { return this._rejectLicense(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "D",
                    ProxyMethod = (user, statusManager) => { return this._preActiveUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "F",
                    ProxyMethod = (user, statusManager) => { return this._preDeactiveUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "9",
                    ProxyMethod = (user, statusManager) => { return this._deactiveUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "A",
                    ProxyMethod = (user, statusManager) => { return this._deactiveDUBUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "E",
                    ProxyMethod = (user, statusManager) => { return this._reactiveUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "B",
                    ProxyMethod = (user, statusManager) => { return this._blockUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "C",
                    ProxyMethod = (user, statusManager) => { return this._disableUser(user, ref statusManager); }
                }
            });
            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "6",
                    ProxyMethod = (user, statusManager) => { return this._pendingCompanyUser(user, ref statusManager); }
                }
            });
            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.OperationManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = null,
                    ProxyMethod = (user, statusManager) => { return this._resetUserStatus(user, ref statusManager); }
                }
            });
            #endregion

            #region Administrator
            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity =
                new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "4",
                    ProxyMethod = (user, statusManager) => { return this._approveLicense(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "5",
                    ProxyMethod = (user, statusManager) => { return this._rejectLicense(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "D",
                    ProxyMethod = (user, statusManager) => { return this._preActiveUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "F",
                    ProxyMethod = (user, statusManager) => { return this._preDeactiveUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "9",
                    ProxyMethod = (user, statusManager) => { return this._deactiveUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "A",
                    ProxyMethod = (user, statusManager) => { return this._deactiveDUBUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "E",
                    ProxyMethod = (user, statusManager) => { return this._reactiveUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "B",
                    ProxyMethod = (user, statusManager) => { return this._blockUser(user, ref statusManager); }
                }
            });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "C",
                    ProxyMethod = (user, statusManager) => { return this._disableUser(user, ref statusManager); }
                }
            });
            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = "6",
                    ProxyMethod = (user, statusManager) => { return this._pendingCompanyUser(user, ref statusManager); }
                }
            });
            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.AdministratorKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = null,
                    ProxyMethod = (user, statusManager) => { return this._resetUserStatus(user, ref statusManager); }
                }
            });
            #endregion

            #region VRent Manager
            _strategy.Add(new UserStatusStrategyHeaderEntity()
               {
                   RoleKey = UserRoleConstants.VRentManagerKey,
                   InnerEntity = new UserStatusStrategyEntity()
                       {
                           UserStatusFlag = "7",
                           ProxyMethod = (user, statusManager) => { return this._approveCompanyUser(user, ref statusManager); }
                       }
               });

            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.VRentManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                    {
                        UserStatusFlag = "8",
                        ProxyMethod = (user, statusManager) => { return this._rejectCompanyUser(user, ref statusManager); }
                    }
            });
            _strategy.Add(new UserStatusStrategyHeaderEntity()
            {
                RoleKey = UserRoleConstants.VRentManagerKey,
                InnerEntity = new UserStatusStrategyEntity()
                {
                    UserStatusFlag = null,
                    ProxyMethod = (user, statusManager) => { return this._resetUserStatus(user, ref statusManager); }
                }
            });
            #endregion   
        }
        /// <summary>
        /// User staus flow controller strategy
        /// </summary>
        /// <param name="user"></param>
        /// <param name="currentUserRoleKey"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        public UserExtension Run(UserExtension user, UserRoleEntityCollection currentUserRoleKey, ref IUserStatusManager statusManager)
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
            if (currentUserRoleKey.IsAdministrationUser())
            {
                key = UserRoleConstants.AdministratorKey;
            }

            var strategyItem = _strategy.Find(r => r.RoleKey == key && r.InnerEntity.UserStatusFlag.ToStr() == user.Status.ToStr());

            if (strategyItem != null && strategyItem.InnerEntity!=null && strategyItem.InnerEntity.ProxyMethod != null)
            {
                return strategyItem.InnerEntity.ProxyMethod(user, statusManager);
            }
            return user;
        }


        public UserExtension DisableAllCompanyStatus(UserExtension user, ref IUserStatusManager statusManager)
        {
            return this._disableCompanyStatus(user, ref statusManager);
        }

        /// <summary>
        /// Deactive dub and return status
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        public UserExtension ExternalDeactiveDUB(UserExtension user, ref IUserStatusManager statusManager)
        {
            return this._deactiveDUBUser(user, ref statusManager);
        }

        /// <summary>
        /// Do reject 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _rejectCompanyUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            //Reset status
            statusManager.Status["7"].Value = 0;
            statusManager.Status["8"].Value = 1;
            statusManager.Status["6"].Value = 0;
            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Disabled user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _disableUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            //Disabled
            statusManager.Status["C"].Value = 1;

            statusManager.Status["4"].Value = 0;
            statusManager.Status["E"].Value = 0;

            user.Enabled = 0;

            //Cancel CCB permission
            _cancelCCBPermission(user);

            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Block the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _blockUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            //Block
            statusManager.Status["B"].Value = 1;
            user.Blocked = 1;

            statusManager.Status["4"].Value = 0;
            statusManager.Status["E"].Value = 0;

            //Cancel CCB permission
            _cancelCCBPermission(user);

            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Reactive booking user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _reactiveUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            //Reactive
            statusManager.Status["E"].Value = 1;
            //CCB & DUB
            statusManager.Status["9"].Value = 0;
            //DUB
            statusManager.Status["A"].Value = 0;
            //Booking_Approved
            this._approveLicense(user, ref statusManager);
            //statusManager.Status["4"].Value = 1;
            //Kemas_Blcok
            statusManager.Status["B"].Value = 0;
            user.Blocked = 0;
            //Kemas_Deactive
            statusManager.Status["C"].Value = 0;
            user.Enabled = 1;
            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Deactive booking user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _deactiveUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            //CCB & DUB
            statusManager.Status["9"].Value = 1;
            //Booking_Reactieved
            statusManager.Status["E"].Value = 0;
            //Booking_Approved
            statusManager.Status["4"].Value = 0;

            //Cancel CCB permission
            _cancelCCBPermission(user);

            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Deactive DUB booking user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _deactiveDUBUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            //CCB & DUB
            statusManager.Status["A"].Value = 1;
            //Booking_Reactieved
            statusManager.Status["E"].Value = 0;
            //Booking_Approved
            statusManager.Status["4"].Value = 0;

            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Reject the driver lincense
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private UserExtension _rejectLicense(UserExtension user, ref IUserStatusManager statusManager)
        {
            statusManager.Status["5"].Value = 1;
            statusManager.Status["4"].Value = 0;
            statusManager.Status["3"].Value = 0;
            //pre active = 0
            statusManager.Status["D"].Value = 0;
            //User initialization Pending - User info = 1
            statusManager.Status["2"].Value = 1;
            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Approve the drivar lincence
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private UserExtension _approveLicense(UserExtension user, ref IUserStatusManager statusManager)
        {
            statusManager.Status["4"].Value = 1;
            statusManager.Status["5"].Value = 0;
            statusManager.Status["3"].Value = 0;

            //Basic status
            statusManager.Status["D"].Value = 0;
            statusManager.Status["F"].Value = 0;

            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Approve corperate user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _approveCompanyUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            //Reset status
            statusManager.Status["7"].Value = 1;
            statusManager.Status["8"].Value = 0;
            statusManager.Status["6"].Value = 0;
            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Pre-active user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _preActiveUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            statusManager.Status["D"].Value = 1;
            statusManager.Status["F"].Value = 0;
            statusManager.Status["3"].Value = 0;
            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// pre deactive user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _preDeactiveUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            statusManager.Status["D"].Value = 0;
            statusManager.Status["F"].Value = 1;
            statusManager.Status["3"].Value = 0;
            //User initialization Pending - User info = 1
            statusManager.Status["2"].Value = 1;
            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Reject corperate user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _pendingCompanyUser(UserExtension user, ref IUserStatusManager statusManager)
        {
            //Reset status
            statusManager.Status["7"].Value = 0;
            statusManager.Status["8"].Value = 0;
            statusManager.Status["6"].Value = 1;
            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// All company status set to 0
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _disableCompanyStatus(UserExtension user, ref IUserStatusManager statusManager)
        {
            //Reset status
            statusManager.Status["7"].Value = 0;
            statusManager.Status["8"].Value = 0;
            statusManager.Status["6"].Value = 0;
            return this._resetUserStatus(user, ref statusManager);
        }

        /// <summary>
        /// Update status/statusName/statusEntities property of UserExtension
        /// </summary>
        /// <param name="user"></param>
        /// <param name="statusManager"></param>
        /// <returns></returns>
        private UserExtension _resetUserStatus(UserExtension user, ref IUserStatusManager statusManager)
        {
            new UserFactory().SetUserStatus(user, statusManager.Status);

            return user;
        }

        /// <summary>
        /// Cancel ccb permission
        /// </summary>
        /// <param name="user"></param>
        private void _cancelCCBPermission(UserExtension user)
        {
            var disable = ServiceImpInstanceFactory.CreateDisableCCBAccoutInstance(_sessionUser);
            var typeofJourney = ServiceImpInstanceFactory.CreateTypeofJourneyStrategyInstance();
            if (disable.DisableCCBPermission(user, new UserFactory().CreateEntity(_sessionUser).RoleEntities))
            {
                user.TypeOfJourney = typeofJourney.GetValueFromApiInputValue(user.TypeOfJourney);
            }
        }
    }

    internal class UserStatusStrategyEntity
    {
        public Func<UserExtension, IUserStatusManager, UserExtension> ProxyMethod { get; set; }
        public string UserStatusFlag { get; set; }
    }

    internal class UserStatusStrategyHeaderEntity
    {
        public string RoleKey { get; set; }
        public UserStatusStrategyEntity InnerEntity { get; set; }
    }
}
