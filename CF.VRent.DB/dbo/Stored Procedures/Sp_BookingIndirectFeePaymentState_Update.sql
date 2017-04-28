CREATE PROCEDURE [dbo].[Sp_BookingIndirectFeePaymentState_Update]
@BIFPObject [dbo].[BookingIndirectFeePayment] READONLY
WITH EXEC AS CALLER
AS
UPDATE BookingIndirectFeePayment
   SET State = o.State, ModifiedOn = getdate (), ModifiedBy = o.ModifiedBy
  FROM BookingIndirectFeePayment
       INNER JOIN @BIFPObject AS o
          ON     BookingIndirectFeePayment.BookingID = o.BookingID
             AND BookingIndirectFeePayment.OrderItemID = o.OrderItemID
