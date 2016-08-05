using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;
using CARS.Backend.DAL;

namespace CARS.Backend.Entity
{
    public class Role : BaseEntity
    {

        #region Private Fields

        private Guid pkRoleID;
        private String name;
        private String description;
        private DateTime knowledgeDate;
        private DateTime createdTime;

        #endregion

        #region Public Properties

        public Guid PKRoleID
        {
            get { return pkRoleID; }
            set { pkRoleID = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
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
            return pkRoleID;
        }

        public override string GetPKIDName()
        {
            return GlobalParams.PKRoleID;
        }

        public override void SetPKID(Guid pkID)
        {
            pkRoleID = pkID;
        }

        public override void Init(DataRow row)
        {
            FillEntity(row);
        }

        public override void FillEntity(DataRow row)
        {
            pkRoleID = row["PKRoleID"] != DBNull.Value ? (Guid)row["PKRoleID"] : Guid.Empty;
            name = row["Name"] != DBNull.Value ? (String)row["Name"] : null;
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
            conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKRoleID, PKRoleID.ToString(), SearchComparator.Equal, SearchType.SearchString));
            CommonDAL<Role>.Update(this, conditions);
        }

        protected override void Insert()
        {
            CommonDAL<Role>.Insert(this);
        }

        public override void Delete()
        {
            CommonDAL<Role>.Delete(this);
        }

        #endregion

        #region Constructures

        private Role() { }

        public Role(int key) { }

        public static Role CreateRole(string name)
        {
            Role role = null;

            if (!string.IsNullOrEmpty(name))
            {
                role = new Role();
                role.Name = name;
            }

            return role;
        }

        public static Role GetRoleByName(string rank)
        {
            Role role = null;

            if (!string.IsNullOrEmpty(rank))
            {
                List<SearchCondition> conditions = new List<SearchCondition>();
                conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Name, rank, SearchComparator.Equal, SearchType.SearchString));
                role = CommonDAL<Role>.GetSingleObject(conditions);
            }

            return role;
        }

        #endregion

    }
}