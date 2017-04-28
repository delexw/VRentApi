using CF.VRent.Email.EmailSender.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email.EmailSender
{
    public interface IEmailSenderValidation
    {
        bool Validate(Action<EmailParameterEntity, EmailType, string[]> evt);
    }
}
