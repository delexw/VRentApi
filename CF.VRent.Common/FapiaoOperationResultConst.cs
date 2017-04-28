using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common
{
    public enum RetrieveFapiaoRequestBySourceResult { Success = 0, InvalidOperator = 1  };
    public enum UpdateFapiaoRequestResult { Success = 0, InvalidOperator = 1 , InvalidFP = 2,BadDataExist = 3, UnChangeableState = 4 };
        //-- 0: update one
        //-- 1: invalid operator
        //-- 2: invalid FapiaoPreference
        //-- 3: bad data exists
        //-- 4: in unchangeable state

    public enum AppendIndirectFeeResult { Success = 0, InvalidOperator = -1, OrderDoesNotExist = -2};
    public class IndirectFeeItemsConst 
    {
        public const string OrderDoesNotExistCode = "CVD000011";
        public const string OrderDoesNotExistMessage = "the order does not exists for the booking {0}";

        public const string UnknownResultCode = "CVD000012";
        public const string UnknownResultMessage = "unknown result for booking {0} and Operator {1}";
    }




    public class FapiaoRequestConst 
    {
        public const string SuccessCode = "CVD000010";
        public const string successMessage = "";

        public const string OperatorIsNotBookingOwnerCode = "CVD000005";
        public const string OperatorIsNotBookingOwnerMessage = "the operator {0} is not the owner of booking {1}";
        public const string OperatorIsNotFPOwnerCode = "CVD000006";
        public const string OperatorIsNotFPOwnerMessage = "the operator {0} is not the owner of FapiaoPreference {1}";
        public const string BadDataCode = "CVD000007";
        public const string BadDataMessage = "bad data exists for the operator {0} and {1}";
        public const string FapiaoRequestsInProcessingCode = "CVD000008";
        public const string FapiaoRequestsInProcessingMessage = "Fapiao request is in processing for booking {0} and User {1}";

        public const string UnknownResultCode = "CVD000009";
        public const string UnknownResultMessage = "unknown result for booking {0} and source {1}";

    }

}
