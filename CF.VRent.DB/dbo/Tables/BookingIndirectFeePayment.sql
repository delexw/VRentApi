CREATE TABLE [dbo].[BookingIndirectFeePayment] (
    [ID]          INT              IDENTITY (1, 1) NOT NULL,
    [BookingID]   INT              NOT NULL,
    [OrderItemID] INT              NOT NULL,
    [UPPaymentID] INT              NOT NULL,
    [State]       TINYINT          NOT NULL,
    [CreateOn]    DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__BookingI__3214EC274A4E069C] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 100)
);




GO



GO



GO



GO



GO



GO



GO



GO


