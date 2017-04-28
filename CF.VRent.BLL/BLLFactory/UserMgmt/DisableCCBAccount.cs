using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Log;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public class DisableCCBAccount:IDisableCCBAccount
    {
        private ProxyUserSetting _sessionUser;

        public DisableCCBAccount(ProxyUserSetting sessionUser)
        {
            _sessionUser = sessionUser;
        }

        /// <summary>
        /// disable ccb permission
        /// </summary>
        /// <param name="inputUser"></param>
        /// <param name="currentUserRole"></param>
        /// <returns></returns>
        public bool DisableCCBPermission(UserExtension inputUser, UserRoleEntityCollection currentUserRole)
        {
            //Service Center
            //Operation Manager
            if (currentUserRole.IsServiceCenterUser() || currentUserRole.IsOperationManagerUser())
            {
                //0 means no ccb permission
                inputUser.TypeOfJourney = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// disable ccb booking
        /// </summary>
        /// <param name="inputUser"></param>
        /// <param name="currentUserRole"></param>
        /// <returns></returns>
        public bool DisableCCBBooking(UserExtension inputUser, UserRoleEntityCollection currentUserRole)
        {
            //Service Center
            //Operation Manager
            if (currentUserRole.IsServiceCenterUser() || currentUserRole.IsOperationManagerUser())
            {
                DataAccessProxyManager manager = new DataAccessProxyManager();

                var allBookings = manager.RetrieveReservations(inputUser.ID.ToGuid(), new string[] { 
                    BookingUtility.TransformToProxyBookingState("created"),
                    BookingUtility.TransformToProxyBookingState("released")
                });

                if (allBookings != null)
                {
                    var ccbBookings = allBookings.Where(r => r.BillingOption == BookingType.Business.GetValue());

                    var kemasApi = KemasAccessWrapper.CreateKemasReservationAPIInstance();

                    //Cancel all bookings
                    foreach (ProxyReservation pr in ccbBookings)
                    {
                        try
                        {
                            kemasApi.CancelReservation2Kemas(pr.KemasBookingID.ToStr(), _sessionUser.SessionID);
                            //Update the DataBase
                            if (manager.UpdateBookingStatusAfterPayment(pr.KemasBookingID.ToStr(), BookingUtility.TransformToProxyBookingState("canceled"), _sessionUser.ID) < 0)
                            {
                                LogInfor.WriteError(MessageCode.CVB000033.ToString(),
                                    String.Format("Local BookingInfor:{0}", pr.ObjectToJson()), _sessionUser.ID);
                            }
                        }
                        catch (FaultException<ReturnResult> ex)
                        {
                            LogInfor.WriteError(MessageCode.CVB000033.ToString(),
                                String.Format("BookingNum:{0},ErrorMessage:{1}", pr.KemasBookingNumber, ex.Detail.ObjectToJson()), _sessionUser.ID);
                        }
                        catch (Exception ex)
                        {
                            LogInfor.WriteError(MessageCode.CVB000033.ToString(),
                                String.Format("BookingNum:{0},ErrorMessage:{1}", pr.KemasBookingNumber, ex.ToStr()), _sessionUser.ID);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }


        public bool DisableDUBBooking(UserExtension inputUser, UserRoleEntityCollection currentUserRole)
        {
            //Service Center
            //Operation Manager
            if (currentUserRole.IsServiceCenterUser() || currentUserRole.IsOperationManagerUser())
            {
                DataAccessProxyManager manager = new DataAccessProxyManager();

                var allBookings = manager.RetrieveReservations(inputUser.ID.ToGuid(), new string[] { 
                    BookingUtility.TransformToProxyBookingState("created"),
                    BookingUtility.TransformToProxyBookingState("released")
                });

                if (allBookings != null)
                {
                    var dcbBookings = allBookings.Where(r => r.BillingOption == BookingType.Private.GetValue()).OrderBy(r => r.UserID);

                    var kemasApi = KemasAccessWrapper.CreateKemasReservationAPIInstance();
                    var kemasUserApi = KemasAccessWrapper.CreateKemasUserAPI2Instance();
                    var paymentApi = new PaymentBLL();

                    findUser2Response user = new findUser2Response() { UserData = new UserData2() };
                    //Cancel all bookings
                    foreach (ProxyReservation pr in dcbBookings)
                    {
                        try
                        {
                            //Get user first
                            if (user.UserData.ID != pr.UserID.ToStr())
                            {
                                user = kemasUserApi.findUser2(pr.UserID.ToStr(), _sessionUser.SessionID);
                            }
                            //Try to cancel the booking from kemas
                            kemasApi.CancelReservation2Kemas(pr.KemasBookingID.ToStr(), _sessionUser.SessionID);
                            //Try to deduct cancel fee or cancel the preauth if no cancel fee
                            paymentApi.FeeDeduction(pr.ProxyBookingID, new ProxyUserSetting()
                            {
                                ID = user.UserData.ID,
                                Name = user.UserData.Name,
                                VName = user.UserData.VName,
                                Mail = user.UserData.Mail,
                                SessionID = _sessionUser.SessionID
                            });

                            //Update the DataBase
                            if (manager.UpdateBookingStatusAfterPayment(pr.KemasBookingID.ToStr(), BookingUtility.TransformToProxyBookingState("canceled"), _sessionUser.ID) < 0)
                            {
                                LogInfor.WriteError(MessageCode.CVB000033.ToString(),
                                    String.Format("Local BookingInfor:{0}", pr.ObjectToJson()), _sessionUser.ID);
                            }
                        }
                        catch (FaultException<ReturnResult> ex)
                        {
                            LogInfor.WriteError(MessageCode.CVB000033.ToString(),
                                String.Format("BookingNum:{0},ErrorMessage:{1}", pr.KemasBookingNumber, ex.Detail.ObjectToJson()), _sessionUser.ID);
                        }
                        catch (Exception ex)
                        {
                            LogInfor.WriteError(MessageCode.CVB000033.ToString(),
                                String.Format("BookingNum:{0},ErrorMessage:{1}", pr.KemasBookingNumber, ex.ToStr()), _sessionUser.ID);
                        }
                    }
                }
            }
            return false;
        }
    }
}
