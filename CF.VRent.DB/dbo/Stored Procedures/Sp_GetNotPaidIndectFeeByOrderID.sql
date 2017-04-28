
CREATE PROCEDURE [dbo].[Sp_GetNotPaidIndectFeeByOrderID]
@OrderID int
WITH EXEC AS CALLER
AS
SELECT *
  FROM UnpaidIndirectFee voi
 WHERE voi.OrderID = @OrderID
