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
    public class RoleUT
    {
        [TestMethod]
        public void TestInsertRole()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.PKRoleID = new Guid();
            role.KnowledgeDate = DateTime.Now;
            role.Save();
            Guid roleGuid = role.PKRoleID;

            // Get the inserted role
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKRoleID", roleGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Role testRole = CommonDAL<Role>.GetSingleObject(conditions);

            Assert.AreEqual(role.Name, testRole.Name);

            // Delete the role
            role.Delete();
            
        }

        [TestMethod]
        public void TestUpdateRole()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.PKRoleID = new Guid();
            role.KnowledgeDate = DateTime.Now;
            role.Save();
            Guid roleGuid = role.PKRoleID;

            // Get the inserted role
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKRoleID", roleGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Role testRole = CommonDAL<Role>.GetSingleObject(conditions);

            // Update the inserted role
            testRole.Name = Guid.NewGuid().ToString().Substring(0, 5);
            testRole.Save();

            // Get the updated rolw
            Role updatedRole = CommonDAL<Role>.GetSingleObject(conditions);

            Assert.AreEqual(testRole.Name, updatedRole.Name);

            // Delete the role
            role.Delete();
        }

        [TestMethod]
        public void TestDeleteRole()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.PKRoleID = new Guid();
            role.KnowledgeDate = DateTime.Now;
            role.Save();
            Guid roleGuid = role.PKRoleID;

            // Delete the role
            role.Delete();

            // Get the deleted role
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKRoleID", roleGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Role testRole = CommonDAL<Role>.GetSingleObject(conditions);

            Assert.IsNull(testRole);
        }

        [TestMethod]
        public void TestRoleTimeStamp()
        {
            // Insert a role
            Role role = Role.CreateRole(Guid.NewGuid().ToString());
            role.PKRoleID = new Guid();
            role.KnowledgeDate = DateTime.Now;
            role.Save();
            Guid roleGuid = role.PKRoleID;

            // Get the inserted role
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKRoleID", roleGuid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            Role testRole1 = CommonDAL<Role>.GetSingleObject(conditions);
            Role testRole2 = CommonDAL<Role>.GetSingleObject(conditions);

            // Update the inserted role
            testRole1.Name = Guid.NewGuid().ToString().Substring(0, 5);
            testRole1.Save();
            testRole2.Name = Guid.NewGuid().ToString().Substring(0, 5);
            testRole2.Save();

            // Get the updated rolw
            Role updatedRole = CommonDAL<Role>.GetSingleObject(conditions);

            Assert.AreEqual(testRole1.Name, updatedRole.Name);
            Assert.AreNotEqual(testRole2.Name, updatedRole.Name);

            // Delete the role
            role.Delete();
        }

    }
}
