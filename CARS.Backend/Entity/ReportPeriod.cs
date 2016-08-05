using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;
using CARS.Backend.DAL;

namespace CARS.Backend.Entity
{
    public class ReportPeriod : BaseEntity
    {

        #region Private Fields

        private Guid pkReportPeriodID;
        private MonthRank month;
        private DateTime? startTime = null;
        private DateTime? endTime = null;
        private DateTime createdTime;
        private DateTime knowledgeDate;

        #endregion

        #region Public Properties

        public Guid PKReportPeriodID
        {
            get { return pkReportPeriodID; }
        }

        public MonthRank Month
        {
            get { return month; }
            set { month = value; }
        }

        public DateTime? StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public DateTime? EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        public DateTime CreatedTime
        {
            get { return createdTime; }
            set { createdTime = value; }
        }

        public DateTime KnowledgeDate
        {
            get { return knowledgeDate; }
            set { knowledgeDate = value; }
        }

        #endregion

        #region Override Fields

        public override Guid GetPKID()
        {
            return pkReportPeriodID;
        }

        public override string GetPKIDName()
        {
            return GlobalParams.PKReportPeriodID;
        }

        public override void SetPKID(Guid pkID)
        {
            pkReportPeriodID = pkID;
        }

        public override void Init(DataRow row)
        {
            FillEntity(row);
        }

        public override void FillEntity(DataRow row)
        {
            pkReportPeriodID = row["PKReportPeriodID"] != DBNull.Value ? (Guid)row["PKReportPeriodID"] : Guid.Empty;
            month = row["Month"] != DBNull.Value ? (MonthRank)Enum.Parse(typeof(MonthRank), row["Month"].ToString()) : (MonthRank)Enum.Parse(typeof(MonthRank), "None");
            startTime = row["StartTime"] != DBNull.Value ? (DateTime)row["StartTime"] : DateTime.MinValue;
            endTime = row["EndTime"] != DBNull.Value ? (DateTime)row["EndTime"] : DateTime.MinValue;
            createdTime = row["CreatedTime"] != DBNull.Value ? (DateTime)row["CreatedTime"] : DateTime.MinValue;
            knowledgeDate = row["KnowledgeDate"] != DBNull.Value ? (DateTime)row["KnowledgeDate"] : DateTime.MinValue;
            timeToken = row["TimeToken"] != DBNull.Value ? (byte[])row["TimeToken"] : null;
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
            conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKReportPeriodID, PKReportPeriodID.ToString(), SearchComparator.Equal, SearchType.SearchString));
            CommonDAL<ReportPeriod>.Update(this, conditions);
        }

        protected override void Insert()
        {
            CommonDAL<ReportPeriod>.Insert(this);
        }

        public override void Delete()
        {
            CommonDAL<ReportPeriod>.Delete(this);
        }

        #endregion

        #region Constructure

		private ReportPeriod() { }

        public ReportPeriod(int key) { }

        public static ReportPeriod CreateReportPeriod(MonthRank month, DateTime start, DateTime end)
		{
            ReportPeriod reportPeriod = null;

			if (month != MonthRank.None && start != DateTime.MinValue && end != DateTime.MinValue)
			{
                reportPeriod = new ReportPeriod();
                reportPeriod.Month = month;
                reportPeriod.StartTime = start;
                reportPeriod.endTime = end;
			}

            return reportPeriod;
		}

		#endregion

    }
}