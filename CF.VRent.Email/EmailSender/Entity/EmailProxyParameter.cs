using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Email.EmailSender.Entity
{
    [MessageContract]
    public class EmailProxyParameter:IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        public EmailParameterEntity ContentParameter { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string EmailType { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public List<string> EmailAddresses { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public EmailAttachmentEntity Attachment { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string GroupType { get; set; }

        [MessageBodyMember(Order=1)]
        public Stream FileStream { get; set; }

        public void Dispose()
        {
            if (FileStream != null)
            {
                FileStream.Close();
                FileStream = null;
            }
        }
    }
}
