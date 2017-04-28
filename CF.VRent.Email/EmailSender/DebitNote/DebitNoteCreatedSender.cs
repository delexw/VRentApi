using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
namespace CF.VRent.Email.EmailSender.DebitNote
{
    public class DebitNoteCreatedSender:IDebitNoteCreatedSender
    {
        public IEmailSenderValidation Validation
        {
            get;
            set;
        }

        public Entity.EmailParameterEntity Parameters
        {
            get;
            private set;
        }

        public EmailType[] EmailTypes
        {
            get
            {
                return new EmailType[] { 
                EmailType.ScheduleJob_DebitNote_Created
            };
            }
        }

        public DebitNoteCreatedSender()
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
