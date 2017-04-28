-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetFapiaoPreferences]
@uid nvarchar(160)
WITH EXEC AS CALLER
AS
BEGIN
   SET  NOCOUNT ON;

   SELECT Unique_ID,
          User_ID,
          Customer_Name,
          Mail_Type,
          Mail_Address,
          Mail_Phone,
          Addressee_Name,
          Fapiao_Type,
          State,
          CreatedOn,
          CreatedBy,
          ModifiedOn,
          ModifiedBy
     FROM GT_FapiaoPreferences
    WHERE User_ID = @uid AND State = 0                 --0: active, 1: deleted
   ORDER BY Customer_Name COLLATE chinese_prc_cs_as_ks_ws
END