using CF.VRent.Email.EmailSender.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Email
{
    [Serializable]
    [DataContract]
    public class EmailAttachmentEntity:IDisposable
    {
        [DataMember]
        public string MimeType { get; set; }
        [DataMember]
        public string FileName { get; set; }

        public Stream FileStream { get; set; }
        [DataMember]
        public string FilePath { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        [DataMember]
        public int Order { get; set; }

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
