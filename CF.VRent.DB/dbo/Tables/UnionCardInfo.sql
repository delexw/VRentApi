CREATE TABLE [dbo].[UnionCardInfo] (
    [ID]              INT              IDENTITY (1, 1) NOT NULL,
    [Encrpty_Card_No] NVARCHAR (MAX)   NOT NULL,
    [Card_User_Tel]   NVARCHAR (MAX)   NOT NULL,
    [User_ID]         UNIQUEIDENTIFIER NOT NULL,
    [Encrypt_Token]   NVARCHAR (MAX)   NOT NULL,
    [Binding_Time]    DATETIME         NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]      DATETIME         NULL,
    [ModifiedBy]      UNIQUEIDENTIFIER NULL,
    [Bank_Code]       NVARCHAR (10)    NULL,
    [Card_ID]         NVARCHAR (50)    NULL,
    [State]           INT              DEFAULT ((1)) NULL,
    CONSTRAINT [PK__UnionCar__3214EC275303482E] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
);






GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO



GO


