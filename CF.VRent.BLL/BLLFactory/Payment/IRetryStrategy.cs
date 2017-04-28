using CF.VRent.Entities.PaymentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public interface IRetryStrategy
    {
        void Retry(IEnumerable<RetryBooking> booking);
    }
}
