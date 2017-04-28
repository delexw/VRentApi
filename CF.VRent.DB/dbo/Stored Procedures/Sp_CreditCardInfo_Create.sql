CREATE PROCEDURE [Sp_CreditCardInfo_Create]
   @CardNo         NVARCHAR (MAX),
   @PhoneNo        NVARCHAR (MAX),
   @UserID         UNIQUEIDENTIFIER,
   @Token          NVARCHAR (MAX),
   @BindingTime    DATETIME,
   @CreatedOn      DATETIME,
   @CreatedBy      UNIQUEIDENTIFIER,
   @ModifiedOn     DATETIME,
   @ModifiedBy     UNIQUEIDENTIFIER,
   @BankCode       NVARCHAR (10),
   @CardID         NVARCHAR (50)
   WITH
   EXEC AS CALLER
AS
   BEGIN
      UPDATE UnionCardInfo
         SET State = 0
       WHERE [User_ID] = @UserID

      INSERT INTO UnionCardInfo (Encrpty_Card_No,
                                 Card_User_Tel,
                                 [User_ID],
                                 Encrypt_Token,
                                 Binding_Time,
                                 CreatedOn,
                                 CreatedBy,
                                 ModifiedOn,
                                 ModifiedBy,
                                 Bank_Code,
                                 Card_ID)
      VALUES (@CardNo,
              @PhoneNo,
              @UserID,
              @Token,
              @BindingTime,
              @CreatedOn,
              @CreatedBy,
              @ModifiedOn,
              @ModifiedBy,
              @BankCode,
              @CardID)
   END
