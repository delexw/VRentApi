
CREATE PROCEDURE [dbo].[Sp_CreditCardInfo_Delete]
@CardID nvarchar(50), @UserID uniqueidentifier
WITH EXEC AS CALLER
AS
DELETE FROM UnionCardInfo
 WHERE Card_ID = @CardID AND [User_ID] = @UserID
