
CREATE PROCEDURE [dbo].[Sp_GetUserCreditCards]
@UserID uniqueidentifier
WITH EXEC AS CALLER
AS
SELECT u.Encrpty_Card_No,
       u.ID,
       u.Bank_Code,
       u.Card_ID,
       u.Card_User_Tel
  FROM UnionCardInfo u
 WHERE u.[User_ID] = @UserID AND State = 1
ORDER BY Binding_Time DESC
