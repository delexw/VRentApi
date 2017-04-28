using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    /// <summary>
    /// Email Template Collection
    /// </summary>
    public class EmailTemplateEntityCollection:IEnumerable<EmailTemplateEntity>
    {
        private EmailTemplateEntity[] _emailEntity;

        private string _sectionName;

        public EmailTemplateEntityCollection()
            : this(new List<EmailTemplateEntity>(), "VRentEmail")
        { }

        public EmailTemplateEntityCollection(IEnumerable<EmailTemplateEntity> templates,string sectionName)
        {
            _sectionName = sectionName;
            _emailEntity = templates.ToArray();
        }

        public EmailType[] this[string templateName]
        {
            get
            {
                var obj = _emailEntity.FirstOrDefault(r => r.TemplateName.Trim() == templateName.Trim());
                if (obj != null)
                {
                    return obj.Types;
                }
                return null;
            }
        }

        public EmailType[] this[int index]
        {
            get
            {
                return _emailEntity[index].Types;
            }
        }

        public string[] this[EmailType type]
        {
            get
            {
                var obj = _emailEntity.Where(r => r.Types.Contains(type));
                List<string> templates = new List<string>();
                foreach (EmailTemplateEntity s in obj)
                {
                    if (!String.IsNullOrWhiteSpace(s.TemplateName) && !String.IsNullOrWhiteSpace(s.TemplateRootPath))
                    {
                        templates.Add(String.Format("{0}{1}", s.TemplateRootPath, s.TemplateName));
                    }
                }
                return templates.ToArray();
            }
        }

        public string GetMailTitle(EmailType type)
        {
            var entity = this._emailEntity.FirstOrDefault(r => r.Types.Contains(type));

            if (entity != null)
            {
                return entity.MailTitle;
            }
            return "";
        }
     
        public IEnumerator<EmailTemplateEntity> GetEnumerator()
        {
            return _emailEntity.ToList().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _emailEntity.GetEnumerator();
        }
    }
}
