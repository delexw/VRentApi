-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetFapiaoPreferenceByUniqueID]
	@UniqueID nvarchar(200)
AS
BEGIN
	
	SET NOCOUNT ON;
    
    SELECT Unique_ID, User_ID, Customer_Name, Mail_Type, Mail_Address, Mail_Phone, 
    Addressee_Name, Fapiao_Type, State,
    CreatedOn,
	CreatedBy,
	ModifiedOn,
	ModifiedBy FROM GT_FapiaoPreferences 
	WHERE Unique_ID = @UniqueID 
	--and [State] != 1
    
END
