CREATE PROCEDURE [dbo].[Sp_PaymentMessageExchangeState_Update]
@ID int, @State int, @Operation nvarchar(50), @UserID nvarchar(50)
WITH EXEC AS CALLER
AS
UPDATE UnionPaymentMessageExchange
   SET State = @State,
       Operation = @Operation,
       ModifiedOn = getdate (),
       ModifiedBy = @UserID
 WHERE ID = @ID
