
CREATE PROCEDURE [dbo].[Sp_BookingIndirectFeePayment_Create]
@BIFPObject [dbo].[BookingIndirectFeePayment] READONLY
WITH EXEC AS CALLER
AS
INSERT INTO BookingIndirectFeePayment (BookingID,
                                       OrderItemID,
                                       UPPaymentID,
                                       State,
                                       CreateOn,
                                       CreatedBy)
   SELECT o.BookingID,
          o.OrderItemID,
          o.UPPaymentID,
          o.State,
          getdate (),
          o.CreatedBy
     FROM @BIFPObject AS o
