-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_DebitNoteExcel_PricingCatalog]
@debitNoteID [dbo].[DebitNoteID] READONLY
WITH EXEC AS CALLER
AS
BEGIN
   SET  NOCOUNT ON;

   SELECT Source.DebitnoteID,
          Source.PeriodID,
          Source.clientID,
          vb.KemasBookingID,
          vb.KemasBookingNumber,
          vb.UserID,
          Source.BookingID,
          Source.OrderID,
          cb.StartLocationID AS StationID,
          cb.StartLocationName AS StationName,
          cb.DateBegin,
          cb.DateEnd,
          cb.KeyOut,
          cb.KeyIn,
          cb.CarID,
          cb.Category AS CarCategory,
          cb.CarName AS CarModel,
          cb.[State] AS BookingState,
          Source.OrderMonth,
          Source.[Group],
          Source.Category,
          Source.[Type],
          ift.Note AS Description,
          Source.TotalAmount
     FROM (SELECT dnh.DebitnoteID,
                  dnh.PeriodID,
                  dnh.clientID,
                  vb.ID AS BookingID,
                  vo.ID AS OrderID,
                  CONVERT (NVARCHAR (6),
                           DATEADD (day, 1, voi.CreatedOn),
                           112)
                     AS OrderMonth,
                  'INDIRECTFEE' AS [Group],
                  CASE
                     WHEN voi.[Type] IN ('Late Ticket Payment Fee',
                                         'Repair Cost',
                                         'Indirect Fee Deduction Failed Penality Fee (0.5% Per Day)')
                     THEN
                        'Primary'
                     WHEN voi.[Type] NOT IN ('Late Ticket Payment Fee',
                                             'Repair Cost',
                                             'Indirect Fee Deduction Failed Penality Fee (0.5% Per Day)')
                     THEN
                        'Additional'
                  END
                     AS Category,
                  voi.Type,
                  voi.TypeID AS TypeID,
                  sum (voi.TotalAmount) AS TotalAmount
             FROM dbo.VrentOrderItems AS voi
                  INNER JOIN VrentOrders AS vo ON voi.OrderID = vo.ID
                  INNER JOIN VrentBookings AS vb ON vb.ID = vo.ProxyBookingID
                  INNER JOIN
                  (SELECT dn.ID AS DebitnoteID,
                          dnh.ID AS PeriodID,
                          dnh.PeriodBegin,
                          dnh.PeriodEnd,
                          dn.clientID
                     FROM @debitNoteID AS targetNotes
                          INNER JOIN DebitNotes AS dn
                             ON dn.ID = targetNotes.ID
                          INNER JOIN DebitNoteHistory AS dnh
                             ON dnh.ID = dn.PeriodID) AS dnh
                     ON     vb.CorporateID = dnh.clientID
                        AND voi.CreatedOn >= dnh.PeriodBegin
                        AND voi.CreatedOn < dnh.PeriodEnd
            WHERE     vb.BookingType = 2
                  AND voi.Category = 'INDIRECTFEE'
                  AND voi.TotalAmount > 0
                  AND vo.State = 0
                  AND voi.State = 0
           GROUP BY dnh.PeriodID,
                    dnh.clientID,
                    dnh.DebitnoteID,
                    vb.ID,
                    vo.ID,
                    CONVERT (NVARCHAR (6),
                             DATEADD (day, 1, voi.CreatedOn),
                             112),
                    voi.Type,
                    voi.TypeID) AS Source
          LEFT JOIN IndirectFeeTypes AS ift ON ift.ID = source.TypeID
          INNER JOIN VrentBookings AS vb ON vb.ID = source.BookingID
          LEFT JOIN CompletedBookings AS cb
             ON cb.KemasBookingID = vb.KemasBookingID
   UNION ALL
   SELECT Raw.DebitNoteID,
          Raw.PeriodID,
          Raw.clientID,
          Raw.KemasBookingID,
          Raw.KemasBookingNumber,
          Raw.UserID,
          Raw.BookingID,
          Raw.OrderID,
          Raw.StationID,
          Raw.StationName,
          Raw.DateBegin,
          Raw.DateEnd,
          Raw.KeyOut,
          Raw.KeyIn,
          Raw.CarID,
          Raw.CarCategory,
          Raw.CarModel,
          Raw.BookingState,
          Raw.OrderMonth,
          'RENTALFEE' AS [Group],
          Child.value ('local-name(.)', 'VARCHAR(20)') AS Category,
          ISNULL (Item.value ('@type', 'VARCHAR(20)'),
                  Child.value ('local-name(.)', 'VARCHAR(20)'))
             AS Type,
          Item.value ('@description', 'varchar(50)') AS Description,
          CASE
             WHEN Item.value ('@total', 'VARCHAR(20)') = ''
             THEN
                0
             WHEN Item.value ('@total', 'VARCHAR(20)') IS NULL
             THEN
                0
             WHEN Item.value ('@total', 'VARCHAR(20)') IS NOT NULL
             THEN
                Item.value ('@total', 'decimal(10,3)')
          END
             AS TotalAmount
     FROM (SELECT dnh.DebitNoteID,
                  dnh.PeriodID,
                  dnh.clientID AS ClientID,
                  cb.KemasBookingID,
                  vb.ID AS BookingID,
                  CASE
                     WHEN (cb.KeyIn IS NULL OR cb.KeyOut IS NULL)
                     THEN
                        CONVERT (NVARCHAR (6), cb.DateEnd, 112)
                     WHEN (cb.KeyIn IS NOT NULL AND cb.KeyOut IS NOT NULL)
                     THEN
                        CONVERT (NVARCHAR (6), cb.KeyIn, 112)
                  END
                     AS OrderMonth,
                  vo.ID AS OrderID,
                  CAST (cb.PricingDetail AS XML) AS Pricing,
                  cb.UserID,
                  cb.KemasBookingNumber,
                  cb.DateBegin,
                  cb.DateEnd,
                  cb.KeyIn,
                  cb.KeyOut,
                  cb.StartLocationID AS StationID,
                  cb.StartLocationName AS StationName,
                  cb.CarID,
                  cb.CarName AS CarModel,
                  cb.Category AS CarCategory,
                  cb.State AS BookingState
             FROM completedBookings AS cb
                  INNER JOIN
                  (SELECT dn.ID AS DebitNoteID,
                          dnh.ID AS PeriodID,
                          dnh.PeriodBegin,
                          dnh.PeriodEnd,
                          dn.clientID
                     FROM @debitNoteID AS targetNotes
                          INNER JOIN DebitNotes AS dn
                             ON dn.ID = targetNotes.ID
                          INNER JOIN DebitNoteHistory AS dnh
                             ON dnh.ID = dn.PeriodID) AS dnh
                     ON     cb.CorporateID = dnh.clientID
                        AND (CASE
                                WHEN (cb.KeyIn IS NULL OR cb.KeyOut IS NULL)
                                THEN
                                   cb.DateEnd
                                WHEN (    cb.KeyIn IS NOT NULL
                                      AND cb.KeyOut IS NOT NULL)
                                THEN
                                   cb.KeyOut
                             END) >= dnh.PeriodBegin
                        AND (CASE
                                WHEN (cb.KeyIn IS NULL OR cb.KeyOut IS NULL)
                                THEN
                                   cb.DateEnd
                                WHEN (    cb.KeyIn IS NOT NULL
                                      AND cb.KeyOut IS NOT NULL)
                                THEN
                                   cb.KeyIn
                             END) < dnh.PeriodEnd
                  LEFT JOIN VrentBookings AS vb
                     ON cb.KemasBookingID = vb.KemasBookingID
                  LEFT JOIN VrentOrders AS vo ON vo.ProxyBookingID = vb.ID)
          AS Raw
          OUTER APPLY Raw.Pricing.nodes ('./Price/*') AS Source (Child)
          OUTER APPLY Child.nodes ('item') AS Rental (Item)
   ORDER BY PeriodID, clientID, DebitnoteID
END