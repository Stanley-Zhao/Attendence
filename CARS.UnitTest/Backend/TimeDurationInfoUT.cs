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
    public class TimeDurationInfoUT
    {
        [TestMethod]
        public void TestInsertTimeDurationInfo()
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

            timeDurationInfo.FKLeaveInfoID = leaveGuid;
            timeDurationInfo.KnowledgeDate = DateTime.Now;
            timeDurationInfo.IsDeleted = false;
            timeDurationInfo.Save();
            Guid tdGuid = timeDurationInfo.PKTDInfoID;

            // Get the inserted time duration info
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKTDInfoID", tdGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            TimeDurationInfo testTimeDuration = CommonDAL<TimeDurationInfo>.GetSingleObject(conditions);

            Assert.AreEqual(timeDurationInfo.StartTime, testTimeDuration.StartTime);
            Assert.AreEqual(timeDurationInfo.EndTime, testTimeDuration.EndTime);

            // Delete inserted items
            timeDurationInfo.Delete();
            leaveInfo.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();
            }

        [TestMethod]
        public void TestUpdateTimeDurationInfo()
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

            timeDurationInfo.FKLeaveInfoID = leaveGuid;
            timeDurationInfo.KnowledgeDate = DateTime.Now;
            timeDurationInfo.IsDeleted = false;
            timeDurationInfo.Save();
            Guid tdGuid = timeDurationInfo.PKTDInfoID;

            // Get the inserted time duration info
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKTDInfoID", tdGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            TimeDurationInfo testTimeDuration = CommonDAL<TimeDurationInfo>.GetSingleObject(conditions);

            // Update the inseted time duratioin info
            testTimeDuration.EndTime = Convert.ToDateTime("02/10/2001");
            testTimeDuration.Save();

            // Get the updated time duration info
            TimeDurationInfo updatedTimeDuration = CommonDAL<TimeDurationInfo>.GetSingleObject(conditions);

            Assert.AreEqual(testTimeDuration.EndTime, updatedTimeDuration.EndTime);

            // Delete inserted items
            timeDurationInfo.Delete();
            leaveInfo.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();
        }

        [TestMethod]
        public void TestDeleteTimeDurationInfo()
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

            timeDurationInfo.FKLeaveInfoID = leaveGuid;
            timeDurationInfo.KnowledgeDate = DateTime.Now;
            timeDurationInfo.IsDeleted = false;
            timeDurationInfo.Save();
            Guid tdGuid = timeDurationInfo.PKTDInfoID;

            // Delete the inserted time duration info
            timeDurationInfo.Delete();

            // Get the inserted time duration info
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKTDInfoID", tdGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            TimeDurationInfo testTimeDuration = CommonDAL<TimeDurationInfo>.GetSingleObject(conditions);

            Assert.IsNull(testTimeDuration);

            // Delete inserted items
            leaveInfo.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();
        }

        [TestMethod]
        public void TestTimeDurationInfoTimeStamp()
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

            timeDurationInfo.FKLeaveInfoID = leaveGuid;
            timeDurationInfo.KnowledgeDate = DateTime.Now;
            timeDurationInfo.IsDeleted = false;
            timeDurationInfo.Save();
            Guid tdGuid = timeDurationInfo.PKTDInfoID;

            // Get the inserted time duration info
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKTDInfoID", tdGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            TimeDurationInfo testTimeDuration1 = CommonDAL<TimeDurationInfo>.GetSingleObject(conditions);
            TimeDurationInfo testTimeDuration2 = CommonDAL<TimeDurationInfo>.GetSingleObject(conditions);

            // Update the inseted time duratioin info
            testTimeDuration1.EndTime = Convert.ToDateTime("02/10/2001");
            testTimeDuration1.Save();
            testTimeDuration2.EndTime = Convert.ToDateTime("02/11/2002");
            testTimeDuration2.Save();

            // Get the updated time duration info
            TimeDurationInfo updatedTimeDuration = CommonDAL<TimeDurationInfo>.GetSingleObject(conditions);

            Assert.AreEqual(testTimeDuration1.EndTime, updatedTimeDuration.EndTime);
            Assert.AreNotEqual(testTimeDuration2.EndTime, updatedTimeDuration.EndTime);

            // Delete inserted items
            timeDurationInfo.Delete();
            leaveInfo.Delete();
            leaveType.Delete();
            employee.Delete();
            manager.Delete();
            employeeRole.Delete();
            managerRole.Delete();
        }
    }
}
