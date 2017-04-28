CREATE PROCEDURE [dbo].[SP_VrentBookings_BulkSync]
@BulkSinkInput [dbo].[Booking] READONLY, @InsertCnt int OUTPUT, @UpdatedCnt int OUTPUT
WITH EXEC AS CALLER
AS
BEGIN
   --   Insert into VrentBookings
   -- ([BookingType]
   -- ,[KemasBookingID]
   -- ,[KemasBookingNumber]
   -- ,[DateBegin]
   -- ,[DateEnd]
   -- ,[TotalAmount]
   -- ,[UserID]
   -- ,[UserFirstName]
   -- ,[UserLastName]
   -- ,[CorporateID]
   -- ,[CorporateName]
   -- ,[CreatorID]
   -- ,[CreatorFirstName]
   -- ,[CreatorLastName]
   -- ,[StartLocationID]
   -- ,[StartLocationName]
   -- ,[State]
   -- ,[CreatedOn]
   -- ,[CreatedBy])
   --select
   -- bookings.[BookingType]
   -- ,bookings.[KemasBookingID]
   -- ,bookings.[KemasBookingNumber]
   -- ,bookings.[DateBegin]
   -- ,bookings.[DateEnd]
   -- ,bookings.[TotalAmount]
   -- ,bookings.[UserID]
   -- ,bookings.[UserFirstName]
   -- ,bookings.[UserLastName]
   -- ,bookings.[CorporateID]
   -- ,bookings.[CorporateName]
   -- ,bookings.[CreatorID]
   -- ,bookings.[CreatorFirstName]
   -- ,bookings.[CreatorLastName]
   -- ,bookings.[StartLocationID]
   -- ,bookings.[StartLocationName]
   -- ,bookings.[State]
   -- ,bookings.[CreatedOn]
   -- ,bookings.[CreatedBy]
   --from @BulkSinkInput as bookings
   --where not exists
   --(
   -- select * from VrentBookings as vb
   -- where vb.KemasBookingID = bookings.KemasBookingID
   -- and vb.State Not in ('swBookingModel/canceled','swBookingModel/completed','swBookingModel/autocanceled')
   --)

   --Set @InsertCnt = @@ROWCOUNT;

   --UPDATE syncTable
   --SET [BookingType] = bookings.BookingType
   --     ,[KemasBookingID] = bookings.KemasBookingID
   --     ,[KemasBookingNumber] = bookings.KemasBookingNumber
   --     ,[DateBegin] = bookings.DateBegin
   --     ,[DateEnd] = bookings.DateEnd
   --     ,[TotalAmount] = bookings.TotalAmount
   --     ,[UserID] = bookings.UserID
   --     ,[UserFirstName] = bookings.UserFirstName
   --     ,[UserLastName] = bookings.UserLastName
   --     ,[CorporateID] = bookings.CorporateID
   --     ,[CorporateName] = bookings.CorporateName
   --     ,[CreatorID] = bookings.CreatorID
   --     ,[CreatorFirstName] = bookings.CreatorFirstName
   --     ,[CreatorLastName] = bookings.CreatorLastName
   --     ,[StartLocationID] = bookings.StartLocationID
   --     ,[StartLocationName] = bookings.StartLocationName
   --     --,[State] = bookings.State
   --     ,[ModifiedOn] = bookings.ModifiedOn
   --     ,[ModifiedBy] = bookings.ModifiedBy
   -- from VrentBookings as syncTable inner join @BulkSinkInput as bookings
   -- on syncTable.KemasBookingID = bookings.KemasBookingID

   -- Set @UpdatedCnt = @@ROWCOUNT;

   SELECT vb.[ID],
          vb.[BookingType],
          vb.[KemasBookingID],
          vb.[KemasBookingNumber],
          vb.[DateBegin],
          vb.[DateEnd],
          vb.TotalAmount,
          --          CASE vb.[State]
          --             WHEN 'swBookingModel/autocanceled' THEN Orders.TotalAmount
          --             WHEN 'swBookingModel/canceled' THEN Orders.TotalAmount
          --             WHEN 'swBookingModel/completed' THEN orders.TotalAmount
          --             ELSE vb.TotalAmount
          --          END
          --             AS TotalAmount,
          vb.[UserID],
          vb.[UserFirstName],
          vb.[UserLastName],
          vb.[CorporateID],
          vb.[CorporateName],
          vb.[CreatorID],
          vb.[CreatorFirstName],
          vb.[CreatorLastName],
          vb.[StartLocationID],
          vb.[StartLocationName],
          vb.[State],
          vb.[CreatedOn],
          vb.[CreatedBy],
          vb.[ModifiedOn],
          vb.[ModifiedBy],
          Orders.TotalAmount AS IndirectFeeAmount,
          bookings.TotalAmount AS CurrentTotalAmount
     FROM VrentBookings AS vb
          INNER JOIN @BulkSinkInput AS bookings
             ON bookings.KemasBookingID = vb.KemasBookingID
          CROSS APPLY
          (SELECT isnull (sum (voi.TotalAmount), 0) AS TotalAmount
             FROM VrentOrders AS vo
                  INNER JOIN VrentOrderItems AS voi ON voi.OrderID = vo.ID
            WHERE vo.ProxyBookingID = vb.ID AND voi.Category = 'INDIRECTFEE')
          AS Orders
END
