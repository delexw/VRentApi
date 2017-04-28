-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_UserTransfer_Retrieve]
	@Role nvarchar(50),
	@UserID uniqueidentifier,
	@TargetClient uniqueidentifier,
	@ReturnValue int output --0: success, 1: no permisstion
WITH EXEC AS CALLER
AS
BEGIN
	if(@Role = 'VM')
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
			where ut.ClientIDTo = @TargetClient

			set @ReturnValue = 0
		end
	else if(@Role = 'BU') 
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
			where ut.UserID = @UserID

			set @ReturnValue = 0
		end
	else
		Begin
		set @ReturnValue = 1
		End
END

GO