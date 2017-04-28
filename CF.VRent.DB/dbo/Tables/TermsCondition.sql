CREATE TABLE [dbo].[TermsCondition] (
    [ID]           INT              IDENTITY (1, 1) NOT NULL,
    [Title]        NVARCHAR (20)    NULL,
    [DisplayTitle] NVARCHAR (50)    NULL,
    [Content]      NTEXT            NULL,
    [Key]          UNIQUEIDENTIFIER NULL,
    [Type]         INT              NULL,
    [Category]     INT              NULL,
    [Version]      NVARCHAR (20)    NULL,
    [IsNewVersion] TINYINT          NULL,
    [IsActive]     TINYINT          NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]   DATETIME         NULL,
    [ModifiedBy]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__TermsCon__3214EC271D7B6025] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
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


