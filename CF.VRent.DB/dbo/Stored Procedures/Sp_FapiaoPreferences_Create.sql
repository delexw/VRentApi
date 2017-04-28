-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_FapiaoPreferences_Create] 
	@Unique_ID nvarchar(200),
	@User_ID nvarchar(160),
	@Customer_Name nvarchar(80),
	@Mail_Type nvarchar(80),
	@Mail_Address nvarchar(80),
	@Mail_Phone nvarchar(20),
	@Addressee_Name nvarchar(50),
	@Fapiao_Type numeric(3,0),
	@State tinyint,
    @CreatedOn datetime,
    @CreatedBy uniqueidentifier
AS
BEGIN
	
    INSERT INTO GT_FapiaoPreferences
	(
		Unique_ID, 
		User_ID, 
		Customer_Name, 
		Mail_Type, 
		Mail_Address,
		Mail_Phone, 
		Addressee_Name, 
		Fapiao_Type,
		State,
		CreatedOn,
		CreatedBy
	) 
	VALUES 
	(
		@Unique_ID, 
		@User_ID, 
		@Customer_Name, 
		@Mail_Type, 
		@Mail_Address, 
		@Mail_Phone, 
		@Addressee_Name, 
		@Fapiao_Type,
		@State,--0: active, 1: deleted
	    @CreatedOn,
		@CreatedBy
	)
    
END
