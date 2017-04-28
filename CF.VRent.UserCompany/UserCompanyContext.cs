using CF.VRent.Common;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.UserCompany.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CF.VRent.UserCompany
{
    /// <summary>
    /// Init static recource
    /// </summary>
    public static class UserCompanyContext
    {
        public static XmlDocument UserCompanyDocument { get; set; }

        static UserCompanyContext()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\UserCompany.xml"))
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\UserCompany.xml";
                if (UserCompanyDocument == null)
                {
                    UserCompanyDocument = new XmlDocument();
                    UserCompanyDocument.Load(filePath);
                }
            }
            else
            {
                var comanyXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                    "<UserCompany>" +
                    "<Company key=\"EUC\" enabled=\"true\">" +
                    "<!--Set End User Client Name-->" +
                    "<KemasCompany name=\"End User\" id=\"\" enabled=\"true\" isDefault=\"true\"></KemasCompany>" +
                    "</Company>" +
                    "</UserCompany>";
                if (UserCompanyDocument == null)
                {
                    UserCompanyDocument = new XmlDocument();
                    UserCompanyDocument.LoadXml(comanyXml);
                }
            }
        }

        /// <summary>
        /// Create company manager instance
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IUserCompanyManager CreateCompanyManager(string param = "")
        {
            return UnityHelper.GetUnityContainer(UnityHelper.UserMgmtContainer).Resolve<IUserCompanyManager>();
        }
    }
}
