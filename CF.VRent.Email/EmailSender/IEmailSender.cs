
using CF.VRent.Email.EmailSender.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email.EmailSender
{
    public interface IEmailSender
    {
        IEmailSenderValidation Validation { get; set; }
        EmailParameterEntity Parameters
        {
            get;
        }
        EmailType[] EmailTypes { get; }
        void Send(EmailParameterEntity parameters, params string[] to);
        event Action<EmailParameterEntity, EmailType, string[]> onSendEvent;
    }
}
