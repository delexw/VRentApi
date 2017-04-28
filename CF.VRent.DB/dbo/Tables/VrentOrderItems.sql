CREATE TABLE [dbo].[VrentOrderItems] (
    [ID]            INT              IDENTITY (1, 1) NOT NULL,
    [OrderID]       INT              NOT NULL,
    [ProductCode]   NVARCHAR (50)    NULL,
    [ProductName]   NVARCHAR (500)   NULL,
    [SpecModel]     NVARCHAR (50)    NULL,
    [Category]      NVARCHAR (15)    NOT NULL,
    [TypeID]        INT              NULL,
    [Type]          NVARCHAR (100)   NOT NULL,
    [UnitOfMeasure] NVARCHAR (50)    NULL,
    [UnitPrice]     DECIMAL (10, 3)  NULL,
    [Quantity]      INT              NULL,
    [NetAmount]     DECIMAL (10, 3)  NOT NULL,
    [TaxRate]       DECIMAL (10, 3)  NOT NULL,
    [TaxAmount]     DECIMAL (10, 3)  NOT NULL,
    [TotalAmount]   DECIMAL (10, 3)  NOT NULL,
    [Remark]        NVARCHAR (50)    NULL,
    [State]         TINYINT          NOT NULL,
    [CreatedOn]     DATETIME         NOT NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]    DATETIME         NULL,
    [ModifiedBy]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [VrentOrderItems_PK] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_VrentOrderItems_VrentOrders] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[VrentOrders] ([ID])
);







