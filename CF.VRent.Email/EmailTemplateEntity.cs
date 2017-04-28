using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    /// <summary>
    /// Email Relation Entity
    /// </summary>
    public class EmailTemplateEntity
    {
        public string TemplateRootPath { get; set; }

        public EmailType[] Types { get; set; }

        public string TemplateName { get; set; }

        public string MailTitle { get; set; }
    }
}
