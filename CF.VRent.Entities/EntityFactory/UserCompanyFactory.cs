using CF.VRent.Common;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
//using CF.VRent.Entities.KEMASWSIF_USERRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace CF.VRent.Entities.EntityFactory
{
    public class UserCompanyFactory
    {
        /// <summary>
        /// From kemas user company to UserCompanyExtenstion
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual UserCompanyExtenstion CreateEntity<TClient>(TClient root)
        {
            var extType = typeof(UserCompanyExtenstion);

            var newExt = new UserCompanyExtenstion();
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

            SplitAddress(newExt);
            SplitBankAccountInfo(newExt);

            return newExt;
        }

        public virtual IEnumerable<UserCompanyExtenstion> CreateEntity<TClient>(TClient[] roots)
        {
            if (roots != null)
            {
                List<UserCompanyExtenstion> extensions = new List<UserCompanyExtenstion>();

                foreach (TClient ud in roots)
                {
                    extensions.Add(this.CreateEntity(ud));
                }


                return extensions;
            }

            return null;
        }


        #region handling Address
        public virtual void SplitAddress(UserCompanyExtenstion ue) 
        {
            if (!string.IsNullOrEmpty(ue.Address))
            {
                string rawAddress = ue.Address.Trim();
                char arrayStart = '[';
                int jsonStart = rawAddress.Trim().IndexOf(arrayStart);

                if (jsonStart < 0)
                {
                    //old format, put old format in the 1st slot
                    ue.RegisteredAddress = rawAddress;
                }
                else
                {
                    while (rawAddress.Contains("&amp;"))
                    {
                        rawAddress = rawAddress.Replace("&amp;", "&");
                    }
                    rawAddress = rawAddress.Replace("&quot;", "\"");

                    string[] addressParts = SerializedHelper.JsonDeserialize<string[]>(rawAddress);
                    ue.RegisteredAddress = addressParts[0];
                    ue.OfficeAddress = addressParts[1];
                }
            }
        }

        public static void CombineAddress(Client configClient, UserCompanyExtenstion ue)
        {
            string[] addressParts = new string[2] 
            { string.IsNullOrEmpty( ue.RegisteredAddress)?null:ue.RegisteredAddress.Trim(),
                string.IsNullOrEmpty( ue.OfficeAddress)?null:ue.OfficeAddress.Trim()  };
            configClient.Address = SerializedHelper.JsonSerialize<string[]>(addressParts);
        }
        #endregion

        #region handling BankAcount
        public virtual void SplitBankAccountInfo(UserCompanyExtenstion ue)
        {
            if (!string.IsNullOrEmpty(ue.BankAccountInfo))
            {
                string rawBankAccountInfo = ue.BankAccountInfo.Trim();
                char arrayStart = '[';
                int jsonStart = rawBankAccountInfo.Trim().IndexOf(arrayStart);

                if (jsonStart < 0)
                {
                    //old format, put old format in the 1st slot
                    ue.BankAccountName = rawBankAccountInfo;
                }
                else
                {
                    while (rawBankAccountInfo.Contains("&amp;"))
                    {
                        rawBankAccountInfo = rawBankAccountInfo.Replace("&amp;", "&");
                    }
                    rawBankAccountInfo = rawBankAccountInfo.Replace("&quot;", "\"");

                    string[] bankAccountParts = SerializedHelper.JsonDeserialize<string[]>(rawBankAccountInfo);
                    ue.BankAccountName = bankAccountParts[0];
                    ue.BankAccountNo = bankAccountParts[1];
                }
            }
        }

        public static void CombineBankAccountInfo(Client configClient, UserCompanyExtenstion ue)
        {
            string[] bankAccountParts = new string[2] 
            {
                string.IsNullOrEmpty(ue.BankAccountName) ? null : ue.BankAccountName.Trim(),
                string.IsNullOrEmpty(ue.BankAccountNo) ? null : ue.BankAccountNo.Trim() };
            configClient.BankAccountInfo = SerializedHelper.JsonSerialize<string[]>(bankAccountParts);
        }
        #endregion


    }
}
