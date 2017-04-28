
CREATE PROCEDURE [dbo].[Sp_GetPaymentExchangeState]
@ID int
WITH EXEC AS CALLER
AS
SELECT State
  FROM UnionPaymentMessageExchange
 WHERE ID = @ID
