-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_UserTransfer_Add]
    @UserID uniqueidentifier,
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@Mail nvarchar(50),
	@ClientIDFrom uniqueidentifier,
	@ClientIDTo uniqueidentifier,
	@TransferResult tinyint,
	@State [tinyint], --0: pending, 1: completed	@State tinyint, 
	@CreatedOn datetime,
	@CreatedBy uniqueidentifier,
	@ReturnValue int output --0: success, 1: have pending request, 2 insert failed

WITH EXEC AS CALLER
AS
BEGIN
   DECLARE @lastID      TABLE (InsertedID   INT)
   DECLARE  @InsertedID int

   if NOT exists(select ut.ID from UserTransfer as ut where ut.UserID = @UserID and ut.[State] = 0)
	   begin
		   INSERT INTO [dbo].[UserTransfer]
				([UserID]
				,[FirstName]
				,[LastName]
				,[Mail]
				,[ClientIDFrom]
				,[ClientIDTo]
				,[TransferResult]
				,[State]
				,[CreatedOn]
				,[CreatedBy])
		   OUTPUT inserted.ID      INTO @lastID
		   VALUES (
				   @UserID,
				   @FirstName,
				   @LastName,
				   @Mail,
				   @ClientIDFrom,
				   @ClientIDTo,
				   @TransferResult,
				   @State, 
				   @CreatedOn,
				   @CreatedBy)
		   set @InsertedID = (SELECT TOP 1 InsertedID FROM @lastID)
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
		  where ut.ID = @InsertedID and ut.[state] = 0 --pending
	        set @ReturnValue = 0
		end
		else
			begin
				set @ReturnValue = 2
			end
		end
   else
		begin
			set @ReturnValue = 1
		end
END