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
    public class EmployeeBLLUT
    {
        [TestMethod]
        public void TestLogin()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Hui", DateTime.Now, "Ji");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid guid = employee.PKEmployeeID;

            Employee testEmployee1 = EmployeeBLL.Login(employee.Email, employee.Password);
            Employee testEmployee2 = EmployeeBLL.Login(employee.Email, employee.Password + "test");

            Assert.IsTrue(testEmployee1.PKEmployeeID.Equals(employee.PKEmployeeID) && testEmployee2 == null);

            employee.Delete();
            role.Delete();
        }

        [TestMethod]
        public void TestAddEmployee()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();

            // Insert an manager.
            Employee manager = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Hui", DateTime.Now, "Ji");
            manager.IsActive = true;
            manager.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            manager.SetRoleList(employeeRoleList);
            manager.Save();
            Guid guid = manager.PKEmployeeID;

            Employee employee = EmployeeBLL.AddEmployee(Guid.NewGuid().ToString(), "Brandon", "None", "Shen", Sex.Male, 10, DateTime.Now, manager.Email, "5030", 0, "");

            Assert.IsTrue(null != employee && employee.FKReportManagerID.Equals(manager.PKEmployeeID));
            employee.Delete();
            manager.Delete();
            role.Delete();
        }

        [TestMethod]
        public void TestUpdateEmployee()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Hui", DateTime.Now, "Ji");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid guidEmployee = employee.PKEmployeeID;

            // Insert an manager.
            Employee manager = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Brandon", DateTime.Now, "Shen");
            manager.IsActive = true;
            manager.ServiceYears = 10;
            employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            manager.SetRoleList(employeeRoleList);
            manager.Save();
            Guid guidManager = manager.PKEmployeeID;

            EmployeeBLL.UpdateEmployee(employee.PKEmployeeID.ToString(), employee.Email, "nono", employee.MiddleName, 
                                       employee.LastName, employee.Gender, employee.ServiceYears, employee.HiredDate, 
                                       manager.Email, employee.Password, "999", employee.Mobile, true, true, true, "5030", 0, "");

            employee = EmployeeBLL.GetEmployeeByID(employee.PKEmployeeID.ToString());

            Assert.IsTrue(employee.IsPointedRole(role.Name)
                          &&  employee.FKReportManagerID == manager.PKEmployeeID 
                          && employee.FirstName.Equals("nono")
                          && employee.Phone.Equals("999"));

            employee.Delete();
            manager.Delete();
            role.Delete();
        }
    }
}
