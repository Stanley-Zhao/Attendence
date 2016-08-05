/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     10/17/2007						                          */
/* History:		     initial version by Scott                                 */
/*                          updated by Stanley                                     */
/*==============================================================*/

USE [CARS]
GO

/*==============================================================*/
/* Drop the tables first                                         */
/*==============================================================*/
IF OBJECT_ID(N'[dbo].[ReportPeriod]', N'U') IS NOT NULL
	DROP TABLE [dbo].[ReportPeriod]
GO
IF OBJECT_ID(N'[dbo].[EmployeeLeaveSummary]', N'U') IS NOT NULL
	DROP TABLE [dbo].[EmployeeLeaveSummary]
GO
IF OBJECT_ID(N'[dbo].[TimeDurationInfo]', N'U') IS NOT NULL
	DROP TABLE [dbo].[TimeDurationInfo]
GO
IF OBJECT_ID('dbo.vLeaveInfo') IS NOT NULL 
DROP VIEW dbo.vLeaveInfo
GO
IF OBJECT_ID(N'[dbo].[LeaveInfo]', N'U') IS NOT NULL
	DROP TABLE [dbo].[LeaveInfo]
GO
IF OBJECT_ID(N'[dbo].[EmployeeRoleRL]', N'U') IS NOT NULL
	DROP TABLE [dbo].[EmployeeRoleRL]
GO
IF OBJECT_ID('dbo.vEmployee') IS NOT NULL 
DROP VIEW dbo.vEmployee
GO
IF OBJECT_ID(N'[dbo].[Employee]', N'U') IS NOT NULL
	DROP TABLE [dbo].[Employee]
GO
IF OBJECT_ID(N'[dbo].[Role]', N'U') IS NOT NULL
	DROP TABLE [dbo].[Role]
GO
IF OBJECT_ID(N'[dbo].[LeaveType]', N'U') IS NOT NULL
	DROP TABLE [dbo].[LeaveType]
GO

/*==============================================================*/
/* Table: [dbo].[Role]                                          */
/*==============================================================*/
IF OBJECT_ID(N'[dbo].[Role]', N'U') IS NOT NULL
	DROP TABLE [dbo].[Role]
GO
CREATE TABLE [dbo].[Role](
	[PKRoleID] 			uniqueidentifier 	NOT NULL	DEFAULT NEWID(),
	[Name] 					nvarchar(50) 			NOT NULL,
	[Description] 	nvarchar(500) 		NULL,
	[KnowledgeDate] datetime					NOT NULL	DEFAULT GETDATE(),
	[TimeToken] 		timestamp 				NOT NULL,
	[CreatedTime] 	datetime 					NOT NULL	DEFAULT GETDATE(),
 CONSTRAINT [PK_Role] PRIMARY KEY ([PKRoleID]), 
 CONSTRAINT [UniqueKeyRoleName] UNIQUE ([Name]) 
) ON [PRIMARY]
GO

/*==============================================================*/
/* Table: [dbo].[LeaveType]                                         */
/*==============================================================*/
IF OBJECT_ID(N'[dbo].[LeaveType]', N'U') IS NOT NULL
	DROP TABLE [dbo].[LeaveType]
GO
CREATE TABLE [dbo].[LeaveType](
	[PKLeaveTypeID] 			uniqueidentifier 	NOT NULL	DEFAULT NEWID(),
	[Name] 								nvarchar(50) 			NOT NULL,
	[TotalHours] 					int 							NOT NULL,
	[StartTime] 					datetime 					NOT NULL,
	[EndTime] 						datetime 					NOT NULL,
	[LongestHoursOneTime] int 							NULL,
	[LeastHoursOneTime] 	int 							NULL,
	[Description] 				nvarchar(500) 		NULL,
	[KnowledgeDate] 			datetime 					NOT NULL	DEFAULT GETDATE(),
	[TimeToken] 					timestamp 				NOT NULL,
	[CreatedTime] 				datetime 					NOT NULL	DEFAULT GETDATE(),
 CONSTRAINT [PK_LeaveType] PRIMARY KEY ([PKLeaveTypeID]), 
 CONSTRAINT [UniqueKeyLeaveTypeName] UNIQUE ([Name]) 
) ON [PRIMARY]
GO

/*==============================================================*/
/* Table: [dbo].[Employee]                                          */
/*==============================================================*/
IF OBJECT_ID(N'[dbo].[Employee]', N'U') IS NOT NULL
	DROP TABLE [dbo].[Employee]
GO
CREATE TABLE [dbo].[Employee](
	[PKEmployeeID]				uniqueidentifier	NOT NULL 	DEFAULT NEWID(),
	[FKReportManagerID]		uniqueidentifier	NULL,
	[Email]								nvarchar(50)			NOT NULL,
	[Password]						nvarchar(50)			NOT NULL,
	[Gender]							smallint					NOT NULL,
	[FirstName]						nvarchar(50)			NOT NULL,
	[MiddleName]					nvarchar(50)			NULL,
	[LastName]						nvarchar(50)			NOT NULL,
	[ServiceYears]				decimal (3,1)								NOT NULL,
	[HiredDate]						datetime					NOT NULL,
	[Phone]								nvarchar(50)			NULL,
	[Mobile]							nvarchar(50)			NULL,
	[Comment]							nvarchar(500)			NULL,
	[IsActive]						bit								NOT NULL,
	[KnowledgeDate]				datetime					NOT NULL	DEFAULT GETDATE(),
	[TimeToken]						timestamp					NOT NULL,
	[CreatedTime]					datetime					NOT NULL	DEFAULT GETDATE(),
 CONSTRAINT [PK_Employee] PRIMARY KEY ([PKEmployeeID]),
 CONSTRAINT [UniqueKeyEmail] UNIQUE ([Email])
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Employee]  WITH CHECK ADD CONSTRAINT [FK_Employee_Employee] FOREIGN KEY([FKReportManagerID])
REFERENCES [dbo].[Employee] ([PKEmployeeID])
GO

/*==============================================================*/
/* Table: [dbo].[EmployeeRoleRL]                                          */
/*==============================================================*/
IF OBJECT_ID(N'[dbo].[EmployeeRoleRL]', N'U') IS NOT NULL
	DROP TABLE [dbo].[EmployeeRoleRL]
GO
CREATE TABLE [dbo].[EmployeeRoleRL](
	[PKEmployeeRoleRLID] 	uniqueidentifier 	NOT NULL	DEFAULT NEWID(),
	[FKEmployeeID] 				uniqueidentifier 	NOT NULL,
	[FKRoleID] 						uniqueidentifier 	NOT NULL,
	[IsDeleted] 					bit 							NOT NULL,
	[KnowledgeDate] 			datetime 					NOT NULL	DEFAULT GETDATE(),
	[TimeToken] 					timestamp 				NOT NULL,
	[CreatedTime] 				datetime	 				NOT NULL	DEFAULT GETDATE(),
 CONSTRAINT [PK_EmployeeRoleRL] PRIMARY KEY ([PKEmployeeRoleRLID]) 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmployeeRoleRL]  WITH CHECK 
ADD  CONSTRAINT [FK_EmployeeRoleRL_Employee] FOREIGN KEY([FKEmployeeID])
REFERENCES [dbo].[Employee] ([PKEmployeeID])
GO

ALTER TABLE [dbo].[EmployeeRoleRL]  WITH CHECK 
ADD  CONSTRAINT [FK_EmployeeRoleRL_Role] FOREIGN KEY([FKRoleID])
REFERENCES [dbo].[Role] ([PKRoleID])
GO

/*==============================================================*/
/* Table: [dbo].[LeaveInfo]                                         */
/*==============================================================*/
IF OBJECT_ID(N'[dbo].[LeaveInfo]', N'U') IS NOT NULL
	DROP TABLE [dbo].[LeaveInfo]
GO
CREATE TABLE [dbo].[LeaveInfo](
	[PKLeaveInfoID] 			uniqueidentifier 	NOT NULL	DEFAULT NEWID(),
	[FKSubmitEmployeeID] 	uniqueidentifier 	NOT NULL,
	[FKReportManagerID] 	uniqueidentifier 	NOT NULL,
	[FKLeaveTypeID] 			uniqueidentifier 	NOT NULL,
	[Status] 							smallint 					NOT NULL,
	[Reason] 							nvarchar(50) 			NOT NULL,
	[FirstStartTime] 			datetime 					NULL,
	[LastEndTime] 				datetime 					NULL,
	[Description] 				nvarchar(500) 		NULL,
	[KnowledgeDate] 			datetime 					NOT NULL	DEFAULT GETDATE(),
	[TimeToken] 					timestamp 				NOT NULL,
	[CreatedTime] 				datetime 					NOT NULL	DEFAULT GETDATE(),
 CONSTRAINT [PK_LeaveInfo] PRIMARY KEY ([PKLeaveInfoID]) 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LeaveInfo]  WITH CHECK 
ADD  CONSTRAINT [FK_LeaveInfo_Employee] FOREIGN KEY([FKReportManagerID])
REFERENCES [dbo].[Employee] ([PKEmployeeID])
GO

ALTER TABLE [dbo].[LeaveInfo]  WITH CHECK 
ADD  CONSTRAINT [FK_LeaveInfo_Employee1] FOREIGN KEY([FKSubmitEmployeeID])
REFERENCES [dbo].[Employee] ([PKEmployeeID])
GO

ALTER TABLE [dbo].[LeaveInfo]  WITH CHECK 
ADD  CONSTRAINT [FK_LeaveInfo_LeaveType] FOREIGN KEY([FKLeaveTypeID])
REFERENCES [dbo].[LeaveType] ([PKLeaveTypeID])
GO

/*==============================================================*/
/* Table: [dbo].[TimeDurationInfo]                                         */
/*==============================================================*/
IF OBJECT_ID(N'[dbo].[TimeDurationInfo]', N'U') IS NOT NULL
	DROP TABLE [dbo].[TimeDurationInfo]
GO
CREATE TABLE [dbo].[TimeDurationInfo](
	[PKTDInfoID] 		uniqueidentifier 	NOT NULL	DEFAULT NEWID(),
	[FKLeaveInfoID] uniqueidentifier 	NOT NULL,
	[StartTime] 		datetime 					NOT NULL,
	[EndTime] 			datetime 					NOT NULL,
	[IsDeleted] 		bit 							NOT NULL,
	[KnowledgeDate] datetime 					NOT NULL	DEFAULT GETDATE(),
	[TimeToken] 		timestamp 				NOT NULL,
	[CreatedTime] 	datetime 					NOT NULL	DEFAULT GETDATE(),
 CONSTRAINT [PK_TimeDurationInfo] PRIMARY KEY ([PKTDInfoID]) 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TimeDurationInfo]  WITH CHECK 
ADD  CONSTRAINT [FK_TimeDurationInfo_LeaveInfo] FOREIGN KEY([FKLeaveInfoID])
REFERENCES [dbo].[LeaveInfo] ([PKLeaveInfoID])
GO

/*==============================================================*/
/* Table: [dbo].[EmployeeLeaveSummary]                                         */
/*==============================================================*/
IF OBJECT_ID(N'[dbo].[EmployeeLeaveSummary]', N'U') IS NOT NULL
	DROP TABLE [dbo].[EmployeeLeaveSummary]
GO
CREATE TABLE [dbo].[EmployeeLeaveSummary](
	[PKELSID] 			uniqueidentifier 		NOT NULL	DEFAULT NEWID(),
	[FKEmployeeID] 	uniqueidentifier 		NOT NULL,
	[FKLeaveTypeID] uniqueidentifier 		NOT NULL,
	[UsedHours] 		float 							NOT NULL,
	[KnowledgeDate] datetime 						NOT NULL	DEFAULT GETDATE(),
	[TimeToken] 		timestamp 					NOT NULL,
	[CreatedTime] 	datetime 						NOT NULL	DEFAULT GETDATE(),
	[Year]  int       NOT NULL ,
 CONSTRAINT [PK_EmployeeLeaveSummary] PRIMARY KEY ([PKELSID]), 
 CONSTRAINT [UniqueKeyEmployeeID_LeaveTypeID] UNIQUE ([FKEmployeeID], [FKLeaveTypeID]) 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmployeeLeaveSummary]  WITH CHECK 
ADD  CONSTRAINT [FK_EmployeeLeaveSummary_Employee] FOREIGN KEY([FKEmployeeID])
REFERENCES [dbo].[Employee] ([PKEmployeeID])
GO

ALTER TABLE [dbo].[EmployeeLeaveSummary]  WITH CHECK 
ADD  CONSTRAINT [FK_EmployeeLeaveSummary_LeaveType] FOREIGN KEY([FKLeaveTypeID])
REFERENCES [dbo].[LeaveType] ([PKLeaveTypeID])
GO

/*==============================================================*/
/* Table: [dbo].[ReportPeriod]                                         */
/*==============================================================*/
IF OBJECT_ID(N'[dbo].[ReportPeriod]', N'U') IS NOT NULL
	DROP TABLE [dbo].[ReportPeriod]
GO
CREATE TABLE [dbo].[ReportPeriod](
	[PKReportPeriodID] 	uniqueidentifier 	NOT NULL	DEFAULT NEWID(),
	[Month] 						smallint 					NOT NULL,
	[StartTime] 				datetime 					NOT NULL,
	[EndTime] 					datetime 					NOT NULL,
	[KnowledgeDate] 		datetime 					NOT NULL	DEFAULT GETDATE(),
	[TimeToken] 				timestamp 				NOT NULL,
	[CreatedTime] 			datetime 					NOT NULL	DEFAULT GETDATE(),
 CONSTRAINT [PK_ReportPeriod] PRIMARY KEY ([PKReportPeriodID]),
 CONSTRAINT [UniqueKeyReportPeriodMonth] UNIQUE ([Month]) 
) ON [PRIMARY]
GO

/*==============================================================*/
/* View: [dbo].[vEmployee]                                         */
/*==============================================================*/
IF OBJECT_ID('dbo.vEmployee') IS NOT NULL 
DROP VIEW dbo.vEmployee
GO

CREATE VIEW [dbo].[vEmployee]
AS
	SELECT
	  e1.PKEmployeeID,
	  e1.FirstName + ISNULL(N' ' + e1.MiddleName + N' ', N' ') + e1.LastName [EmployeeName],
	  e1.Email,
	  e1.Password,
	  e2.FirstName + ISNULL(N' ' + e2.MiddleName + N' ', N' ') + e2.LastName [ReportManager],
	  CASE e1.Gender
	    WHEN '1' THEN 'Male'
	    ELSE 'Female'
	  END [Gender],
	  e1.ServiceYears,
	  e1.HiredDate,
	  e1.Phone,
	  e1.Mobile,
	  e1.Comment,
	  e1.IsActive
	FROM dbo.Employee e1
	  INNER JOIN dbo.Employee e2
	  ON e1.FKReportManagerID = e2.PKEmployeeID
	WHERE e1.IsActive = '1'
GO

/*==============================================================*/
/* View: [dbo].[vLeaveInfo]                                         */
/*==============================================================*/
IF OBJECT_ID('dbo.vLeaveInfo') IS NOT NULL 
DROP VIEW dbo.vLeaveInfo
GO

CREATE VIEW [dbo].[vLeaveInfo]
AS
	SELECT 
	  li.PKLeaveInfoID,
	  li.FKSubmitEmployeeID, 
	  e.EmployeeName [SubmitEmployee],
	  e.ReportManager [ReportManager],
	  lt.Name [LeaveTypeName],
	  li.Status, --
	  li.Reason,
	  li.Description,
	  tdi.StartTime,
	  tdi.EndTime,
	  tdi.IsDeleted
	FROM dbo.LeaveInfo li 
	  INNER JOIN dbo.TimeDurationInfo tdi 
	  ON tdi.FKLeaveInfoID = li.PKLeaveInfoID
	  INNER JOIN dbo.LeaveType lt
	  ON li.FKLeaveTypeID = lt.PKLeaveTypeID
	  INNER JOIN dbo.vEmployee e
	  ON li.FKSubmitEmployeeID = e.PKEmployeeID
	WHERE tdi.IsDeleted = '0'
GO

/* ############################################################################################### */
/****************************************************************************
 * File: cars_crsd.sql
 * Purpose: This file is used to insert necessary data in the new database.
 * 
*****************************************************************************/

/** [dbo].[LeaveType] **/
INSERT INTO [dbo].[LeaveType]
([Name], [TotalHours],	[StartTime], [EndTime])
VALUES
(N'Annual', 120, '2011-01-01', '2011-12-31')

INSERT INTO [dbo].[LeaveType]
([Name], [TotalHours],	[StartTime], [EndTime])
VALUES
(N'Sick', 80, '2011-01-01', '2011-12-31')

INSERT INTO [dbo].[LeaveType]
([Name], [TotalHours],	[StartTime], [EndTime])
VALUES
(N'Marriage', 80, '2011-01-01', '2011-12-31')

INSERT INTO [dbo].[LeaveType]
([Name], [TotalHours],	[StartTime], [EndTime])
VALUES
(N'Maternity', 960, '2011-01-01', '2011-12-31')

INSERT INTO [dbo].[LeaveType]
([Name], [TotalHours],	[StartTime], [EndTime])
VALUES
(N'Paternity', 40, '2011-01-01', '2011-12-31')

INSERT INTO [dbo].[LeaveType]
([Name], [TotalHours],	[StartTime], [EndTime])
VALUES
(N'Bereavement', 40, '2011-01-01', '2011-12-31')

--INSERT INTO [dbo].[LeaveType]
--([PKLeaveTypeID], [Name], [TotalHours],	[StartTime], [EndTime],	[LongestHoursOneTime], [LeastHoursOneTime],	[Description], [KnowledgeDate],	[CreatedTime])
--VALUES
--(NEWID(), N'Regular Check', , '2011-01-01', '2011-12-31', 0, 0, NULL, GETDATE(), GETDATE())


/** [dbo].[Role] **/
INSERT INTO [dbo].[Role]
([Name])
VALUES
(N'Admin')

INSERT INTO [dbo].[Role]
([Name])
VALUES
(N'Manager')

INSERT INTO [dbo].[Role]
([Name])
VALUES
(N'Employee')


/** [dbo].[Employee] **/
--Richard
INSERT INTO [dbo].[Employee]
([Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(N'RChou@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Richard', N'Chou', 0, '2009-11-01', 1)

DECLARE @RChouID uniqueidentifier
SELECT @RChouID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'RChou@Advent.com'

--Helen
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@RChouID, N'HNiu@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 2, N'Helen', N'Niu', 0, '2009-11-01', 1)

DECLARE @HNiuID uniqueidentifier
SELECT @HNiuID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'HNiu@Advent.com'
DECLARE @AdminRoleID uniqueidentifier
SELECT @AdminRoleID = PKRoleID FROM [dbo].[Role] WHERE [Name] = N'Admin'
DECLARE @ManagerRoleID uniqueidentifier
SELECT @ManagerRoleID = PKRoleID FROM [dbo].[Role] WHERE [Name] = N'Manager'
DECLARE @EmployeeRoleID uniqueidentifier
SELECT @EmployeeRoleID = PKRoleID FROM [dbo].[Role] WHERE [Name] = N'Employee'

--Belinda
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@HNiuID, N'BYin@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 2, N'Belinda', N'Yin', 0, '2009-11-01', 1)

DECLARE @BYinID uniqueidentifier
SELECT @BYinID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'BYin@Advent.com'

/** [dbo].[EmployeeRoleRL] **/
INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@RChouID, @AdminRoleID, 0)
INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@RChouID, @ManagerRoleID, 0)
INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@RChouID, @EmployeeRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@HNiuID, @AdminRoleID, 0)
INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@HNiuID, @ManagerRoleID, 0)
INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@HNiuID, @EmployeeRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@BYinID, @AdminRoleID, 0)
INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@BYinID, @ManagerRoleID, 0)
INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@BYinID, @EmployeeRoleID, 0)

/* ############################################################################################### */
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

set @sql = @sql + ' and not ( LastEndTime < ''' + cast(@StartTime as varchar(50)) + ''' or FirstStartTime > ''' + cast(@EndTime as varchar(50)) + ''' )'

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
/* SP: dbo.pGetAdminAnnualLeaveReportData                             */
/*==============================================================*/
IF OBJECT_ID('dbo.pGetAdminAnnualLeaveReportData') IS NOT NULL
    DROP PROCEDURE dbo.pGetAdminAnnualLeaveReportData
GO
/*  
TESTING:
    EXEC dbo.pGetAdminAnnualLeaveReportData 
*/
CREATE PROCEDURE dbo.pGetAdminAnnualLeaveReportData
AS
BEGIN
  BEGIN TRY
    DECLARE @JAN_StartDate		datetime,
	        @JAN_EndDate		datetime,
	        @FEB_StartDate		datetime,
	        @FEB_EndDate		datetime,
	        @MAR_StartDate		datetime,
	        @MAR_EndDate		datetime,
	        @APR_StartDate		datetime,
	        @APR_EndDate		datetime,
	        @MAY_StartDate		datetime,
	        @MAY_EndDate		datetime,
	        @JUN_StartDate		datetime,
	        @JUN_EndDate		datetime,
	        @JUL_StartDate		datetime,
	        @JUL_EndDate		datetime,
	        @AUG_StartDate		datetime,
	        @AUG_EndDate		datetime,
	        @SEP_StartDate		datetime,
	        @SEP_EndDate		datetime,
	        @OCT_StartDate		datetime,
	        @OCT_EndDate		datetime,
	        @NOV_StartDate		datetime,
	        @NOV_EndDate		datetime,
	        @DEC_StartDate		datetime,
	        @DEC_EndDate		datetime,
	        @CurrentMonth		int
	        
	SET @CurrentMonth = MONTH(GETDATE());
	IF(@CurrentMonth >= 1)
	BEGIN
	  SELECT 
	    @JAN_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @JAN_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 1
	END
	IF(@CurrentMonth >= 2)
	BEGIN
	  SELECT 
	    @FEB_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @FEB_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 2
	END
	IF(@CurrentMonth >= 3)
	BEGIN
	  SELECT 
	    @MAR_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @MAR_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 3
	END
	IF(@CurrentMonth >= 4)
	BEGIN
	  SELECT 
	    @APR_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @APR_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 4
	END
	IF(@CurrentMonth >= 5)
	BEGIN
	  SELECT 
	    @MAY_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @MAY_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 5
	END
	IF(@CurrentMonth >= 6)
	BEGIN
	  SELECT 
	    @JUN_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @JUN_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 6
	END
	IF(@CurrentMonth >= 7)
	BEGIN
	  SELECT 
	    @JUL_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @JUL_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 7
	END
	IF(@CurrentMonth >= 8)
	BEGIN
	  SELECT 
	    @AUG_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @AUG_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 8
	END
	IF(@CurrentMonth >= 9)
	BEGIN
	  SELECT 
	    @SEP_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @SEP_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 9
	END
	IF(@CurrentMonth >= 10)
	BEGIN
	  SELECT 
	    @OCT_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @OCT_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 10
	END
	IF(@CurrentMonth >= 11)
	BEGIN
	  SELECT 
	    @NOV_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @NOV_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 11
	END
	IF(@CurrentMonth >= 12)
	BEGIN
	  SELECT 
	    @DEC_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @DEC_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 12
	END
    SELECT
    EmployeeName,
	CONVERT(date, HiredDate) HiredDate,
	CASE
      WHEN DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') + 1 >= 12 THEN 12
      ELSE DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') + 1
    END ValidMonths,
    PKEmployeeID,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @JAN_StartDate, @JAN_EndDate) AS JANLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @FEB_StartDate, @FEB_EndDate) AS FEBLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @MAR_StartDate, @MAR_EndDate) AS MARLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @APR_StartDate, @APR_EndDate) AS APRLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @MAY_StartDate, @MAY_EndDate) AS MAYLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @JUN_StartDate, @JUN_EndDate) AS JUNLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @JUL_StartDate, @JUL_EndDate) AS JULLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @AUG_StartDate, @AUG_EndDate) AS AUGLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @SEP_StartDate, @SEP_EndDate) AS SEPLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @OCT_StartDate, @OCT_EndDate) AS OCTLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @NOV_StartDate, @NOV_EndDate) AS NOVLeaveTakenHours,
    dbo.fGetLeaveTaken('Annual', PKEmployeeID, @DEC_StartDate, @DEC_EndDate) AS DECLeaveTakenHours
    FROM dbo.vEmployee
	ORDER BY EmployeeName
  
	RETURN @@ERROR
  END TRY
  BEGIN CATCH
	RETURN ERROR_NUMBER()
  END CATCH
END
GO


/*==============================================================*/
/* SP: dbo.pGetAdminSickLeaveReportData                             */
/*==============================================================*/
IF OBJECT_ID('dbo.pGetAdminSickLeaveReportData') IS NOT NULL
    DROP PROCEDURE dbo.pGetAdminSickLeaveReportData
GO
/*  
TESTING:
    EXEC dbo.pGetAdminSickLeaveReportData 
*/
CREATE PROCEDURE dbo.pGetAdminSickLeaveReportData
AS
BEGIN
  BEGIN TRY
    DECLARE @JAN_StartDate		datetime,
	        @JAN_EndDate		datetime,
	        @FEB_StartDate		datetime,
	        @FEB_EndDate		datetime,
	        @MAR_StartDate		datetime,
	        @MAR_EndDate		datetime,
	        @APR_StartDate		datetime,
	        @APR_EndDate		datetime,
	        @MAY_StartDate		datetime,
	        @MAY_EndDate		datetime,
	        @JUN_StartDate		datetime,
	        @JUN_EndDate		datetime,
	        @JUL_StartDate		datetime,
	        @JUL_EndDate		datetime,
	        @AUG_StartDate		datetime,
	        @AUG_EndDate		datetime,
	        @SEP_StartDate		datetime,
	        @SEP_EndDate		datetime,
	        @OCT_StartDate		datetime,
	        @OCT_EndDate		datetime,
	        @NOV_StartDate		datetime,
	        @NOV_EndDate		datetime,
	        @DEC_StartDate		datetime,
	        @DEC_EndDate		datetime,
	        @CurrentMonth		int
	        
	SET @CurrentMonth = MONTH(GETDATE());
	IF(@CurrentMonth >= 1)
	BEGIN
	  SELECT 
	    @JAN_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @JAN_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 1
	END
	IF(@CurrentMonth >= 2)
	BEGIN
	  SELECT 
	    @FEB_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @FEB_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 2
	END
	IF(@CurrentMonth >= 3)
	BEGIN
	  SELECT 
	    @MAR_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @MAR_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 3
	END
	IF(@CurrentMonth >= 4)
	BEGIN
	  SELECT 
	    @APR_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @APR_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 4
	END
	IF(@CurrentMonth >= 5)
	BEGIN
	  SELECT 
	    @MAY_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @MAY_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 5
	END
	IF(@CurrentMonth >= 6)
	BEGIN
	  SELECT 
	    @JUN_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @JUN_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 6
	END
	IF(@CurrentMonth >= 7)
	BEGIN
	  SELECT 
	    @JUL_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @JUL_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 7
	END
	IF(@CurrentMonth >= 8)
	BEGIN
	  SELECT 
	    @AUG_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @AUG_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 8
	END
	IF(@CurrentMonth >= 9)
	BEGIN
	  SELECT 
	    @SEP_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @SEP_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 9
	END
	IF(@CurrentMonth >= 10)
	BEGIN
	  SELECT 
	    @OCT_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @OCT_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 10
	END
	IF(@CurrentMonth >= 11)
	BEGIN
	  SELECT 
	    @NOV_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @NOV_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 11
	END
	IF(@CurrentMonth >= 12)
	BEGIN
	  SELECT 
	    @DEC_StartDate = DATEADD(hour, 9, CONVERT(nvarchar, StartTime, 23)),
	    @DEC_EndDate = DATEADD(hour, 17, CONVERT(nvarchar, EndTime, 23))
	  FROM dbo.ReportPeriod
	  WHERE Month = 12
	END
    SELECT
    EmployeeName,
    CONVERT(date, HiredDate) HiredDate,
	CASE
      WHEN DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') + 1 >= 12 THEN 12
      ELSE DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') + 1
    END ValidMonths,
    PKEmployeeID,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @JAN_StartDate, @JAN_EndDate) AS JANLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @FEB_StartDate, @FEB_EndDate) AS FEBLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @MAR_StartDate, @MAR_EndDate) AS MARLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @APR_StartDate, @APR_EndDate) AS APRLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @MAY_StartDate, @MAY_EndDate) AS MAYLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @JUN_StartDate, @JUN_EndDate) AS JUNLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @JUL_StartDate, @JUL_EndDate) AS JULLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @AUG_StartDate, @AUG_EndDate) AS AUGLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @SEP_StartDate, @SEP_EndDate) AS SEPLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @OCT_StartDate, @OCT_EndDate) AS OCTLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @NOV_StartDate, @NOV_EndDate) AS NOVLeaveTakenHours,
    dbo.fGetLeaveTaken('Sick', PKEmployeeID, @DEC_StartDate, @DEC_EndDate) AS DECLeaveTakenHours
    FROM dbo.vEmployee
    ORDER BY EmployeeName
  
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
	        @leavetypename		nvarchar(500)
AS
BEGIN
  BEGIN TRY
    SELECT
    EmployeeName,
    CONVERT(date, HiredDate) HiredDate,
	CASE
      WHEN DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') + 1 >= 12 THEN 12
      ELSE DATEDIFF(MONTH, HiredDate, CONVERT(nvarchar, YEAR(GETDATE())) + '-12-31') + 1
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
    ORDER BY EmployeeName
	RETURN @@ERROR
  END TRY
  BEGIN CATCH
	RETURN ERROR_NUMBER()
  END CATCH
END
GO
/* ############################################################################################### */
DECLARE @RChouID uniqueidentifier
SELECT @RChouID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'RChou@Advent.com'

DECLARE @HNiuID uniqueidentifier
SELECT @HNiuID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'HNiu@Advent.com'

DECLARE @AdminRoleID uniqueidentifier
SELECT @AdminRoleID = PKRoleID FROM [dbo].[Role] WHERE [Name] = N'Admin'

DECLARE @ManagerRoleID uniqueidentifier
SELECT @ManagerRoleID = PKRoleID FROM [dbo].[Role] WHERE [Name] = N'Manager'

DECLARE @EmployeeRoleID uniqueidentifier
SELECT @EmployeeRoleID = PKRoleID FROM [dbo].[Role] WHERE [Name] = N'Employee'

--Scott
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@HNiuID, N'SSun@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Scott', N'Sun', 0, '2009-11-01', 1)

DECLARE @SSunID uniqueidentifier
SELECT @SSunID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'SSun@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@SSunID, @ManagerRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@SSunID, @EmployeeRoleID, 0)

--Stanley
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@SSunID, N'SZhao@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Stanley', N'Zhao', 0, '2010-12-01', 1)

DECLARE @SZhaoID uniqueidentifier
SELECT @SZhaoID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'SZhao@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@SZhaoID, @EmployeeRoleID, 0)

--Jing
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@HNiuID, N'JLi@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 2, N'Jing', N'Li', 0, '2009-11-01', 1)

DECLARE @JLiID uniqueidentifier
SELECT @JLiID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'JLi@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@JLiID, @ManagerRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@JLiID, @EmployeeRoleID, 0)

--Hao
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@JLiID, N'HWu@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Hao', N'Wu', 0, '2009-11-01', 1)

DECLARE @HWuID uniqueidentifier
SELECT @HWuID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'HWu@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@HWuID, @ManagerRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@HWuID, @EmployeeRoleID, 0)

--Shan
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@HWuID, N'SHao@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 2, N'Shan', N'Hao', 0, '2009-11-01', 1)

DECLARE @SHaoID uniqueidentifier
SELECT @SHaoID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'SHao@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@SHaoID, @EmployeeRoleID, 0)

--Joel
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@HNiuID, N'JZhao@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Joel', N'Zhao', 0, '2009-11-01', 1)

DECLARE @JZhaoID uniqueidentifier
SELECT @JZhaoID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'JZhao@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@JZhaoID, @ManagerRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@JZhaoID, @EmployeeRoleID, 0)

--Brandon
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@JZhaoID, N'BShen@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Brandon', N'Shen', 0, '2009-11-01', 1)

DECLARE @BShenID uniqueidentifier
SELECT @BShenID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'BShen@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@BShenID, @EmployeeRoleID, 0)

--fill ReportPeriod for reports
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (1
           ,'2011-1-1'
           ,'2011-1-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (2
           ,'2011-1-26'
           ,'2011-2-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (3
           ,'2011-2-26'
           ,'2011-3-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (4
           ,'2011-3-26'
           ,'2011-4-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (5
           ,'2011-4-26'
           ,'2011-5-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (6
           ,'2011-5-26'
           ,'2011-6-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (7
           ,'2011-6-26'
           ,'2011-7-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (8
           ,'2011-7-26'
           ,'2011-8-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (9
           ,'2011-8-26'
           ,'2011-9-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (10
           ,'2011-9-26'
           ,'2011-10-25')
GO


