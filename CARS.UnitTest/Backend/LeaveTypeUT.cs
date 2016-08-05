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
    public class LeaveTypeUT
    {
        [TestMethod]
        public void TestInsertLeaveType()
        {
            // Insert a leave type
            DateTime startTime = Convert.ToDateTime("01/01/2000");
            DateTime endTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, startTime, endTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid guid = leaveType.PKLeaveTypeID;

            // Get the leave type inserted just now
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveTypeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveType testLeaveType = CommonDAL<LeaveType>.GetSingleObject(conditions);

            Assert.AreEqual(testLeaveType.StartTime, leaveType.StartTime);

            // Delete the leave type
            leaveType.Delete();
        }

        [TestMethod]
        public void TestUpdateLeaveType()
        {
            // Insert a leave type
            DateTime startTime = Convert.ToDateTime("01/01/2000");
            DateTime endTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, startTime, endTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid guid = leaveType.PKLeaveTypeID;

            // Get the leave type inserted just now
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveTypeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveType testLeaveType = CommonDAL<LeaveType>.GetSingleObject(conditions);

            // Update the leave type inserted just now
            testLeaveType.Name = "UpdatedLeaveType";
            testLeaveType.Save();

            // Get the leave type updated just now
            LeaveType updatedLeaveType = CommonDAL<LeaveType>.GetSingleObject(conditions);

            Assert.AreEqual(testLeaveType.Name, updatedLeaveType.Name);

            // Delete the leave type
            leaveType.Delete();
        }

        [TestMethod]
        public void TestDeleteLeaveType()
        {
            // Insert a leave type
            DateTime startTime = Convert.ToDateTime("01/01/2000");
            DateTime endTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, startTime, endTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();

            // Delete the leave type
            leaveType.Delete();

            // Get the leave type deleted just now
            Guid guid = leaveType.PKLeaveTypeID;
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveTypeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveType testLeaveType = CommonDAL<LeaveType>.GetSingleObject(conditions);

            Assert.IsNull(testLeaveType);
        }

        [TestMethod]
        public void TestLeaveTypeTimeStamp()
        {
            // Insert a leave type
            DateTime startTime = Convert.ToDateTime("01/01/2000");
            DateTime endTime = Convert.ToDateTime("01/01/3000");
            LeaveType leaveType = LeaveType.CreateLeaveType(Guid.NewGuid().ToString(), 100, startTime, endTime);
            leaveType.PKLeaveTypeID = Guid.NewGuid();
            leaveType.SetKnowledgeDate(DateTime.Now);
            leaveType.Save();
            Guid guid = leaveType.PKLeaveTypeID;

            // Get the leave type inserted just now
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKLeaveTypeID", guid.ToString(), SearchComparator.Equal, SearchType.SearchString));
            LeaveType testLeaveType1 = CommonDAL<LeaveType>.GetSingleObject(conditions);
            LeaveType testLeaveType2 = CommonDAL<LeaveType>.GetSingleObject(conditions);

            // Update the leave type inserted just now
            testLeaveType1.Name = "UpdatedLeaveType";
            testLeaveType1.Save();
            testLeaveType2.Name = "SecUpdatedLeaveType";
            testLeaveType2.Save();

            // Get the leave type updated just now
            LeaveType updatedLeaveType = CommonDAL<LeaveType>.GetSingleObject(conditions);

            Assert.AreEqual(testLeaveType1.Name, updatedLeaveType.Name);
            Assert.AreNotEqual(testLeaveType2.Name, updatedLeaveType.Name);

            // Delete the leave type
            leaveType.Delete();
        }
    }
}
