-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_FapiaoPreferences_UpdateExisting] 

--insert new one
	@NewUnique_ID nvarchar(200),
	@User_ID nvarchar(160),
	@Customer_Name nvarchar(80),
	@Mail_Type nvarchar(80),
	@Mail_Address nvarchar(80),
	@Mail_Phone nvarchar(20),
	@Addressee_Name nvarchar(50),
	@Fapiao_Type numeric(3,0),
	@NewState tinyint,
    @CreatedOn datetime,
    @CreatedBy uniqueidentifier,
--deactivate old one

	@OldUnique_ID nvarchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
DECLARE	@CreateRet int,
@UpdateRet int,
@OverAllRet int

exec @CreateRet = Sp_FapiaoPreferences_Create 
	@Unique_ID = @NewUnique_ID,
	@User_ID=@User_ID,
	@Customer_Name=@Customer_Name,
	@Mail_Type= @Mail_Type,
	@Mail_Address = @Mail_Address,
	@Mail_Phone = @Mail_Phone,
	@Addressee_Name = @Addressee_Name,
	@Fapiao_Type = @Fapiao_Type,
    @State = @NewState,
    @CreatedOn = @CreatedOn,
    @CreatedBy = @CreatedBy

set @OverAllRet = @CreateRet + @OverAllRet  
  
exec @UpdateRet = Sp_FapiaoPreferences_Delete
     @UniqueID = @OldUnique_ID

set @OverAllRet = @UpdateRet + @OverAllRet    
    
END
return @OverAllRet