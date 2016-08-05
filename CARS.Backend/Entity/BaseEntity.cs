using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;

namespace CARS.Backend.Entity
{
    public class BaseEntity
    {
        protected bool isNew;
        [CustomAttribute]
        public bool IsNew
        {
            get { return isNew; }
        }

        protected byte[] timeToken;
        public byte[] TimeToken
        {
            get { return timeToken; }
        }

        public virtual Guid GetPKID() { return Guid.Empty; }

        public virtual string GetPKIDName() { return null; }

        public virtual void SetPKID(Guid pkID) { }

        public virtual void FillEntity(DataRow row) { }

        public virtual void Init(DataRow row) { }

        public virtual void SetKnowledgeDate(DateTime knowledgeDate) { }

        public virtual void SetCreatedDate(DateTime createdTime) { }

        protected virtual void Insert() { }

        protected virtual void Update() { }

        public virtual void Delete() { }

        public void SetIsNewFlag(bool isNew)
        {
            this.isNew = isNew;
        }

        public void SetTimeToker(byte[] timeToken)
        {
            this.timeToken = timeToken;
        }

        public void Save()
        {
            if (this.IsNew)
            {
                Insert();
            }
            else
            {
                Update();
            }
        }

        protected BaseEntity()
        {
            this.isNew = true;
        }
		
        internal BaseEntity(int key)
        {
            if (key != 523)
            {
                throw new NotSupportedException("Cannot invoke the constructure manually.");
            }
        }
    }
}
