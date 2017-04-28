using CF.VRent.Common;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.UserRole.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CF.VRent.UserRole
{
    /// <summary>
    /// Init static recource
    /// </summary>
    public static class UserRoleContext
    {
        public static XmlDocument UserRoleDocument { get; set; }

        static UserRoleContext()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\UserRole.xml"))
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\UserRole.xml";
                if (UserRoleDocument == null)
                {
                    UserRoleDocument = new XmlDocument();
                    UserRoleDocument.Load(filePath);
                }
            }
            else
            {
                var role = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                            "<UserRole>" +
                              "<Role key=\"BU\" enabled=\"true\">" +
                                "<!--Set the \"Booking User Role Name\" here-->" +
                                "<KemasRole name=\"Employee\" id=\"\" enabled=\"true\" isDefault=\"true\"></KemasRole>" +
                              "</Role>" +
                              "<Role key=\"SC\" enabled=\"true\">" +
                                "<!--Set the \"Service Center User Role Name\" here-->" +
                                "<KemasRole name=\"Customer Service Agent\" id=\"\" enabled=\"true\" isDefault=\"true\"></KemasRole>" +
                              "</Role>" +
                              "<Role key=\"SCL\" enabled=\"true\">" +
                                "<!--Set the \"Operation Manager User Role Name\" here-->" +
                                "<KemasRole name=\"Operation Manager\" id=\"\" enabled=\"true\" isDefault=\"true\"></KemasRole>" +
                              "</Role>" +
                              "<Role key=\"VM\" enabled=\"true\">" +
                                "<!--Set the \"VRent Manager User Role Name\" here-->" +
                                "<KemasRole name=\"VRent Manager\" id=\"\" enabled=\"true\" isDefault=\"true\"></KemasRole>" +
                              "</Role>" +
                              "<Role key=\"ADMIN\" enabled=\"true\">" +
                                "<!--Set the \"Administration User Role Name\" here-->" +
                                "<KemasRole name=\"Administration\" id=\"\" enabled=\"true\" isDefault=\"true\"></KemasRole>" +
                              "</Role>" +
                            "</UserRole>";
                if (UserRoleDocument == null)
                {
                    UserRoleDocument = new XmlDocument();
                    UserRoleDocument.LoadXml(role);
                }
            }
        }

        /// <summary>
        /// Create role manager instance
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IUserRoleManager CreateRoleManager(string param = "")
        {
            return UnityHelper.GetUnityContainer(UnityHelper.UserMgmtContainer).Resolve<IUserRoleManager>();
        }
    }
}
