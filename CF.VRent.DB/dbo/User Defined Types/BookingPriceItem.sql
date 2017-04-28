CREATE TYPE [dbo].[BookingPriceItem] AS TABLE (
    [PrincingID]  INT              NOT NULL,
    [Description] NVARCHAR (500)   NULL,
    [Group]       NVARCHAR (20)    NOT NULL,
    [Category]    NVARCHAR (20)    NULL,
    [Type]        NVARCHAR (20)    NULL,
    [UnitPrice]   DECIMAL (10, 3)  NULL,
    [Quantity]    DECIMAL (18)     NULL,
    [Total]       DECIMAL (18, 3)  NULL,
    [State]       TINYINT          NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL);



