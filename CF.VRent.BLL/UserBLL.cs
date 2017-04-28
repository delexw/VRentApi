using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Contract;
using CF.VRent.Common;
using CF.VRent.Log;
using System.ServiceModel.Web;
using System.Net;
using CF.VRent.Entities.DataAccessProxy;

using CF.VRent.Entities.KEMASWSIF_USERRef;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.BLL
{
    public class UserBLL :AbstractBLL
    {

        public UserBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        {
        }
        public UserBLL()
            : this(null)
        {
        }

        /// <summary>
        /// forgot the password for the current user
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public ForgotPwdRes ForgotPassword(string mail, string lang)
        {
            //fix issue https://jira.mcon-group.com/browse/VRENT-829
            string BElang = string.IsNullOrEmpty(lang) ? CompanyUtility.DefaultLanguage : lang;

            if (!LanguageHelper.CheckExistLang(BElang))
            {

                var webEx = new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000002.ToString(),
                    Message = MessageCode.CVB000002.GetDescription(),
                    Type = MessageCode.CVB000002.GetMessageType()
                },
                HttpStatusCode.BadRequest);

                throw webEx;
            }
            else
            {
                using (KemasUserAPI clientUser = new KemasUserAPI())
                {
                    var res = clientUser.forgotPassword(mail, BElang);

                    ForgotPwdRes forgetPwdRes = new ForgotPwdRes() { message = res.message, Result = Convert.ToInt32(res.Result), success = Convert.ToInt32(res.success) };

                    return forgetPwdRes;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="bookingId"></param>
        /// <param name="dateBegin"></param>
        /// <param name="dateEnd"></param>
        /// <param name="category"></param>
        /// <param name="typeofJourney"></param>
        /// <returns></returns>
        public int GetCountForAvaliableCars(string startLocation, string uid, string bookingId, string dateBegin, string dateEnd, string category, string typeofJourney)
        {
            Condition[] arr = new Condition[]{ 
                  new Condition(){ key="CSCarModel.vehicle_category", value=category},
            new Condition(){ key="CarGroupModel.TypeOfJourney", value=typeofJourney}};

            Data data = new Data();
            data.DateBegin = dateBegin;
            data.DateEnd = dateEnd;
            data.Conditions = arr;
            data.StartLocation = startLocation;

            using (CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.WSKemasPortTypeClient client = new CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.WSKemasPortTypeClient())
            {
                var res = client.getCountAvailableCars(bookingId, uid, data);
                return Convert.ToInt32(res);
            }
        }

        public string[] GetAvailableCarCategories(string startLocation, string uid, string bookingId, string dateBegin, string dateEnd, string typeofJourney, string creatorId, string driverId)
        {

            string[] availableCategories = null;
            if (string.IsNullOrEmpty(driverId))
            {
                driverId = creatorId;
            }

            string kemasBookingID = null;

            if (!string.IsNullOrEmpty(bookingId))
            {
                ProxyReservation proxyBooking = ProxyReservationImpl.FindReservationByBookingID(Convert.ToInt32(bookingId));
                if (proxyBooking != null && proxyBooking.ProxyBookingID > 0)
                {
                    kemasBookingID = proxyBooking.KemasBookingID.ToString();
                }
                else
                {
                    throw new VrentApplicationException(ErrorConstants.BookingNodeExistCode, ErrorConstants.BookingNodeExistMessage, ResultType.VRENT);
                }
            }

            Condition[] condition = new Condition[]{ 
                new Condition(){ key="CarGroupModel.TypeOfJourney", value=typeofJourney}};

            BookingData bookingData = new BookingData()
            {
                ID = kemasBookingID,
                Creator = creatorId,
                Driver = driverId,
                StartLocation = startLocation,
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                TypeOfJourney = typeofJourney,
                Conditions = condition
            };
            using (CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.WSKemasPortTypeClient client = new CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.WSKemasPortTypeClient())
            {
                availableCategories = client.getAvailableCategories(kemasBookingID, uid, bookingData);
            }

            return availableCategories;
        }

        public ProxyUserSetting GetUserStatus(ProxyUserSetting userSetting)
        {
            IKemasUserAPI api = KemasAccessWrapper.CreateKemasUserAPI2Instance();
            var userData = api.findUser2(userSetting.ID, userSetting.SessionID);
            userSetting.Blocked = userData.UserData.Blocked;
            userSetting.Enabled = userData.UserData.Enabled;
            userSetting.AllowChangePwd = userData.UserData.AllowChangePwd;
            userSetting.Status = userData.UserData.Status;
            return userSetting;
        }

        public bool CheckUserStatus(ProxyUserSetting userSetting)
        {
            var u = this.GetUserStatus(userSetting);
            if (u.Status == "blocked")
            {
                return false;
            }
            return true;
        }
    }
}
