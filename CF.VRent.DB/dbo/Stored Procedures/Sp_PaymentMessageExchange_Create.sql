CREATE PROCEDURE [dbo].[Sp_PaymentMessageExchange_Create]
@UniqueID nvarchar(50), @CreatedOn datetime, @Operation nvarchar(50), @UserID nvarchar(50), @PreAuthID nvarchar(MAX), @PreAuthQueryID nvarchar(MAX), @PreAuthDateTime nvarchar(50), @PreAuthPrice nvarchar(10), @PreAuthTempOrderID nvarchar(50), @SmsCode nvarchar(MAX), @State int, @CardID nvarchar(50), @LastPaymentID int, @NewID int OUTPUT, @DeductionPrice nvarchar(10), @RealPreAuthPrice nvarchar(10)
WITH EXEC AS CALLER
AS
INSERT INTO UnionPaymentMessageExchange (Unique_ID,
                                         CreatedOn,
                                         Operation,
                                         [User_ID],
                                         PreAuthID,
                                         PreAuthQueryID,
                                         PreAuthDateTime,
                                         PreAuthPrice,
                                         PreAuthTempOrderID,
                                         SmsCode,
                                         State,
                                         Card_ID,
                                         LastPaymentID,
                                         DeductionPrice,
                                         RealPreAuthPrice)
VALUES (@UniqueID,
        @CreatedOn,
        @Operation,
        @UserID,
        @PreAuthID,
        @PreAuthQueryID,
        @PreAuthDateTime,
        @PreAuthPrice,
        @PreAuthTempOrderID,
        @SmsCode,
        @State,
        @CardID,
        @LastPaymentID,
        @DeductionPrice,
        @RealPreAuthPrice);

SELECT @NewID = IDENT_CURRENT ('UnionPaymentMessageExchange')
