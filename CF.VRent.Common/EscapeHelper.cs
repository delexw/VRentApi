using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common
{
    public class EscapeHelper
    {
        public static string Escape(string orgString)
        {
           return  System.Web.HttpUtility.HtmlEncode(orgString);
        }
    }
}
