using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CARS.Backend.Entity;
using CARS.Backend.DAL;
using CARS.Backend.Common;

namespace CARS.UnitTest.Backend
{
    [TestClass]
    public class LeaveInfoUT
    {
        [TestMethod]
        public void TestInsertLeaveInfo()
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

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert a leave time duration for this leave
            DateTime durationStartTime = Convert.ToDateTime("02/02/2001");
            DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
            TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
            List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
            timeDurationList.Add(timeDurationInfo);

            // Insert a leave info
            LeaveInfo leaveInfo = LeaveInfo.CreateLeaveInfo(employeeGuid, managerGuid, leaveTypeGuid, LeaveStatus.Accepted,
                "For Test", "Test Description", timeDurationList);
            leaveInfo.Save();
            Guid leaveGuid = leaveInfo.PKLeaveInfoID;

            // Get the leave info inserted just now
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveInfoID", leaveGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveInfo testLeaveInfo = CommonDAL<LeaveInfo>.GetSingleObject(conditions);

            Assert.AreEqual(testLeaveInfo.Reason, leaveInfo.Reason);

            // Delete the leave, leave type, employees and roles
            leaveInfo.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();

        }

        [TestMethod]
        public void TestUpdateLeaveInfo()
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

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert a leave time duration for this leave
            DateTime durationStartTime = Convert.ToDateTime("02/02/2001");
            DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
            TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
            List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
            timeDurationList.Add(timeDurationInfo);

            // Insert a leave info
            LeaveInfo leaveInfo = LeaveInfo.CreateLeaveInfo(employeeGuid, managerGuid, leaveTypeGuid, LeaveStatus.Accepted,
                "For Test", "Test Description", timeDurationList);
            leaveInfo.Save();
            Guid leaveGuid = leaveInfo.PKLeaveInfoID;

            // Get the leave info inserted just now
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveInfoID", leaveGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveInfo testLeaveInfo = CommonDAL<LeaveInfo>.GetSingleObject(conditions);

            // Update the leave info
            testLeaveInfo.Reason = "Update test leave reason";
            testLeaveInfo.Save();

            // Get the updated leave info
            LeaveInfo updatedLeaveInfo = CommonDAL<LeaveInfo>.GetSingleObject(conditions);

            Assert.AreEqual(testLeaveInfo.Reason, updatedLeaveInfo.Reason);

            // Delete the leave, leave type, employees and roles
            leaveInfo.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();
        }

        [TestMethod]
        public void TestDeleteLeaveInfo()
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

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert a leave time duration for this leave
            DateTime durationStartTime = Convert.ToDateTime("02/02/2001");
            DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
            TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
            List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
            timeDurationList.Add(timeDurationInfo);

            // Insert a leave info
            LeaveInfo leaveInfo = LeaveInfo.CreateLeaveInfo(employeeGuid, managerGuid, leaveTypeGuid, LeaveStatus.Accepted,
                "For Test", "Test Description", timeDurationList);
            leaveInfo.Save();
            Guid leaveGuid = leaveInfo.PKLeaveInfoID;

            // Delete the leave info
            leaveInfo.Delete();

            // Get the leave info deleted just now
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveInfoID", leaveGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveInfo testLeaveInfo = CommonDAL<LeaveInfo>.GetSingleObject(conditions);

            Assert.IsNull(testLeaveInfo);

            // Delete the leave type, employees and roles
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();
        }

        [TestMethod]
        public void TestInsertLeaveInfoTransact()
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

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert a leave time duration for this leave
            DateTime durationStartTime = Convert.ToDateTime("02/06/2001");
            DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
            TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
            List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
            timeDurationList.Add(timeDurationInfo);

            // Insert a leave info
            LeaveInfo leaveInfo = LeaveInfo.CreateLeaveInfo(employeeGuid, managerGuid, leaveTypeGuid, LeaveStatus.Accepted,
                "For Test", "Test Description", timeDurationList);
            try
            {
                leaveInfo.Save();
            }
            catch
            {
                Console.WriteLine("Throw excetption for transact unit test");
            }
            Guid leaveGuid = leaveInfo.PKLeaveInfoID;

            // Get the leave info inserted just now
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveInfoID", leaveGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveInfo testLeaveInfo = CommonDAL<LeaveInfo>.GetSingleObject(conditions);

            Assert.IsNull(testLeaveInfo);

            // Delete the leave, leave type, employees and roles
            leaveInfo.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();
        }

        [TestMethod]
        public void TestUpdateLeaveInfoTransact()
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

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert a leave time duration for this leave
            DateTime durationStartTime = Convert.ToDateTime("02/02/2001");
            DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
            TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
            List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
            timeDurationList.Add(timeDurationInfo);

            // Insert a leave info
            LeaveInfo leaveInfo = LeaveInfo.CreateLeaveInfo(employeeGuid, managerGuid, leaveTypeGuid, LeaveStatus.Accepted,
                "For Test", "Test Description", timeDurationList);
            leaveInfo.Save();
            Guid leaveGuid = leaveInfo.PKLeaveInfoID;

            // Get the leave info inserted just now
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveInfoID", leaveGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveInfo testLeaveInfo = CommonDAL<LeaveInfo>.GetSingleObject(conditions);

            // Update the leave info
            testLeaveInfo.Reason = "Update test leave reason";
            try
            {
                testLeaveInfo.Save();
            }
            catch
            {
                Console.WriteLine("Throw exception for transact unit test");
            }

            // Get the updated leave info
            LeaveInfo updatedLeaveInfo = CommonDAL<LeaveInfo>.GetSingleObject(conditions);

            Assert.AreNotEqual(testLeaveInfo.Reason, updatedLeaveInfo.Reason);

            // Delete the leave, leave type, employees and roles
            leaveInfo.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();
        }

        [TestMethod]
        public void TestLeaveInfoTimeStamp()
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

            // Insert a leave type
            DateTime leaveTypeStartTime = Convert.ToDateTime("01/01/2000");
            DateTime leaveTypeEndTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, leaveTypeStartTime, leaveTypeEndTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid leaveTypeGuid = leaveType.PKLeaveTypeID;

            // Insert a leave time duration for this leave
            DateTime durationStartTime = Convert.ToDateTime("02/02/2001");
            DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
            TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
            List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
            timeDurationList.Add(timeDurationInfo);

            // Insert a leave info
            LeaveInfo leaveInfo = LeaveInfo.CreateLeaveInfo(employeeGuid, managerGuid, leaveTypeGuid, LeaveStatus.Accepted,
                "For Test", "Test Description", timeDurationList);
            leaveInfo.Save();
            Guid leaveGuid = leaveInfo.PKLeaveInfoID;

            // Get the leave info inserted just now
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveInfoID", leaveGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveInfo testLeaveInfo1 = CommonDAL<LeaveInfo>.GetSingleObject(conditions);
            LeaveInfo testLeaveInfo2 = CommonDAL<LeaveInfo>.GetSingleObject(conditions);

            // Update the leave info
            testLeaveInfo1.Reason = "Update test leave reason";
            testLeaveInfo1.Save();
            testLeaveInfo2.Reason = "Second Update";
            testLeaveInfo2.Save();

            // Get the updated leave info
            LeaveInfo updatedLeaveInfo = CommonDAL<LeaveInfo>.GetSingleObject(conditions);

            Assert.AreEqual(testLeaveInfo1.Reason, updatedLeaveInfo.Reason);
            Assert.AreNotEqual(testLeaveInfo2.Reason, updatedLeaveInfo.Reason);

            // Delete the leave, leave type, employees and roles
            leaveInfo.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();
        }
    }
}
