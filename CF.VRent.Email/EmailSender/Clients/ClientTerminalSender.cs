using CF.VRent.Common;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email.EmailSender.Clients
{
     public class ClientTerminalSender:IClientTerminalSender
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
            get { return new EmailType[] { 
                EmailType.Portal_Client_Terminal
            }; }
        }

        public ClientTerminalSender()
        {
            if (Validation == null)
            {
                Validation = new EmailSenderValidation();
            }
        }

        public void Send(Entity.EmailParameterEntity parameters, params string[] to)
        {
            if (this.Validation.Validate(this.onSendEvent))
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
