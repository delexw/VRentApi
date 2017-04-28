CREATE TABLE [dbo].[UnionPaymentLog] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [CreatedOn]          DATETIME       NULL,
    [UPApi_txnType]      NVARCHAR (5)   NULL,
    [UPApi_txnSubType]   NVARCHAR (10)  NULL,
    [UPApi_bizType]      NVARCHAR (10)  NULL,
    [UPApi_channelType]  NVARCHAR (5)   NULL,
    [User_ID]            NVARCHAR (50)  NULL,
    [Encrpty_Message]    NVARCHAR (MAX) NULL,
    [Order_ID]           NVARCHAR (50)  NULL,
    [UPApi_txnTime]      NVARCHAR (50)  NULL,
    [UPApi_currencyCode] NVARCHAR (10)  NULL,
    [Operation_Type]     INT            NULL,
    [UPApi_queryId]      NVARCHAR (50)  NULL,
    [UPApi_traceNo]      NVARCHAR (50)  NULL,
    [UPApi_traceTime]    NVARCHAR (50)  NULL,
    [UPApi_UniqueID]     NVARCHAR (500) NULL,
    [UpiApi_respCode]    NVARCHAR (10)  NULL,
    [UpiApi_respMsg]     NVARCHAR (512) NULL,
    [CreatedBy]          NVARCHAR (50)  NULL,
    CONSTRAINT [PK__UnionPay__3214EC27778AC167] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
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



GO



GO



GO



GO



GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Sensitive data from UP', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UnionPaymentLog', @level2type = N'COLUMN', @level2name = N'Encrpty_Message';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Request/Response/MerInform', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UnionPaymentLog', @level2type = N'COLUMN', @level2name = N'Operation_Type';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Custom Guid or UPApi(queryId)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UnionPaymentLog', @level2type = N'COLUMN', @level2name = N'UPApi_UniqueID';

