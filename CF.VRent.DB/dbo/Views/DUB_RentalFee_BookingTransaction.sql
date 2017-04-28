CREATE VIEW [dbo].[DUB_RentalFee_BookingTransaction]
AS
SELECT bp.ID,
       bp.BookingID,
       ume.Unique_ID AS TransactionId,
       bp.UPPaymentID,
       bp.CreatedOn,
       bp.CreatedBy,
       bp.ModifiedOn,
       bp.ModifiedBy
  FROM BookingPayment bp, UnionPaymentMessageExchange ume
 WHERE bp.UPPaymentID = ume.ID and bp.state = 1