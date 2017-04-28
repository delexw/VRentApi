using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities;
using CF.VRent.Entities.TermsConditionService;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface ITermsCondition : IBLL
    {
        IEnumerable<TermsConditionExtension> GetLastestTC(string type, int isIncludeContent, UserRoleEntityCollection currentUserRole = null);

        ReturnResult AddOrUpgradeTC(TermsCondition entity, UserRoleEntityCollection currentUserRole = null);

        ReturnResult AcceptTC(UserTermsConditionAgreement entity, UserRoleEntityCollection currentUserRole = null);
    }
}
