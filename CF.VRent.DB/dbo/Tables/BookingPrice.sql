CREATE TABLE [dbo].[BookingPrice] (
    [ID]         INT              IDENTITY (1, 1) NOT NULL,
    [BookingID]  INT              NOT NULL,
    [Total]      DECIMAL (10, 3)  NULL,
    [TimeStamp]  NVARCHAR (20)    NULL,
    [TagID]      NVARCHAR (50)    NULL,
    [state]      TINYINT          NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn] DATETIME         NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_BookingPrice] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_BookingPrice_VrentBookings] FOREIGN KEY ([BookingID]) REFERENCES [dbo].[VrentBookings] ([ID])
);



