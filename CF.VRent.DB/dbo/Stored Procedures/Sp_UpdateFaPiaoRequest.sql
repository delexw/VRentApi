-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_UpdateFaPiaoRequest]
	@ProxyBookingID int, 
	
	@OperatorID uniqueidentifier,
	@CorporateID uniqueidentifier,
	
	@FapiaoPreferenceID uniqueidentifier, 
	@FapiaoSource tinyint,
	@State tinyint, 
	@ModifiedOn datetime,
    @ModifiedBy uniqueidentifier,

    @return_value int output
    
--Ret
--public enum UpdateFapiaoRequestResult { Success = 0, InvalidOperator = 1 , InvalidFP = 2,BadDataExist = 3, UnChangeableState = 4 };
-- 0: update one
-- 1: invalid operator
-- 2: invalid FapiaoPreference
-- 3: bad data exists
-- 4: in unchangeable state


WITH EXEC AS CALLER AS

DECLARE @existingID int
DECLARE @lastID      TABLE (InsertedID   INT)
	--check result
Declare	@IsBookingOnwer int,
	@IsFapiaoPreferenceOwner int



BEGIN
	SET NOCOUNT ON
	
	--check
	EXEC @IsBookingOnwer = [dbo].[Sp_IsBookingOwner]
		@ProxyBookingID = @ProxyBookingID,
		@OperatorID = @OperatorID
	
	if(@IsBookingOnwer = 1)
	    Begin
			if(@FapiaoPreferenceID is not NULL)
				Begin
					EXEC @IsFapiaoPreferenceOwner = [dbo].[Sp_IsFapiaoPreferenceOwner]
						@OperatorID = @OperatorID,
						@FapaioPreferenceID = @FapiaoPreferenceID
				End
			else
				begin
					set @IsFapiaoPreferenceOwner = 1
				end
			
			if(@IsFapiaoPreferenceOwner = 1)
				begin	
					UPDATE [dbo].[FapiaoRequests]		
						SET
							[FapiaoPreferenceID] = @FapiaoPreferenceID
							,[State] = @State
							,[ModifiedOn] = @ModifiedOn
							,[ModifiedBy] = @ModifiedBy
					OUTPUT inserted.ID INTO @lastID 
						WHERE
							ProxyBookingID = @ProxyBookingID
							AND FapiaoSource = @FapiaoSource
							AND [State] = 0
			
					--bad data exists
					if(@@ROWCOUNT = 1)
						Begin	
							set @return_value = 0
						End
					Else if(@@ROWCOUNT = 0)
						Begin
							set @return_value = 3
						End
					Else if(@@ROWCOUNT > 1)
						Begin
							set @return_value = 4
						End

					set @existingID = (SELECT TOP 1 InsertedID FROM @lastID)
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
					WHERE fr.ID = @existingID and State != 1
				End
			else
				begin
					set @return_value = 2
				End
		End
    else
		Begin
			Set @return_value = 1
		End
--Valid Fapiao State
-- Active = 0, Deleted = 1,Generated = 2, Exported = 3, Imported = 4, Delivered = 5
END

return @return_value