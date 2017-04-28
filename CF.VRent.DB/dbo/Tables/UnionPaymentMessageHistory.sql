CREATE TABLE [dbo].[UnionPaymentMessageHistory] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [ArchiveTime]        DATETIME       NULL,
    [PaymentMessageID]   INT            NULL,
    [Unique_ID]          NVARCHAR (50)  NOT NULL,
    [Message]            NVARCHAR (MAX) NULL,
    [CreatedOn]          DATETIME       NOT NULL,
    [State]              INT            DEFAULT ((0)) NULL,
    [Retry_Count]        INT            DEFAULT ((0)) NULL,
    [Operation]          NVARCHAR (50)  NOT NULL,
    [User_ID]            NVARCHAR (50)  NULL,
    [PreAuthID]          NVARCHAR (MAX) NOT NULL,
    [PreAuthQueryID]     NVARCHAR (MAX) NOT NULL,
    [PreAuthDateTime]    NVARCHAR (50)  NULL,
    [PreAuthPrice]       NVARCHAR (10)  NULL,
    [RealPreAuthPrice]   NVARCHAR (10)  NULL,
    [PreAuthTempOrderID] NVARCHAR (50)  NULL,
    [DeductionPrice]     NVARCHAR (10)  NULL,
    [SmsCode]            NVARCHAR (MAX) NOT NULL,
    [Card_ID]            NVARCHAR (50)  NULL,
    [ModifiedBy]         NVARCHAR (50)  NULL,
    [ModifiedOn]         DATETIME       NULL,
    [LastPaymentID]      INT            NULL,
    [Retry]              BIT            NULL,
    [Retry_On]           DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1：OK/0：Cancel', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UnionPaymentMessageHistory', @level2type = N'COLUMN', @level2name = N'State';

