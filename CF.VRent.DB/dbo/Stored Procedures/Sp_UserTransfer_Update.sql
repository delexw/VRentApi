-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_UserTransfer_Update]
    @Role nvarchar(50),
	@VMClientID uniqueidentifier,

    @UserID uniqueidentifier,
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@Mail nvarchar(50),
	@ApproverID uniqueidentifier,
	@TransferResult tinyint, --0:approve, 1: reject, 2: pending
	@State tinyint, --0: active, 1: completed	@State tinyint, 
	@ModifiedOn datetime,
	@ModifiedBy uniqueidentifier,
	@ReturnValue int output --0: success, 1: update failed, 2, no pending user transfer requests
WITH EXEC AS CALLER
AS
BEGIN
    if((@Role = 'VM' 
		AND EXISTS(select ut.ID from UserTransfer as ut where ut.UserID = @UserID and ut.ClientIDTo = @VMClientID and ut.[State] = 0)) 
    OR @Role='SC' )
--    if((@Role = 'VRent Manager') AND EXISTS(select ut.ID from UserTransfer as ut where ut.UserID = @UserID and ut.ClientIDTo = @VMClientID and ut.[State] = 0))
	   begin
		   	Update [dbo].[UserTransfer]
				set 
				[FirstName] = @FirstName,
				[LastName] = @LastName,
				[Mail] = @Mail,
				[ApproverID] = @ApproverID,
				[TransferResult] = @TransferResult,
				[State] = @State,
				[ModifiedOn] = @ModifiedOn,
				[ModifiedBy] = @ModifiedBy
				where UserID = @UserID and [state] = 0			
			if(@@ROWCOUNT = 1)
				begin
					SELECT 
					   [ID]
					  ,[UserID]
					  ,[FirstName]
					  ,[LastName]
					  ,[Mail]
					  ,[ClientIDFrom]
					  ,[ClientIDTo]
					  ,[ApproverID]
					  ,[TransferResult]
					  ,[State]
					  ,[CreatedOn]
					  ,[CreatedBy]
					  ,[ModifiedOn]
					  ,[ModifiedBy]
					  From UserTransfer as ut
					  where ut.UserID = @UserID and ut.[state] = 1 --pending
					set @ReturnValue = 0
				end
			else
				begin
					set @ReturnValue = 1
				end
		end
	else
		begin
			set @ReturnValue = 2
		end
END

GO
