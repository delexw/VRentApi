-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_UserTransfer_Pending]
	@UserID uniqueidentifier,
	@ReturnValue int output --0: success, 1: no permisstion
WITH EXEC AS CALLER
AS
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
		where ut.UserID = @UserID and ut.[State] = 0
	set @ReturnValue = 0