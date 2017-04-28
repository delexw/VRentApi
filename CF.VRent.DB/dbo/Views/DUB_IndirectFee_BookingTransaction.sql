CREATE VIEW [dbo].[DUB_IndirectFee_BookingTransaction]
AS
SELECT bif.ID,
       bif.BookingID,
       bif.OrderItemID,
       ume.Unique_ID AS TransactionId,
       bif.UPPaymentID,
       bif.CreateOn,
       bif.CreatedBy,
       bif.ModifiedOn,
       bif.ModifiedBy
  FROM BookingIndirectFeePayment bif, UnionPaymentMessageExchange ume
 WHERE bif.UPPaymentID = ume.ID and bif.State = 1