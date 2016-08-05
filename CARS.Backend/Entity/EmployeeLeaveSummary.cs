using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;
using CARS.Backend.DAL;

namespace CARS.Backend.Entity
{
    public class EmployeeLeaveSummary : BaseEntity
    {

        #region Private Fields

        private Guid pkELSID;
        private Guid fkEmployeeID;
        private Guid fkLeaveTypeID;
        private double usedHours;
        private DateTime knowledgeDate;
        private DateTime createdTime;
        private int year;

        #endregion

        #region Public Properties

        public Guid PKELSID
        {
            get { return pkELSID; }
            set { pkELSID = value; }
        }

        public Guid FKEmployeeID
        {
            get { return fkEmployeeID; }
            set { fkEmployeeID = value; }
        }

        public Guid FKLeaveTypeID
        {
            get { return fkLeaveTypeID; }
            set { fkLeaveTypeID = value; }
        }

        public double UsedHours
        {
            get { return usedHours; }
            set { usedHours = value; }
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

        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        #endregion

        #region Override Fields

        public override Guid GetPKID()
        {
            return pkELSID;
        }

        public override string GetPKIDName()
        {
            return GlobalParams.PKELSID;
        }

        public override void SetPKID(Guid pkID)
        {
            pkELSID = pkID;
        }

        public override void Init(DataRow row)
        {
            FillEntity(row);
        }

        public override void FillEntity(DataRow row)
        {
            pkELSID = row["PKELSID"] != DBNull.Value ? (Guid)row["PKELSID"] : Guid.Empty;
            fkEmployeeID = row["FKEmployeeID"] != DBNull.Value ? (Guid)row["FKEmployeeID"] : Guid.Empty;
            fkLeaveTypeID = row["FKLeaveTypeID"] != DBNull.Value ? (Guid)row["FKLeaveTypeID"] : Guid.Empty;
            usedHours = row["UsedHours"] != DBNull.Value ? (double)row["UsedHours"] : 0;
            knowledgeDate = row["KnowledgeDate"] != DBNull.Value ? (DateTime)row["KnowledgeDate"] : DateTime.MinValue;
            timeToken = row["TimeToken"] != DBNull.Value ? (byte[])row["TimeToken"] : null;
            createdTime = row["CreatedTime"] != DBNull.Value ? (DateTime)row["CreatedTime"] : DateTime.MinValue;
            year = row["Year"] != DBNull.Value ? (int)row["Year"] : 0;
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
            conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKELSID, PKELSID.ToString(), SearchComparator.Equal, SearchType.SearchString));
            CommonDAL<EmployeeLeaveSummary>.Update(this, conditions);
        }

        protected override void Insert()
        {
            CommonDAL<EmployeeLeaveSummary>.Insert(this);
        }

        public override void Delete()
        {
            CommonDAL<EmployeeLeaveSummary>.Delete(this);
        }

        #endregion

        #region Constructure

		private EmployeeLeaveSummary() { }

        public EmployeeLeaveSummary(int key) { }

        public static EmployeeLeaveSummary CreateEmployeeLeaveSummary(Guid fkEmployeeID, Guid fkLeaveTypeID, int year)
        {
            EmployeeLeaveSummary emLeaveSummary = new EmployeeLeaveSummary();
            emLeaveSummary.FKEmployeeID = fkEmployeeID;
            emLeaveSummary.FKLeaveTypeID = fkLeaveTypeID;
            emLeaveSummary.Year = year;

            return emLeaveSummary;
        }

		#endregion

    }
}