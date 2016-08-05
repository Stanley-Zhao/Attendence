using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;
using CARS.Backend.DAL;

namespace CARS.Backend.Entity
{
    public class TimeDurationInfo : BaseEntity
    {

        #region Private Fields

        private Guid pkTDInfoID;
        private Guid fkLeaveInfoID;
        private DateTime startTime;
        private DateTime endTime;
        private DateTime knowledgeDate;
        private bool isDeleted;
        private DateTime createdTime;

        #endregion

        #region Public Properties

        public Guid PKTDInfoID
        {
            get { return pkTDInfoID; }
            set { pkTDInfoID = value; }
        }

        public Guid FKLeaveInfoID
        {
            get { return fkLeaveInfoID; }
            set { fkLeaveInfoID = value; }
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

        public DateTime KnowledgeDate
        {
            get { return knowledgeDate; }
            set { knowledgeDate = value; }
        }

        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
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
            return pkTDInfoID;
        }

        public override string GetPKIDName()
        {
            return GlobalParams.PKTDInfoID;
        }

        public override void SetPKID(Guid pkID)
        {
            pkTDInfoID = pkID;
        }

        public override void Init(DataRow row)
        {
            FillEntity(row);
        }

        public override void FillEntity(DataRow row)
        {
            pkTDInfoID = row["PKTDInfoID"] != DBNull.Value ? (Guid)row["PKTDInfoID"] : Guid.Empty;
            fkLeaveInfoID = row["FKLeaveInfoID"] != DBNull.Value ? (Guid)row["FKLeaveInfoID"] : Guid.Empty;
            startTime = row["StartTime"] != DBNull.Value ? (DateTime)row["StartTime"] : DateTime.MinValue;
            endTime = row["EndTime"] != DBNull.Value ? (DateTime)row["EndTime"] : DateTime.MinValue;
            knowledgeDate = row["KnowledgeDate"] != DBNull.Value ? (DateTime)row["KnowledgeDate"] : DateTime.MinValue;
            isDeleted = row["IsDeleted"] != DBNull.Value ? (bool)row["IsDeleted"] : false;
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
            conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKTDInfoID, PKTDInfoID.ToString(), SearchComparator.Equal, SearchType.SearchString));
            CommonDAL<TimeDurationInfo>.Update(this, conditions);
        }

        protected override void Insert()
        {
            CommonDAL<TimeDurationInfo>.Insert(this);
        }

        public override void Delete()
        {
            CommonDAL<TimeDurationInfo>.Delete(this);
        }

        #endregion

        #region Constructures

        private TimeDurationInfo() { }

        public TimeDurationInfo(int key) { }

        public static TimeDurationInfo CreateTimeDurationInfo(DateTime startTime, DateTime endTime)
        {
            TimeDurationInfo info = new TimeDurationInfo();
            info.StartTime = startTime;
            info.EndTime = endTime;
            info.IsDeleted = false;

            return info;
        }

        #endregion

    }
}