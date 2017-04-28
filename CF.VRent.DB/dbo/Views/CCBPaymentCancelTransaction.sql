CREATE VIEW [dbo].[CCBPaymentCancelTransaction]
AS
SELECT vb.ID,
       vb.KemasBookingNumber,
       vb.State AS BookingState,
       '3' AS TransactionState,
       ISNULL (bp.Total, 0) AS CancelFee
  FROM VrentBookings vb
       LEFT JOIN BookingPrice bp ON vb.ID = bp.BookingID AND bp.state = 0
 WHERE     vb.BookingType = 2
       AND vb.State IN ('swBookingModel/canceled',
                        'swBookingModel/autocanceled')
       AND NOT EXISTS
              (SELECT 'A'
                 FROM BookingPayment bp
                WHERE     bp.BookingID = vb.ID
                      AND bp.UPPaymentID > 0
                      AND bp.State = 1)