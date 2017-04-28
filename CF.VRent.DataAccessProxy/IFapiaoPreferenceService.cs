using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace CF.VRent.DataAccessProxy
{    
    [ServiceContract]
    public interface IFapiaoPreferenceService
    {
        [OperationContract]
        List<ProxyFapiaoPreference> GetAllFapiaoPreference(string uid);
     
        [OperationContract]
        ProxyFapiaoPreference GetFapiaoPreferenceDetail(string fpid);

        [OperationContract]
        ProxyFapiaoPreference CreateFapiaoPreference(ProxyFapiaoPreference fp);

        [OperationContract]
        void DeleteFapiaoPreference(string fpid);

        [OperationContract]
        ProxyFapiaoPreference UpdateFapiaoPreference(ProxyFapiaoPreference oldfp, ProxyFapiaoPreference newFP);        
    }
}
