using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Entities.FapiaoPreferenceProxy;

namespace CF.VRent.Contract
{
    public interface IFapiaoPreference
    {

        ProxyFapiaoPreference[] GetAllFapiaoPreference(string uid);

        ProxyFapiaoPreference GetFapiaoPreferenceDetail(string uuid);

        ProxyFapiaoPreference SaveFapiaoPreference(ProxyFapiaoPreference fp);

        ProxyFapiaoPreference UpdateFapiaoPreference(ProxyFapiaoPreference fp);

        void DeleteFapiaoPreference(string uuid);
    }
}
