CREATE TABLE [dbo].[BusinessArea] (
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    [Branch]           NVARCHAR (20) NOT NULL,
    [BranchCHE]        NVARCHAR (20) NULL,
    [ParentID]         INT           NOT NULL,
    [CompanyCode]      NVARCHAR (10) NOT NULL,
    [BusinessAreaCode] NVARCHAR (10) NOT NULL,
    [State]            INT           NOT NULL,
    CONSTRAINT [PK__Business__3214EC2765370702] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
);

