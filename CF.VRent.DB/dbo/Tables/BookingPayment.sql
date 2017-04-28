CREATE TABLE [dbo].[BookingPayment] (
    [ID]          INT              IDENTITY (1, 1) NOT NULL,
    [BookingID]   INT              NOT NULL,
    [UPPaymentID] INT              NOT NULL,
    [state]       TINYINT          NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_BookingPayment] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_BookingPayment_VrentBookings] FOREIGN KEY ([BookingID]) REFERENCES [dbo].[VrentBookings] ([ID])
);



