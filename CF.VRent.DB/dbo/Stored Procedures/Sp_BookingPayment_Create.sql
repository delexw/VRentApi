
CREATE PROCEDURE [dbo].[Sp_BookingPayment_Create]
@BookingId int, @PaymentId int, @State tinyint, @UserId uniqueidentifier
WITH EXEC AS CALLER
AS
INSERT INTO BookingPayment (BookingID,
                            UPPaymentID,
                            state,
                            CreatedOn,
                            CreatedBy)
VALUES (@BookingId,
        @PaymentId,
        @State,
        getdate (),
        @UserId)
