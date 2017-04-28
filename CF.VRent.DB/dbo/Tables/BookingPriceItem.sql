CREATE TABLE [dbo].[BookingPriceItem] (
    [ID]          INT              IDENTITY (1, 1) NOT NULL,
    [PrincingID]  INT              NOT NULL,
    [Description] NVARCHAR (500)   NULL,
    [Group]       NVARCHAR (20)    NOT NULL,
    [Category]    NVARCHAR (20)    NULL,
    [Type]        NVARCHAR (20)    NULL,
    [UnitPrice]   DECIMAL (10, 3)  NULL,
    [Quantity]    DECIMAL (10, 3)  NULL,
    [Total]       DECIMAL (10, 3)  NULL,
    [State]       TINYINT          NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_BookingPriceItem] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_BookingPriceItem_BookingPriceItem] FOREIGN KEY ([PrincingID]) REFERENCES [dbo].[BookingPrice] ([ID])
);







