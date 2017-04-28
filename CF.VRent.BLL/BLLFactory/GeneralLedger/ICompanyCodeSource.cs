using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.GeneralLedger
{
    public interface ICompanyCodeSource
    {
        BusinessArea GetDataSource();
    }
}
