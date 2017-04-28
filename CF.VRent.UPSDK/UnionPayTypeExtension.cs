using CF.VRent.UPSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace CF.VRent.UPSDK
{
    public static class UnionPayTypeExtension
    {
        public static bool IsMiddleStatus(this PaymentStatusEnum code)
        {
            var cusEnum = code.GetType().GetField(code.ToString());
            if (cusEnum != null)
            {
                var attrs = cusEnum.GetCustomAttributes(typeof(Attribute), true);
                if (attrs.Length == 1)
                {
                    var t = attrs[0].GetType();
                    var type = t.GetProperty("IsMiddleStatus");
                    if (type != null)
                    {
                        return type.GetValue(attrs[0], null).ToBool();
                    }
                }
            }
            return false;
        }

        public static bool IsFailedStatus(this PaymentStatusEnum code)
        {
            var cusEnum = code.GetType().GetField(code.ToString());
            if (cusEnum != null)
            {
                var attrs = cusEnum.GetCustomAttributes(typeof(Attribute), true);
                if (attrs.Length == 1)
                {
                    var t = attrs[0].GetType();
                    var type = t.GetProperty("IsFailedStatus");
                    if (type != null)
                    {
                        return type.GetValue(attrs[0], null).ToBool();
                    }
                }
            }
            return false;
        }

        public static bool IsSuccessStatus(this PaymentStatusEnum code)
        {
            var cusEnum = code.GetType().GetField(code.ToString());
            if (cusEnum != null)
            {
                var attrs = cusEnum.GetCustomAttributes(typeof(Attribute), true);
                if (attrs.Length == 1)
                {
                    var t = attrs[0].GetType();
                    var type = t.GetProperty("IsSuccessStatus");
                    if (type != null)
                    {
                        return type.GetValue(attrs[0], null).ToBool();
                    }
                }
            }
            return false;
        }

        public static PayOperationEnum GetBelonging(this PaymentStatusEnum status)
        {
            var cusEnum = status.GetType().GetField(status.ToString());
            if (cusEnum != null)
            {
                var attrs = cusEnum.GetCustomAttributes(typeof(Attribute), true);
                if (attrs.Length == 1)
                {
                    var t = attrs[0].GetType();
                    var type = t.GetProperty("BelongTo");
                    if (type != null)
                    {
                        return (PayOperationEnum)(type.GetValue(attrs[0], null));
                    }
                }
            }
            return PayOperationEnum.Nothing;
        }
    }
}
