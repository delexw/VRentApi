using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace CF.VRent.Email.EmailSender.UserMgmt
{
    public class PortalUserCreatedSender : IPortalUserCreatedSender
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

        public EmailType[] EmailTypes
        {
            get
            {
                return new EmailType[] { 
                EmailType.Portal_UserMgmt_CorporateUserCreation
            };
            }
        }

        public PortalUserCreatedSender()
        {
            if (Validation == null)
            {
                Validation = new EmailSenderValidation();
            }
        }

        public void Send(Entity.EmailParameterEntity parameters, params string[] to)
        {
            if (this.Validation.Validate(onSendEvent))
            {
                foreach (EmailType type in this.EmailTypes)
                {
                    LogInfor.EmailLogWriter.WriteInfo(this.GetType().Name + ":" + type.ToStr() + " in", to.ObjectToJson(), "Email");
                    var callBack = onSendEvent.BeginInvoke(parameters, type, to, null, null);
                    onSendEvent.EndInvoke(callBack);
                    LogInfor.EmailLogWriter.WriteInfo(this.GetType().Name + ":" + type.ToStr() + " out", to.ObjectToJson(), "Email");
                }
            }
        }

        public event Action<Entity.EmailParameterEntity, EmailType, string[]> onSendEvent;
    }
}
