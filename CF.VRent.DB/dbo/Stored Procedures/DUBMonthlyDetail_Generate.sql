CREATE Procedure DUBMonthlyDetail_Generate
@dubClosingID int,
@createdOn datetime,
@createdBy uniqueidentifier

as
declare @tempBeginDate datetime,@tempEndDate datetime,
@begindate datetime,
@enddate datetime

select @tempBeginDate = DATEADD(day,-1,@beginDate), @tempEndDate = DATEADD(day,1,@enddate)

insert into DUBMonthlyDetail 
			(
		   [DUBClosingID]
		   ,[BookingID]
           ,[UserID]
           ,[OrderDate]
           ,[PaymentID]
           ,[OrderID]
           ,[OrderItemID]
           ,[Category]
           ,[TotalAmount]
           ,[State]
           ,[CreatedOn]
           ,[CreatedBy])
		select
		@dubClosingID,
		vb.ID as BookingID,
		vb.UserID as UserID,
		vb.DateEnd as OrderDate,
		PaymentSource.PaymentID,
		OrderSource.OrderID,
		OrderSource.OrderItemId,
		OrderSource.Category,
		OrderSource.TotalAmount,
		0 as state,
		@createdOn as CreatedOn,
		@createdBy as CreatedBy
		from 
		(
			Select vb.ID,vb.CorporateID,vb.UserID,vb.DateEnd,vb.State,vb.TotalAmount from VrentBookings as vb
			where vb.BookingType = 3 AND vb.TotalAmount > 0
				AND vb.DateEnd > @tempBeginDate AND vb.DateEnd < @tempEndDate
		) as vb
		outer apply
		(
			select upme.id as PaymentID,upme.State,upme.Operation
			from BookingPayment as bp
			left join UnionPaymentMessageExchange as upme on upme.ID = bp.UPPaymentID
			where bp.BookingID = vb.ID 
			and bp.state = 1
		) as PaymentSource
		outer apply
		(
			select vo.ID as OrderID, voi.ID as OrderItemId,voi.Category, voi.TotalAmount as TotalAmount  
			from VrentOrders as vo
			left join VrentOrderItems as voi on vo.ID = voi.OrderID
			where vo.ProxyBookingID = vb.ID and vo.State = 0 and voi.State = 0 and voi.Category = 'RENTALFEE'
		) as OrderSource

		union

		select 
		@dubClosingID
		,vb.ID
		,vb.UserID
		,vb.OrderDate,
		PaymentSource.PaymentID
		,vb.OrderID
		,vb.OrderItemID
		,vb.Category
		,vb.TotalAmount,
		0 as state,
		@createdOn as CreatedOn,
		@createdBy as CreatedBy
		from 
		(
			Select
			vb.ID,vb.CorporateID,vb.UserID,vb.State, voi.Category,vb.TotalAmount as Total,
			voi.OrderID as OrderID, voi.ID as OrderItemID,voi.TotalAmount, voi.CreatedOn as OrderDate 
			from VrentOrderItems as voi
			inner join VrentOrders as vo on vo.ID = voi.OrderID
			inner join VrentBookings as vb on vo.ProxyBookingID = vb.ID
			where vo.State = 0 and voi.State = 0 and voi.Category = 'INDIRECTFEE' AND voi.CreatedOn > @tempBeginDate AND voi.CreatedOn < @tempEndDate
			and vb.BookingType = 3 AND vb.TotalAmount > 0
		) as vb
		outer apply
		(
			select upme.id as PaymentID,upme.State,upme.Operation
			from BookingIndirectFeePayment as bifp
			left join UnionPaymentMessageExchange as upme on upme.ID = bifp.UPPaymentID
			where bifp.BookingID = vb.ID and bifp.OrderItemID = vb.OrderItemID
			and bifp.state = 1
		) as PaymentSource
		order by OrderDate