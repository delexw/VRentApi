using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    /// <summary>
    /// Email type
    /// </summary>
    public enum EmailType
    {
        #region Fee deduction
        [EmailTypeHeader]
        Preauthorization_FeeDeduction_Successful,
        [EmailTypeHeader(IncludeSC=true)]
        Preauthorization_FeeDeduction_Failed,
        #endregion

        #region Indirect fee deduction
        [EmailTypeHeader]
        Preauthorization_IndirectFeeDeduction_Successful,
        [EmailTypeHeader(IncludeSC = true)]
        Preauthorization_IndirectFeeDeduction_Failed,
        [EmailTypeHeader]
        Preauthorization_IndirectFeeRemainder,
        #endregion

        #region Preauth
        [EmailTypeHeader]
        Preauthorization_Preauth_Successful,
        [EmailTypeHeader(IncludeSC = true)]
        Preauthorization_Preauth_Failed,
        #endregion

        #region Preauth cancellation
        [EmailTypeHeader]
        Preauthorization_PreauthCacellation_Successful,
        [EmailTypeHeader(IncludeSC = true)]
        Preauthorization_PreauthCacellation_Failed,
        #endregion

        #region Preauth completion
        [EmailTypeHeader]
        Preauthorization_PreauthComplete_Successful,
        [EmailTypeHeader(IncludeSC = true)]
        Preauthorization_PreauthComplete_Failed,
        #endregion

        #region User mgmt
        [EmailTypeHeader]
        Portal_UserMgmt_CorporateUserCreation,
        [EmailTypeHeader]
        Portal_UserMgmt_UserTransfer_VM_Approved,
        [EmailTypeHeader]
        Portal_UserMgmt_UserTransfer_VM_Reject,
        [EmailTypeHeader]
        Portal_UserMgmt_License_SC_BasicApproved,
        [EmailTypeHeader]
        Portal_UserMgmt_License_SC_BasicReject,
        [EmailTypeHeader]
        Portal_UserMgmt_License_SC_Approved,
        [EmailTypeHeader]
        Portal_UserMgmt_License_SC_Reject,
        [EmailTypeHeader]
        Portal_UserMgmt_UserDeactivation,
        [EmailTypeHeader]
        Portal_UserMgmt_UserReactivation_FromBookingDeactive,
        [EmailTypeHeader]
        Portal_UserMgmt_UserReactivation_FromKemasDisabledAndBlock,
        [EmailTypeHeader]
        Portal_UserMgmt_UserTransfer_End2Corporate,
        [EmailTypeHeader]
        Portal_UserMgmt_UserTransfer_Corporate2End,
        #endregion

        #region User registration
        [EmailTypeHeader]
        App_UserMgmt_UserCreation,
        #endregion

        #region Client
        [EmailTypeHeader]
        Portal_Client_Terminal,
        [EmailTypeHeader]
        Portal_Client_CreatedVM,
        #endregion

        #region DebitNote
        [EmailTypeHeader]
        ScheduleJob_DebitNote_Created
        #endregion
    }

    public class EmailTypeHeaderAttribute : Attribute
    {
        /// <summary>
        /// Service center agent user
        /// </summary>
        public bool IncludeSC { get; set; }
        /// <summary>
        /// VRent manager user
        /// </summary>
        public bool IncludeVM { get; set; }
        /// <summary>
        /// Operations manager user
        /// </summary>
        public bool IncludeSCL { get; set; }
        /// <summary>
        /// Administration user
        /// </summary>
        public bool IncludeADMIN { get; set; }

        /// <summary>
        /// Test user
        /// </summary>
        public bool IncludeTester { get; set; }

        public EmailTypeHeaderAttribute()
        {
            this.IncludeADMIN = false;
            this.IncludeSC = false;
            this.IncludeSCL = false;
            this.IncludeVM = false;
            this.IncludeTester = true;
        }
    }
}
