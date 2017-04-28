

CREATE PROCEDURE [dbo].[Sp_VrentOrders_AppendItems]
	@ProxyBookingID int,
	@OrderID int, 
	@State tinyint,
	@CreatedOn DateTime,
	@CreatedBy uniqueidentifier

WITH EXEC AS CALLER
AS
	insert into VrentOrderItems
	(
		[OrderID],
		[ProductCode],
		[ProductName],
		[SpecModel],
		[Category],
		[TypeID],
		[Type],
		[UnitOfMeasure],
		[UnitPrice],
		[Quantity],
		[NetAmount],
		[TaxRate],
		[TaxAmount],
		[TotalAmount],
		[Remark],
		[State],
		[CreatedOn],
		[CreatedBy]
	)
	select
	@OrderID as OrderID,
	bpi.[Type] as ProductCode,--Product Code
	bpi.[Description] as ProductName, --Product Name
	null as SpecModel, --Spec Model
	bpi.[Group] as Category, --category
	null as [TypeID],
	bpi.Category as [Type], --Type
	null as UnitOfMeasure, --units of measures
	null as UnitPrice, --unit price
	bpi.Quantity as Quantity, --Quantity
	bpi.Total as NetAmount, --net Amount
	0 as TaxRate,--tax rate
	0 as TaxAmount,--taxamount
	bpi.Total as TotalAmount, --totalamount
	null as Remark,
	0 as State,
	@CreatedOn as CreatedOn,
	@CreatedBy as CreatedBy -- replace with a guid later
	from BookingPrice as bp inner join BookingPriceItem as bpi 
	on bp.ID = bpi.PrincingID and bp.state = 0 and bpi.State = 0
	where bp.BookingID = @ProxyBookingID
	
	--update price
	update vb
	set vb.TotalAmount = 
	(Select SUM(voi.TotalAmount) 
	from VrentOrders as vo inner join VrentOrderItems as voi 
	on vo.ID = voi.OrderID and vo.ProxyBookingID = @ProxyBookingID
	where vo.State = 0 and voi.State = 0)
	from VrentBookings as vb
	where vb.ID = @ProxyBookingID
