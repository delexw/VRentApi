CREATE TABLE [dbo].[DebitNotes] (
    [ID]             INT              IDENTITY (1, 1) NOT NULL,
    [ClientID]       UNIQUEIDENTIFIER NOT NULL,
    [PeriodID]       INT              NOT NULL,
    [BillingDate]    DATETIME         NOT NULL,
    [DueDate]        DATETIME         NOT NULL,
    [PaymentDate]    DATETIME         NULL,
    [TotalAmount]    DECIMAL (18, 3)  NOT NULL,
    [PaymentStatus]  TINYINT          NOT NULL,
    [Note]           NVARCHAR (200)   NULL,
    [State]          TINYINT          NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]     DATETIME         NULL,
    [ModifiedBy]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_DebitNotes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

