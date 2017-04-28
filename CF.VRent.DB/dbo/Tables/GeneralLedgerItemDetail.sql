CREATE TABLE [dbo].[GeneralLedgerItemDetail] (
    [ID]          NUMERIC (18)     IDENTITY (1, 1) NOT NULL,
    [HeaderID]    NUMERIC (18)     NOT NULL,
    [ItemID]      NUMERIC (18)     NOT NULL,
    [PaymentID]   INT              NULL,
    [DebitNoteID] INT              NULL,
    [DetailType]  NVARCHAR (10)    NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__GeneralL__3214EC273F115E1A] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
);

