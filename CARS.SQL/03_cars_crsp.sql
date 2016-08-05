/*==============================================================*/
/* SP: [dbo].[FindLeaves]                                          */
/*==============================================================*/
IF OBJECT_ID('dbo.FindLeaves') IS NOT NULL
    DROP PROCEDURE dbo.FindLeaves
GO

CREATE PROCEDURE [dbo].[FindLeaves]
(
	@SubmitEmployeeID nvarchar(100),
	@ReportManagerID nvarchar(100),
	@LeaveTypeID nvarchar(100),
	@LeaveStatus smallint,
	@StartTime datetime,
	@EndTime datetime,
	@SupervisorIDs nvarchar(1000)
)
AS
BEGIN
declare @sql nvarchar(1000)
set @sql = 'select * from LeaveInfo where 1=1'

if (@SubmitEmployeeID <> '')
begin
	set @sql = @sql + ' and FKSubmitEmployeeID = ''' + @SubmitEmployeeID + ''''
end

if (@ReportManagerID <> '')
begin
	set @sql = @sql + ' and FKReportManagerID = ''' + @ReportManagerID + ''''
end

if (@ReportManagerID = '' and @SubmitEmployeeID = '')
begin
	set @sql = @sql + ' and FKReportManagerID in ' + @SupervisorIDs
end

if (@LeaveTypeID <> '')
begin
	set @sql = @sql + ' and FKLeaveTypeID = ''' + @LeaveTypeID + ''''
end

if (@LeaveStatus <> 0)
begin
	set @sql = @sql + ' and Status = ' + cast(@LeaveStatus as varchar(10))
end

set @sql = @sql + ' and not ( LastEndTime < ''' + cast(@StartTime as varchar(50)) 
		   + ''' or FirstStartTime > ''' + cast(@EndTime as varchar(50)) + ''' )'
		   + ' order by Status, FKSubmitEmployeeID, FirstStartTime ASC'

exec(@sql)
END
GO


/*==============================================================*/
/* Function: dbo.fCalcLeaveTakenHours                                          */
/*==============================================================*/
IF OBJECT_ID('dbo.fCalcLeaveTakenHours') IS NOT NULL
    DROP FUNCTION dbo.fCalcLeaveTakenHours
GO
/*  
TESTING:
	SELECT dbo.fCalcLeaveTakenHours ('2011-10-17 09:00:00.000', '2011-10-18 17:00:00.000')
*/
CREATE FUNCTION dbo.fCalcLeaveTakenHours
(
	@StartTime			datetime,
	@EndTime			datetime
)
RETURNS INT
AS 
BEGIN
  DECLARE @TakenHours INT
  SET @TakenHours = DATEDIFF(DAY, @StartTime, @EndTime) * 8
  IF (DATEPART(HOUR,@StartTime) = '9')
  BEGIN
    IF (DATEPART(HOUR,@EndTime) = '9')
    BEGIN
      RETURN @TakenHours
    END
    ELSE IF (DATEPART(HOUR,@EndTime) = '13')
    BEGIN
      SET @TakenHours += 4
    END
    ELSE IF (DATEPART(HOUR,@EndTime) = '17')
    BEGIN
      SET @TakenHours += 8
    END
  END
  ELSE IF (DATEPART(HOUR,@StartTime) = '13')
  BEGIN
    IF (DATEPART(HOUR,@EndTime) = '9')
    BEGIN
      SET @TakenHours -= 4
    END
    ELSE IF (DATEPART(HOUR,@EndTime) = '13')
    BEGIN
      RETURN @TakenHours
    END
    ELSE IF (DATEPART(HOUR,@EndTime) = '17')
    BEGIN
      SET @TakenHours += 4
    END
  END
  ELSE IF (DATEPART(HOUR,@StartTime) = '17')
  BEGIN
    IF (DATEPART(HOUR,@EndTime) = '9')
    BEGIN
      SET @TakenHours -= 8
    END
    ELSE IF (DATEPART(HOUR,@EndTime) = '13')
    BEGIN
      SET @TakenHours -= 4
    END
    ELSE IF (DATEPART(HOUR,@EndTime) = '17')
    BEGIN
      RETURN @TakenHours
    END
  END
  RETURN @TakenHours 
END
GO


/*==============================================================*/
/* Function: dbo.fGetLeaveTakenHours                                         */
/*==============================================================*/
IF OBJECT_ID('dbo.fGetLeaveTakenHours') IS NOT NULL
    DROP FUNCTION dbo.fGetLeaveTakenHours
GO
/*  
TESTING:
	SELECT dbo.fGetLeaveTakenHours ('2011-10-17 09:00:00.000', '2011-10-18 17:00:00.000', '2011-09-26 09:00:00.000', '2011-10-25 17:00:00.000')
*/
CREATE FUNCTION dbo.fGetLeaveTakenHours
(
	@StartTime			datetime,
	@EndTime			datetime,
	@ReportStartTime	datetime,
	@ReportEndTime		datetime
)
RETURNS INT
AS 
BEGIN
  DECLARE @TakenHours INT
  IF (@ReportStartTime <= @StartTime AND @EndTime <= @ReportEndTime)
  BEGIN
    SET @TakenHours = dbo.fCalcLeaveTakenHours(@StartTime, @EndTime)
  END
  ELSE IF (@ReportStartTime <= @StartTime AND @StartTime <= @ReportEndTime AND @ReportEndTime <= @EndTime)
  BEGIN
    SET @TakenHours = dbo.fCalcLeaveTakenHours(@StartTime, @ReportEndTime)
  END
  ELSE IF (@StartTime <= @ReportStartTime AND @ReportStartTime <= @EndTime AND @EndTime <= @ReportEndTime)
  BEGIN
    SET @TakenHours = dbo.fCalcLeaveTakenHours(@ReportStartTime, @EndTime)
  END
  RETURN @TakenHours 
END
GO


/*==============================================================*/
/* Function: dbo.fGetLeaveTaken                             */
/*==============================================================*/

IF OBJECT_ID('dbo.fGetLeaveTaken') IS NOT NULL
    DROP FUNCTION dbo.fGetLeaveTaken
GO
/*  
TESTING:
    SELECT dbo.fGetLeaveTaken ('Annual Leave', 'CF17BEC9-AABA-496E-9A1B-4359015D8892', '2011-09-26 09:00:00.000', '2011-10-25 17:00:00.000')
    SELECT dbo.fGetLeaveTaken ('Annual Leave', 'CF17BEC9-AABA-496E-9A1B-4359015D8892', '2011-09-26', '2011-10-25')
*/
CREATE FUNCTION dbo.fGetLeaveTaken
(
	@LeaveTypeName		nvarchar(50),
	@EmployeeID			uniqueidentifier,
	@StartDate			datetime,
	@EndDate			datetime
)
RETURNS INT
AS
BEGIN
    DECLARE @TakenHours INT
	SELECT 
	  @TakenHours = SUM(dbo.fGetLeaveTakenHours(StartTime, EndTime, @StartDate, @EndDate))
	FROM dbo.vLeaveInfo 
	WHERE FKSubmitEmployeeID = @EmployeeID
	  AND Status = '3' -- 1=Applying, 2=Rejected, 3=Accepted
	  AND LeaveTypeName = @LeaveTypeName
	  AND 
	    ((@StartDate <= StartTime AND EndTime <= @EndDate) 
	    OR (@StartDate <= StartTime AND StartTime <= @EndDate AND @EndDate <= EndTime)
	    OR (StartTime <= @StartDate AND @StartDate <= EndTime AND EndTime <= @EndDate))
	GROUP BY FKSubmitEmployeeID
	
	RETURN @TakenHours
END
GO


/*==============================================================*/
/* SP: dbo.pGetAdminLeaveReportData                             */
/*==============================================================*/
IF OBJECT_ID('dbo.pGetAdminLeaveReportData') IS NOT NULL
    DROP PROCEDURE dbo.pGetAdminLeaveReportData
GO
/*  
TESTING:
    EXEC dbo.pGetAdminLeaveReportData 
*/
CREATE PROCEDURE [dbo].[pGetAdminLeaveReportData]
@month1start		datetime = null,
	        @month1end			datetime = null,
			@month2start		datetime = null,
	        @month2end			datetime = null,
			@month3start		datetime = null,
	        @month3end			datetime = null,
			@month4start		datetime = null,
	        @month4end			datetime = null,
			@month5start		datetime = null,
	        @month5end			datetime = null,
			@month6start		datetime = null,
	        @month6end			datetime = null,
			@month7start		datetime = null,
	        @month7end			datetime = null,
			@month8start		datetime = null,
	        @month8end			datetime = null,
			@month9start		datetime = null,
	        @month9end			datetime = null,
			@month10start		datetime = null,
	        @month10end			datetime = null,
			@month11start		datetime = null,
	        @month11end			datetime = null,
			@month12start		datetime = null,
	        @month12end			datetime = null,
	        @leavetypename		nvarchar(500)
AS
BEGIN
  BEGIN TRY
    SELECT
    EmployeeName,
	CONVERT(date, HiredDate) HiredDate,
	CASE
      WHEN DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') >= 12 THEN 12
      WHEN DAY(HiredDate) < 20 THEN DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') + 1
      ELSE DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31')
    END ValidMonths,
    PKEmployeeID,
    CostCenter,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month1start, @month1end) AS JANLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month2start, @month2end) AS FEBLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month3start, @month3end) AS MARLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month4start, @month4end) AS APRLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month5start, @month5end) AS MAYLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month6start, @month6end) AS JUNLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month7start, @month7end) AS JULLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month8start, @month8end) AS AUGLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month9start, @month9end) AS SEPLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month10start, @month10end) AS OCTLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month11start, @month11end) AS NOVLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month12start, @month12end) AS DECLeaveTakenHours
    FROM dbo.vEmployee
	ORDER BY HiredDate, EmployeeName
  
	RETURN @@ERROR
  END TRY
  BEGIN CATCH
	RETURN ERROR_NUMBER()
  END CATCH
END
GO

/*==============================================================*/
/* SP: dbo.pGetSupervisorLeaveReportData                             */
/*==============================================================*/
IF OBJECT_ID('dbo.pGetSupervisorLeaveReportData') IS NOT NULL
    DROP PROCEDURE dbo.pGetSupervisorLeaveReportData
GO
/*  
TESTING:
    EXEC dbo.pGetSupervisorLeaveReportData 
*/
CREATE PROCEDURE dbo.pGetSupervisorLeaveReportData
@month1start		datetime = null,
	        @month1end			datetime = null,
			@month2start		datetime = null,
	        @month2end			datetime = null,
			@month3start		datetime = null,
	        @month3end			datetime = null,
			@month4start		datetime = null,
	        @month4end			datetime = null,
			@month5start		datetime = null,
	        @month5end			datetime = null,
			@month6start		datetime = null,
	        @month6end			datetime = null,
			@month7start		datetime = null,
	        @month7end			datetime = null,
			@month8start		datetime = null,
	        @month8end			datetime = null,
			@month9start		datetime = null,
	        @month9end			datetime = null,
			@month10start		datetime = null,
	        @month10end			datetime = null,
			@month11start		datetime = null,
	        @month11end			datetime = null,
			@month12start		datetime = null,
	        @month12end			datetime = null,
	        @leavetypename		nvarchar(500),
	        @supervisorID		nvarchar(500)
AS
BEGIN
  BEGIN TRY
	declare @countRows int;
	create table #SummaryIDsTable(EID uniqueidentifier);
	create table #OneLoopIDsTable(EID uniqueidentifier);
	create table #TempIDsTable(EID uniqueidentifier);
	
	--1. Get all employee IDs and insert them into #SummaryIDsTable, #OneLoopIDsTable.
	--2. Go into the loop
	  --2.1 Get all employee IDs whose managerID in #OneLoopIDsTable 
	  --	and insert into #SummaryIDsTable
	  --2.2 Select * from #OneLoopIDsTable and insert into #TempIDsTable
	  --2.3 Delete * from #OneLoopIDsTable
	  --2.4 Get all employee IDs whose managerID in #TempIDsTable 
	  --	and insert into #OneLoopIDsTable
	  --2.5 Delete * from #TempIDsTable
	  --2.6 If count(*) of #OneLoopIDsTable = 0, break the loop.
	--3. Add himself/herself into the list.
	
	--1.
	insert into #SummaryIDsTable
		select PKEmployeeID FROM Employee where FKReportManagerID = @supervisorID
	insert into #OneLoopIDsTable select * FROM #SummaryIDsTable
		
	set @countRows = (select count(*) from #OneLoopIDsTable)
	while @countRows > 0
	begin
		--2.1
		insert into #SummaryIDsTable 
			select PKEmployeeID from Employee where FKReportManagerID in
			(select * from #OneLoopIDsTable)
			
		--2.2
		insert into #TempIDsTable select * from #OneLoopIDsTable
		
		--2.3
		delete from #OneLoopIDsTable
		
		--2.4
		insert into #OneLoopIDsTable 
			select PKEmployeeID from Employee where FKReportManagerID in
			(select * from #TempIDsTable)
			
		--2.5
		delete from #TempIDsTable
		
		--2.6
		set @countRows = (select count(*) from #OneLoopIDsTable)
	end
	
	--3
	insert into #SummaryIDsTable values (@supervisorID)
  
    SELECT
    EmployeeName,
    CONVERT(date, HiredDate) HiredDate,
	CASE
      WHEN DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') >= 12 THEN 12
      WHEN DAY(HiredDate) < 20 THEN DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') + 1
      ELSE DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31')
    END ValidMonths,
    PKEmployeeID,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month1start, @month1end) AS JANLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month2start, @month2end)  AS FEBLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month3start, @month3end)  AS MARLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month4start, @month4end)  AS APRLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month5start, @month5end)  AS MAYLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month6start, @month6end)  AS JUNLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month7start, @month7end)  AS JULLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month8start, @month8end)  AS AUGLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month9start, @month9end)  AS SEPLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month10start, @month10end)  AS OCTLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month11start, @month11end) AS NOVLeaveTakenHours,
    dbo.fGetLeaveTaken(@leavetypename, PKEmployeeID, @month12start, @month12end)  AS DECLeaveTakenHours
    FROM dbo.vEmployee
    where PKEmployeeID = @supervisorID or FKReportManagerID in (select * from #SummaryIDsTable)
    ORDER BY HiredDate, EmployeeName
	RETURN @@ERROR
	
	drop table #SummaryIDsTable
	drop table #OneLoopIDsTable
	drop table #TempIDsTable
  END TRY
  BEGIN CATCH
	RETURN ERROR_NUMBER()
  END CATCH
END
GO