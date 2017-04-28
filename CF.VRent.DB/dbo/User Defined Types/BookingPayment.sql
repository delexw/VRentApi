CREATE TYPE [dbo].[BookingPayment] AS TABLE (
    [BookingID]   INT              NOT NULL,
    [UPPaymentID] INT              NOT NULL,
    [state]       TINYINT          NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL);

