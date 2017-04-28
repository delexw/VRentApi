using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.BLL
{

    public class RequestFapiaoBLL : AbstractBLL
    {
        private UserProfile _up = null;

        public RequestFapiaoBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        {
            if (UserInfo != null)
            {
                _up = new UserProfile()
                {
                    UserID = Guid.Parse(UserInfo.ID),
                };
            }
        }


        //owner
        public ProxyFapiaoRequest UpdateFapiaoRequest(ProxyFapiaoRequest requestFE, string language)
        {
            ProxyFapiaoRequest fpr = null;
            if (Enum.IsDefined(typeof(FapiaoSource), requestFE.FapiaoSource))
            {
                ProxyReservation proxyBooking = ProxyReservationImpl.FindReservationByBookingID(requestFE.ProxyBookingID);
                if (proxyBooking != null)
                {
//                    findReservation2_Response res = ProxyReservationImpl.FindReservationAndSync(proxyBooking.KemasBookingID.ToString(), _pus.SessionID, language, ref proxyBooking);
                    findReservation2_Response res = ProxyReservationImpl.FindReservation(proxyBooking.KemasBookingID.ToString(), UserInfo.SessionID, language);
                    if (res.Reservation != null)
                    {
                        if (proxyBooking.BillingOption == 3)
                        {
                            ProxyFapiaoRequest request = new ProxyFapiaoRequest()
                            {
                                ProxyBookingID = requestFE.ProxyBookingID,
                                FapiaoSource = requestFE.FapiaoSource,
                                FapiaoPreferenceID = requestFE.FapiaoPreferenceID,
                                ModifiedOn = DateTime.Now,
                                ModifiedBy = _up.UserID
                            };
                            IDataService proxyReplicate = new DataAccessProxyManager();
                            ReturnResultExt ret = proxyReplicate.UpdateFapiaoRequest(request, _up);

                            if (ret.Success == BookingUtility.OperationSuccess)
                            {
                                fpr = (ProxyFapiaoRequest)ret.Data;
                            }
                            else
                            {
                                throw new VrentApplicationException(ret);
                            }
                        }
                        else
                        {
                            throw new VrentApplicationException(ErrorConstants.ForbidenCCBBookingFapiaoCode,
                                string.Format(ErrorConstants.ForbidenCCBBookingFapiaoMessage,
                                proxyBooking.KemasBookingID,
                                res.Reservation.BillingOption.ID,
                                res.Reservation.Driver.ID), 
                                ResultType.VRENT);
                        }
                    }
                    else
                    {
                        Error locateKemasBookingError = res.Error;
                        throw new VrentApplicationException(locateKemasBookingError.ErrorCode, locateKemasBookingError.ErrorMessage, ResultType.KEMAS);
                    }
                }
                else
                {
                    throw new VrentApplicationException(ErrorConstants.BookingNodeExistCode, ErrorConstants.BookingNodeExistCode, ResultType.VRENT);
                }
            }
            else
            {
            }

            return fpr;
        }

        public ProxyFapiaoRequest[] RetrieveFapiaoRequestBySource(int proxyBookingID, int[] source, string language)
        {
            ProxyFapiaoRequest[] fprs = null;
            
            ProxyReservation proxyBooking = ProxyReservationImpl.FindReservationByBookingID(proxyBookingID);
            if (proxyBooking != null)
            {
                //findReservation2_Response res = ProxyReservationImpl.FindReservationAndSync(proxyBooking.KemasBookingID.ToString(), _pus.SessionID, language, ref proxyBooking);
                findReservation2_Response res = ProxyReservationImpl.FindReservation(proxyBooking.KemasBookingID.ToString(), UserInfo.SessionID, language);
                if (res.Reservation != null)
                {
                    IDataService proxyReplicate = new DataAccessProxyManager();
                    ReturnResultExtRetrieve ret = proxyReplicate.RetrieveFapiaoRequestDetailByFapiaoSource(proxyBookingID, source, _up);

                    if (ret.Success == BookingUtility.OperationSuccess)
                    {
                        fprs = (ProxyFapiaoRequest[])ret.Data.Where(m => m.FapiaoPreferenceID != null).ToArray();
                    }
                    else
                    {
                        throw new VrentApplicationException(ret);
                    }
                }
                else
                {
                    Error locateKemasBookingError = res.Error;
                    throw new VrentApplicationException(locateKemasBookingError.ErrorCode, locateKemasBookingError.ErrorMessage, ResultType.KEMAS);
                }
            }
            else
            {
                throw new VrentApplicationException(ErrorConstants.BookingNodeExistCode, ErrorConstants.BookingNodeExistCode, ResultType.VRENT);
            }
            return fprs;
        }
    }
}
