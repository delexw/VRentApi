
CREATE procedure [dbo].[Sp_VrentBookings_RetrieveID]
@kemasIDs dbo.KemasID readonly
as
begin
	select exter.KemasBookingID,vb.ID as BookingID,vo.ID as OrderID
	from @kemasIDs as exter
	left join VrentBookings as vb on vb.KemasBookingID = exter.KemasBookingID
	left join VrentOrders as vo on vo.ProxyBookingID = vb.ID and vo.State = 0
end