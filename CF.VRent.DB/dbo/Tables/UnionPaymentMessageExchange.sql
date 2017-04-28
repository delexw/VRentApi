CREATE TABLE [dbo].[UnionPaymentMessageExchange] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
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
    CONSTRAINT [PK__UnionPay__3214EC271367E606] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
);














GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1：OK/0：Cancel', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UnionPaymentMessageExchange', @level2type = N'COLUMN', @level2name = N'State';


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



GO

CREATE TRIGGER [ArchivePaymentMessage] ON [dbo].[UnionPaymentMessageExchange]
WITH EXEC AS CALLER
AFTER UPDATE
AS
DECLARE @ID   INT
DECLARE
   @OldState   INT,
   @NewState   INT

IF (UPDATE (State))
   BEGIN
      SELECT @OldState = State FROM deleted;
      SELECT @NewState = State FROM inserted;

      IF (@OldState != @NewState)
         BEGIN
            INSERT INTO UnionPaymentMessageHistory (ArchiveTime,
                                                    PaymentMessageID,
                                                    Unique_ID,
                                                    [Message],
                                                    CreatedOn,
                                                    State,
                                                    Retry_Count,
                                                    Operation,
                                                    [User_ID],
                                                    PreAuthID,
                                                    PreAuthQueryID,
                                                    PreAuthDateTime,
                                                    PreAuthPrice,
                                                    RealPreAuthPrice,
                                                    PreAuthTempOrderID,
                                                    DeductionPrice,
                                                    SmsCode,
                                                    Card_ID,
                                                    ModifiedBy,
                                                    ModifiedOn,
                                                    LastPaymentID,
                                                    Retry,
                                                    Retry_On)
               SELECT getDate () AS ArchiveTime, * FROM deleted
         END
   END