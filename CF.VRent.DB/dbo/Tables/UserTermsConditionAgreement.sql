CREATE TABLE [dbo].[UserTermsConditionAgreement] (
    [ID]         INT              IDENTITY (1, 1) NOT NULL,
    [UserID]     UNIQUEIDENTIFIER NULL,
    [TCID]       INT              NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn] DATETIME         NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__UserTermsConditions] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100),
    CONSTRAINT [FK__UserTermsConditions__TCID] FOREIGN KEY ([TCID]) REFERENCES [dbo].[TermsCondition] ([ID])
);














GO



GO


