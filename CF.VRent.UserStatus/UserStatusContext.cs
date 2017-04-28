using CF.VRent.Common;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.UserStatus.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CF.VRent.UserStatus
{
    /// <summary>
    /// Init static recource
    /// </summary>
    public static class UserStatusContext
    {
        public static XmlDocument UserStatusDocument { get; set; }

        static UserStatusContext()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\UserStatus.xml"))
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\UserStatus.xml";
                if (UserStatusDocument == null)
                {
                    UserStatusDocument = new XmlDocument();
                    UserStatusDocument.Load(filePath);
                }
            }
            else
            {
                var status = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                                "<Root>" +
                                  "<Status name=\"User initialization Pending - Pwd\" value=\"0\" flag=\"1\"></Status>" +
                                  "<Status name=\"User initialization Pending - User info\" value=\"0\" flag=\"2\"></Status>" +
                                  "<Status name=\"Pending\" value=\"0\" flag=\"3\"></Status>" +

                                  "<Status name=\"Booking_Approval\" value=\"0\" flag=\"4\"></Status>" +
                                  "<Status name=\"Booking_Rejected\" value=\"0\" flag=\"5\"></Status>" +

                                  "<Status name=\"Company_Join_Pending\" value=\"0\" flag=\"6\"></Status>" +
                                  "<Status name=\"Company_Join_Approve\" value=\"0\" flag=\"7\"></Status>" +
                                  "<Status name=\"Company_Join_Reject\" value=\"0\" flag=\"8\"></Status>" +

                                  "<Status name=\"Booking_Deactived\" value=\"0\" flag=\"9\"></Status>" +
                                  "<Status name=\"DUB_Booking_Deactived\" value=\"0\" flag=\"A\"></Status>" +

                                  "<Status name=\"KEMAS_Block\" value=\"0\" flag=\"B\"></Status>" +
                                  "<Status name=\"KEMAS_Deactive\" value=\"0\" flag=\"C\"></Status>" +

                                  "<Status name=\"Basic_Approval\" value=\"0\" flag=\"D\"></Status>" +
                                  "<Status name=\"Basic_Rejected\" value=\"0\" flag=\"F\"></Status>" +

                                  "<Status name=\"Booking_Reactived\" value=\"0\" flag=\"E\"></Status>" +
                                "</Root>";
                if (UserStatusDocument == null)
                {
                    UserStatusDocument = new XmlDocument();
                    UserStatusDocument.LoadXml(status);
                }
            }
        }

        /// <summary>
        /// Create company manager instance
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IUserStatusManager CreateStatusManager(string binaryPattern = "")
        {
            return UnityHelper.GetUnityContainer(UnityHelper.UserMgmtContainer).Resolve<IUserStatusManager>(new ParameterOverride("binaryPattern", binaryPattern));
        }
    }
}
