using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Text;

namespace CF.VRent.WCFExtension.MessageFormatter
{
    public class SoupMessageSerilizer : BodyWriter
    {
        private byte[] _content;

        public SoupMessageSerilizer(byte[] content):base(true)
        {
            _content = content;
        }

        protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Binary");
            writer.WriteBase64(_content, 0, _content.Length);
            writer.WriteEndElement();
        }
    }
}
