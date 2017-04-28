
CREATE PROCEDURE [dbo].[Sp_GetUserCreditCardToken]
@CardID nvarchar(50), @UserID uniqueidentifier
WITH EXEC AS CALLER
AS
SELECT u.Encrypt_Token
  FROM UnionCardInfo u
 WHERE u.Card_ID = @CardID AND [User_ID] = @UserID
