using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public interface IBookingStatusSync
    {
        /// <summary>
        /// Update local booking status
        /// </summary>
        /// <param name="bookings"></param>
        /// <param name="state"></param>
        /// <param name="operationUserId"></param>
        /// <returns></returns>
        List<ProxyReservationPayment> BookingSync(List<ProxyReservationPayment> bookings);
    }
}
