using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common
{
    public class LanguageHelper
    {
        /// <summary>
        /// Check if it is the exist language
        /// </summary>
        /// <param name="value">input language</param>
        /// <returns></returns>
        public static bool CheckExistLang(string value)
        {
            foreach (int myCode in Enum.GetValues(typeof(Lang)))
            {
                string strName = Enum.GetName(typeof(Lang), myCode);
                if (value == strName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
