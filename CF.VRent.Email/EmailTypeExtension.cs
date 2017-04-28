using CF.VRent.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    public static class EmailTypeExtension
    {

        /// <summary>
        /// Email type enum - check the type header property "IncludeSC" 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIncludeSC(this EmailType type)
        {
            var cusEnum = type.GetType().GetField(type.ToString());
            if (cusEnum != null)
            {
                var attrs = cusEnum.GetCustomAttributes(typeof(EmailTypeHeaderAttribute), true);
                if (attrs.Length == 1)
                {
                    var attr = attrs[0] as EmailTypeHeaderAttribute;
                    return attr.IncludeSC;
                }
            }
            return false;
        }

        /// <summary>
        /// Email type enum - check the type header property "IncludeSCL" 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIncludeSCL(this EmailType type)
        {
            var cusEnum = type.GetType().GetField(type.ToString());
            if (cusEnum != null)
            {
                var attrs = cusEnum.GetCustomAttributes(typeof(EmailTypeHeaderAttribute), true);
                if (attrs.Length == 1)
                {
                    var attr = attrs[0] as EmailTypeHeaderAttribute;
                    return attr.IncludeSCL;
                }
            }
            return false;
        }

        /// <summary>
        /// Email type enum - check the type header property "IncludeADMIN" 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIncludeADMIN(this EmailType type)
        {
            var cusEnum = type.GetType().GetField(type.ToString());
            if (cusEnum != null)
            {
                var attrs = cusEnum.GetCustomAttributes(typeof(EmailTypeHeaderAttribute), true);
                if (attrs.Length == 1)
                {
                    var attr = attrs[0] as EmailTypeHeaderAttribute;
                    return attr.IncludeADMIN;
                }
            }
            return false;
        }

        /// <summary>
        /// mail type enum - check the type header property "IncludeTester" 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIncludeTester(this EmailType type)
        {
            var cusEnum = type.GetType().GetField(type.ToString());
            if (cusEnum != null)
            {
                var attrs = cusEnum.GetCustomAttributes(typeof(EmailTypeHeaderAttribute), true);
                if (attrs.Length == 1)
                {
                    var attr = attrs[0] as EmailTypeHeaderAttribute;
                    return attr.IncludeTester;
                }
            }
            return false;
        }
    }
}
