using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{
    public class KemasReservationAPIProxy : KemasReservationAPI
    {
        public override KEMASWSIF_RESERVATIONRef.Error CancelReservation2Kemas(string bookingID, string sessionID)
        {
            //Kemas
            var error = base.CancelReservation2Kemas(bookingID, sessionID);

            var kemasObj = new { Error = error };

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<object, Error>();

            validator.Validate(kemasObj);

            return error;
        }

        public override findReservations2_Response findReservations2Kemas(findReservations2_Request request)
        {
            var bookings = base.findReservations2Kemas(request);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<findReservations2_Response, Error>();

            validator.Validate(bookings);

            return bookings;
        }

        public override checkPrice2_Response checkPrice2Advanced(string sessionID, checkPrice2_RequestBookingData crertia)
        {
            var price =  base.checkPrice2Advanced(sessionID, crertia);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<checkPrice2_Response, Error>();

            validator.Validate(price);

            return price;
        }
    }
}
