using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.AccountingService;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.UPSDK;
using CF.VRent.UserCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL
{
    public class DUBUtility 
    {
        public const int DefaultItemsPerPage = 100;
        public const int DefaultPageNumber = 1;

        public const string DUBDateTime = "yyyy-MM-dd";

        public static DUBDetailSearchConditions GenerateDUBSearchConditions(string bookingNumber, string userID, string userName,string beginDate, string endDate, string status,string itemsPerPage, string pageNumber)
        {
            DUBDetailSearchConditions ddsc = new DUBDetailSearchConditions();

            ddsc.KemasBookingNumber = bookingNumber;

            if (!string.IsNullOrEmpty(userID))
            {
                ddsc.UserID = Guid.Parse(userID);
            }

            ddsc.UserName = userName;

            if (!string.IsNullOrEmpty(beginDate))
            {
                ddsc.DateBegin = DateTime.ParseExact(beginDate, DUBDateTime, null);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                ddsc.DateEnd = DateTime.ParseExact(endDate, DUBDateTime, null);
            }

            if (!string.IsNullOrEmpty(status) && ErrorConstants.IsNumber(status))
            {
                UPProcessingState state;

                if (Enum.TryParse<UPProcessingState>(status, out state))
                {
                    ddsc.UPState = state;
                }
                else
                {
                    ddsc.UPState = new Nullable<UPProcessingState>(); 
                }
            }

            if (!string.IsNullOrEmpty(itemsPerPage))
            {
                ddsc.ItemsPerPage = Convert.ToInt32(itemsPerPage);
            }
            else
            {
                ddsc.ItemsPerPage = DefaultItemsPerPage;
            }

            if (!string.IsNullOrEmpty(pageNumber))
            {
                ddsc.PageNumber = Convert.ToInt32(pageNumber);
            }
            else
            {
                ddsc.PageNumber = DefaultPageNumber;
            }

            ddsc.TotalPages = -1;

            return ddsc;
        }

        public static DebitNoteDetailSearchConditions GenerateDebitNoteDetailSearchConditions(string debitNoteID,string bookingNumber, string userID, string userName,string beginDate, string endDate,string itemsPerPage, string pageNumber)
        {
            DebitNoteDetailSearchConditions ddsc = new DebitNoteDetailSearchConditions();

            if (!string.IsNullOrEmpty(debitNoteID))
            {
                ddsc.DebitNoteID = Convert.ToInt32(debitNoteID);
            }

            ddsc.KemasBookingNumber = bookingNumber;

            if (!string.IsNullOrEmpty(userID))
            {
                ddsc.UserID = Guid.Parse(userID);
            }

            if (!string.IsNullOrEmpty(userName))
            {
                ddsc.UserName = userName;
            }

            if (!string.IsNullOrEmpty(beginDate))
            {
                ddsc.DateBegin = DateTime.ParseExact(beginDate, DUBDateTime, null);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                ddsc.DateEnd = DateTime.ParseExact(endDate, DUBDateTime, null);
            }

            if (!string.IsNullOrEmpty(itemsPerPage))
            {
                ddsc.ItemsPerPage = Convert.ToInt32(itemsPerPage);
            }
            else
            {
                ddsc.ItemsPerPage = DefaultItemsPerPage;
            }

            if (!string.IsNullOrEmpty(pageNumber))
            {
                ddsc.PageNumber = Convert.ToInt32(pageNumber);
            }
            else
            {
                ddsc.PageNumber = DefaultPageNumber;
            }

            ddsc.TotalPage = -1;

            return ddsc;
        }
    }

    public interface IDUB
    {
        DUBDetailSearchConditions RetrieveDUBDetails(DUBDetailSearchConditions conditions);
    }

    public class DUBBLL : AbstractBLL, IDUB
    {
        public DUBBLL(ProxyUserSetting userInfo) : base(userInfo) 
        {
        }

        public DUBDetailSearchConditions RetrieveDUBDetails(DUBDetailSearchConditions conditions)
        {
            IAccountingService ias = new DataAccessProxyManager();
            DUBDetailSearchConditions output = ias.RetrieveDUBDetailsByConditions(conditions, UserInfo);

//            //retrieve all user in default company
//            var companyManager = UserCompanyContext.CreateCompanyManager();
//            var defaultCompany = companyManager.Companies[UserCompanyConstants.EndUserCompanyKey].GetDefaultKemasCompany();
//            var kemasExtApi = KemasAccessWrapper.CreateKemasExtensionAPIInstance();
//            var defaultCompanyID = kemasExtApi.GetCompanyID(defaultCompany.Name, UserInfo.SessionID);

//            KemasUserAPI kemasUser = new KemasUserAPI();
//            getUsers2Request gu2Req = new getUsers2Request();
//            gu2Req.SessionID = UserInfo.SessionID;
//            gu2Req.SearchCondition = new getUsers2RequestSearchCondition()
//            {
//                //ClientID = defaultCompanyID
////                ClientID = "e1c286c4-ae86-4c7d-810f-1b6357892f9f"
//            };

//            getUsers2Response gu2Res = kemasUser.getUsers2(gu2Req);

//            if (gu2Res.Error.ErrorCode.Equals("E0000"))
//            {
//                foreach (var detail in output.Items)
//                {
//                    UserData2 ud2 = gu2Res.Users.FirstOrDefault(m => m.ID.Equals(detail.UserID.ToString()));

//                    if (ud2 != null)
//                    {
//                        detail.UserName = string.Format("{0} {1}", ud2.Name, ud2.VName);
//                    }

//                    detail.KemasBookingNumber = detail.KemasBookingNumber.TrimStart('0');
//                }
//            }

            return output;
        }
    }
}
