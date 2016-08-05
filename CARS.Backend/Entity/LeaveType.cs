using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;
using CARS.Backend.DAL;

namespace CARS.Backend.Entity
{
    public class LeaveType : BaseEntity
    {

        #region Private Fields

        private Guid pkLeaveTypeID;
        private String name;
        private int totalHours;
        private DateTime startTime;
        private DateTime endTime;
        private int longestHoursOneTime;
        private int leastHoursOneTime;
        private String description;
        private DateTime knowledgeDate;
        private DateTime createdTime;

        #endregion

        #region Public Properties

        public Guid PKLeaveTypeID
        {
            get { return pkLeaveTypeID; }
            set { pkLeaveTypeID = value; }
        }
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public int TotalHours
        {
            get { return totalHours; }
            set { totalHours = value; }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        public int LongestHoursOneTime
        {
            get { return longestHoursOneTime; }
            set { longestHoursOneTime = value; }
        }

        public int LeastHoursOneTime
        {
            get { return leastHoursOneTime; }
            set { leastHoursOneTime = value; }
        }

        public String Description
        {
            get { return description; }
            set { description = value; }
        }

        public DateTime KnowledgeDate
        {
            get { return knowledgeDate; }
            set { knowledgeDate = value; }
        }

        public DateTime CreatedTime
        {
            get { return createdTime; }
            set { createdTime = value; }
        }

        #endregion

        #region Override Fields

        public override Guid GetPKID()
        {
            return pkLeaveTypeID;
        }

        public override string GetPKIDName()
        {
            return GlobalParams.PKLeaveTypeID;
        }

        public override void SetPKID(Guid pkID)
        {
            pkLeaveTypeID = pkID;
        }

        public override void Init(DataRow row)
        {
            FillEntity(row);
        }

        public override void FillEntity(DataRow row)
        {
            pkLeaveTypeID = row["PKLeaveTypeID"] != DBNull.Value ? (Guid)row["PKLeaveTypeID"] : Guid.Empty;
            name = row["Name"] != DBNull.Value ? (String)row["Name"] : null;
            totalHours = row["TotalHours"] != DBNull.Value ? (int)row["TotalHours"] : 0;
            startTime = row["StartTime"] != DBNull.Value ? (DateTime)row["StartTime"] : DateTime.MinValue;
            endTime = row["EndTime"] != DBNull.Value ? (DateTime)row["EndTime"] : DateTime.MinValue;
            longestHoursOneTime = row["LongestHoursOneTime"] != DBNull.Value ? (int)row["LongestHoursOneTime"] : 0;
            leastHoursOneTime = row["LeastHoursOneTime"] != DBNull.Value ? (int)row["LeastHoursOneTime"] : 0;
            description = row["Description"] != DBNull.Value ? (String)row["Description"] : null;
            knowledgeDate = row["KnowledgeDate"] != DBNull.Value ? (DateTime)row["KnowledgeDate"] : DateTime.MinValue;
            timeToken = row["TimeToken"] != DBNull.Value ? (byte[])row["TimeToken"] : null;
            createdTime = row["CreatedTime"] != DBNull.Value ? (DateTime)row["CreatedTime"] : DateTime.MinValue;
        }

        public override void SetKnowledgeDate(DateTime knowledgeDate)
        {
            this.knowledgeDate = knowledgeDate;
        }

        public override void SetCreatedDate(DateTime createdTime)
        {
            this.createdTime = createdTime;
        }

        protected override void Update()
        {
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKLeaveTypeID, PKLeaveTypeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
            CommonDAL<LeaveType>.Update(this, conditions);
        }

        protected override void Insert()
        {
            CommonDAL<LeaveType>.Insert(this);
        }

        public override void Delete()
        {
            CommonDAL<LeaveType>.Delete(this);
        }

        #endregion

        #region Constructures

        private LeaveType() { }

        public LeaveType(int key) { }

        public static LeaveType CreateLeaveType(string name, int totalHours, DateTime start, DateTime end)
        {
            LeaveType leaveType = null;

            if (!string.IsNullOrEmpty(name) && totalHours > 0 && start != DateTime.MinValue && end != DateTime.MinValue)
            {
                leaveType = new LeaveType();
                leaveType.Name = name;
                leaveType.TotalHours = totalHours;
                leaveType.StartTime = start;
                leaveType.EndTime = end;
            }

            return leaveType;
        }

        #endregion

    }
}