using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using CF.VRent.Common;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace CF.VRent.WCFExtension.MessageFormatter
{
    public class SoupMessageFormatter:IDispatchMessageFormatter
    {
        private IDispatchMessageFormatter _orginalFormatter;
        private OperationDescription _operation;
        private Formatting _jsonFormatting;
        private string _responseFormatting;

        public SoupMessageFormatter(IDispatchMessageFormatter orginalFormatter, OperationDescription operation, string format, string responseForamt)
        {
            _orginalFormatter = orginalFormatter;
            _operation = operation;

            try
            {
                _jsonFormatting = format.ToEnum<Formatting>();
            }
            catch
            {
                _jsonFormatting = Formatting.None;
            }

            _responseFormatting = responseForamt;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            _orginalFormatter.DeserializeRequest(message, parameters);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            byte[] body;
            JsonSerializer serializer = new JsonSerializer();
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
                    {
                        writer.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                        writer.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                        writer.Formatting = _jsonFormatting;
                        serializer.Serialize(writer, result);
                        sw.Flush();
                        body = ms.ToArray();
                    }
                }
            }

            Message replyMessage = Message.CreateMessage(messageVersion, _operation.Messages[1].Action, new SoupMessageSerilizer(body));
            replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
            HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
            respProp.Headers[HttpResponseHeader.ContentType] = "application/json";
            replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);

            if (!String.IsNullOrWhiteSpace(_responseFormatting))
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = _responseFormatting;
            }

            return replyMessage;
        }
    }
}
