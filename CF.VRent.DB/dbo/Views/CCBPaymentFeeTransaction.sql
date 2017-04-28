CREATE VIEW [dbo].[CCBPaymentFeeTransaction]
AS
SELECT vb.ID,
       vb.KemasBookingNumber,
       vb.State AS BookingState,
       '12' AS TransactionState,
       ISNULL (bp.Total, 0) AS Fee
  FROM VrentBookings vb
       LEFT JOIN BookingPrice bp ON vb.ID = bp.BookingID AND bp.state = 0
 WHERE     vb.BookingType = 2
       AND NOT EXISTS
              (SELECT 'A'
                 FROM BookingPayment bp
                WHERE     bp.BookingID = vb.ID
                      AND bp.UPPaymentID > 0
                      AND bp.State = 1)