CREATE PROCEDURE [dbo].[Sp_VrentOrders_CreateAndItems]
@ProxyBId int, 
@BookingUserID uniqueidentifier, 
@State tinyint, 
@CreatedBy uniqueidentifier
WITH EXEC AS CALLER
AS
   DECLARE @lastID      TABLE (InsertedID INT,createdOn datetime)
   DECLARE  @InsertedID int
   DECLARE  @CreatedOn datetime
Begin
	if not exists(select * from VrentOrders as vo where vo.ProxyBookingID = @ProxyBId and State = 0)
		Begin
			INSERT INTO VrentOrders (ProxyBookingID,
									 BookingUserID,
									 State,
									 CreatedOn,
									 CreatedBy)

		   OUTPUT inserted.ID,inserted.CreatedOn INTO @lastID
	     
 			VALUES (@ProxyBId,
				@BookingUserID,
				@State,
				getdate (),
				@CreatedBy)
		
			set @InsertedID = (SELECT TOP 1 InsertedID FROM @lastID)
			set @CreatedOn = (SELECT TOP 1 createdOn FROM @lastID)
		
			EXEC	
				[dbo].[Sp_VrentOrders_AppendItems]
				@ProxyBookingID = @ProxyBId,
				@OrderID = @InsertedID,
				@State = @State,
				@CreatedOn = @CreatedOn,
				@CreatedBy = @CreatedBy	
		END	
END