-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[Sp_RetrieveFapiaoRequestsByFapiaoRequestID]
	@FapiaoRequestID int
WITH EXEC AS CALLER AS
BEGIN

	SELECT 
			[ID]
		  ,[ProxyBookingID]
		  ,[FapiaoPreferenceID]
		  ,[FapiaoSource]
		  ,[State]
		  ,[CreatedOn]
		  ,[CreatedBy]
		  ,[ModifiedOn]
		  ,[ModifiedBy]
	  FROM [dbo].[FapiaoRequests] as fr
		WHERE fr.ID = @FapiaoRequestID and State != 1

--Valid Fapiao State
-- Active = 0, Deleted = 1,Generated, Exported, Imported, Delivered,Unknown

END