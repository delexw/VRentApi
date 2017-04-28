using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Entities.TermsConditionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Entities.KemasWrapper;

namespace CF.VRent.Entities.EntityFactory
{
    public class TermsConditionFactory : IEntityDBFactory<TermsConditionExtension>
    {
        /// <summary>
        /// Create a complicated object for tc
        /// </summary>
        /// <param name="root"></param>
        /// <param name="otherValues"></param>
        /// <returns></returns>
        public TermsConditionExtension CreateEntity(IDBEntityAggregationRoot root, params object[] otherValues)
        {
            var extension = new TermsConditionExtension();

            extension.TC = (TermsCondition)root;

            extension.CreatedDate = extension.TC.CreatedOn.ToString();

            var kapi = new KemasUserAPI();

            var userDetails = kapi.findUser(extension.TC.CreatedBy.ToStr());

            extension.CreatedUserName = String.Format("{0} {1}", userDetails.VName, userDetails.Name);

            return extension;

        }

        /// <summary>
        /// Create a group of complicated object for tc
        /// </summary>
        /// <param name="roots"></param>
        /// <returns></returns>
        public IEnumerable<TermsConditionExtension> CreateEntity(IEnumerable<IDBEntityAggregationRoot> roots, params object[] otherValues)
        {
            List<TermsConditionExtension> extensions = new List<TermsConditionExtension>();
            var kapi = new KemasUserAPI();
            foreach (IDBEntityAggregationRoot root in roots)
            {
                var extension = new TermsConditionExtension();
                extension.TC = (TermsCondition)root;
                extension.CreatedDate = extension.TC.CreatedOn.ToString();
                var userDetails = kapi.findUser(extension.TC.CreatedBy.ToStr());
                extension.CreatedUserName = String.Format("{0} {1}", userDetails.VName, userDetails.Name);
                extensions.Add(extension);
            }

            return extensions;
        }
    }
}
