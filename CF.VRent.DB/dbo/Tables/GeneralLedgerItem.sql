CREATE TABLE [dbo].[GeneralLedgerItem] (
    [ID]           NUMERIC (18)     IDENTITY (1, 1) NOT NULL,
    [HeaderID]     NUMERIC (18)     NOT NULL,
    [ItemType]     INT              NOT NULL,
    [PostingBody]  NVARCHAR (500)   NOT NULL,
    [ClientID]     UNIQUEIDENTIFIER NULL,
    [CompanyCode]  NVARCHAR (50)    NULL,
    [BusinessArea] NVARCHAR (10)    NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]   DATETIME         NULL,
    [ModifiedBy]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__GeneralL__3214EC2737703C52] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
);

