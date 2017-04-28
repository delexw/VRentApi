CREATE TYPE [dbo].[IndirectFeeType] AS TABLE (
    [Type]       NVARCHAR (100)    NOT NULL,
    [Group]      NVARCHAR (50)    NOT NULL,
	[SourceType] tinyint          NOT NULL,
    [Note]       NVARCHAR (100)   NULL,
    [State]      TINYINT          NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn] DATETIME         NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL);

