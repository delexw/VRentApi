using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace Proxy
{
    [ServiceContract]
    public interface IListenUnionService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "ListenPreauth", Method = "POST", RequestFormat = WebMessageFormat.Json)]
        void ListenPreauthorization(Stream response);

        [OperationContract]
        [WebInvoke(UriTemplate = "ListenCancelPreauth", Method = "POST", RequestFormat = WebMessageFormat.Json)]
        void ListenCancelPreauthorization(Stream response);

        [OperationContract]
        [WebInvoke(UriTemplate = "ListenCancelPreauthOfRedoPreauth", Method = "POST", RequestFormat = WebMessageFormat.Json)]
        void ListenCancelPreauthorizationForRedoPreauth(Stream response);

        [OperationContract]
        [WebInvoke(UriTemplate = "ListenCompletePreauth", Method = "POST", RequestFormat = WebMessageFormat.Json)]
        void ListenCompletePreauthorization(Stream response);

        [OperationContract]
        [WebInvoke(UriTemplate = "ListenCompleteConsuming", Method = "POST", RequestFormat = WebMessageFormat.Json)]
        void ListenCompleteConsuming(Stream response);
    }
}