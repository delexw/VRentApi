using CF.VRent.Email;
using CF.VRent.Email.EmailSender.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CF.VRent.Common;
using System.IO;
using System.Threading.Tasks;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BigFileService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select BigFileService.svc or BigFileService.svc.cs at the Solution Explorer and start debugging.
    public class BigFileService : IBigFileService
    {
        public void SendEmailWithAttachments(EmailProxyParameter paras)
        {
            EmailManager manager = EmailContext.CreateManager(paras.EmailType.ToEnum<EmailType>());
            manager.EmailTempParamsValue = paras.ContentParameter;

            using (MemoryStream copy = new MemoryStream())
            {
                paras.FileStream.CopyTo(copy);
                paras.FileStream.Close();

                manager.OnBeforeSend += r =>
                {
                    //this stream will be disposed in EmailHelper
                    paras.Attachment.FileStream = new MemoryStream();

                    copy.WriteTo(paras.Attachment.FileStream);

                    paras.Attachment.FileStream.Position = 0;

                    r.Attachments(paras.Attachment);

                };

                //Add emails in a specified group to list
                if (!String.IsNullOrWhiteSpace(paras.GroupType))
                {
                    paras.EmailAddresses.AddRange(manager.EmailAddressesGroups[paras.GroupType].GetAllAddresses());
                }

                manager.SendEmail(paras.EmailAddresses.ToArray());

                //Test user
                if (manager.CurrentType.IsIncludeTester())
                {
                    manager.SendEmail(manager.EmailAddressesGroups[EmailConstants.TestUserKey].GetAllAddresses());
                }
            }
        }
    }
}
