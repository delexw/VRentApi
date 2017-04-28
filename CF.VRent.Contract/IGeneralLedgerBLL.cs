using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface IGeneralLedgerBLL : IBLL
    {
        List<GeneralLedgerLine> GenerateDUBLedger(long headerId, DateTime from, DateTime end);
        List<GeneralLedgerLine> GenerateCCBLedger(long headerId, DateTime from, DateTime end);
        long AddGeneralLedgerHeader(GeneralLedgerHeader header);
    }
}
