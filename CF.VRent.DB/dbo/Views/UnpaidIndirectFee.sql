CREATE VIEW [dbo].[UnpaidIndirectFee]
AS
SELECT *
  FROM VrentOrderItems voi
 WHERE     voi.Category = 'INDIRECTFEE'
       AND voi.State = 0
       AND NOT EXISTS
              (SELECT 'A'
                 FROM BookingIndirectFeePayment bifp, VrentOrders vo
                WHERE     vo.ID = voi.OrderID
                      AND vo.ProxyBookingID = bifp.BookingID
                      AND voi.ID = bifp.OrderItemID
                      AND bifp.UPPaymentID > 0
                      AND bifp.State = 1)
