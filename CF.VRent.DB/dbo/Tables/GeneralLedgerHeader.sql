CREATE TABLE [dbo].[GeneralLedgerHeader] (
    [ID]          NUMERIC (18)     IDENTITY (100000000000, 1) NOT NULL,
    [PostingFrom] DATETIME         NOT NULL,
    [PostingEnd]  DATETIME         NOT NULL,
    [HeaderType]  INT              NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__GeneralL__3214EC27339FAB6E] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
);

