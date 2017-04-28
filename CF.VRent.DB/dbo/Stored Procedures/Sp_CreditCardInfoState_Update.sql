
CREATE PROCEDURE [dbo].[Sp_CreditCardInfoState_Update]
@CardId nvarchar(50), @State int, @UserID nvarchar(50)
WITH EXEC AS CALLER
AS
UPDATE UnionCardInfo
   SET State = @State, ModifiedOn = getdate (), ModifiedBy = @UserID
 WHERE Card_ID = @CardId AND [User_ID] = @UserID
