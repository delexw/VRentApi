using CF.VRent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using CF.VRent.UPSDK.Entities;

namespace CF.VRent.UPSDK
{
    public class UPSDKFactory
    {
        /// <summary>
        /// Create instance
        /// </summary>
        /// <returns></returns>
        public static IUnionPayUtils CreateUtls(UnionPayCustomInfo card = null, string containerName = "UnionPayFunctionalityContainer")
        {
            var container = UnityHelper.GetUnityContainer(containerName);
            if (card == null)
            {
                card = new UnionPayCustomInfo();
            }
            var interf = container.Resolve<IUnionPayUtils>(new ParameterOverride("upc", card));
            return interf;
        }
    }
}
