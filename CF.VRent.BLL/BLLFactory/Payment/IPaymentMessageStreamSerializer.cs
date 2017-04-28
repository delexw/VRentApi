using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public interface IPaymentMessageStreamSerializer
    {
        UnionPay Message { get; }
        string UserID { get; }
        string UniqueValue { get; }

        bool Serialize(Stream message);
    }
}
