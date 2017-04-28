using CF.VRent.Entities.KEMASWSIF_USERRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace CF.VRent.Entities.EntityFactory
{
    public class UserLicenseFactory
    {
        /// <summary>
        /// From kemas user license to UserExtension
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual UserLicenseExtension CreateEntity(License root)
        {
            var extType = typeof(UserLicenseExtension);

            var newExt = new UserLicenseExtension();
            if (root != null)
            {
                var udType = root.GetType();
                //Set property value
                foreach (PropertyInfo pi in extType.GetProperties())
                {
                    var rootProperty = udType.GetProperty(pi.Name);
                    if (rootProperty != null)
                    {
                        var pV = rootProperty.GetValue(root, null);
                        if (rootProperty.PropertyType == typeof(string))
                        {
                            pV = HttpUtility.HtmlDecode(pV.ToString());
                        }
                        pi.SetValue(newExt, pV, null);
                    }
                }
            }

            return newExt;
        }

        /// <summary>
        /// From UserExtension to kemas user license
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public virtual License CreateEntity(UserLicenseExtension proxy)
        {
            var extType = typeof(License);

            var newLicense = new License();

            if (proxy != null)
            {
                var udType = proxy.GetType();
                //Set property value
                foreach (PropertyInfo pi in extType.GetProperties())
                {
                    var rootProperty = udType.GetProperty(pi.Name);
                    if (rootProperty != null)
                    {
                        pi.SetValue(newLicense, rootProperty.GetValue(proxy, null), null);
                    }
                }
            }

            return newLicense;
        }
    }
}
