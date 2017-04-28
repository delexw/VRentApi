using CF.VRent.Common.Entities;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using CF.VRent.Common;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Email.EmailSender;
using System.Threading.Tasks;

namespace CF.VRent.Email
{
    /// <summary>
    /// Manage the email configuration
    /// </summary>
    public class EmailManager
    {
        public EmailTemplateEntityCollection EmailTemplateEntities { get; private set; }

        public EmailAddressGroup EmailAddressesGroups { get; private set; }

        public EmailHostEntity EmailHost { get; private set; }

        public EmailParameterEntity EmailTempParamsValue { get; set; }

        public EmailType CurrentType { get; set; }

        public string ManagerName { get; private set; }

        public string TemplateRootPath { get; private set; }

        public IEmailAddressValidation EmailAddressValidation { get; private set; }

        public delegate void BeforeSend(EmailHelper helper);

        /// <summary>
        /// Event invoked before send a email
        /// </summary>
        public event BeforeSend OnBeforeSend; 


        public EmailManager(string sectionName)
            : this(sectionName, new EmailType())
        {
        }

        public EmailManager(string sectionName, EmailType currentType)
        {
            //Validation
            this.EmailAddressValidation = new EmailAddressValidation();
            this.CurrentType = currentType;
            _init(sectionName);
        }

        /// <summary>
        /// Init the relationship between template and email type
        /// </summary>
        private void _init(string sectionName)
        {
            EmailConfiguration config = ConfigurationManager.GetSection(sectionName) as EmailConfiguration;

            this.ManagerName = config.Name;

            this.TemplateRootPath = config.TemplateRootPath;

            List<EmailTemplateEntity> templates = new List<EmailTemplateEntity>();

            foreach (EmailTempleateConfiguration s in config.Templates)
            {
                var types = s.TypeName;
                List<EmailType> emailTypes = new List<EmailType>();
                foreach (string type in types.Split(','))
                {
                    var etype = new EmailType();
                    if (Enum.TryParse<EmailType>(type, out etype))
                    {
                        emailTypes.Add(etype);
                    }
                }

                templates.Add(new EmailTemplateEntity() { 
                    TemplateName = s.FilePath,
                    TemplateRootPath = config.TemplateRootPath,
                    Types = emailTypes.ToArray(),
                    MailTitle = s.MailTitle
                });
            }

            //templates
            this.EmailTemplateEntities = new EmailTemplateEntityCollection(templates, sectionName);

            List<EmailAddressEntityCollection> emailColl = new List<EmailAddressEntityCollection>();
            //email groups
            foreach (EmailAddressGroupConfiguration agc in config.AddressGroups)
            {
                List<EmailAddressEntity> emailEntity = new List<EmailAddressEntity>();
                foreach (EmailAddressConfiguration ac in agc.Addresses)
                {
                    if (!String.IsNullOrWhiteSpace(ac.Address) && this.EmailAddressValidation.Validate(ac.Address))
                    {
                        emailEntity.Add(new EmailAddressEntity(ac.Address));
                    }
                    else
                    {
                        LogInfor.EmailLogWriter.WriteError(MessageCode.CVCE000008.ToStr(), String.Format(MessageCode.CVCE000008.GetDescription(), ac.Address), "System");
                    }
                }
                emailColl.Add(new EmailAddressEntityCollection(emailEntity, agc.Name));
            }

            //Addresses
            this.EmailAddressesGroups = new EmailAddressGroup(emailColl);

            //Host
            this.EmailHost = new EmailHostEntity(config.Host.Address, config.Host.From, config.Host.Password, config.Host.Async);

        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailAddress">the receiver emails</param>
        /// <param name="addressGroup">SC/VM/Test or something else would be added in the further</param>
        public virtual void SendEmail(string[] emailAddress)
        {
            List<string> cc = new List<string>();

            //CC service center agent
            if (this.CurrentType.IsIncludeSC())
            {
                cc.AddRange(this.EmailAddressesGroups[EmailConstants.ServiceCenterKey].GetAllAddresses());
            }

            //CC operations manager
            if (this.CurrentType.IsIncludeSCL())
            {
                cc.AddRange(this.EmailAddressesGroups[EmailConstants.OperationManagerKey].GetAllAddresses());
            }

            //CC administration
            if (this.CurrentType.IsIncludeADMIN())
            {
                cc.AddRange(this.EmailAddressesGroups[EmailConstants.AdministrationKey].GetAllAddresses());
            }

            this.SendEmail(emailAddress, cc.ToArray());
        }

        /// <summary>
        /// Send email with cc
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="ccAddress"></param>
        public virtual void SendEmail(string[] emailAddress, string[] ccAddress)
        {
            if (emailAddress.Length > 0)
            {
                //Address is valid
                if (this.EmailAddressValidation.Validate(this.EmailHost.From))
                {
                    List<string> validAddresses = new List<string>();

                    //Validate
                    foreach (string addr in emailAddress)
                    {
                        if (this.EmailAddressValidation.Validate(addr))
                        {
                            validAddresses.Add(addr);
                        }
                        else
                        {
                            LogInfor.EmailLogWriter.WriteError(MessageCode.CVCE000008.ToStr(), String.Format(MessageCode.CVCE000008.GetDescription(), addr), "System");
                        }
                    }

                    var title = this.EmailTemplateEntities.GetMailTitle(CurrentType);

                    EmailHelper email = new EmailHelper(this.EmailHost.From, String.Empty,
                    title, this.EmailHost.Password, this.EmailHost.Address, ccAddress, validAddresses.ToArray());

                    var templateFileNames = this.EmailTemplateEntities[CurrentType];
                    foreach (string templateFileName in templateFileNames)
                    {
                        if (!String.IsNullOrEmpty(templateFileName))
                        {
                            //Template
                            email.GetEmailTemplate(templateFileName,
                                "//font[@face='Open Sans,Arial,sans-serif'][@color='#333333']",
                                this.EmailTempParamsValue, (Match m) =>
                                {
                                    #region define mime contend id in emmail template
                                    switch (m.Value)
                                    {
                                        case "{$logo}":
                                            return "cid:logo";
                                        case "{$top}":
                                            return "cid:top";
                                        case "{$bottom}":
                                            return "cid:bottom";
                                        default:
                                            return "";
                                    }
                                    #endregion

                                }, (string html) =>
                                {
                                    #region embeded image in email template
                                    List<AlternateView> views = new List<AlternateView>();

                                    AlternateView viewlogo = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);
                                    LinkedResource logo = new LinkedResource(String.Join("", this.TemplateRootPath, "images\\logo.png"));
                                    logo.ContentId = "logo";
                                    viewlogo.LinkedResources.Add(logo);

                                    AlternateView viewtop = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);
                                    LinkedResource top = new LinkedResource(String.Join("", this.TemplateRootPath, "images\\top.png"));
                                    top.ContentId = "top";
                                    viewtop.LinkedResources.Add(top);

                                    AlternateView viewbottom = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);
                                    LinkedResource bottom = new LinkedResource(String.Join("", this.TemplateRootPath, "images\\bottom.png"));
                                    bottom.ContentId = "bottom";
                                    viewbottom.LinkedResources.Add(bottom);

                                    views.Add(viewlogo);
                                    views.Add(viewtop);
                                    views.Add(viewbottom);

                                    return views.ToArray();
                                    #endregion
                                });
                            email.sendComplete += email_sendComplete;

                            if (OnBeforeSend != null)
                            {
                                OnBeforeSend(email);
                            }

                            Task.Factory.StartNew(() => {
                                email.Send(this.EmailHost.Async);
                            }, TaskCreationOptions.PreferFairness);
                        }
                    }
                }
                else
                {
                    LogInfor.EmailLogWriter.WriteError(MessageCode.CVCE000008.ToStr(), String.Format(MessageCode.CVCE000008.GetDescription(), this.EmailHost.From), "System");
                }
            }
            else
            {
                LogInfor.EmailLogWriter.WriteError(MessageCode.CVCE000001.ToStr(), MessageCode.CVCE000001.GetDescription(), "System");
            }
        }

        void email_sendComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e != null && e.Error != null)
            {
                LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(), e.Error.ToStr(), "System");
            }
        }
    }
}
