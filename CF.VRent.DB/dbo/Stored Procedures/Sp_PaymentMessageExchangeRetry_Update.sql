CREATE PROCEDURE [dbo].[Sp_PaymentMessageExchangeRetry_Update]
@ID int, @Flag int, @UserID nvarchar(50)
WITH EXEC AS CALLER
AS
UPDATE UnionPaymentMessageExchange
   SET Retry = @Flag,
       ModifiedOn = GETDATE (),
       ModifiedBy = @UserID,
       Retry_On = GETDATE ()
 WHERE ID = @ID