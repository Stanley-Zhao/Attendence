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
    public class LeaveBLLUT
    {
        [TestMethod]
        public void TestApplyLeave()
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

            LeaveInfo leave = LeaveBLL.ApplyLeave(employeeGuid.ToString(), "Test", leaveType.Name, "Test Desc", timeDurationList);

            // Get the employee's leave list
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("FKSubmitEmployeeID", employeeGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            List<LeaveInfo> leaveList = CommonDAL<LeaveInfo>.GetObjects(conditions);

            // Compare these two list
            bool result = leave.PKLeaveInfoID == leaveList[0].PKLeaveInfoID;
			//if (result)
			//{
			//    foreach (LeaveInfo leave in leave)
			//    {
			//        bool existAndEqual = false;
			//        foreach (LeaveInfo leaveItem in leaveList)
			//        {
			//            if (leave.FirstStartTime == leaveItem.FirstStartTime && leave.LastEndTime == leaveItem.LastEndTime)
			//            {
			//                existAndEqual = true;
			//                break;
			//            }
			//        }

			//        result = existAndEqual;
			//        if (!result) break;
			//    }
			//}

            Assert.IsTrue(result);

			//foreach (LeaveInfo leave in leave)
			//{
                leave.Delete();
			//}
            leaveType.Delete();
            employee.Delete();
            role.Delete();
            manager.Delete();
            managerRole.Delete();
        }

        [TestMethod]
        public void TestGetMyLeaves()
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

            LeaveInfo leave = LeaveBLL.ApplyLeave(employeeGuid.ToString(), "Test", leaveType.Name, "Test Desc", timeDurationList);

            // Get the employee's leave list
            List<LeaveInfo> leaveList = LeaveBLL.GetMyLeaves(employee.PKEmployeeID.ToString());

            // Compare these two list
            bool result = leave.PKLeaveInfoID == leaveList[0].PKLeaveInfoID;
            if (result)
            {
				//foreach (LeaveInfo leave in leave)
				//{
				//    bool existAndEqual = false;
				//    foreach (LeaveInfo leaveItem in leaveList)
				//    {
				//        if (leave.FirstStartTime == leaveItem.FirstStartTime && leave.LastEndTime == leaveItem.LastEndTime)
				//        {
				//            existAndEqual = true;
				//            break;
				//        }
				//    }

				//    result = existAndEqual;
				//    if (!result) break;
				//}
            }

            Assert.IsTrue(result);

			//foreach (LeaveInfo leave in leave)
			//{
                leave.Delete();
			//}
            leaveType.Delete();
            employee.Delete();
            role.Delete();
            manager.Delete();
            managerRole.Delete();
        }

        [TestMethod]
        public void TestGetMyTeamLeaves()
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

            LeaveInfo leave = LeaveBLL.ApplyLeave(employeeGuid.ToString(), "Test", leaveType.Name, "Test Desc", timeDurationList);

            // Get the employee's leave list
            List<LeaveInfo> leaveList = LeaveBLL.GetMyTeamLeaves(managerGuid.ToString(), true);

            // Compare these two list
            bool result = leave.PKLeaveInfoID == leaveList[0].PKLeaveInfoID;
            if (result)
            {
				//foreach (LeaveInfo leave in leave)
				//{
				//    bool existAndEqual = false;
				//    foreach (LeaveInfo leaveItem in leaveList)
				//    {
				//        if (leave.FirstStartTime == leaveItem.FirstStartTime && leave.LastEndTime == leaveItem.LastEndTime)
				//        {
				//            existAndEqual = true;
				//            break;
				//        }
				//    }

				//    result = existAndEqual;
				//    if (!result) break;
				//}
            }

            Assert.IsTrue(result);

			//foreach (LeaveInfo leave in leave)
			//{
                leave.Delete();
			//}
            leaveType.Delete();
            employee.Delete();
            role.Delete();
            manager.Delete();
            managerRole.Delete();
        }

        [TestMethod]
        public void TestApproveLeave()
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

            LeaveInfo leave = LeaveBLL.ApplyLeave(employeeGuid.ToString(), "Test", leaveType.Name, "Test Desc", timeDurationList);

            // Approve the applying leave.
            LeaveBLL.ApproveLeave(managerGuid.ToString(), leave.PKLeaveInfoID.ToString(), LeaveStatus.Accepted);

            // Get the employee's leave list
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("FKSubmitEmployeeID", employeeGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            List<LeaveInfo> leaveList = CommonDAL<LeaveInfo>.GetObjects(conditions);

            Assert.IsTrue(leaveList[0].PKLeaveInfoID == leave.PKLeaveInfoID && leaveList[0].Status == LeaveStatus.Accepted);

            leave.Delete();
            leaveType.Delete();
            employee.Delete();
            role.Delete();
            manager.Delete();
            managerRole.Delete();
        }
    }
}
