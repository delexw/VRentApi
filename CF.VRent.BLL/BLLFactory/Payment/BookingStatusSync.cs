using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using System.Threading.Tasks;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public class BookingStatusSync : IBookingStatusSync
    {
        private ProxyUserSetting _operaionUser;

        public BookingStatusSync(ProxyUserSetting operaionUser)
        {
            _operaionUser = operaionUser;
        }

        public List<ProxyReservationPayment> BookingSync(List<ProxyReservationPayment> bookings)
        {
            var pb = ServiceImpInstanceFactory.CreatePaymentInstance(_operaionUser);

            var kemasApi = KemasAccessWrapper.CreateKemasReservationAPIInstance();

            List<ProxyReservationPayment> subList = new List<ProxyReservationPayment>();

            Parallel.ForEach<ProxyReservationPayment>(bookings, booking => {
                try
                {
                    var kemasBooking = kemasApi.findReservation2Kemas(booking.KemasBookingID, _operaionUser.SessionID, "english");
                    if (kemasBooking.Reservation != null && kemasBooking.Reservation.State != null)
                    {
                        //Only sync completed/autocanceled bookings
                        if (kemasBooking.Reservation.State == BookingUtility.TransformToProxyBookingState("completed") ||
                         kemasBooking.Reservation.State == BookingUtility.TransformToProxyBookingState("autocanceled"))
                        {
                            pb.UpdateBookingStatusAfterPayment(booking.KemasBookingID, kemasBooking.Reservation.State, _operaionUser.ID);
                            booking.KemasState = kemasBooking.Reservation.State;
                            subList.Add(booking);
                        }
                    }
                    else
                    {
                        //Can't find booking from kemas
                        LogInfor.WriteError(ErrorConstants.BookingNodeExistCode, String.Format("Local booking infor:{0}", booking.ObjectToJson()), _operaionUser.ID);
                    }
                }
                catch (Exception ex)
                {
                    LogInfor.WriteError(MessageCode.CVB000051.ToStr(), String.Format("Local booking infor:{0}", booking.ObjectToJson()), _operaionUser.ID);
                    LogInfor.WriteError(MessageCode.CVB000051.ToStr(), ex.ObjectToJson(), _operaionUser.ID);
                }
            });

            return subList;
        }
    }
}
