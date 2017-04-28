using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CF.VRent.Email.EmailSender
{
    public class EmailSenderValidation : IEmailSenderValidation
    {
        public virtual bool Validate(Action<EmailParameterEntity, EmailType, string[]> evt)
        {
            if (evt == null)
            {
                StackTrace st = new StackTrace(true);
                LogInfor.EmailLogWriter.WriteError(MessageCode.CVCE000011.ToStr(), String.Format(MessageCode.CVCE000011.GetDescription(), st.GetFrame(1).GetMethod().Name), "Email");
                return false;
            }
            return true;
        }
    }
}
