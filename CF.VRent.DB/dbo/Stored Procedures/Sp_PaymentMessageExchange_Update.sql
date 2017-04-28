CREATE PROCEDURE [Sp_PaymentMessageExchange_Update]
   @ID                    INT,
   @State                 INT,
   @RCount                INT,
   @PreAuthID             NVARCHAR (MAX),
   @PreAuthQueryID        NVARCHAR (MAX),
   @PreAuthDateTime       NVARCHAR (50),
   @PreAuthPrice          NVARCHAR (10),
   @PreAuthTempOrderID    NVARCHAR (50),
   @Operation             NVARCHAR (50),
   @UserID                NVARCHAR (50),
   @DeductionPrice        NVARCHAR (10),
   @Message               NVARCHAR (MAX),
   @RealPreAuthPrice      NVARCHAR (10)
   WITH
   EXEC AS CALLER
AS
   UPDATE UnionPaymentMessageExchange
      SET State = @State,
          Retry_Count = @RCount,
          PreAuthID = @PreAuthID,
          PreAuthQueryID = @PreAuthQueryID,
          PreAuthDateTime = @PreAuthDateTime,
          PreAuthPrice = @PreAuthPrice,
          PreAuthTempOrderID = @PreAuthTempOrderID,
          Operation = @Operation,
          ModifiedOn = getDate (),
          ModifiedBy = @UserID,
          DeductionPrice = @DeductionPrice,
          [Message] = @Message,
          RealPreAuthPrice = @RealPreAuthPrice
    WHERE ID = @ID
