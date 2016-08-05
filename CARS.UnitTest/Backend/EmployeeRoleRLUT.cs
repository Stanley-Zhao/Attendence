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
    public class EmployeeRoleRLUT
    {
        [TestMethod]
        public void TestInsertEmployeeRoleRL()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();
            Guid roleGuid = role.PKRoleID;

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "TIAN", DateTime.Now, "LIU");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            EmployeeRoleRL employeeRoleRL = EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleGuid);           
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(employeeRoleRL);
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;
            Guid employeeRoleRLGuid = employeeRoleRL.PKEmployeeRoleRLID;

            // Get the inserted employee role relation
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeRoleRLID", employeeRoleRLGuid.ToString(), 
                SearchComparator.Equal, SearchType.SearchString));
            EmployeeRoleRL testEemployeeRoleRL = CommonDAL<EmployeeRoleRL>.GetSingleObject(conditions);

            Assert.AreEqual(employeeRoleRL.FKRoleID, testEemployeeRoleRL.FKRoleID);

            // Delete inseted items
            employeeRoleRL.Delete();
            employee.Delete();
            role.Delete();

        }

        [TestMethod]
        public void TestUpdateEmployeeRoleRL()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();
            Guid roleGuid = role.PKRoleID;

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "TIAN", DateTime.Now, "LIU");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            EmployeeRoleRL employeeRoleRL = EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleGuid);
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(employeeRoleRL);
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;
            Guid employeeRoleRLGuid = employeeRoleRL.PKEmployeeRoleRLID;

            // Get the inserted employee role relation
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeRoleRLID", employeeRoleRLGuid.ToString(),
                SearchComparator.Equal, SearchType.SearchString));
            EmployeeRoleRL testEemployeeRoleRL = CommonDAL<EmployeeRoleRL>.GetSingleObject(conditions);

            // Insert another role 
            Role updatedRole = Role.CreateRole("UpdatedRole");
            updatedRole.Save();
            Guid updatedRoleGuid = updatedRole.PKRoleID;

            // Update the inserted emmployee role relation
            testEemployeeRoleRL.FKRoleID = updatedRoleGuid;
            testEemployeeRoleRL.Save();

            // Get the employee role relation updated just now
            EmployeeRoleRL updatedEmployeeRoleRL = CommonDAL<EmployeeRoleRL>.GetSingleObject(conditions);

            Assert.AreEqual(testEemployeeRoleRL.FKRoleID, updatedEmployeeRoleRL.FKRoleID);

            // Delete inseted items
            employeeRoleRL.Delete();
            employee.Delete();
            role.Delete();
            updatedRole.Delete();

        }

        [TestMethod]
        public void TestDeleteEmployeeRoleRL()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.Save();
            Guid roleGuid = role.PKRoleID;

            // Insert an employee.
            Employee employee = Employee.CreateEmployee(Guid.NewGuid().ToString(), "123", Sex.Male, "TIAN", DateTime.Now, "LIU");
            employee.IsActive = true;
            employee.ServiceYears = 10;
            EmployeeRoleRL employeeRoleRL = EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleGuid);
            List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
            employeeRoleList.Add(employeeRoleRL);
            employee.SetRoleList(employeeRoleList);
            employee.Save();
            Guid employeeGuid = employee.PKEmployeeID;
            Guid employeeRoleRLGuid = employeeRoleRL.PKEmployeeRoleRLID;

            // Delete the inserted employee role relation
            employeeRoleRL.Delete();

            // Get the inserted employee role relation
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeRoleRLID", employeeRoleRLGuid.ToString(),
                SearchComparator.Equal, SearchType.SearchString));
            EmployeeRoleRL testEemployeeRoleRL = CommonDAL<EmployeeRoleRL>.GetSingleObject(conditions);

            Assert.IsNull(testEemployeeRoleRL);

            // Delete inseted items
            employee.Delete();
            role.Delete();
        }
    }
}
