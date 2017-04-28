CREATE TABLE [dbo].[DUBDetail] (
    [ID]                 INT              IDENTITY (1, 1) NOT NULL,
    [DUBClosingID]       INT              NULL,
    [BookingID]          INT              NOT NULL,
    [KemasBookingID]     UNIQUEIDENTIFIER NOT NULL,
    [KemasBookingNumber] NVARCHAR (20)    NOT NULL,
    [UserID]             UNIQUEIDENTIFIER NOT NULL,
    [OrderDate]          DATETIME         NOT NULL,
    [PaymentID]          INT              NULL,
    [UPState]            TINYINT          NULL,
    [OrderID]            INT              NULL,
    [OrderItemID]        INT              NULL,
    [Category]           NVARCHAR (20)    NULL,
    [TotalAmount]        DECIMAL (18, 3)  NULL,
    [State]              TINYINT          NOT NULL,
    [CreatedOn]          DATETIME         NOT NULL,
    [CreatedBy]          UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]         DATETIME         NULL,
    [ModifiedBy]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_DUBMonthlyDetail] PRIMARY KEY CLUSTERED ([ID] ASC)
);



