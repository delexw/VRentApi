using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.UPSDK;
using CF.VRent.Log;
using CF.VRent.Common.Entities;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public class PaymentMessageStreamSerializer : IPaymentMessageStreamSerializer
    {
        private UnionPay _message;
        public UnionPay Message
        {
            get { return _message; }
        }

        private string _userId;
        public string UserID
        {
            get { return _userId; }
        }

        private string _uniqueValue;
        public string UniqueValue
        {
            get { return _uniqueValue; }
        }

        public virtual bool Serialize(Stream message)
        {
            try
            {
                //Get response
                StreamReader sr = new StreamReader(message);
                string resKeyValue = sr.ReadToEnd();
                sr.Dispose();

                //resKeyValue is a value that pattern is kay/value
                var resDic = resKeyValue.ToDictionary();

                var resObj = resDic.ToEntity<UnionPay, string, string>();
                _message = resObj;

                var reservedMessageJson = resObj.ReqReserved;

                Dictionary<string, string> reservedMessage = null;
                try
                {
                    reservedMessage = reservedMessageJson.JsonDeserialize<Dictionary<string, string>>();
                }
                catch
                {
                    //Do nothing
                }

                if (reservedMessage != null)
                {
                    resObj.UniqueID = reservedMessageJson;
                    this._userId = reservedMessage.Keys.Contains(UnionPayUtils.ReservedMessageKey2) ? reservedMessage[UnionPayUtils.ReservedMessageKey2] : "No UserID";
                    this._uniqueValue = reservedMessage.Keys.Contains(UnionPayUtils.ReservedMessageKey1) ? reservedMessage[UnionPayUtils.ReservedMessageKey1] : "";
                }
                else
                {
                    _userId = "No UserID";
                    _uniqueValue = "";
                }
            }
            catch (Exception ex)
            {
                LogInfor.WriteError(MessageCode.CVB000021.ToStr(), ex.ToString(), "Listen Union Pay Request");
                return false;
            }

            return true;
        }
    }
}
