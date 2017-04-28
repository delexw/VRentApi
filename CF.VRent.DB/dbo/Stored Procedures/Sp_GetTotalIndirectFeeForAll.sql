CREATE PROCEDURE [dbo].[Sp_GetTotalIndirectFeeForAll]
WITH EXEC AS CALLER
AS
SELECT vo.ID AS OrderID,
       vb.ID AS BookingID,
       fee.feeCount AS Fee,
       uc.Card_ID AS CardID,
       vb.UserID,
       Stuff ( (SELECT ',' + CONVERT (NVARCHAR, vois.ID)
                  FROM UnpaidIndirectFee vois
                 WHERE vois.OrderID = fee.OrderID
                FOR XML PATH ( '' )),
              1,
              1,
              '')
          AS OrderItemIDs
  FROM VrentBookings vb,
       VrentOrders vo,
       (SELECT sum (voi.TotalAmount) AS feeCount, voi.OrderID
          FROM UnpaidIndirectFee voi
        GROUP BY voi.OrderID) AS fee,
       UnionCardInfo uc
 WHERE     vb.ID = vo.ProxyBookingID
       AND fee.OrderID = vo.ID
       AND vb.UserID = uc.[User_ID]
       AND vb.State = 'swBookingModel/completed'
       AND uc.State = 1
       AND vb.BookingType = 3
