using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using System.ServiceModel.Web;
using System.Net;
using CF.VRent.Log;
using CF.VRent.Common;
using CF.VRent.Contract;
using CF.VRent.Entities.KEMASWSIF_CATALOGRef;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Cache;

namespace CF.VRent.BLL
{
    public class OptionsBLL : AbstractBLL, IOptionsBLL
    {
        public OptionsBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        {
        }

        public List<ProxyCategory> GetAllCategories(string lang)
        {
            IKemasOptionsAPI kemasOption = new KemasOptionsAPI();
            return kemasOption.getCategories().Select(m => new ProxyCategory { key = m.key, value = m.value }).ToList();
        }

        public List<ProxyJourneyType> GetAllJourneyTypes(string userID, string lang)
        {
            IKemasOptionsAPI kemasOption = new KemasOptionsAPI();
            return kemasOption.getUserTypeOfJourney(userID,lang).Select(m => new ProxyJourneyType { Key = m.key, Value = m.value }).ToList();
        }

        public List<ProxyLocation> GetAllLocation(string uid)
        {
            IKemasReservation kemasOption = new KemasReservationAPI();
            return kemasOption.getLocations(uid).Select(m => new ProxyLocation {ID = m.id, Text = m.text}).ToList();
        }

        public BookingOptions GetAllBookingOptions(string sessionID,string lang)
        {
            IKemasReservation kemasOption = new KemasReservationAPI();
            getBookingOptions_Response optionsRes = kemasOption.getBookingOptions(sessionID, lang);

            if (optionsRes.BillingOptions == null || optionsRes.Locations == null || optionsRes.Drivers == null)
            {
                Error error = optionsRes.Error;
                throw new VrentApplicationException(error.ErrorCode,error.ErrorMessage,ResultType.KEMAS);
            }

            return KemasHelper.ConvertFromKemasBookingOptions(optionsRes);
        }

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Country> GetAllCountries(CacheModel cacheIt = null)
        {
            if (cacheIt != null && cacheIt.Exist("GetAllCounties"))
            {
                return cacheIt.Get<List<Country>>("GetAllCounties");
            }
            else
            {
                DataAccessProxyManager manager = new DataAccessProxyManager();
                var contries = manager.GetAllCountries(null);
                cacheIt.Set("GetAllCounties", contries.Entities.ToList());
                return contries != null ? contries.Entities : null;
            }
        }
    }
}
