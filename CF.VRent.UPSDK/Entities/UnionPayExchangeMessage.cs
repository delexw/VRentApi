using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UPSDK.Entities
{
    /// <summary>
    /// The entity is used to represent the message
    /// The first character of the every field is lowcase in order to adjust the old data stored in db
    /// </summary>
    public class UnionPayExchangeMessage
    {
        public string type { get; set; }

        public string priceTotal { get; set; }

        public string kemasBookingId { get; set; }

        public string bookingId { get; set; }

        public string userId { get; set; }

        public string userName { get; set; }

        public string userVName { get; set; }

        public string userMail { get; set; }

        public string adminUserId { get; set; }

        public string kemasState { get; set; }

        public string feeTypeDescription { get; set; }

        public bool isSendEmail { get; set; }

    }
}
