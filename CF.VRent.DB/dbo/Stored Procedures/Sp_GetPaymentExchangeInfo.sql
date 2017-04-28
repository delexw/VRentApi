CREATE PROCEDURE [dbo].[Sp_GetPaymentExchangeInfo]
@ID int
WITH EXEC AS CALLER
AS
SELECT *
  FROM UnionPaymentMessageExchange
 WHERE ID = @ID
