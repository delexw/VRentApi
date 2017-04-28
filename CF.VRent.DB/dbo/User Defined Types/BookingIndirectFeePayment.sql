CREATE TYPE [dbo].[BookingIndirectFeePayment] AS TABLE (
    [ID]          INT              NULL,
    [BookingID]   INT              NULL,
    [OrderItemID] INT              NULL,
    [UPPaymentID] INT              NULL,
    [State]       TINYINT          NULL,
    [CreateOn]    DATETIME         NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL);



