
CREATE VIEW [dbo].[CCBPaymentIndirectFeeTransaction]
AS
SELECT a.ID,
       a.KemasBookingNumber,
       a.BookingState,
       a.TransactionState,
       ISNULL (sum (a.TotalAmount), 0) AS IndirectFee
  FROM (SELECT vb.ID,
               vb.KemasBookingNumber,
               vb.State AS BookingState,
               '12' AS TransactionState,
               voi.TotalAmount,
               voi.ID AS ItemID
          FROM VrentBookings vb
               INNER JOIN VrentOrders vo
                  ON     vb.ID = vo.ProxyBookingID
                     AND vb.BookingType = 2
                     AND vb.State = 'swBookingModel/completed'
               LEFT JOIN VrentOrderItems voi
                  ON     vo.ID = voi.OrderID
                     AND voi.Category = 'INDIRECTFEE'
                     AND voi.State = 0
                     AND voi.TypeID IS NOT NULL) a
 WHERE NOT EXISTS
          (SELECT 'A'
             FROM BookingIndirectFeePayment bfp
            WHERE     bfp.BookingID = a.ID
                  AND bfp.OrderItemID = a.ItemID
                  AND bfp.UPPaymentID > 0
                  AND bfp.State = 1)
GROUP BY a.ID,
         a.KemasBookingNumber,
         a.BookingState,
         a.TransactionState