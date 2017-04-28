using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CF.VRent.Common
{
    public class UnityHelper
    {
        public const string KemasApiWapperContainer = "KemasApiWapperContainer";
        public const string UserMgmtContainer = "UserMgmtContainer";

        public static IUnityContainer GetUnityContainer(string containerName)
        {
            IUnityContainer _uContainer = new UnityContainer();
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Configure(_uContainer, containerName); // named container element
            return _uContainer;
        }
    }
}
