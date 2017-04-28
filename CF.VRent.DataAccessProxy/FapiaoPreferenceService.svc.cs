using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FapiaoPreferenceService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FapiaoPreferenceService.svc or FapiaoPreferenceService.svc.cs at the Solution Explorer and start debugging.
    public class FapiaoPreferenceService : IFapiaoPreferenceService
    {
        public List<ProxyFapiaoPreference> GetAllFapiaoPreference(string uid)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<List<ProxyFapiaoPreference>>
                (
                    () => FapiaoPreferenceDAL.GetAllFapiaoPreference(uid)
                );
        }

        public ProxyFapiaoPreference GetFapiaoPreferenceDetail(string fpid)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyFapiaoPreference>
                (
                    () => FapiaoPreferenceDAL.GetFapiaoPreferenceDetail(fpid)
                );
        }

        public ProxyFapiaoPreference CreateFapiaoPreference(ProxyFapiaoPreference fp)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyFapiaoPreference>
                (
                    () => FapiaoPreferenceDAL.SaveFapiaoPreference(fp)
                );
        }

        public void DeleteFapiaoPreference(string fpid)
        {
            DataAccessProxyConstantRepo.DataAccessExceptionGuard
            (
                () => FapiaoPreferenceDAL.DeleteFapiaoPreference(fpid)
            );
        }

        public ProxyFapiaoPreference UpdateFapiaoPreference(ProxyFapiaoPreference oldfp, ProxyFapiaoPreference newFP)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<ProxyFapiaoPreference>
                (
                    () => FapiaoPreferenceDAL.UpdateFapiaoPreference(oldfp, newFP)
                ); 
        }
    }
}
