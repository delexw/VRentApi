CREATE Procedure [dbo].[Sp_DUBDetail_Retrieve_Dynamic]
@beginDate datetime,
@endDate datetime,
@bookingNumber nvarchar(50),
@userID uniqueidentifier,
@userName nvarchar(50),
@paymentStatus int,

@itemsPerPage int,
@pageNumber int,
@totalPages int output -- meaningless

as
declare @tempBeginDate nvarchar(8),@tempEndDate nvarchar(8)

declare @selectSql nvarchar(max) = ''
declare @TotalCountSql nvarchar(max) = ''
declare @TotalCount int
declare @totalcountParameterDef nvarchar(1000)=''

declare @rentalbeginParam nvarchar(100)
declare @indirectbeginParam nvarchar(100)
declare @rentalendParam nvarchar(100)
declare @indirectendParam nvarchar(100)
declare @bookingNumberParam nvarchar(100)
declare @userIDParam nvarchar(100)
declare @userNameParam nvarchar(100)

declare @rentalUpStateParam nvarchar(1000)
declare @indirectUpStateParam nvarchar(100)

	declare @startID int
	declare @endID int
	declare @startIDParam nvarchar(100)
	declare @endIDParam nvarchar(100)

	set @startID = @itemsPerPage * (@pageNumber - 1) + 1
	set @startIDParam = ' AND ID >= '+ cast(@startID as nvarchar(4))
	print @startIDParam
	set @endID = @startID + @itemsPerPage
	set @endIDParam = ' AND ID < '+ cast(@endID as nvarchar(4))
	print @endIDParam

begin
	if(@beginDate is NOT NULL)
		begin
			set @tempBeginDate = CONVERT(nvarchar(8), @beginDate, 112)
			set @rentalbeginParam = ' AND vb.DateEnd >= '''+ @tempBeginDate+''''
			set @indirectbeginParam = ' AND voi.CreatedOn >= ''' + @tempBeginDate + ''''
		end
	else
		begin
			set @rentalbeginParam = ' '
			set @indirectbeginParam = ' '
		end

	if(@endDate is NOT NULL)
		begin
			set @tempEndDate = CONVERT(nvarchar(8), DATEADD(day,1,@endDate), 112) 
			set @rentalendParam = ' AND vb.DateEnd < ''' + @tempEndDate + ''''
			set @indirectendParam = ' AND voi.CreatedOn < ''' + @tempEndDate + ''''
		end
	else
		begin
			set @rentalendParam = ' '
			set @indirectendParam = ' '
		end

	if(@bookingNumber IS NOT NULL)
		begin
			set @bookingNumberParam = ' AND vb.KemasBookingNumber like ''%'+@bookingNumber + '%'''
		end
	else 
		begin 
			set @bookingNumberParam = ' '
		end

	if(@userID IS NOT NULL)
		begin
			set @userIDParam = ' AND vb.UserID = '''+ cast(@userID as nvarchar(36)) + ''''
		end
	else 
		begin 
			set @userIDParam = ' '
		end

	if(@userName IS NOT NULL)
		begin
			set @userNameParam = ' AND (vb.UserFirstName like ''%'+ @userName + '%'' OR vb.UserLastName like ''%' +@userName + '%'')'
		end
	else 
		begin 
			set @userNameParam = ' '
		end

        --//[StatusGroupAttribute(Description = "NULL")]
        --//NULL = -2,
        --//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        --//BeforePreAuth = -1,
        --//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        --//PreAuthorizing = 0,
        --//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        --//PreAuthCompleting = 6,
        --//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        --//PreAuthCancelling = 4,
        --//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        --//PreAuthRetryShortTime = 9,
        --//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        --//PreAuthCancelRetryShortTime = 10,
        --//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        --//PreAuthCompleteRetryShortTime = 11,
        --////Deduction
        --//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        --//PreDeduction = 12,
        --//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        --//Deducting = 13,
		--//[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
  --      //PreAuthCancelRetryLongTime = 16,
  --      //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
  --      //PreAuthCompleteRetryLongTime = 17,
  --      //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
  --      //DeductionRetryShortTime = 18,
  --      //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
  --      //DeductionRetryLongTime = 19,
  --      //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
  --      //PreAuthRetryLongTime = 20,

        --//[StatusGroupAttribute(Description = "Pre-authorization - Success", IsMiddleStatus = false, IsSuccessStatus = true)]
        --//PreAuthorized = 1,
        --//[StatusGroupAttribute(Description = "Pre-authorization - Failed", IsMiddleStatus = false, IsFailedStatus = true)]
        --//PreAuthFailed = 2,

        --//[StatusGroupAttribute(Description = "Pre-authorization cancellation - Success", IsMiddleStatus = false, IsSuccessStatus = true)]
        --//PreAuthCanceled = 3,
        --//[StatusGroupAttribute(Description = "Pre-authorization cancellation - Failed", IsMiddleStatus = false, IsFailedStatus = true)]
        --//PreAuthCancelFailed = 5,
        --//[StatusGroupAttribute(Description = "Fee deduction - Failed", IsMiddleStatus = false, IsFailedStatus = true)]
        --//PreAuthCompleteFailed = 7,
        --//[StatusGroupAttribute(Description = "Fee deduction - Failed", IsMiddleStatus = false, IsFailedStatus = true)]
        --//DeductionFailed = 15,
        --//[StatusGroupAttribute(Description = "Fee deduction - Success", IsMiddleStatus = false, IsSuccessStatus = true)]
        --//PreAuthCompleted = 8,
        --//[StatusGroupAttribute(Description = "Fee deduction - Success", IsMiddleStatus = false, IsSuccessStatus = true)]
        --//Deducted = 14,

        --//[StatusGroupAttribute(Description = "No fee", IsMiddleStatus = false)]
        --//NoFee = 21


	set @startID = @itemsPerPage * (@pageNumber - 1) + 1
	set @endDate = @startID + @itemsPerPage

		--enum UPProcessingState { Processing = 0, PreAuthSuccess,PreAuthFail, PreAuthCancelSuccess, PreAuthCancelFail, FeeDeductionSuccess, FeeDeductionFail, NoFee };

	if(@paymentStatus IS NOT NULL)
		begin
			if(@paymentStatus = 0)
				set @rentalUpStateParam = 'AND (upme.State = -1 OR upme.State = 0 OR upme.State = 6 OR upme.State = 4 OR upme.State = 9 OR upme.State = 10 OR upme.State = 11 OR upme.State = 12 OR upme.State = 13 OR upme.State = 16 OR upme.State = 17 OR upme.State = 18 OR upme.State = 19 OR upme.State = 20) '
			else if(@paymentStatus = 1)
				set @rentalUpStateParam = 'AND (upme.State = 1) '
			else if(@paymentStatus = 2)
				set @rentalUpStateParam = 'AND (upme.State = 2) '
			else if(@paymentStatus = 3)
				set @rentalUpStateParam = 'AND (upme.State = 3) '
			else if(@paymentStatus = 4)
				set @rentalUpStateParam = 'AND (upme.State = 5) '
			else if(@paymentStatus = 5)
				set @rentalUpStateParam = 'AND (upme.State = 8 OR upme.State = 14) '
			else if(@paymentStatus = 6)
				set @rentalUpStateParam = 'AND (upme.State = 7 OR upme.State = 15) '
			else if(@paymentStatus = 7)
				set @rentalUpStateParam = 'AND (upme.State = 21) '		
		end
	else 
		begin 
			set @rentalUpStateParam = ' '
		end


	Set @totalcountParameterDef = '@TotalCount int output'

	set @TotalCountSql = @TotalCountSql + 
		'select @TotalCount = Count(*)
				from
				(
					Select 
						vb.ID,
						''INDIRECTFEE'' as Category,
						bifp.UPPaymentID as PaymentID
						,CONVERT(nvarchar(8), voi.CreatedOn, 112) as OrderDate,
						vo.ID as OrderID,
						sum(voi.TotalAmount) as Amount 
					from VrentBookings as vb 
					left join BookingIndirectFeePayment as bifp on vb.ID = bifp.BookingID
					left join VrentOrderItems as voi on voi.ID = bifp.OrderItemID
					left join VrentOrders as vo on vo.ID = voi.OrderID
					where
					vb.BookingType = 3 and vb.TotalAmount > 0 and bifp.State = 1
					AND vo.State = 0 and voi.State = 0 and voi.Category = ''INDIRECTFEE'' and voi.TotalAmount > 0
					'
					+
						@indirectbeginParam
						+@indirectendParam
						+@userIDParam
						+@userNameParam
						+@bookingNumberParam
					+
					'
					group by vb.ID,vo.ID,bifp.UPPaymentID,CONVERT(nvarchar(8), voi.CreatedOn, 112)

					union all

					Select 
						vb.ID,
						''RENTALFEE'' as Category,
						bp.UPPaymentID as PaymentID,	
						CONVERT(nvarchar(8), vb.DateEnd, 112) as OrderDate,
						vo.ID,
						sum(voi.TotalAmount) as Amount
					from VrentBookings as vb 
					left join BookingPayment as bp on bp.BookingID = vb.ID
					left join VrentOrders as vo on vo.ProxyBookingID = vb.ID
					left join VrentOrderItems as voi on voi.OrderID = vo.ID
					where 1 =1  		
					and bp.State = 1 
					and	voi.Category = ''RENTALFEE'' and voi.TotalAmount > 0 
					and vo.State = 0 and voi.State = 0
					and vb.BookingType = 3 and vb.TotalAmount > 0'
					+
						@rentalbeginParam
						+@rentalendParam
						+@userIDParam
						+@userNameParam
						+@bookingNumberParam
					+
					'group by vb.ID,vo.ID,bp.UPPaymentID,CONVERT(nvarchar(8), vb.DateEnd, 112)
				) as Source
			inner join VrentBookings as vb on source.ID = vb.ID
			left join UnionPaymentMessageExchange as upme on upme.ID = source.PaymentID
			Where 1 = 1'
			+@rentalUpStateParam
	
	print @TotalCountSql
	EXEC sp_executesql @TotalCountSql, @totalcountParameterDef, @TotalCount output
	print @TotalCount
	
	if(@TotalCount > 0)
		begin
			if(@TotalCount % @itemsPerPage = 0)
				begin
					set @totalPages = @TotalCount/@itemsPerPage
				end
			else
				begin
					set @totalPages = @TotalCount/@itemsPerPage + 1
				end
		end
	else
		begin
			set @totalPages = 0
		end

	set @selectSql = @selectSql + '
			select * from
				(
					Select				
						ROW_NUMBER() Over(order by source.ID desc, Source.OrderDate desc,Source.PaymentID desc) as ID,
						Source.ID as BookingID,
						vb.KemasBookingID,
						vb.KemasBookingNumber,
						vb.UserID,
						vb.UserFirstName,
						vb.UserLastName,
						vb.State,
						Source.OrderID,
						Source.PaymentID,
						Source.Category,
						Source.OrderDate,
						Source.Amount,
						case upme.State 
							when -1 then 0
							when 0 then 0 
							when 6 then 0 
							when 4 then 0
							when 9 then 0 
							when 10 then 0 
							when 11 then 0 
							when 12 then 0 
							when 13 then 0 
							when 16 then 0
							when 17 then 0
							when 18 then 0
							when 19 then 0
							when 20 then 0

							when 1 then 1
							when 2 then 2
							when 3 then 3
							when 5 then 4

							when 7 then 6
							when 15 then 6
							when 8 then 5
							when 14 then 5
							when 21 then 7
						end as UPState
					from
						(
					Select 
						vb.ID,
						''INDIRECTFEE'' as Category,
						bifp.UPPaymentID as PaymentID
						,CONVERT(nvarchar(8), voi.CreatedOn, 112) as OrderDate,
						vo.ID as OrderID,
						sum(voi.TotalAmount) as Amount 
					from VrentBookings as vb 
					left join BookingIndirectFeePayment as bifp on vb.ID = bifp.BookingID
					left join VrentOrderItems as voi on voi.ID = bifp.OrderItemID
					left join VrentOrders as vo on vo.ID = voi.OrderID
					where
					vb.BookingType = 3 and vb.TotalAmount > 0 and bifp.State = 1
					AND vo.State = 0 and voi.State = 0 and voi.Category = ''INDIRECTFEE'' and voi.TotalAmount > 0
					'
					+
						@indirectbeginParam
						+@indirectendParam
						+@userIDParam
						+@userNameParam
						+@bookingNumberParam
					+
					'
					group by vb.ID,vo.ID,bifp.UPPaymentID,CONVERT(nvarchar(8), voi.CreatedOn, 112)

					union all

					Select 
						vb.ID,
						''RENTALFEE'' as Category,
						bp.UPPaymentID as PaymentID,	
						CONVERT(nvarchar(8), vb.DateEnd, 112) as OrderDate,
						vo.ID,
						sum(voi.TotalAmount) as Amount
					from VrentBookings as vb 
					left join BookingPayment as bp on bp.BookingID = vb.ID
					left join VrentOrders as vo on vo.ProxyBookingID = vb.ID
					left join VrentOrderItems as voi on voi.OrderID = vo.ID
					where 1 =1  		
					and bp.State = 1 
					and	voi.Category = ''RENTALFEE'' and voi.TotalAmount > 0 
					and vo.State = 0 and voi.State = 0
					and vb.BookingType = 3 and vb.TotalAmount > 0'
					+
						@rentalbeginParam
						+@rentalendParam
						+@userIDParam
						+@userNameParam
						+@bookingNumberParam
					+
					'group by vb.ID,vo.ID,bp.UPPaymentID,CONVERT(nvarchar(8), vb.DateEnd, 112)
				) as Source 
				inner join VrentBookings as vb on source.ID = vb.ID
				left join UnionPaymentMessageExchange as upme on upme.ID = source.PaymentID
				Where 1 = 1'
				+
					+@rentalUpStateParam
				+'
				)as Source
				Where 1=1 
				'
				+ @startIDParam
				+ @endIDParam
				

	print @selectSql
	EXEC sp_executesql @selectSql
end