using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CF.VRent.Log;

namespace CF.VRent.SAPSDK
{
    public class SAPSDKConstants
    {
        #region Item
        public static string Sign { get { return "Sign"; } }

        public static string DebitOrCredit { get { return "DebitOrCredit"; } }

        public static string KindOfAccount { get { return "KindOfAccount"; } }

        public static string Account { get { return "Account"; } }

        public static string Amount { get { return "Amount"; } }

        public static string AmountInForeignCurrency { get { return "AmountInForeignCurrency"; } }

        public static string BaselineDate { get { return "BaselineDate"; } }

        public static string Assignment { get { return "Assignment"; } }

        public static string PositionText { get { return "PositionText"; } }

        public static string CostCenter { get { return "CostCenter"; } }

        public static string TaxKey { get { return "TaxKey"; } }
        public static string AlternativeReconciliationAccount { get { return "AlternativeReconciliationAccount"; } }
        public static string PaymentBlock { get { return "PaymentBlock"; } }
        public static string PartnerBankType { get { return "PartnerBankType"; } }
        public static string Reference1 { get { return "Reference1"; } }
        public static string Reference2 { get { return "Reference2"; } }
        public static string Reference3 { get { return "Reference3"; } }
        public static string BusinessArea { get { return "BusinessArea"; } }
        public static string TaxBaseAmount { get { return "TaxBaseAmount"; } }
        public static string DetermineTaxBase { get { return "DetermineTaxBase"; } }
        public static string SpecialGLIndicator { get { return "SpecialGLIndicator"; } } 
        #endregion

        #region Header
        public static string CompanyCode { get { return "CompanyCode"; } }
        public static string Currency { get { return "Currency"; } }
        public static string DocumentType { get { return "DocumentType"; } }
        public static string PostingDate { get { return "PostingDate"; } }
        public static string DocumentDate { get { return "DocumentDate"; } }
        public static string Reference { get { return "Reference"; } }
        public static string DocumentHeaderText { get { return "DocumentHeaderText"; } } 
        #endregion

        #region FileName
        public static string Country { get { return "Country"; } }
        public static string ClientInTargetSystem { get { return "ClientInTargetSystem"; } }
        public static string Module { get { return "Module"; } }
        public static string ShortName { get { return "ShortName"; } }
        public static string SendingSystem { get { return "SendingSystem"; } }
        public static string ConstantValue { get { return "ConstantValue"; } }
        public static string ReceivingsSystem { get { return "ReceivingsSystem"; } } 
        #endregion

        #region Common
        public static string RemoteSharedFolder { get { return "RemoteSharedFolder"; } }
        public static string LocalMapDeviceName { get { return "LocalMapDeviceName"; } }
        public static string RemoteUserName { get { return "RemoteUserName"; } }
        public static string RemoteUserPassword { get { return "RemoteUserPassword"; } }
        public static string FileExtension { get { return "FileExtension"; } }
        public static string DateFormat { get { return "DateFormat"; } }
        public static string FieldDelimiter { get { return "FieldDelimiter"; } }
        public static string DUBRentalFeeHeaderDes { get { return "DUBRentalFeeHeaderDes"; } }
        public static string DUBIndirectFeeHeaderDes { get { return "DUBIndirectFeeHeaderDes"; } }
        public static string CCBRentalFeeHeaderDes { get { return "CCBRentalFeeHeaderDes"; } }
        public static string CCBIndirectFeeHeaderDes { get { return "CCBIndirectFeeHeaderDes"; } }
        public static string Tax { get { return "Tax"; } }
        #endregion
    }
}
