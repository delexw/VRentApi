CREATE Procedure [dbo].[Sp_DebitNoteDetail_RetrieveByConditions_Dynamic]
	@debitNoteID int,
	@dateBegin datetime,
	@dateEnd datetime,
	@bookingNumber nvarchar(50),
	@userID uniqueidentifier,
	@userName nvarchar(50)
as
declare @ClientID uniqueidentifier
declare @paymentStatus int
declare @bookingNumberPara nvarchar(1000)
declare @userIDPara nvarchar(1000)
declare @userNamePara nvarchar(100)

declare @tempBeginDate nvarchar(8)
declare @tempEndDate nvarchar(8)
declare @indirectbeginParam nvarchar(100)
declare @indirectendParam nvarchar(100)

declare @selectSql nvarchar(max) = ''
declare @selectSqlParameterDef nvarchar(max)

Begin
	if exists(select * from DebitNotes as dn where dn.ID = @debitNoteID)
	begin
		select @ClientID = dn.ClientID,@paymentStatus = dn.PaymentStatus from DebitNotes as dn where dn.ID = @debitNoteID
		print @ClientID
		if(@bookingNumber IS NULL)
			begin
				set @bookingNumberPara = '' 
			end
		else 
			begin 
				set @bookingNumberPara = ' AND vb.KemasBookingNumber like ''%'+@bookingNumber + '%'''
			end
		if(@userID IS NULL)
			begin
				set @userIDPara = '' 
			end
		else 
			begin 
				set @userIDPara = ' AND vb.UserID ='''+ cast( @userID as nvarchar(36)) + ''''
			end

		if(@userName IS NOT NULL)
			begin
				set @userNamePara = ' AND (UserFirstName like ''%'+ @userName + '%'' OR UserLastName like ''%' +@userName + '%'')'
			end
		else 
			begin 
				set @userNamePara = ' '
			end

	if(@dateBegin is NOT NULL)
		begin
			set @tempBeginDate = CONVERT(nvarchar(8), @dateBegin, 112)
			set @indirectbeginParam = ' AND voi.CreatedOn >= ''' + @tempBeginDate + ''''
		end
	else
		begin
			set @indirectbeginParam = ' '
		end

	if(@dateEnd is NOT NULL)
		begin
			set @tempEndDate = CONVERT(nvarchar(8), DATEADD(day,1,@dateEnd), 112) 
			set @indirectendParam = ' AND voi.CreatedOn < ''' + @tempEndDate + ''''
		end
	else
		begin
			set @indirectendParam = ' '
		end

		
				--[ID]
  --    ,[DebitNoteID]
  --    ,[ClientID]
  --    ,[UserID]
  --    ,[BookingID]
  --    ,[KemasBookingID]
  --    ,[KemasBookingNumber]
  --    ,[OrderID]
  --    ,[OrderItemID]
  --    ,[Category]
  --    ,[OrderDate]
  --    ,[TotalAmount]
  --    ,[PaymentStatus]
  --    ,[State]
		set @selectSqlParameterDef = '@paymentStatus int, @debitNoteID int, @ClientID uniqueidentifier'

		set @selectSql = @selectSql +
			N'
				Select 
					-1 as ID,
					1 as DebitNoteID,
					vb.CorporateID as ClientID,
					vb.UserID as UserID,
					vb.UserFirstName,
					vb.UserLastName,
					vb.ID as BookingID,
					vb.KemasBookingID,
					vb.KemasBookingNumber,
					vo.ID as OrderID,
					vo.Category,
					vo.OrderDate,
					vo.TotalAmount,
					3 as PaymentStatus,
					0 as State
				from 
				(
					select 
						vo.ID,
						vo.ProxyBookingID,
						''INDIRECTFEE'' as Category,
						CONVERT(nvarchar(8), voi.CreatedOn, 112) as OrderDate,
						SUM(voi.TotalAmount) as TotalAmount
					from dbo.VrentOrderItems as voi 
					inner join VrentOrders as vo on voi.OrderID = vo.ID AND	voi.Category = ''INDIRECTFEE'' 
					and voi.TotalAmount > 0 and vo.State = 0 AND voi.State = 0
					where 1 = 1'
					+
						+@indirectbeginParam
						+@indirectendParam					
					+
					'group by vo.ID,vo.ProxyBookingID,CONVERT(nvarchar(8), voi.CreatedOn, 112)
				) as vo
				inner join VrentBookings as vb on vb.ID = vo.ProxyBookingID and vb.BookingType = 2 and vb.CorporateID = @ClientID
				where 1 = 1'   
					+ @bookingNumberPara
					+ @userIDPara
					+ @userNamePara
		
		print @selectSql

		EXEC sp_executesql @selectSql, @selectSqlParameterDef,@paymentStatus, @debitNoteID,@ClientID

	end

End