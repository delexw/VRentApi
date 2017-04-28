-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_FapiaoPreferences_Delete] 
	@UniqueID nvarchar(200)
AS	
BEGIN
	
	SET NOCOUNT ON;
	
	UPDATE GT_FapiaoPreferences 
	set State = 1 --0: active, 1: deleted
	WHERE Unique_ID = @UniqueID
    
END
