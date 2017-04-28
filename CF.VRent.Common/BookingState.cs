using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace CF.VRent.Common
{
    public class BookingUtility 
    {
        public const string KemasBookingStateBase = "swBookingModel/";

        #region Check Booking State
        public static List<string> ValidBookingStates = new List<string>()
        {
            "created",
            "released",
            "taken",
            "interrupted",
            "latereturn",
            "lostitem",
            "lostitem latereturn",
            "completed",
            "canceled",
            "autocanceled",
            "deleted"
        };

        public static string TransformToProxyBookingState(string bookingState) 
        {
            return KemasBookingStateBase + bookingState;
        }

        public static bool IsValidBookingState(string state) 
        {
            return ValidBookingStates.Contains(state);
        }

        #endregion

        public static BookingType TransformToProxyBookingType(string billingOption)
        {
            BookingType proxyState = BookingType.Unknown;
            //  "Key":"1",
            //  "Value":"business & private"
            //},
            //{  
            //  "Key":"2",
            //  "Value":"business"
            //},
            //{  
            //  "Key":"3",
            //  "Value":"private"
            //}
            switch (billingOption)
            {
                case "1":
                    proxyState = BookingType.Business_Private;
                    break;
                case "3":
                    proxyState = BookingType.Private;
                    break;
                case "2":
                    proxyState = BookingType.Business;
                    break;
                default:
                    proxyState = BookingType.Unknown;
                    break;
            }

            return proxyState;

        }

        public static bool IsSingleByteString(string input)
        {
            bool IsSingleByte = true;
            char[] charArray = input.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                byte[] cnt = Encoding.Default.GetBytes(charArray,i,1);
                if (cnt.Length == 2)
                {
                    IsSingleByte = false;
                    break;
                }
            }

            return IsSingleByte;
        }

        public const int OperationSuccess = 0;
    }


    public enum CommonState { Active = 0, Deleted = 1};


      //"Value":"business & private"
      //"Value":"business"
      //"Value":"private"
    public enum BookingType
    {
        Business = 2,
        Private = 3,
        Business_Private = 1,
        Unknown
    }

    public enum FapiaoDeliverType 
    {
        Express
    }

    public enum FapiaoType
    {
        RentalFee = 1,
        IndirectFee = 2,
        CancellationFee = 3,
        VAT = 4,
        UnKnown = -1
    }


    public enum FapiaoSource
    {
        RentalFee = 1,
        IndirectFee = 2,
        UnKnown = -1
    }

    public enum FapiaoState { Active = 0, Deleted = 1,Generated = 2, Exported = 3, Imported = 4, Delivered = 5};

    public class Constants 
    {
        public const string ProxyRestfulInfo = "ProxyRestfulInfo";
        public const string ProxyDebuggingInfo = "ProxyDebuggingInfo";
        public const string DataAccessProxyDebuggingInfo = "DataAccessProxyDebuggingInfo";

        public const string KemasDebuggingInfoTitle = "KemasDebuggingInfo";
        public const string KemasExceptionTitle = "KemasExceptionTitle";
        public const string DBExceptionTitle = "DBExceptionTitle";
        public const string DBResultTitle = "DBResultTitle";

        public const string SqlExceptionLogPattern = "Message:{0}---StackTrace:{1}";
        public const string SqlConnStrKey = "SQLConnString";
    }
}
