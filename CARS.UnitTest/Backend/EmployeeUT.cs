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
    public class EmployeeUT
    {
        [TestMethod]
        public void TestInsertEmployee()
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

            // Get the employee who is inserted just now.
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Employee testEmployee = CommonDAL<Employee>.GetSingleObject(conditions);

            Assert.AreEqual(testEmployee.Email, employee.Email);

            employee.Delete();
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
            employee.Password = "123";
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid guid = employee.PKEmployeeID;

            // Get the employee who is inserted just now.
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Employee testEmployee = CommonDAL<Employee>.GetSingleObject(conditions);

            // Update the employee.
            testEmployee.Email = Guid.NewGuid().ToString();
            testEmployee.Save();

            // Get the employee who is updated just now.
            Employee updatedEmployee = CommonDAL<Employee>.GetSingleObject(conditions);

            Assert.AreEqual(testEmployee.Email, updatedEmployee.Email);
            employee.Delete();
            role.Delete();
        }

        [TestMethod]
        public void TestDeleteEmployee()
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

            // Delete the employee
            employee.Delete();

            // Get the employee who is deleted
            Guid guid = employee.PKEmployeeID;
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Employee testEmployee = CommonDAL<Employee>.GetSingleObject(conditions);

            Assert.IsNull(testEmployee);
            role.Delete();
        }

        [TestMethod]
        public void TestInsertEmployeeTransact()
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
            try
            {
                employee.Save();
            }
            catch
            {
                Console.WriteLine("Throw exception for transact unit test");
            }

            Guid guid = employee.PKEmployeeID;

            // Get the employee who is inserted just now.
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Employee testEmployee = CommonDAL<Employee>.GetSingleObject(conditions);

            Assert.IsNull(testEmployee);

            employee.Delete();
            role.Delete();
        }

        [TestMethod]
        public void TestUpdateEmployeeTransact()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Hui", DateTime.Now, "Ji");
            employee.IsActive = true;
            employee.Password = "123";
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid guid = employee.PKEmployeeID;

            // Get the employee who is inserted just now.
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Employee testEmployee = CommonDAL<Employee>.GetSingleObject(conditions);

            // Update the employee.
            testEmployee.Email = "BrandonBrandon";
            try
            {
                testEmployee.Save();
            }
            catch
            {
                Console.WriteLine("Throw exception for transact unit test");
            }
            

            // Get the employee who is updated just now.
            Employee updatedEmployee = CommonDAL<Employee>.GetSingleObject(conditions);

            Assert.AreNotEqual(testEmployee.Email, updatedEmployee.Email);
 
            employee.Delete();
            role.Delete();
        }

        [TestMethod]
        public void TestEmployeeTimeStamp()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "Hui", DateTime.Now, "Ji");
            employee.IsActive = true;
            employee.Password = "123";
            employee.ServiceYears = 10;
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid guid = employee.PKEmployeeID;

            // Get the employee who is inserted just now.
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Employee testEmployee1 = CommonDAL<Employee>.GetSingleObject(conditions);
            Employee testEmployee2 = CommonDAL<Employee>.GetSingleObject(conditions);

            // Update the employee.
            testEmployee1.Email = "BrandonBrandon";
            testEmployee1.Save();
            testEmployee2.Email = "ShanShan";
            testEmployee2.Save();

            // Get the employee who is updated just now.
            Employee updatedEmployee = CommonDAL<Employee>.GetSingleObject(conditions);

            Assert.AreEqual(testEmployee1.Email, updatedEmployee.Email);
            Assert.AreNotEqual(testEmployee2.Email, updatedEmployee.Email);
            
            // Delete inserted itmes
            employee.Delete();
            role.Delete();
        }
    }
}
