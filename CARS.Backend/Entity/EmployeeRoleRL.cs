using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;
using CARS.Backend.DAL;

namespace CARS.Backend.Entity
{
    public class EmployeeRoleRL : BaseEntity
    {

        #region Private Fields

        private Guid pkEmployeeRoleRLID;
        private Guid fkEmployeeID;
        private Guid fkRoleID;
        private bool isDeleted;
        private DateTime knowledgeDate;
        private DateTime createdTime;

        private Role employeeRole;

        #endregion

        #region Public Properties

        public Guid PKEmployeeRoleRLID
        {
            get { return pkEmployeeRoleRLID; }
            set { pkEmployeeRoleRLID = value; }
        }

        public Guid FKEmployeeID
        {
            get { return fkEmployeeID; }
            set { fkEmployeeID = value; }
        }

        public Guid FKRoleID
        {
            get { return fkRoleID; }
            set { fkRoleID = value; }
        }

        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
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

        [Custom]
        public Role EmployeeRole
        {
            get { return employeeRole; }
        }

        #endregion

        #region Override Fields

        public override Guid GetPKID()
        {
            return pkEmployeeRoleRLID;
        }

        public override string GetPKIDName()
        {
            return "PKEmployeeRoleRLID";
        }

        public override void SetPKID(Guid pkID)
        {
            pkEmployeeRoleRLID = pkID;
        }

        public override void SetKnowledgeDate(DateTime knowledgeDate)
        {
            this.knowledgeDate = knowledgeDate;
        }

        public override void SetCreatedDate(DateTime createdTime)
        {
            this.createdTime = createdTime;
        }

        public override void Init(DataRow row)
        {
            FillEntity(row);
            InitChildren();
        }

        public override void FillEntity(DataRow row)
        {
            pkEmployeeRoleRLID = row["PKEmployeeRoleRLID"] != DBNull.Value ? (Guid)row["PKEmployeeRoleRLID"] : Guid.Empty;
            fkEmployeeID = row["FKEmployeeID"] != DBNull.Value ? (Guid)row["FKEmployeeID"] : Guid.Empty;
            fkRoleID = row["FKRoleID"] != DBNull.Value ? (Guid)row["FKRoleID"] : Guid.Empty;
            isDeleted = row["IsDeleted"] != DBNull.Value ? (bool)row["IsDeleted"] : false;
            knowledgeDate = row["KnowledgeDate"] != DBNull.Value ? (DateTime)row["KnowledgeDate"] : DateTime.MinValue;
            timeToken = row["TimeToken"] != DBNull.Value ? (byte[])row["TimeToken"] : null;
            createdTime = row["CreatedTime"] != DBNull.Value ? (DateTime)row["CreatedTime"] : DateTime.MinValue;
        }

        protected override void Update()
        {
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeRoleRLID", PKEmployeeRoleRLID.ToString(), SearchComparator.Equal, SearchType.SearchString));
            CommonDAL<EmployeeRoleRL>.Update(this, conditions);
        }

        protected override void Insert()
        {
            CommonDAL<EmployeeRoleRL>.Insert(this);
        }

        public override void Delete()
        {
            CommonDAL<EmployeeRoleRL>.Delete(this);
        }

        #endregion

        #region Private Methods

        private void InitChildren()
        {
            List<SearchCondition> conditions = new List<SearchCondition>();
            conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKRoleID, FKRoleID.ToString(), SearchComparator.Equal, SearchType.SearchString));
            employeeRole = CommonDAL<Role>.GetSingleObject(conditions);
        }

        #endregion

        #region Constructures

        private EmployeeRoleRL() { }

        public EmployeeRoleRL(int key) { }

        public static EmployeeRoleRL CreateEmployeeRoleRL(Guid fkEmployeeID, Guid fkRoleID)
        {
            EmployeeRoleRL emRole = null;

            if (Guid.Empty != fkRoleID)
            {
                emRole = new EmployeeRoleRL();
                emRole.FKEmployeeID = fkEmployeeID;
                emRole.FKRoleID = fkRoleID;
                emRole.IsDeleted = false;
                emRole.InitChildren();
            }

            return emRole;
        }

        #endregion

    }
}