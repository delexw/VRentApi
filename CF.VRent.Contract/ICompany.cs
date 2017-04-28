using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface ICompany:IBLL
    {
        IEnumerable<UserCompanyExtenstion> GetAllCompanies(UserRoleEntityCollection currentUserRole = null);
    }
}
