using System;
using System.Collections.Generic;
//using System.Linq;
using System.ServiceModel;
//using System.ServiceModel.Activation;
using System.ServiceModel.Web;
//using System.Text;
using CF.VRent.Log;
using System.Net;
//using CF.VRent.Contract;
using CF.VRent.Entities;
using System.Web;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.BLL;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.BLL.BLLFactory;
//using CF.VRent.Common;
//using System.Diagnostics;

namespace Proxy
{

    public partial class DataService
    {

        /// <summary>
        /// Get all rights for current user
        /// </summary>
        /// <returns>right list</returns>
        [WebGet(UriTemplate = "IndirectFeeTypes", ResponseFormat = WebMessageFormat.Json)]
        public IndirectFeeType[] GetIndirectFeeTypes()
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            IndirectFeeBLL ifb = new IndirectFeeBLL(setting);
            return ifb.GetIndirectFeeTypes();
        }

        [WebInvoke(UriTemplate = "IndirectFeeTypes", Method="POST", ResponseFormat = WebMessageFormat.Json)]
        public int AddIndirectFeeTypes(IndirectFeeType[] types)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            IndirectFeeBLL ifb = new IndirectFeeBLL(setting);
            return ifb.AddIndirectFeeTypes(types);
        }


        /// <summary>
        /// Get all rights for current user
        /// </summary>
        /// <returns>right list</returns>
        [WebGet(UriTemplate = "AllRights", ResponseFormat = WebMessageFormat.Json)]
        public List<ProxyRight> GetAllRights()
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            RightBLL rb = new RightBLL(setting);
            List<ProxyRight> list = rb.GetAllRights(setting.ID);
            return list;
        }


        /// <summary>
        /// Get avaliable count for cars
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "AvaliableCars?Uid={uid}&StartLocation={startLocation}&BookingId={bId}&DateBegin={dateBegin}&DateEnd={dateEnd}&VehicleCategory={category}&TypeOfJourney={typeofJourney}", ResponseFormat = WebMessageFormat.Json)]
        public int GetCountAvaliableCars(string uid, string startLocation, string bId, string dateBegin, string dateEnd, string category, string typeofJourney)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);

            UserBLL ub = new UserBLL(setting);
            var res = ub.GetCountForAvaliableCars(startLocation, uid, bId, dateBegin, dateEnd, category, typeofJourney);
            return res;

        }

        [WebGet(UriTemplate = "AvailableCarCategories?Uid={uid}&StartLocationID={StartLocationID}&BookingId={bId}&BookingCreator={creatorId}&Driver={driverId}&DateBegin={dateBegin}&DateEnd={dateEnd}&BillingOption={BillingOption}", ResponseFormat = WebMessageFormat.Json)]
        public string[] GetAvailableCarCategories(string uid, string StartLocationID, string bId, string dateBegin, string dateEnd, string BillingOption, string creatorId, string driverId)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);

            UserBLL userBLL = new UserBLL(setting);

            var res = userBLL.GetAvailableCarCategories(StartLocationID, uid, bId, dateBegin, dateEnd, BillingOption, creatorId, driverId);

            return res;

        }


        /// <summary>
        /// Get all the drivers
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "Drivers", ResponseFormat = WebMessageFormat.Json)]
        public List<ProxyDriver> FindAllDrivers()
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            DriverBLL driverBLL = new DriverBLL(setting);
            List<ProxyDriver> list = driverBLL.FindAllDrivers(setting.ID);

            return list;
        }

        /// <summary>
        /// Get all locations
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "Locations", ResponseFormat = WebMessageFormat.Json)]
        public List<ProxyLocation> GetAllLocations()
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            OptionsBLL ob = new OptionsBLL(setting);
            
            return ob.GetAllLocation(setting.ID);

        }
        /// <summary>
        /// Get all configurations
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "Configurations", ResponseFormat = WebMessageFormat.Json)]
        public SystemConfig FindSystemConfiguration()
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            SystemConfigurationBLL scb = new SystemConfigurationBLL(setting);
            var res = scb.GetSystemConfiguration();
            return res;
        }

        /// <summary>
        /// Methods for Category related service
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "Categories?Lang={lang}", ResponseFormat = WebMessageFormat.Json)]
        public List<ProxyCategory> GetAllCategories(string lang)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            OptionsBLL ob = new OptionsBLL(setting);

            var res = ob.GetAllCategories(lang);
            return res;
        }

        /// <summary>
        /// Get all type of the Journey
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        //Modified by Liu for CR0001, add parameter userID
        [WebGet(UriTemplate = "TypeOfJourney?Uid={uid}&Lang={lang}", ResponseFormat = WebMessageFormat.Json)]
        public List<ProxyJourneyType> GetTypeOfJourney(string uid, string lang)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);
            OptionsBLL ob = new OptionsBLL(setting);
            return ob.GetAllJourneyTypes(uid, lang);
        }

        /// <summary>
        /// Get all counties
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "Countries", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<Country> GetAllCountries()
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            IOptionsBLL ob = ServiceImpInstanceFactory.CreateOptionInstance(setting);
            return ob.GetAllCountries();
        }
    }
}