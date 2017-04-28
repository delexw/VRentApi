
CREATE PROCEDURE [dbo].[Sp_GetTotalIndirectFeeByOrderID]
@OrderID int
WITH EXEC AS CALLER
AS
DECLARE @Total AS DECIMAL (18, 0)
SELECT @Total = sum (voi.TotalAmount)
FROM UnpaidIndirectFee voi
WHERE voi.OrderID = @OrderID
RETURN @Total
