CREATE TABLE [dbo].[IndirectFeeTypes] (
    [ID]         INT              IDENTITY (1, 1) NOT NULL,
    [Type]       NVARCHAR (100)   NULL,
    [Group]      NVARCHAR (50)    NOT NULL,
	[SourceType] tinyint          NOT NULL,
    [Note]       NVARCHAR (100)   NULL,
    [State]      TINYINT          NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn] DATETIME         NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_IndirectFeeTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);



