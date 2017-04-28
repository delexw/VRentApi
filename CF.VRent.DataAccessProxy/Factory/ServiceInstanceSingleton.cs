using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CF.VRent.DataAccessProxy.Factory
{
    public class ServiceInstanceSingleton
    {
        /// <summary>
        /// Get payment sercvice instance
        /// </summary>
        public static readonly IPaymentService PaymentService = new PaymentService();

        /// <summary>
        /// Get data service instance
        /// </summary>
        public static readonly IDataService DataService = new DataService();

        /// <summary>
        /// Get terms condition service instance
        /// </summary>
        public static readonly ITermsConditionService TermsConditionService = new TermsConditionService();

        /// <summary>
        /// Get fapiao service instance
        /// </summary>
        public static readonly IFapiaoPreferenceService FapiaoService = new FapiaoPreferenceService();

        /// <summary>
        /// Get account service instance
        /// </summary>
        public static readonly IAccountingService AccountingService = new AccountingService();

        /// <summary>
        /// Get big file service instance
        /// </summary>
        public static readonly IBigFileService BigFileService = new BigFileService();
    }
}