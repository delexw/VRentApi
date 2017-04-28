
CREATE PROCEDURE [dbo].[Sp_VrentBookings_UpdateState]
	@BookingType tinyint,
	@KemasBookingID uniqueidentifier,
	@KemasBookingNumber nvarchar(20),

	@DateBegin DateTime,
	@DateEnd DateTime,
	@TotalAmount decimal(10,3),
	@UserID uniqueidentifier ,
	@CorporateID nvarchar(50),		
	
	@State nvarchar(50),
	@ModifiedOn datetime,
    @ModifiedBy uniqueidentifier
AS
BEGIN
	
	Update [dbo].[VrentBookings]
		set  
		BookingType = @BookingType
		,KemasBookingNumber = @KemasBookingNumber
		
		,DateBegin = @DateBegin
		,DateEnd = @DateEnd
		,TotalAmount = @TotalAmount		
		,UserID = @UserID
		,CorporateID = @CorporateID

		,[State] = @State
		,[ModifiedOn] = @ModifiedOn
		,[ModifiedBy] = @ModifiedBy
    where KemasBookingID = @KemasBookingID
    
       SELECT 
        [ID],
        [BookingType]
        ,[KemasBookingID]
        ,[KemasBookingNumber]
        ,[DateBegin]
        ,[DateEnd]
        ,[TotalAmount]
        ,[UserID]
        ,[CorporateID]
        ,[State]
        ,[CreatedOn]
        ,[CreatedBy],
          [ModifiedOn],
          [ModifiedBy]
     FROM VrentBookings AS vb
    WHERE     vb.KemasBookingID = @KemasBookingID
    
    return 0
END