using CF.VRent.Common.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities
{
    public class VRentDataDictionay
    {
        public enum BookingPaymentState
        {
            Enable = 1,
            Disable = 0
        }

        public enum BookingIndirectFeePaymentState
        {
            Enable = 1,
            Disable = 0
        }

        public enum UnionCardState
        {
            Enable = 1,
            Disable = 0
        }

        public enum UserStatusFlagValue
        {
            Enable = 1,
            Disable = 0
        }

        public enum TransactionRetry
        {
            Retry = 1,
            Default = 0
        }

        /// <summary>
        /// The type of general ledger item type
        /// </summary>
        public enum GLItemType
        {
            Debit = 1,
            Credit = 2
        }

        /// <summary>
        /// The type of detal of general ledger item
        /// </summary>
        public enum GLItemDetailType
        {
            IndirectFee = 1,
            RentalFee = 2,
            DebitNote = 3
        }

        /// <summary>
        /// The type of general ledger header
        /// </summary>
        public enum GLHeaderType
        {
            DUB = 1,
            CCB = 2
        }
    }
}
