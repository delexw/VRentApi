-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[Sp_RetrieveBookingsByUserID] 
	(@UserID uniqueidentifier,
	@KemasBookingState dbo.BookingState readonly)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[ID]
      ,[BookingType]
      ,[KemasBookingID]
      ,[KemasBookingNumber]
      ,[DateBegin]
      ,[DateEnd]
      ,[TotalAmount]

      ,[UserID]
      ,[UserFirstName]
      ,[UserLastName]

      ,[CorporateID]
      ,[CorporateName]

      ,[CreatorID]
      ,[CreatorFirstName]
      ,[CreatorLastName]

      ,[StartLocationID]
      ,[StartLocationName]

      ,[State]
      ,[CreatedOn]
      ,[CreatedBy]
      ,[ModifiedOn]
      ,[ModifiedBy]
	FROM [dbo].[VrentBookings]
	Where UserID = @UserID 
	and [State] in (select kbs.KemasBookingState from @KemasBookingState as kbs)
END
