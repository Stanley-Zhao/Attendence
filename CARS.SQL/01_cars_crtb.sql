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
	[CostCenter]						nvarchar(50)			NULL	DEFAULT '5030',
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
	  e1.IsActive,
	  e1.FKReportManagerID,
	  e1.CostCenter
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