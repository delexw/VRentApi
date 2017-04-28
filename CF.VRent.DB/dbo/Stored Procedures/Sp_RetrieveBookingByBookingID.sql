-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[Sp_RetrieveBookingByBookingID]
	@ProxyBookingID int
AS
BEGIN
	
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
  WHERE ID = @ProxyBookingID AND [State] != 'swBookingModel/deleted'; 
  
END
