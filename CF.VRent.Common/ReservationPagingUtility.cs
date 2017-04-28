using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Common
{
    public class ReservationPagingUtility
    {
        public const int DefaultItemsPerPage = 10;
        public const int DefaultMaxItemsPerPage = 50;
        public const int DefaultPageNumber = 1;
        public const string DefaultOrderBy = "PROXYBOOKINGID DESC";

        //booking number, drvier name, start date, end date, startlocationname
        #region FE paging fields
        public const string ProxyBookingIDField = "PROXYBOOKINGID";
        public const string KemasBookingIDField = "KEMASBOOKINGID";
        public const string KemasBookingNumberField = "NUMBER";
        public const string DateBeginField = "DATEBEGIN";
        public const string DateEndField = "DATEEND";

        public const string DriverIDField = "DRIVERID";
        public const string DriverFirstNameField = "DRIVERFIRSTNAME";
        public const string DriverLastNameField = "DRIVERLASTNAME";
        //FE has a single input field
        public const string FuzzyDriverName = "NAME";

        public const string CorporateIDField = "CORPORATEID";
        public const string CorporateNameField = "CORPORATENAME";

        public const string CreatorIDField = "CREATORID";
        public const string CreatorFirstNameField = "CREATORFIRSTNAME";
        public const string CreatorLastNameField = "CREATORLASTNAME";

        public const string StartLocationIDField = "STARTLOCATIONID";
        public const string StartLocationNameField = "STARTLOCATIONNAME";
        
        public const string StateField = "STATE";
        public const string BillingOptionField = "BILLINGOPTION";
        public const string PriceField = "PRICE";

        public const string CreatedOnField = "CREATEDON";
        public const string CreatedByField = "CREATEDBY";
        public const string ModifiedOnField = "MODIFIEDON";
        public const string ModifiedByField = "MODIFIEDBY";

        public const string SortOrderASC = "ASC";
        public const string SortOrderDESC = "DESC";


        public const string NULLValueFromFE = "NULL";

        private static string[] ConditionDelim = new string[7] { ">", "=", "<", ">=", "<=", " ", "!=" };

        public const string BadWhereConditionCode = "CVC000111";
        public const string BadWhereConditionMessage = "Where Condition {0} is not valid";

        public const string BadOrderByConditionCode = "CVC000112";
        public const string BadOrderByConditionMessage = "Order By Condition {0} is not valid";

        public const string UnknownPagingExceptionCode = "CVC000113";
        public const string UnknownPagingExceptionMessage = "Msg:{0}, StackTrace:{1}";

        public const string UnexpectedWhereConditionPartsCode = "CVC000114";
        public const string UnexpectedWhereConditionPartsMessage = "Unexpceted Where columns:{0}, Bad Format values:{1}";

        public const string UnexpectedOrderByConditionPartsCode = "CVC000115";
        public const string UnexpectedOrderByConditionPartsMessage = "Unexpceted Order columns:{0}, unique columns:{1}, duplicated columns:{2}, invalid Descs:{3}, Bad Format values:{4}";


        public const string BadStateCode = "CVC000116";
        public const string BadStateMessage = "State {0} is not valid";

        public const string BadDateTimeCode = "CVC000117";
        public const string BadDateTimeMessage = "{0} {1} is not valid";


        //booking number, drvier name, start date, end date, startlocationname
        public static List<string> ValidWhereColumns = new List<string>()
        {
            //ProxyBookingIDField,
            //KemasBookingIDField, // guid
            DateBeginField,//"yyyy-MM-dd HH:mm";
            DateEndField, //"yyyy-MM-dd HH:mm";
            KemasBookingNumberField,

            //CreatorIDField,
            //CreatorFirstNameField,
            //CreatorLastNameField,

            //DriverIDField,
            //DriverFirstNameField,
            //DriverLastNameField,
            FuzzyDriverName,

            //CorporateIDField,
            //CorporateNameField,

            //StartLocationIDField,
            StartLocationNameField,

            //StateField, //kemas state
            //BillingOptionField,
            //PriceField,

            //CreatedOnField,
            //CreatedByField,
            //ModifiedOnField,
            //ModifiedByField,
        };

        public static List<string> ValidOrderByColumns = new List<string>()
        {
            ProxyBookingIDField,
            //KemasBookingIDField,// guid
            //DateBeginField,//"yyyy-MM-dd HH:mm";
            //DateEndField, //"yyyy-MM-dd HH:mm";
            //KemasBookingNumberField,

            //CreatorIDField,
            //CreatorFirstNameField,
            //CreatorLastNameField,

            //DriverIDField,
            //DriverFirstNameField,
            //DriverLastNameField,

            //CorporateIDField,
            //CorporateNameField,

            //StartLocationIDField,
            //StartLocationNameField,

            //StateField, //kemas state
            //BillingOptionField,
            //PriceField,

            //CreatedOnField,
            //CreatedByField,
            //ModifiedOnField,
            //ModifiedByField
        };

        public static List<string> ValidUniqueColumns = new List<string>()
        {
            ProxyBookingIDField,
            //"KEMASBOOKINGID", // guid
            //"DATEBEGIN",//"yyyy-MM-dd HH:mm";
            //"DATEEND", //"yyyy-MM-dd HH:mm";
            //"NUMBER",
            //"CREATEDON",
            //"ModifiedOn"
        };

        public static List<string> ValidOrderByDesc = new List<string>()
        {
            SortOrderASC,
            SortOrderDESC // guid
        };

        #endregion

        private static string IsValidState(string stateKey,string stateValue)
        {
            string fullState = null;

            string[] parts = stateValue.Split(new char[1]{' '});
            if (parts.Length == 3 && !string.IsNullOrEmpty(parts[2]))
            {
                foreach(string state in BookingUtility.ValidBookingStates)
                {
                    if(state.ToUpper().Equals(parts[2]))
                    {
                        fullState = BookingUtility.TransformToProxyBookingState(parts[2]);
                        break;
                    }
                }
            }

            return fullState;

        }

        public static Dictionary<string, string> ParseWhereConditions(string[] whereConditions)
        {
            Dictionary<string, string> whereParts = null;

            Dictionary<string, string> wherePartsTemp = whereConditions.ToDictionary<string, string, string>
                (
                    (m) => (m.Trim().Split(ConditionDelim, StringSplitOptions.RemoveEmptyEntries))[0].Trim(),
                    (m) => m.Trim()
                );

            //should be valid where columns
            List<string> badWhereConditionParts = wherePartsTemp.Keys.Where(m => !ValidWhereColumns.Contains(m)).ToList();

            //should has 3 or 4 parts
            List<string> IlleagalWhereConditionParts = wherePartsTemp.Values
                .Where(m => !m.Equals(ReservationPagingUtility.FuzzyDriverName))
                .Where(m => m.Split(' ').Length > 4).ToList();


            if (badWhereConditionParts.Count > 0 && IlleagalWhereConditionParts.Count > 0)
            {
                throw new VrentApplicationException(
                    UnexpectedWhereConditionPartsCode,
                    string.Format(UnexpectedWhereConditionPartsMessage, string.Join(",", badWhereConditionParts), string.Join(",", IlleagalWhereConditionParts)),
                    ResultType.VRENTFE);
            }
            else
            {
                if (wherePartsTemp.ContainsKey(StateField))
                {
                    string stateValue = wherePartsTemp[StateField];
                    if (IsValidState(StateField, stateValue) == null)
                    {
                        throw new VrentApplicationException(BadStateCode, string.Format(BadStateMessage, stateValue),ResultType.VRENTFE);
                    }
                }

                if(wherePartsTemp.Keys.Contains(DateBeginField) || wherePartsTemp.Keys.Contains(DateEndField))
                {
                    if (wherePartsTemp.Keys.Contains(DateBeginField))
                    {
                        string beginDate = wherePartsTemp[DateBeginField];

                        string[] beginDateparts = beginDate.Split(new string[]{"<",">",">=","<="}, StringSplitOptions.None);

                        //change to () later
                        string beginDateStr = beginDateparts[1].Trim();
                        if (beginDateStr[0].Equals('='))
                        {
                            beginDateStr = beginDateStr.Substring(1).Trim();
                        }

                        bool isValid = IsValidDate(beginDateStr);
                        if (!isValid)
                        {
                            throw new VrentApplicationException(BadDateTimeCode, string.Format(BadDateTimeMessage, DateBeginField, beginDateStr), ResultType.VRENTFE);
                        }
                    }

                if (wherePartsTemp.Keys.Contains(DateEndField))
                {
                    string endDate = wherePartsTemp[DateEndField];
                    string[] endDateparts = endDate.Split(new string[] { "<", ">", ">=", "<=" }, StringSplitOptions.None);

                    //change to () later
                    string beginDateStr = endDateparts[1].Trim();
                    if (beginDateStr[0].Equals('='))
                    {
                        beginDateStr = beginDateStr.Substring(1).Trim();
                    }

                    bool isValid = IsValidDate(beginDateStr);
                    if (!isValid)
                    {
                        throw new VrentApplicationException(BadDateTimeCode, string.Format(BadDateTimeMessage, DateEndField, beginDateStr), ResultType.VRENTFE);
                    }
                }

                }


                whereParts = wherePartsTemp;
            }

            return whereParts;
        }

        public static Dictionary<string, string> ParseOrderByConditions(string[] orderByConditions)
        {
            Dictionary<string, string> orderParts = null;

            Dictionary<string, string> orderByPartsTemp = orderByConditions.ToDictionary<string, string, string>
                (
                    (m) => (m.Split(ConditionDelim, StringSplitOptions.RemoveEmptyEntries))[0].Trim(),
                    (m) => (m.Split(ConditionDelim, StringSplitOptions.RemoveEmptyEntries))[1].Trim()
                );

            List<string> badOrderParts = orderByPartsTemp.Keys.Where(m => !ValidOrderByColumns.Contains(m)).ToList();
            List<string> uniqueParts = orderByPartsTemp.Keys.Where(m => ValidUniqueColumns.Contains(m)).ToList();
            List<string> duplicatedParts = orderByPartsTemp.Keys.Distinct().ToList();
            List<string> invalidOrderByDesc = orderByPartsTemp.Values.Where(m => !ValidOrderByDesc.Contains(m)).ToList();
            //should has 3 parts
            List<string> IlleagalOrderByParts = orderByPartsTemp.Values.Where(m => m.Trim().Split(' ').Length != 1).ToList();

            if (orderByPartsTemp.Keys.Count > 0)
            {
                if (badOrderParts.Count > 0 
                    //|| uniqueParts.Count == 0 
                    || duplicatedParts.Count != orderByPartsTemp.Count || invalidOrderByDesc.Count > 0 || IlleagalOrderByParts.Count > 0)
                {
                    throw new VrentApplicationException(
                        UnexpectedOrderByConditionPartsCode,
                        string.Format(UnexpectedOrderByConditionPartsMessage,
                            string.Join(",", badOrderParts),
                            string.Join(",", uniqueParts),
                            string.Join(",", duplicatedParts),
                            string.Join(",", invalidOrderByDesc),
                            string.Join(",", IlleagalOrderByParts)),
                        ResultType.VRENTFE);

                } 
            }

            orderParts = orderByPartsTemp;

            return orderParts;
        }

        public static bool IsValidDate(string date) 
        {
            bool IsValid = true;

            date.Split();

            try
            {
                DateTime.Parse(date);
            }
            catch
            {
                IsValid = false;
            }

            return IsValid;
        }
    }


}
