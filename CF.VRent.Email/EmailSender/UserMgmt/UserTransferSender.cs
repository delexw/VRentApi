using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace CF.VRent.Email.EmailSender.UserMgmt
{
    public class UserTransferSender:IUserTransferSender
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

        public Dictionary<int, EmailType> TransferMap
        {
            get;
            private set;
        }

        public EmailType[] EmailTypes
        {
            get { return new EmailType[] {
                EmailType.Portal_UserMgmt_UserTransfer_Corporate2End,
                EmailType.Portal_UserMgmt_UserTransfer_End2Corporate
            }; }
        }

        private Dictionary<int, string[]> _inputUserTransfer;

        public UserTransferSender(Dictionary<int, string[]> inputUserTransfer)
        {
            if (Validation == null)
            {
                Validation = new EmailSenderValidation();
            }

            _inputUserTransfer = inputUserTransfer;
            TransferMap = new Dictionary<int, EmailType>();

            TransferMap.Add(1, EmailType.Portal_UserMgmt_UserTransfer_End2Corporate);
            TransferMap.Add(0, EmailType.Portal_UserMgmt_UserTransfer_Corporate2End);
        }

        public void Send(EmailParameterEntity parameters, params string[] to)
        {
            if (this.Validation.Validate(onSendEvent))
            {
                foreach (int userTransfer in _inputUserTransfer.Keys)
                {
                    var type = this.TransferMap[userTransfer];

                    LogInfor.EmailLogWriter.WriteInfo(this.GetType().Name + ":" + type.ToStr() + " in", to.ObjectToJson(), "Email");
                    var callBack = onSendEvent.BeginInvoke(parameters, type, _inputUserTransfer[userTransfer], null, null);
                    onSendEvent.EndInvoke(callBack);
                    LogInfor.EmailLogWriter.WriteInfo(this.GetType().Name + ":" + type.ToStr() + " out", to.ObjectToJson(), "Email");
                }
            }
        }

        public event Action<Entity.EmailParameterEntity, EmailType, string[]> onSendEvent;
    }
}
