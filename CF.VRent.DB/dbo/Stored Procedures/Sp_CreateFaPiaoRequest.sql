-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_CreateFaPiaoRequest]
	@ProxyBookingID int, 
	@FapiaoPreferenceID uniqueidentifier, 
	@FapiaoSource tinyint,
	@State tinyint, 
	@CreatedOn datetime, 
	@CreatedBy uniqueidentifier
	
WITH EXEC AS CALLER AS
BEGIN
   DECLARE @lastID      TABLE (InsertedID   INT)
   DECLARE	@return_value int
   DECLARE  @InsertedID int

   INSERT INTO [dbo].[FapiaoRequests]
           ([ProxyBookingID]
           ,[FapiaoPreferenceID]
           ,[FapiaoSource]
           ,[State]
           ,[CreatedOn]
           ,[CreatedBy])
     OUTPUT inserted.ID INTO @lastID
     VALUES
           (
			   @ProxyBookingID
			   ,@FapiaoPreferenceID
			   ,@FapiaoSource
			   ,@State
			   ,@CreatedOn
			   ,@CreatedBy
           )
set @InsertedID = (SELECT TOP 1 InsertedID FROM @lastID)

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
    WHERE fr.ID = @InsertedID and State != 1

--Valid Fapiao State
-- Active = 0, Deleted = 1,Generated, Exported, Imported, Delivered,Unknown

END