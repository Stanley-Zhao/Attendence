using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;
using CARS.Backend.DAL;
using System.Transactions;
using CARS.Backend.BLL;

namespace CARS.Backend.Entity
{
	public class Employee : BaseEntity
	{

		#region Private Fields

		private Guid pkEmployeeID;
		private Guid fkReportManagerID;
		private String email;
		private String password;
		private Sex gender;
		private float serviceYears;
		private String firstName;
		private DateTime hiredDate;
		private String lastName;
		private bool isActive;
		private String comment;
		private String middleName;
		private String legalName;
		private String phone;
		private String mobile;
        private DateTime knowledgeDate;
        private DateTime createdTime;
        private int employeeNum;

		private List<EmployeeRoleRL> employeeRoleList;
		private Employee manager;
		private bool isAdmin;
		private bool isManager;
		private String costCenter;

		#endregion

		#region Public Properties

		public Guid PKEmployeeID
		{
			get { return pkEmployeeID; }
			set { pkEmployeeID = value; }
		}

		public Guid FKReportManagerID
		{
			get { return fkReportManagerID; }
			set { fkReportManagerID = value; }
		}

		public String Email
		{
			get { return email; }
			set { email = value; }
		}

        /// <summary>
        /// Encrypted password, you need to decrypt it to get the raw string.
        /// </summary>
		public String Password
		{
			get { return password; }
			set { password = value; }
		}

		public Sex Gender
		{
			get { return gender; }
			set { gender = value; }
		}

		public float ServiceYears
		{
			get { return serviceYears; }
			set { serviceYears = value; }
		}

		public String FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}

		public DateTime HiredDate
		{
			get { return hiredDate; }
			set { hiredDate = value; }
		}

		public String LastName
		{
			get { return lastName; }
			set { lastName = value; }
		}

		public bool IsActive
		{
			get { return isActive; }
			set { isActive = value; }
		}

		public String Comment
		{
			get { return comment; }
			set { comment = value; }
		}

		public String MiddleName
		{
			get { return middleName; }
			set { middleName = value; }
		}

        public String LegalName
		{
			get { return legalName; }
			set { legalName = value; }
		}

		public String Phone
		{
			get { return phone; }
			set { phone = value; }
		}

		public String Mobile
		{
			get { return mobile; }
			set { mobile = value; }
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

		public String CostCenter
		{
			get { return costCenter; }
			set { costCenter = value; }
		}

        public int EmployeeNum
        {
            get { return employeeNum; }
            set { employeeNum = value; }
        }

		[Custom]
		public List<EmployeeRoleRL> ActiveEmployeeRoleList
		{
			get 
            {
                List<EmployeeRoleRL> list = new List<EmployeeRoleRL>();
                foreach (EmployeeRoleRL item in employeeRoleList)
                {
                    if (!item.IsDeleted) list.Add(item);
                }
                return list; 
            }
		}

		[Custom]
		public bool IsAdmin
		{
			get { return isAdmin; }
			set { isAdmin = value; }
		}

		[Custom]
		public bool IsManager
		{
			get { return isManager; }
			set { isManager = value; }
		}

		[Custom]
		public Employee Manager
		{
			get { return EmployeeBLL.GetEmployeeByID(this.fkReportManagerID.ToString()); }
			set { }
		}

		[Custom]
		public List<Employee> Leaders
		{
			get;
			set;
		}

		[Custom]
		public List<Employee> TeamMembers
		{
			get;
			set;
		}
		#endregion

		#region Override Fields

		public override Guid GetPKID()
		{
			return pkEmployeeID;
		}

		public override string GetPKIDName()
		{
			return GlobalParams.PKEmployeeID;
		}

		public override void SetPKID(Guid pkID)
		{
			pkEmployeeID = pkID;
		}

		public override void Init(DataRow row)
		{
			FillEntity(row);
			InitChildren();
		}

		public override void FillEntity(DataRow row)
		{
			pkEmployeeID = row["PKEmployeeID"] != DBNull.Value ? (Guid)row["PKEmployeeID"] : Guid.Empty;
			fkReportManagerID = row["FKReportManagerID"] != DBNull.Value ? (Guid)row["FKReportManagerID"] : Guid.Empty;
			email = row["Email"] != DBNull.Value ? (String)row["Email"] : null;
			password = row["Password"] != DBNull.Value ? (String)row["Password"] : null;
			gender = row["Gender"] != DBNull.Value ? (Sex)Enum.Parse(typeof(Sex), row["Gender"].ToString()) : (Sex)Enum.Parse(typeof(Sex), "None");
			serviceYears = row["ServiceYears"] != DBNull.Value ? float.Parse(row["ServiceYears"].ToString()) : 0f;
			firstName = row["FirstName"] != DBNull.Value ? (String)row["FirstName"] : null;
			hiredDate = row["HiredDate"] != DBNull.Value ? (DateTime)row["HiredDate"] : DateTime.MinValue;
			lastName = row["LastName"] != DBNull.Value ? (String)row["LastName"] : null;
			isActive = row["IsActive"] != DBNull.Value ? (bool)row["IsActive"] : false;
			comment = row["Comment"] != DBNull.Value ? (String)row["Comment"] : null;
			middleName = row["MiddleName"] != DBNull.Value ? (String)row["MiddleName"] : null;
            legalName = row["LegalName"] != DBNull.Value ? (String)row["LegalName"] : null;
			phone = row["Phone"] != DBNull.Value ? (String)row["Phone"] : null;
			mobile = row["Mobile"] != DBNull.Value ? (String)row["Mobile"] : null;
			knowledgeDate = row["KnowledgeDate"] != DBNull.Value ? (DateTime)row["KnowledgeDate"] : DateTime.MinValue;
			timeToken = row["TimeToken"] != DBNull.Value ? (byte[])row["TimeToken"] : null;
            createdTime = row["CreatedTime"] != DBNull.Value ? (DateTime)row["CreatedTime"] : DateTime.MinValue;
			costCenter = row["CostCenter"] != DBNull.Value ? (String)row["CostCenter"] : null;
            employeeNum = row["EmployeeNum"] != DBNull.Value ? Int32.Parse(row["EmployeeNum"].ToString()) : 0;
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
			using (TransactionScope ts = new TransactionScope())
			{
				List<SearchCondition> conditions = new List<SearchCondition>();
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKEmployeeID, PKEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
				CommonDAL<Employee>.Update(this, conditions);

				//#region Transact unit test
				//throw new Exception();
				//#endregion

				foreach (EmployeeRoleRL item in employeeRoleList)
				{
					if (item.IsNew) item.FKEmployeeID = this.PKEmployeeID;
					item.Save();
				}

				ts.Complete();
			}
		}

		protected override void Insert()
		{
			using (TransactionScope ts = new TransactionScope())
			{
				CommonDAL<Employee>.Insert(this);

				//#region transact unit test
				//throw new Exception();
				//#endregion

				foreach (EmployeeRoleRL item in employeeRoleList)
				{
					item.FKEmployeeID = this.PKEmployeeID;
					item.Save();
				}

				ts.Complete();
			}
		}

		public override void Delete()
		{
			using (TransactionScope ts = new TransactionScope())
			{
				foreach (EmployeeRoleRL item in employeeRoleList)
				{
					item.Delete();
				}

				CommonDAL<Employee>.Delete(this);

				ts.Complete();
			}
		}

		#endregion

		#region Private Methods

		private void InitChildren()
		{
			List<SearchCondition> conditions = new List<SearchCondition>();
			conditions.Add(SearchCondition.CreateSearchCondition("FKEmployeeID", PKEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
			conditions.Add(SearchCondition.CreateSearchCondition("IsDeleted", Convert.ToString(0), SearchComparator.Equal, SearchType.SearchNotString));
			employeeRoleList = CommonDAL<EmployeeRoleRL>.GetObjects(conditions);

			foreach (EmployeeRoleRL item in employeeRoleList)
			{
				if (item.EmployeeRole.Name.Equals("Admin"))
				{
					isAdmin = true;
				}

				if (item.EmployeeRole.Name.Equals("Manager"))
				{
					isManager = true;
				}
			}

			conditions.Clear();
			conditions.Add(SearchCondition.CreateSearchCondition("PKEmployeeID", FKReportManagerID.ToString(), SearchComparator.Equal, SearchType.SearchString));
			manager = CommonDAL<Employee>.GetSingleObject(conditions);
		}

		#endregion

		#region Public Methods

		public void SetPrivilege(bool value, RoleRank role)
		{
			bool isExistRole = false;
			EmployeeRoleRL emRole = null;

			int index = 0;
			foreach (EmployeeRoleRL item in employeeRoleList)
			{
				if ((role == RoleRank.Admin && item.EmployeeRole.Name.Equals(RoleRank.Admin.ToString())) ||
					(role == RoleRank.Manager && item.EmployeeRole.Name.Equals(RoleRank.Manager.ToString())))
				{
					isExistRole = true;
					emRole = item;
					break;
				}

				index++;
			}

			if (value)
			{
				if (!isExistRole)
				{
					Role roleObject = Role.GetRoleByName(role.ToString());
					emRole = EmployeeRoleRL.CreateEmployeeRoleRL(PKEmployeeID, roleObject.PKRoleID);
					employeeRoleList.Add(emRole);
				}
			}
			else
			{
				if (isExistRole)
				{
					//employeeRoleList.RemoveAt(index);
					emRole.IsDeleted = true;					
				}
			}
		}

		public void SetRoleList(List<EmployeeRoleRL> employeeRoleList)
		{
			this.employeeRoleList = employeeRoleList;
		}

		public bool IsPointedRole(string roleName)
		{
			bool result = !string.IsNullOrEmpty(roleName);

			if (result)
			{
				foreach (EmployeeRoleRL item in employeeRoleList)
				{
					if (item.EmployeeRole.Name.Equals(roleName))
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		#endregion

		#region Constructure

		private Employee() { }
		
		public Employee(int key) { }

		public static Employee CreateEmployee(string email, string password, Sex gender, string firstName, DateTime dateOfHire, string lastName)
		{
			Employee employee = null;

			if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && gender != Sex.None &&
				!string.IsNullOrEmpty(firstName) && dateOfHire != DateTime.MinValue && !string.IsNullOrEmpty(lastName))
			{
				employee = new Employee();
				employee.Email = email;
				employee.Password = password;
				employee.Gender = gender;
				employee.FirstName = firstName;
				employee.HiredDate = dateOfHire;
				employee.LastName = lastName;
			}

			return employee;
		}

		#endregion

	}
}