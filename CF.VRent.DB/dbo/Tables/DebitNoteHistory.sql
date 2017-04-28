CREATE TABLE [dbo].[DebitNoteHistory] (
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Period] [nvarchar](50) NOT NULL,
	[PeriodBegin] [datetime] NOT NULL,
	[PeriodEnd] [datetime] NOT NULL,
	[BillingDate] [datetime] NOT NULL,
	[DueDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[ModifiedOn] [datetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
    CONSTRAINT [PK_DebitNoteHistory] PRIMARY KEY CLUSTERED ([ID] ASC)
);



GO
