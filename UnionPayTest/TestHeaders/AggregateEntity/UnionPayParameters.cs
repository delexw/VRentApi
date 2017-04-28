using CF.VRent.UPSDK.Entities;
using CF.VRent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionPayTest.TestHeaders.AggregateEntity
{
    public class UnionPayParameters : IAggregateRoot
    {
        public string QueryId { get; set; }

        private string _orderId;
        public string OrderId
        {
            get
            {
                if (_orderId.ToStr() == "")
                    _orderId = DateTime.Now.ToString("yyyyMMddHHmmss") + (new Random().Next(900) + 100).ToString().Trim();
                return _orderId;
            }
            set
            {
                _orderId = value;
            }
        }

        private string _orderTime;
        public string OrderTime
        {
            get
            {
                if (_orderTime.ToStr() == "")
                    _orderTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                return _orderTime;
            }
            set
            {
                _orderTime = value;
            }
        }

        public UnionPay Response { get; set; }

        private string _upToken;
        public string UpToken
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_upToken))
                {
                    _upToken = "{token=6251640000027165&trId=62000000001&tokenLevel=80&tokenBegin=20150714145238&tokenEnd=20171130235959&tokenType=01}";
                }
                return _upToken;
            }
            set
            {
                _upToken = value;
            }
        }
    }
}
