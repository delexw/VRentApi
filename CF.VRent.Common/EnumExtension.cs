using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common
{
    public static class EnumExtension
    {
        /// <summary>
        /// Get the description value of enum Attribute
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum code)
        {
            var cusEnum = code.GetType().GetField(code.ToString());
            if (cusEnum != null)
            {
                var attrs = cusEnum.GetCustomAttributes(typeof(Attribute), true);
                if (attrs.Length == 1)
                {
                    string format = "";
                    var t = attrs[0].GetType();
                    var type = t.GetProperty("Type");
                    if (type != null)
                    {
                        format = "[{0}]{1}";
                        return String.Format(format, type.GetValue(attrs[0], null).ToString(), t.GetProperty("Description").GetValue(attrs[0], null).ToString());
                    }
                    return t.GetProperty("Description").GetValue(attrs[0], null).ToString();
                }
            }
            return "";
        }

        

        /// <summary>
        /// Get the error message type
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ResultType GetMessageType(this MessageCode code)
        {
            var attrs = code.GetType().GetField(code.ToString()).GetCustomAttributes(typeof(MessageCodeAttribute), true);
            if (attrs.Length == 1)
            {
                return (ResultType)Enum.Parse(typeof(ResultType), attrs[0].GetType().GetProperty("Type").GetValue(attrs[0], null).ToString());
            }
            return ResultType.OTHER;
        }

        /// <summary>
        /// Get enum value
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static int GetValue(this Enum code)
        {
            return Convert.ToInt16(code);
        }
    }
}
