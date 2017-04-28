using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.TermsConditionService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Entities;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Contract;
using CF.VRent.Common.UserContracts;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.UserRole;
using System.Web;

namespace CF.VRent.BLL
{
    public class TermsConditionBLL : AbstractBLL, ITermsCondition
    {

        public TermsConditionBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        {
        }
        public TermsConditionBLL()
            : this(null)
        {
        }

        /// <summary>
        /// Get the latest TC
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isIncludeContent"></param>
        /// <returns></returns>
        public IEnumerable<TermsConditionExtension> GetLastestTC(string type, int isIncludeContent, UserRoleEntityCollection currentUserRole = null)
        {
            DBConditionObject condition = new DBConditionObject();

            condition.RawWhereConditions["type"] = type;
            condition.RawWhereConditions["isIncludeContent"] = isIncludeContent.ToString();

            var manager = new DataAccessProxyManager();

            var returnObj = manager.GetLatestTC(condition);

            if (returnObj != null)
            {
                foreach (TermsCondition tc in returnObj.Entities)
                {
                    tc.Content = HttpUtility.HtmlDecode(tc.Content);
                }
            }

            return new TermsConditionFactory().CreateEntity(returnObj == null ? 
                new List<IDBEntityAggregationRoot>().AsEnumerable() : returnObj.Entities);
        }

        /// <summary>
        /// Add or upgrade one tc
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ReturnResult AddOrUpgradeTC(TermsCondition entity, UserRoleEntityCollection currentUserRole = null)
        {
            var manager = new DataAccessProxyManager();

            if (entity.Key == Guid.Empty)
            {
                entity.Key = Guid.NewGuid();
            }

            var i = manager.AddOrUpdateTC(entity);

            if (i > 0)
            {
                return new ReturnResult() { Success = 1 };
            }

            return new ReturnResult() { Success = 0 };
        }

        /// <summary>
        /// User accept the latest tc
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ReturnResult AcceptTC(UserTermsConditionAgreement entity, UserRoleEntityCollection currentUserRole = null)
        {
            var manager = new DataAccessProxyManager();

            var i = manager.AcceptTC(entity);

            if (i > 0)
            {
                return new ReturnResult() { Success = 1 };
            }
            return new ReturnResult() { Success = 0 };
        }
    }
}
