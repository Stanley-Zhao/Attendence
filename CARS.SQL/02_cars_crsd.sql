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