declare 
  @p_DTimeBegin datetime
, @p_DTimeEnd datetime
, @p_TerminalCode varchar = ''
, @p_IncreaseKind varchar = ''

select
  @p_DTimeBegin = '2022-06-10'
, @p_DTimeEnd = '2022-06-11'
	
select
*
, SUM(Amount) OVER
	(
		ORDER BY t.DTimeBegin
		ROWS BETWEEN UNBOUNDED PRECEDING
		AND CURRENT ROW
	) AS Total
from
(
	select
	ps.DTimeBegin
	, d.Code
	, d.Name TerminalName
	, p.Name ProgramName
	, sum(ei.amount) - sum(coalesce(ep.amount, 0)) Amount
	 , min(ei.Code) IncreaseKind
	from PaySession ps
	join Device d on d.IDDevice = ps.IDDevice
	join Program p on p.IDProgram = ps.IDProgram
	join PayEvent pe on pe.IDPaySession = ps.IDPaySession
	join 
	(
		select
		e.IDPayEvent
		, ei.amount
		, ek.Code
		from 
		EventIncrease ei
		join PayEvent e on e.IDPayEvent = ei.IDPayEvent
		join EventKind ek on ek.IDEventKind = e.IDEventKind
	) ei on ei.IDPayEvent = pe.IDPayEvent
	left join EventPayout ep on ep.IDPayEvent = pe.IDPayEvent
	where 1=1
		and ps.DTimeBegin >= @p_DTimeBegin
		and ps.DTimeBegin < @p_DTimeEnd
		and (coalesce(@p_TerminalCode, '') = '' or @p_TerminalCode = d.Code)
		and (coalesce(@p_IncreaseKind, '') = '' or @p_IncreaseKind = ei.Code)
	group by ps.DTimeBegin, d.Code, d.Name, p.Name
) t
order by t.DTimeBegin


