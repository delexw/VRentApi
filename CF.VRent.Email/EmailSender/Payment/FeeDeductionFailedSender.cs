
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;

using CF.VRent.Log;
using CF.VRent.Email.EmailSender.Entity;

namespace CF.VRent.Email.EmailSender.Payment
{
    /// <summary>
    /// Email sender
    /// </summary>
    public class FeeDeductionFailedSender : IFeeDeductionFailedSender
    {
        /// <summary>
        /// email templates of fee deduction failed 
        /// </summary>
        public EmailType[] EmailTypes
        {
            get
            {
                EmailType[] types = new EmailType[] { 
                    EmailType.Preauthorization_FeeDeduction_Failed
                };

                return types;
            }
        }

        /// <summary>
        /// Send email to end users
        /// </summary>
        /// <param name="to"></param>
        public virtual void Send(EmailParameterEntity parameters,params string[] to)
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

        public FeeDeductionFailedSender()
        {
            if (Validation == null)
            {
                Validation = new EmailSenderValidation();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event Action<EmailParameterEntity, EmailType, string[]> onSendEvent;


        /// <summary>
        /// Validate send event
        /// </summary>
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

    }
}
