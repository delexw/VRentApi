-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_IsFapiaoPreferenceOwner]	
	@OperatorID uniqueidentifier,
	@FapaioPreferenceID uniqueidentifier


--Ret
-- 1: valid
-- 0: invalid
	
WITH EXEC AS CALLER AS
declare @return_value int
BEGIN
    set @return_value = 
    (
		Select COUNT(*) from GT_FapiaoPreferences as fp
		where 
			fp.[User_ID] = @OperatorID 
			AND fp.Unique_ID = @FapaioPreferenceID
			AND fp.[State] != 1
	)
END

return @return_value