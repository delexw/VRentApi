
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.UserCompany.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CF.VRent.Common;

namespace CF.VRent.UserCompany
{
    /// <summary>
    /// User Company Manager 
    /// </summary>
    public class UserCompanyManager : IUserCompanyManager
    {
        private UserComanyEntityCollection _companies;
        public UserComanyEntityCollection Companies
        {
            get { return _companies; }
        }

        [InjectionConstructor]
        public UserCompanyManager()
        {
            _init();
        }

        private void _init()
        {
            //var filePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Entities\UserExt\UserCompany.xml";
            //XmlDocument document = new XmlDocument();
            //document.Load(filePath);
            var roles = UserCompanyContext.UserCompanyDocument.SelectNodes("//UserCompany/Company");

            List<UserCompanyEntity> entities = new List<UserCompanyEntity>();

            foreach (XmlNode node in roles)
            {
                if (entities.FirstOrDefault(r => r.Key.Trim() == node.Attributes["key"].Value.Trim()) != null)
                {
                    throw new Exception("Multiple keys in " + node.Attributes["key"].Value);
                }

                //Associated Kemas roles
                List<KemasCompanyEntity> kemsCompany = new List<KemasCompanyEntity>();
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Element && child.Attributes["enabled"].Value.ToBool())
                    {
                        kemsCompany.Add(new KemasCompanyEntity()
                        {
                            Enable = child.Attributes["enabled"].Value.ToBool(),
                            Name = child.Attributes["name"].Value.ToStr(),
                            ID = child.Attributes["id"].Value.ToStr(),
                            IsDefault = child.Attributes["isDefault"].Value.ToBool(),
                            CompanyCode = child.Attributes["companyCode"].Value.ToStr(),
                            CustomerCode = child.Attributes["customerCode"].Value.ToStr()
                        });
                    }
                }

                if (node.Attributes["enabled"].Value.ToBool())
                {
                    entities.Add(new UserCompanyEntity()
                    {
                        Enable = node.Attributes["enabled"].Value.ToBool(),
                        Key = node.Attributes["key"].Value.ToStr(),
                        KemasCompany = kemsCompany.ToArray()
                    });
                }
            }

            _companies = new UserComanyEntityCollection(entities);
        }
    }
}
