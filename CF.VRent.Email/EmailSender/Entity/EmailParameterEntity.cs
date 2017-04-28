using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email.EmailSender.Entity
{
    public class EmailParameterEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Price { get; set; }
        public string IOSUrl { get; set; }
        public string AndroidUrl { get; set; }
        public string VRentUrl { get; set; }
        public string Vehicle { get; set; }
        public string Reason { get; set; }
        public string InvoiceNo { get; set; }
        public string BookingNumber { get; set; }
        public string BookingStartTime { get; set; }
        public string BookingEndTime { get; set; }
        public string BookingCarCategory { get; set; }
        public string BookingFeeTotal { get; set; }

        public string VRentManagerName { get; set; }
        public string Company { get; set; }
        /// <summary>
        /// day/month/year
        /// </summary>
        public string CompanyTerminalDate { get; set; }
        public string ResignDate { get; set; }

        /// <summary>
        /// format: month/year
        /// </summary>
        public string DebitNoteDate { get; set; }
    }
}
