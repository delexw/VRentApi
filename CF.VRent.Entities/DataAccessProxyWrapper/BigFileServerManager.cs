using CF.VRent.Entities.BigFileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CF.VRent.Entities.DataAccessProxyWrapper
{
    public class BigFileServerManager : IBigFileService
    {
        /// <summary>
        /// send email with one attachment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SendEmailWithAttachmentsResponse SendEmailWithAttachments(EmailProxyParameter request)
        {
            BigFileServiceClient client = new BigFileServiceClient();

            return DataAccessProxyManager.InnerTryCatchInvoker<SendEmailWithAttachmentsResponse>(() =>
            {
                client.SendEmailWithAttachments(request.Attachment,
                    request.ContentParameter,
                    request.EmailAddresses,
                    request.EmailType,
                    request.GroupType,
                    request.FileStream);
                return new SendEmailWithAttachmentsResponse();
            }, client, MethodInfo.GetCurrentMethod().Name);
        }
    }
}
