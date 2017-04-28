
using CF.VRent.Common;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Log;
using CF.VRent.UserStatus;
using CF.VRent.UserStatus.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email.EmailSender.UserMgmt
{
    public class UserMgmtSender : IUserMgmtSender
    {
        public IEmailSenderValidation Validation
        {
            get;
            set;
        }

        public EmailParameterEntity Parameters
        {
            get;
            private set;
        }

        private Dictionary<string, string[]> _inputUserStatus;
        private UserStatusEntityCollection _currentStatusCollection;
        private UserStatusEntityCollection _originalStatusCollection;

        public UserMgmtSender(Dictionary<string, string[]> inputUserStatus, string currentUserStatus, UserStatusEntityCollection originalStatus)
        {
            if (Validation == null)
            {
                Validation = new EmailSenderValidation();
            }

            Parameters = new EmailParameterEntity();

            _uStatusMapEmial = new Dictionary<UserStatusEntity, EmailType>();
            //UserStatus map
            IUserStatusManager status = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserStatusManager>(new ParameterOverride("binaryPattern", currentUserStatus));

            _uStatusMapEmial.Add(status.Status["1"], EmailType.Portal_UserMgmt_CorporateUserCreation);

            _uStatusMapEmial.Add(status.Status["7"], EmailType.Portal_UserMgmt_UserTransfer_VM_Approved);

            _uStatusMapEmial.Add(status.Status["8"], EmailType.Portal_UserMgmt_UserTransfer_VM_Reject);

            _uStatusMapEmial.Add(status.Status["4"], EmailType.Portal_UserMgmt_License_SC_Approved);

            _uStatusMapEmial.Add(status.Status["5"], EmailType.Portal_UserMgmt_License_SC_Reject);

            _uStatusMapEmial.Add(status.Status["A"], EmailType.Portal_UserMgmt_UserDeactivation);

            _uStatusMapEmial.Add(status.Status["E"],
                EmailType.Portal_UserMgmt_UserReactivation_FromBookingDeactive |
                EmailType.Portal_UserMgmt_UserReactivation_FromKemasDisabledAndBlock);

            _uStatusMapEmial.Add(status.Status["9"], EmailType.Portal_UserMgmt_UserDeactivation);

            _uStatusMapEmial.Add(status.Status["D"], EmailType.Portal_UserMgmt_License_SC_BasicApproved);

            _uStatusMapEmial.Add(status.Status["F"], EmailType.Portal_UserMgmt_License_SC_BasicReject);

            _currentStatusCollection = status.Status;
            _originalStatusCollection = originalStatus;

            _inputUserStatus = inputUserStatus;
        }

        public EmailType[] EmailTypes
        {
            get
            {
                return new EmailType[]{
                EmailType.Portal_UserMgmt_CorporateUserCreation,
                EmailType.Portal_UserMgmt_UserTransfer_VM_Approved,
                EmailType.Portal_UserMgmt_License_SC_Approved,
                EmailType.Portal_UserMgmt_License_SC_Reject,
                EmailType.Portal_UserMgmt_UserTransfer_VM_Reject,
                EmailType.Portal_UserMgmt_UserReactivation_FromBookingDeactive,
                EmailType.Portal_UserMgmt_UserReactivation_FromKemasDisabledAndBlock,
                EmailType.Portal_UserMgmt_UserDeactivation
            };
            }
        }

        private Dictionary<UserStatusEntity, EmailType> _uStatusMapEmial;
        public Dictionary<UserStatusEntity, EmailType> UStatusMapEmail
        {
            get
            {
                return _uStatusMapEmial;
            }
        }

        public virtual void Send(EmailParameterEntity parameters, params string[] to)
        {
            if (this.Validation.Validate(onSendEvent))
            {
                //Current status is updated successfully
                foreach (string userStatus in _inputUserStatus.Keys)
                {
                    if (_currentStatusCollection[userStatus].Value == 1 && this.UStatusMapEmail.Keys.Contains(_currentStatusCollection[userStatus]))
                    {
                        EmailType type = new EmailType();
                        //Current status is "reactived"
                        if (userStatus == "E" && _originalStatusCollection != null)
                        {
                            //From kemas block or kemas deactive to reactived
                            if (_originalStatusCollection["B"].Value == 1 || _originalStatusCollection["C"].Value == 1)
                            {
                                type = EmailType.Portal_UserMgmt_UserReactivation_FromKemasDisabledAndBlock & this.UStatusMapEmail[_currentStatusCollection[userStatus]];
                            }
                            //From booking deactive or dub booking deactive to reactived
                            if (_originalStatusCollection["9"].Value == 1 || _originalStatusCollection["A"].Value == 1)
                            {
                                type = EmailType.Portal_UserMgmt_UserReactivation_FromBookingDeactive & this.UStatusMapEmail[_currentStatusCollection[userStatus]];
                            }
                        }
                        else
                        {
                            type = this.UStatusMapEmail[_currentStatusCollection[userStatus]];
                        }
                        LogInfor.EmailLogWriter.WriteInfo(this.GetType().Name + ":" + type.ToStr() + " in", to.ObjectToJson(), "Email");
                        var callBack = onSendEvent.BeginInvoke(parameters, type, _inputUserStatus[userStatus], null, null);
                        onSendEvent.EndInvoke(callBack);
                        LogInfor.EmailLogWriter.WriteInfo(this.GetType().Name + ":" + type.ToStr() + " out", to.ObjectToJson(), "Email");
                        
                    }
                }
            }
        }

        public event Action<EmailParameterEntity, EmailType, string[]> onSendEvent;
 
    }
}
