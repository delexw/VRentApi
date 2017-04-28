using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using System.ServiceModel.Description;
using CF.VRent.WCFExtension.Behavior;


namespace CF.VRent.WCFExtension
{
    /// <summary>
    /// Extension for WCF security
    /// </summary>
    public class SecureWebServiceHostFactory : WebServiceHostFactory
    {
        /// <summary>
        /// Set the class MyServiceAuthorizationManager as the ServiceAuthorizationManager
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="baseAddresses"></param>
        /// <returns></returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var host = base.CreateServiceHost(serviceType, baseAddresses);
            host.Authorization.ServiceAuthorizationManager = new MyServiceAuthorizationManager();
            return host;
        }

        /// <summary>
        /// Set the class MyServiceAuthorizationManager as the ServiceAuthorizationManager
        /// </summary>
        /// <param name="constructorString"></param>
        /// <param name="baseAddresses"></param>
        /// <returns></returns>
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var host = base.CreateServiceHost(constructorString, baseAddresses);
            host.Authorization.ServiceAuthorizationManager = new MyServiceAuthorizationManager();
            return host;
        } 

       
    }
}
