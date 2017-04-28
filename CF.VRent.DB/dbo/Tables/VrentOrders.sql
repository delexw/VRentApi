CREATE TABLE [dbo].[VrentOrders] (
    [ID]             INT              IDENTITY (1, 1) NOT NULL,
    [ProxyBookingID] INT              NOT NULL,
    [BookingUserID]  UNIQUEIDENTIFIER NOT NULL,
    [State]          TINYINT          NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]     DATETIME         NULL,
    [ModifiedBy]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__VrentOrd__3214EC271EC48A19] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100),
    CONSTRAINT [FK_VrentOrders_VrentBookings] FOREIGN KEY ([ProxyBookingID]) REFERENCES [dbo].[VrentBookings] ([ID])
);





