-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[Sp_CreateFaPiaoRequestItems]
	@ProxyBookingID int, 
	@CreatedOn datetime, 
	@CreatedBy uniqueidentifier

WITH EXEC AS CALLER AS
BEGIN

   INSERT INTO [dbo].[FapiaoRequests]
           ([ProxyBookingID]
           ,[FapiaoPreferenceID]
           ,[FapiaoSource]
           ,[State]
           ,[CreatedOn]
           ,[CreatedBy])
     VALUES
           (
			   @ProxyBookingID
			   ,NULL
			   ,1 --rental
			   ,0
			   ,@CreatedOn
			   ,@CreatedBy
           )
           ,
           (
           	   @ProxyBookingID
			   ,NULL
			   ,2 --indirect fee
			   ,0
			   ,@CreatedOn
			   ,@CreatedBy)

--Valid Fapiao State
-- Active = 0, Deleted = 1,Generated, Exported, Imported, Delivered,Unknown

END