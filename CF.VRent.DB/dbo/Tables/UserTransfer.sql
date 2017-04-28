CREATE TABLE [dbo].[UserTransfer] (
    [ID]             INT              IDENTITY (1, 1) NOT NULL,
    [UserID]         UNIQUEIDENTIFIER NOT NULL,
    [FirstName]      NVARCHAR (50)    NULL,
    [LastName]       NVARCHAR (50)    NULL,
    [Mail]           NVARCHAR (50)    NOT NULL,
    [ClientIDFrom]   UNIQUEIDENTIFIER NULL,
    [ClientIDTo]     UNIQUEIDENTIFIER NOT NULL,
    [ApproverID]     UNIQUEIDENTIFIER NULL,
    [TransferResult] TINYINT          NOT NULL,
    [State]          TINYINT          NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]     DATETIME         NULL,
    [ModifiedBy]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_UserTransfer] PRIMARY KEY CLUSTERED ([ID] ASC)
);

