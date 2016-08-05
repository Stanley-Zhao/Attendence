using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CARS.Backend.Entity;
using CARS.Backend.Common;
using CARS.Backend.BLL;
using CARS.Backend.DAL;
using System.Threading;

namespace AutomateGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //GenerateClasses();
            ShowGenerateData();
            //Test();
        }

        private static void GenerateClasses()
        {
            GenerateEntity.Generate("Employee", "Employee");
            GenerateEntity.Generate("EmployeeRoleRL", "EmployeeRoleRL");
            GenerateEntity.Generate("LeaveInfo", "LeaveInfo");
            GenerateEntity.Generate("LeaveType", "LeaveType");
            GenerateEntity.Generate("Role", "Role");
            GenerateEntity.Generate("TimeDurationInfo", "TimeDurationInfo");
            GenerateEntity.Generate("EmployeeLeaveSummary", "EmployeeLeaveSummary");
        }

        private static void ShowGenerateData()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GenerateData());
        }

        private static void Test()
        {
            //TestAddEmployee();
            //TestGetEmployee();
            //TestUpdateEmployee();
            //TestLog();
            TestMultiThreadApproveLeave();
        }

        private static void TestLog()
        {
            Log.Info("info");
            Log.Debug("debug");
            Log.Exception("Exception");
            Log.Warn("warn");
        }

        private static void TestUpdateEmployee()
        {
            Employee employee = EmployeeBLL.GetEmployeeByID("4419029e-0492-44a1-b06f-4bf66648724b");
            employee.FirstName = "fasdfas";
            employee.Save();
        }

        private static void TestGetEmployee()
        {
            Employee employee = EmployeeBLL.GetEmployeeByID("844f2731-bed3-4574-bc5f-0c5131aef8b3");
        }

        private static void TestAddEmployee()
        {
            Role role = Role.GetRoleByName(RoleRank.Employee.ToString());

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(string.Format("GOGOGOGGO{0}", Guid.NewGuid().ToString().Substring(0, 4)), "123", Sex.Male, "Hui", DateTime.Now, "Ji");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
        }

        private static void TestMultiThreadApproveLeave()
        {
            Employee employee = PrepareData();

            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("FKSubmitEmployeeID", employee.PKEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
            conditions.Add(SearchCondition.CreateSearchCondition("Status", "1", SearchComparator.Equal, SearchType.SearchNotString));
            List<LeaveInfo> leaves = CommonDAL<LeaveInfo>.GetObjects(conditions);

            foreach (LeaveInfo item in leaves)
            {
                Thread t = new Thread(new ParameterizedThreadStart(ApplyLeave));
                t.Start(item);
            }
        }

        private static Employee PrepareData()
        {
            List<SearchCondition> conditions = new List<SearchCondition>();
					conditions.Add(SearchCondition.CreateSearchCondition("Email", "bshen@Advent.com", SearchComparator.Equal, SearchType.SearchString));
            Employee employee = CommonDAL<Employee>.GetSingleObject(conditions);

            List<TimeDurationInfo> list = new List<TimeDurationInfo>();
            list.Add(TimeDurationInfo.CreateTimeDurationInfo(DateTime.Now.AddDays(-1), DateTime.Now));

            for (int i = 0; i < 150; i++)
            {
                LeaveBLL.ApplyLeave(employee.PKEmployeeID.ToString(), "test multi thread", "Annual", "test multi thread", list);
            }

            return employee;
        }

        public static void ApplyLeave(object seed)
        {
            LeaveInfo leave = seed as LeaveInfo;
            LeaveBLL.ApproveLeave(leave.FKReportManagerID.ToString(), leave.PKLeaveInfoID.ToString(), LeaveStatus.Accepted);
        }
    }
}
