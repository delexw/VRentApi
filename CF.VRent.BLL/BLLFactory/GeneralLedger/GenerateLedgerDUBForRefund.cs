using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.GeneralLedger
{
    public class GenerateLedgerDUBForRefund : GenerateLedger
    {
        public GenerateLedgerDUBForRefund(ProxyUserSetting loginUser) : base(loginUser) { }

        public override List<GeneralLedgerLine> Generate(long headerId, DateTime from, DateTime end)
        {
            return new List<GeneralLedgerLine>();
        }
    }
}
