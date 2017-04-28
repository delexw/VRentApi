using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace CF.VRent.UPSDK
{
    /// <summary>
    /// Union pay interaction
    /// </summary>
    public enum UnionPayEnum
    {
        Resquest = 0,
        Response = 1,
        MerInform = 2
    }

    /// <summary>
    /// Payment transaction status
    /// </summary>
    public enum PaymentStatusEnum
    {
        [StatusGroupAttribute(Description = "NULL")]
        NULL = -2,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo= PayOperationEnum.Preauth)]
        BeforePreAuth = -1,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.Preauth)]
        PreAuthorizing = 0,
        [StatusGroupAttribute(Description = "Pre-authorization - Success", IsMiddleStatus = false,IsSuccessStatus = true, BelongTo = PayOperationEnum.Preauth)]
        PreAuthorized = 1,
        [StatusGroupAttribute(Description = "Pre-authorization - Failed", IsMiddleStatus = false, IsFailedStatus = true, BelongTo = PayOperationEnum.Preauth)]
        PreAuthFailed = 2,

        [StatusGroupAttribute(Description = "Pre-authorization cancellation - Success", IsMiddleStatus = false, BelongTo = PayOperationEnum.PreauthCancellation, IsSuccessStatus = true)]
        PreAuthCanceled = 3,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.PreauthCancellation)]
        PreAuthCancelling = 4,
        [StatusGroupAttribute(Description = "Pre-authorization cancellation - Failed", IsMiddleStatus = false, BelongTo = PayOperationEnum.PreauthCancellation, IsFailedStatus = true)]
        PreAuthCancelFailed = 5,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.PreauthCompletion)]
        PreAuthCompleting = 6,
        [StatusGroupAttribute(Description = "Fee deduction - Failed", IsMiddleStatus = false, BelongTo = PayOperationEnum.PreauthCompletion, IsFailedStatus = true)]
        PreAuthCompleteFailed = 7,
        [StatusGroupAttribute(Description = "Fee deduction - Success", IsMiddleStatus = false, BelongTo = PayOperationEnum.PreauthCompletion, IsSuccessStatus = true)]
        PreAuthCompleted = 8,

        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.Preauth)]
        PreAuthRetryShortTime = 9,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.PreauthCancellation)]
        PreAuthCancelRetryShortTime = 10,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.PreauthCompletion)]
        PreAuthCompleteRetryShortTime = 11,

        //Deduction
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.FeeDeduction)]
        PreDeduction = 12,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.FeeDeduction)]
        Deducting = 13,
        [StatusGroupAttribute(Description = "Fee deduction - Success", IsMiddleStatus = false, BelongTo = PayOperationEnum.FeeDeduction, IsSuccessStatus = true)]
        Deducted = 14,
        [StatusGroupAttribute(Description = "Fee deduction - Failed", IsMiddleStatus = false, BelongTo = PayOperationEnum.FeeDeduction, IsFailedStatus = true)]
        DeductionFailed = 15,

        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.PreauthCancellation)]
        PreAuthCancelRetryLongTime = 16,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.PreauthCompletion)]
        PreAuthCompleteRetryLongTime = 17,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.FeeDeduction)]
        DeductionRetryShortTime = 18,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.FeeDeduction)]
        DeductionRetryLongTime = 19,
        [StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true, BelongTo = PayOperationEnum.Preauth)]
        PreAuthRetryLongTime = 20,
        [StatusGroupAttribute(Description = "No fee", IsMiddleStatus = false)]
        NoFee = 21
    }

    /// <summary>
    /// Payment opertation
    /// </summary>
    public enum PayOperationEnum
    {
        [StatusGroup(Description = "Rental Fee")]
        FeeDeduction = 0,
        [StatusGroup(Description = "Cancellation Fee")]
        CancelFeeDeduction = 1,
        [StatusGroup(Description = "Indirect Fee")]
        IndirectFeeDeduction = 2,
        [StatusGroup(Description = "Preauth")]
        PreauthCancellation = 3,
        [StatusGroup(Description = "Preauth")]
        PreauthCompletion = 4,
        [StatusGroup(Description = "Preauth")]
        Preauth = 5,
        Nothing = -1
    }

    public class StatusGroupAttribute : Attribute
    {
        public string Description { get; set; }
        public bool IsMiddleStatus { get; set; }
        public PayOperationEnum BelongTo { get; set; }
        public bool IsFailedStatus { get; set; }
        public bool IsSuccessStatus { get; set; }

        public StatusGroupAttribute()
        {
            IsMiddleStatus = false;
            IsFailedStatus = false;
            IsSuccessStatus = false;
            BelongTo = PayOperationEnum.Nothing;
        }
    }
}
