using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CARS.Backend.Entity;
using CARS.Backend.DAL;
using CARS.Backend.Common;
using CARS.Backend.BLL;

namespace CARS.UnitTest.Backend
{
    [TestClass]
    public class EmployeeLeaveSummaryUT
    {
        [TestMethod]
        public void TestInsertEmployeeLeaveSummaryByApproveLeave()
        {
            // Insert a manager role
            Role managerRole = Role.CreateRole(Guid.NewGuid().ToString());
            managerRole.Save();

            // Insert an employee who is a manager
            Employee manager = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Female, "Helen", DateTime.Now, "Niu");
            manager.IsActive = true;
            manager.ServiceYears = 8;
            List<EmployeeRoleRL> managerRoleList = new List<EmployeeRoleRL>();
            managerRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, managerRole.PKRoleID));
            manager.SetRoleList(managerRoleList);
            manager.Save();
            Guid managerGuid = manager.PKEmployeeID;

            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Hui", DateTime.Now, "Ji");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            employee.FKReportManagerID = managerGuid;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert a leave time duration for this leave
            DateTime durationStartTime = Convert.ToDateTime("02/02/2001");
            DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
            TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
            DateTime durationStartTime2 = Convert.ToDateTime("02/02/2011");
            DateTime durationEndTime2 = Convert.ToDateTime("02/05/2011");
            TimeDurationInfo timeDurationInfo2 = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime2, durationEndTime2);
            List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
            timeDurationList.Add(timeDurationInfo);
            timeDurationList.Add(timeDurationInfo2);
            int leaveHours = CommonMethods.ComputeHours(timeDurationList);

            LeaveInfo leave = LeaveBLL.ApplyLeave(employeeGuid.ToString(), "Test", leaveType.Name, "Test Desc", timeDurationList);

            // Approve the applying leave.
            LeaveBLL.ApproveLeave(managerGuid.ToString(), leave.PKLeaveInfoID.ToString(), LeaveStatus.Accepted);


            // Get the inserted employee leave summary
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("FKEmployeeID", employeeGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            conditions.Add(SearchCondition.CreateSearchCondition("FKLeaveTypeID", leaveTypeGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            EmployeeLeaveSummary employeeLeaveSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);

            Assert.IsNotNull(employeeLeaveSummary);
            Assert.AreEqual(leaveHours, employeeLeaveSummary.UsedHours);

            // Delete inserted items
            employeeLeaveSummary.Delete();
            leave.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            managerRole.Delete();
            role.Delete();
        }

        [TestMethod]
        public void TestUpdateEmployeeLeaveSummaryByApproveLeave()
        {
            // Insert a manager role
            Role managerRole = Role.CreateRole(Guid.NewGuid().ToString());
            managerRole.Save();

            // Insert an employee who is a manager
            Employee manager = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Female, "Helen", DateTime.Now, "Niu");
            manager.IsActive = true;
            manager.ServiceYears = 8;
            List<EmployeeRoleRL> managerRoleList = new List<EmployeeRoleRL>();
            managerRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, managerRole.PKRoleID));
            manager.SetRoleList(managerRoleList);
            manager.Save();
            Guid managerGuid = manager.PKEmployeeID;

            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Hui", DateTime.Now, "Ji");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            employee.FKReportManagerID = managerGuid;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert a leave time duration for this leave
            DateTime durationStartTime = Convert.ToDateTime("02/02/2001");
            DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
            TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
            DateTime durationStartTime2 = Convert.ToDateTime("02/02/2011");
            DateTime durationEndTime2 = Convert.ToDateTime("02/05/2011");
            TimeDurationInfo timeDurationInfo2 = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime2, durationEndTime2);
            List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
            timeDurationList.Add(timeDurationInfo);
            timeDurationList.Add(timeDurationInfo2);
            int leaveHours = CommonMethods.ComputeHours(timeDurationList);

            LeaveInfo leave = LeaveBLL.ApplyLeave(employeeGuid.ToString(), "Test", leaveType.Name, "Test Desc", timeDurationList);

            // Approve the applying leave.
            LeaveBLL.ApproveLeave(managerGuid.ToString(), leave.PKLeaveInfoID.ToString(), LeaveStatus.Accepted);


            // Get the inserted employee leave summary
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("FKEmployeeID", employeeGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            conditions.Add(SearchCondition.CreateSearchCondition("FKLeaveTypeID", leaveTypeGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            EmployeeLeaveSummary employeeLeaveSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);

            // Apply another leave
            DateTime durationStartTime3 = DateTime.Parse("03/04/2011");
            DateTime durationEndtime3 = DateTime.Parse("03/08/2011");
            TimeDurationInfo timeDurationInfo3 = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime3, durationEndtime3);
            DateTime durationStartTime4 = DateTime.Parse("03/10/2011");
            DateTime durationEndtime4 = DateTime.Parse("03/11/2011");
            TimeDurationInfo timeDurationInfo4 = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime4, durationEndtime4);
            List<TimeDurationInfo> timeDurationList2 = new List<TimeDurationInfo>();
            timeDurationList2.Add(timeDurationInfo3);
            timeDurationList2.Add(timeDurationInfo4);
            int newLeaveHours = CommonMethods.ComputeHours(timeDurationList2);

            LeaveInfo newLeave = LeaveBLL.ApplyLeave(employeeGuid.ToString(), "New Leave", leaveType.Name, "New Leave Desc", timeDurationList2);
            LeaveBLL.ApproveLeave(managerGuid.ToString(), newLeave.PKLeaveInfoID.ToString(), LeaveStatus.Accepted);

            // Get the update leave summary
            EmployeeLeaveSummary updatedLeaveSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);
            Assert.AreEqual(updatedLeaveSummary.UsedHours, leaveHours + newLeaveHours);

            // Delete inserted items
            employeeLeaveSummary.Delete();
            leave.Delete();
            newLeave.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            managerRole.Delete();
            role.Delete();
        }

        [TestMethod]
        public void TestDeleteEmployeeLeaveSummaryByApproveLeave()
        {
            // Insert a manager role
            Role managerRole = Role.CreateRole(Guid.NewGuid().ToString());
            managerRole.Save();

            // Insert an employee who is a manager
            Employee manager = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Female, "Helen", DateTime.Now, "Niu");
            manager.IsActive = true;
            manager.ServiceYears = 8;
            List<EmployeeRoleRL> managerRoleList = new List<EmployeeRoleRL>();
            managerRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, managerRole.PKRoleID));
            manager.SetRoleList(managerRoleList);
            manager.Save();
            Guid managerGuid = manager.PKEmployeeID;

            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Hui", DateTime.Now, "Ji");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            employee.FKReportManagerID = managerGuid;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert a leave time duration for this leave
            DateTime durationStartTime = Convert.ToDateTime("02/02/2001");
            DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
            TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
            DateTime durationStartTime2 = Convert.ToDateTime("02/02/2011");
            DateTime durationEndTime2 = Convert.ToDateTime("02/05/2011");
            TimeDurationInfo timeDurationInfo2 = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime2, durationEndTime2);
            List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
            timeDurationList.Add(timeDurationInfo);
            timeDurationList.Add(timeDurationInfo2);
            int leaveHours = CommonMethods.ComputeHours(timeDurationList);

            LeaveInfo leave = LeaveBLL.ApplyLeave(employeeGuid.ToString(), "Test", leaveType.Name, "Test Desc", timeDurationList);

            // Approve the applying leave.
            LeaveBLL.ApproveLeave(managerGuid.ToString(), leave.PKLeaveInfoID.ToString(), LeaveStatus.Accepted);


            // Get the inserted employee leave summary
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("FKEmployeeID", employeeGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            conditions.Add(SearchCondition.CreateSearchCondition("FKLeaveTypeID", leaveTypeGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            EmployeeLeaveSummary employeeLeaveSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);

            // Apply another leave
            DateTime durationStartTime3 = DateTime.Parse("03/04/2011");
            DateTime durationEndtime3 = DateTime.Parse("03/08/2011");
            TimeDurationInfo timeDurationInfo3 = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime3, durationEndtime3);
            DateTime durationStartTime4 = DateTime.Parse("03/10/2011");
            DateTime durationEndtime4 = DateTime.Parse("03/11/2011");
            TimeDurationInfo timeDurationInfo4 = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime4, durationEndtime4);
            List<TimeDurationInfo> timeDurationList2 = new List<TimeDurationInfo>();
            timeDurationList2.Add(timeDurationInfo3);
            timeDurationList2.Add(timeDurationInfo4);
            int newLeaveHours = CommonMethods.ComputeHours(timeDurationList2);

            LeaveInfo newLeave = LeaveBLL.ApplyLeave(employeeGuid.ToString(), "New Leave", leaveType.Name, "New Leave Desc", timeDurationList2);
            LeaveInfo delLeave = LeaveBLL.ApproveLeave(managerGuid.ToString(), newLeave.PKLeaveInfoID.ToString(), LeaveStatus.Accepted);
            delLeave.Delete();

            // Get the update leave summary
            EmployeeLeaveSummary updatedLeaveSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);
            Assert.AreEqual(updatedLeaveSummary.UsedHours, leaveHours);

            // Delete inserted items
            employeeLeaveSummary.Delete();
            leave.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            managerRole.Delete();
            role.Delete();
        }

        [TestMethod]
        public void TestInsertEmployeeLeaveSummary()
        {
            // Insert an employee role
            Role employeeRole = Role.CreateRole(Guid.NewGuid().ToString());
            employeeRole.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Tian", DateTime.Now, "Liu");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, employeeRole.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType1 = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType1.PKLeaveTypeID = Guid.NewGuid();
            leaveType1.SetKnowledgeDate(DateTime.Now);
            leaveType1.Save();
            Guid leaveTypeGuid = leaveType1.PKLeaveTypeID;

            // Insert another leave type
            DateTime secLeaveTypeStartTime = Convert.ToDateTime("01/01/2010");
            DateTime secLeaveTypeEndTime = Convert.ToDateTime("01/01/3010");
            LeaveType leaveType2 = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 200, secLeaveTypeStartTime, secLeaveTypeEndTime);
            leaveType2.PKLeaveTypeID = Guid.NewGuid();
            leaveType2.SetKnowledgeDate(DateTime.Now);
            leaveType2.Save();
            Guid secleaveTypeGuid = leaveType2.PKLeaveTypeID;

            // Insert an employee leave summary
            EmployeeLeaveSummary employeeLeaveSummary = EmployeeLeaveSummary.CreateEmployeeLeaveSummary(employeeGuid, leaveTypeGuid, DateTime.Now.Year);
            employeeLeaveSummary.PKELSID = Guid.NewGuid();
            employeeLeaveSummary.UsedHours = 10;
            employeeLeaveSummary.Save();
            Guid summaryGuid = employeeLeaveSummary.PKELSID;

            // Get the inserted employee leave summary
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKELSID", summaryGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            EmployeeLeaveSummary testSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);

            Assert.AreEqual(testSummary.FKEmployeeID, employeeLeaveSummary.FKEmployeeID);

            // Delete inserted items
            employeeLeaveSummary.Delete();
            leaveType1.Delete();
            leaveType2.Delete();
            employee.Delete();
            employeeRole.Delete();
        }

        [TestMethod]
        public void TestUpdateEmployeeLeaveSummary()
        {
            // Insert an employee role
            Role employeeRole = Role.CreateRole(Guid.NewGuid().ToString());
            employeeRole.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Tian", DateTime.Now, "Liu");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, employeeRole.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType1 = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType1.PKLeaveTypeID = Guid.NewGuid();
            leaveType1.SetKnowledgeDate(DateTime.Now);
            leaveType1.Save();
            Guid leaveTypeGuid = leaveType1.PKLeaveTypeID;

            // Insert another leave type
            DateTime secLeaveTypeStartTime = Convert.ToDateTime("01/01/2010");
            DateTime secLeaveTypeEndTime = Convert.ToDateTime("01/01/3010");
            LeaveType leaveType2 = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 200, secLeaveTypeStartTime, secLeaveTypeEndTime);
            leaveType2.PKLeaveTypeID = Guid.NewGuid();
            leaveType2.SetKnowledgeDate(DateTime.Now);
            leaveType2.Save();
            Guid secleaveTypeGuid = leaveType2.PKLeaveTypeID;

            // Insert an employee leave summary
            EmployeeLeaveSummary employeeLeaveSummary = EmployeeLeaveSummary.CreateEmployeeLeaveSummary(employeeGuid, leaveTypeGuid, DateTime.Now.Year);
            employeeLeaveSummary.PKELSID = Guid.NewGuid();
            employeeLeaveSummary.UsedHours = 10;
            employeeLeaveSummary.Save();
            Guid summaryGuid = employeeLeaveSummary.PKELSID;

            // Get the inserted employee leave summary
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKELSID", summaryGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            EmployeeLeaveSummary testSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);

            // Update the inserted employee leave summary
            testSummary.FKLeaveTypeID = secleaveTypeGuid;
            testSummary.Save();

            // Get the updated employee leave summary
            EmployeeLeaveSummary updatedSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);

            Assert.AreEqual(testSummary.FKEmployeeID, updatedSummary.FKEmployeeID);

            // Delete inserted items
            employeeLeaveSummary.Delete();
            leaveType1.Delete();
            leaveType2.Delete();
            employee.Delete();
            employeeRole.Delete();
        }

        [TestMethod]
        public void TestDeleteEmployeeLeaveSummary()
        {
            // Insert an employee role
            Role employeeRole = Role.CreateRole(Guid.NewGuid().ToString());
            employeeRole.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Tian", DateTime.Now, "Liu");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, employeeRole.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert an employee leave summary
            EmployeeLeaveSummary employeeLeaveSummary = EmployeeLeaveSummary.CreateEmployeeLeaveSummary(employeeGuid, leaveTypeGuid, DateTime.Now.Year);
            employeeLeaveSummary.PKELSID = Guid.NewGuid();
            employeeLeaveSummary.UsedHours = 10;
            employeeLeaveSummary.Save();
            Guid summaryGuid = employeeLeaveSummary.PKELSID;

            // Delete the employee leave summary
            employeeLeaveSummary.Delete();

            // Get the inserted employee leave summary
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKELSID", summaryGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            EmployeeLeaveSummary testSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);

            Assert.IsNull(testSummary);

            // Delete inserted items
            leaveType.Delete();
            employee.Delete();
            employeeRole.Delete();
        }

        [TestMethod]
        public void TestEmployeeLeaveSummaryTimeStamp()
        {
            // Insert an employee role
            Role employeeRole = Role.CreateRole(Guid.NewGuid().ToString());
            employeeRole.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Tian", DateTime.Now, "Liu");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, employeeRole.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert an employee leave summary
            EmployeeLeaveSummary employeeLeaveSummary = EmployeeLeaveSummary.CreateEmployeeLeaveSummary(employeeGuid, leaveTypeGuid, DateTime.Now.Year);
            employeeLeaveSummary.PKELSID = Guid.NewGuid();
            employeeLeaveSummary.UsedHours = 10;
            employeeLeaveSummary.Save();
            Guid summaryGuid = employeeLeaveSummary.PKELSID;

            // Get the inserted employee leave summary
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKELSID", summaryGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            EmployeeLeaveSummary testSummary1 = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);
            EmployeeLeaveSummary testSummary2 = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);

            // Update the employee leave summary
            testSummary1.UsedHours = 15;
            testSummary1.Save();
            testSummary2.UsedHours = 20;
            testSummary2.Save();

            // Get the updated employee leave summary 
            EmployeeLeaveSummary updatedSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);

            Assert.AreEqual(testSummary1.UsedHours, updatedSummary.UsedHours);
            Assert.AreNotEqual(testSummary2.UsedHours, updatedSummary.UsedHours);

            // Delete inserted items
            employeeLeaveSummary.Delete();
            leaveType.Delete();
            employee.Delete();
            employeeRole.Delete();
        }

    }
}
