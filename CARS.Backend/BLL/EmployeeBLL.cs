using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CARS.Backend.DAL;
using CARS.Backend.Entity;
using CARS.Backend.Common;

namespace CARS.Backend.BLL
{
	public class EmployeeBLL
	{
		#region Public Methods

		public static Employee GetEmployee(string email, string password)
		{
			List<Employee> teamMembers = new List<Employee>();
			List<SearchCondition> conditions = new List<SearchCondition>();
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Email, email, SearchComparator.Equal, SearchType.SearchString));
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Password, password, SearchComparator.Equal, SearchType.SearchString));
			Employee employee = CommonDAL<Employee>.GetSingleObject(conditions);

			if (employee == null)
				throw new Exception("Email account or password is not correct.");

			GetAllLeaders(employee, teamMembers); // meanwhile, set list of team members
			employee.TeamMembers = teamMembers;

			return employee;
		}

		private static void GetAllLeaders(Employee employee, List<Employee> teamMembers)
		{
			GetAllLeaders(employee, null, teamMembers);
		}

		private static void GetAllLeaders(Employee employee, List<string> pkIDList, List<Employee> teamMembers)
		{
			List<Employee> leaders = new List<Employee>();
			List<SearchCondition> conditions = new List<SearchCondition>();
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Name, "Manager", SearchComparator.Equal, SearchType.SearchString));
			Role managerRole = CommonDAL<Role>.GetSingleObject(conditions); // manager role

			conditions.Clear();
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKReportManagerID, employee.PKEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.IsActive, true.ToString(), SearchComparator.Equal, SearchType.SearchString));
			List<Employee> members = CommonDAL<Employee>.GetObjects(conditions); // members for current manager

			foreach (Employee member in members)
			{
				if (teamMembers != null)
					teamMembers.Add(member); // every one is employee.
				foreach (EmployeeRoleRL memberRole in member.ActiveEmployeeRoleList)
				{
					if (memberRole.FKRoleID.ToString() == managerRole.PKRoleID.ToString())
					{
						leaders.Add(member);
						if (null != pkIDList) pkIDList.Add(member.PKEmployeeID.ToString());
					}
				}
			}

			employee.Leaders = leaders;

			if (leaders == null || leaders.Count == 0)
				return;

			foreach (Employee leader in leaders)
				GetAllLeaders(leader, pkIDList, teamMembers);
		}

		public static void DeleteEmployee(Employee employee)
		{
			CommonDAL<Employee>.Delete(employee);
		}

		public static Employee GetEmployeeByID(string employeeID)
		{
			Employee employee = null;

			if (!string.IsNullOrEmpty(employeeID))
			{
				List<SearchCondition> conditions = new List<SearchCondition>();
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKEmployeeID, employeeID, SearchComparator.Equal, SearchType.SearchString));
				employee = CommonDAL<Employee>.GetSingleObject(conditions);
			}

			return employee;
		}

		public static Employee GetEmployeeByEmail(string email)
		{
			List<SearchCondition> conditions = new List<SearchCondition>();
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Email, email, SearchComparator.Equal, SearchType.SearchString));

			Employee employee = CommonDAL<Employee>.GetSingleObject(conditions);

			return employee;
		}

		#endregion

		#region Service Methods

		public static Employee Login(string email, string password)
		{
			return GetEmployee(email, password);
		}

		public static Employee AddEmployee(string email, string firstName, string middleName, string lastName, Sex gender, float serviceYear, DateTime dateOfHire, string supervisorEmail, string costCenterCode, int employeeNum, string legalName)
		{
			Employee employee = null;

			if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
			{
				employee = Employee.CreateEmployee(email, CryptographyStuff.AES_EncryptString(GlobalParams.DefaultPwd), gender, firstName, dateOfHire, lastName);
				employee.MiddleName = middleName;
				employee.Gender = gender;
				employee.ServiceYears = serviceYear;
				employee.IsActive = true;
                employee.EmployeeNum = employeeNum;
                employee.LegalName = legalName;

				Role role = Role.GetRoleByName(RoleRank.Employee.ToString());
				List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
				employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role.PKRoleID));
				employee.SetRoleList(employeeRoleList);

				if (!string.IsNullOrEmpty(supervisorEmail))
				{
					List<SearchCondition> conditions = new List<SearchCondition>();
					conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Email, supervisorEmail, SearchComparator.Equal, SearchType.SearchString));
					Employee manager = CommonDAL<Employee>.GetSingleObject(conditions);
					employee.FKReportManagerID = manager.PKEmployeeID;
				}

				employee.CostCenter = costCenterCode;

				employee.Save();
			}

			return employee;
		}

        public static void UpdateEmployee(string employeeID, string email, string firstName, string middleName, string lastName, Sex gender, float serviceYear, DateTime dateOfHire, string supervisorEmail, string password, string phone, string mobile, bool isAdmin, bool isActive, bool isManager, string costCenterCode, int employeeNum, string legalName)
		{
			if (!string.IsNullOrEmpty(employeeID) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
			{
				Employee employee = EmployeeBLL.GetEmployeeByID(employeeID);

				if (null != employee)
				{
					if (!string.IsNullOrEmpty(employee.Email) &&
						!string.IsNullOrEmpty(supervisorEmail) &&
						employee.Email.Equals(supervisorEmail)) throw new Exception(GlobalParams.SelectSupervisorError);

					employee.Email = email;
					employee.FirstName = firstName;
					employee.MiddleName = middleName;
					employee.LastName = lastName;
					employee.Gender = gender;
					employee.ServiceYears = serviceYear;
					employee.HiredDate = dateOfHire;
					employee.Password = password;
					employee.Phone = phone;
					employee.Mobile = mobile;
					employee.IsActive = isActive;
					employee.SetPrivilege(isAdmin, RoleRank.Admin);
					employee.SetPrivilege(isManager, RoleRank.Manager);
					employee.CostCenter = costCenterCode;
                    employee.EmployeeNum = employeeNum;
                    employee.LegalName = legalName;

					if (!string.IsNullOrEmpty(supervisorEmail))
					{
						List<SearchCondition> conditions = new List<SearchCondition>();
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Email, supervisorEmail, SearchComparator.Equal, SearchType.SearchString));
						Employee manager = CommonDAL<Employee>.GetSingleObject(conditions);
						employee.FKReportManagerID = manager.PKEmployeeID;
					}
					else
					{
						employee.FKReportManagerID = Guid.Empty;
					}

					employee.Save();
				}
			}
		}

		public static List<Employee> GetManagers()
		{
			List<Employee> managers = null;
			Role role = Role.GetRoleByName(RoleRank.Manager.ToString());

			if (null != role)
			{
				List<SearchCondition> conditions = new List<SearchCondition>();
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKRoleID, role.PKRoleID.ToString(), SearchComparator.Equal, SearchType.SearchString));
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.IsDeleted, Convert.ToString(0), SearchComparator.Equal, SearchType.SearchNotString));
				List<EmployeeRoleRL> employeeRoleRLs = CommonDAL<EmployeeRoleRL>.GetObjects(conditions);

				if (null != employeeRoleRLs)
				{
					managers = new List<Employee>();
					foreach (EmployeeRoleRL item in employeeRoleRLs)
					{
						if(item.IsDeleted == false) // remove the supervisor who is not actived
							managers.Add(EmployeeBLL.GetEmployeeByID(item.FKEmployeeID.ToString()));
					}
				}
			}

			return managers;
		}

		public static List<Employee> GetAllEmployees()
		{
			List<SearchCondition> conditions = new List<SearchCondition>();
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.IsActive, Convert.ToString(1), SearchComparator.Equal, SearchType.SearchNotString));
			return CommonDAL<Employee>.GetObjects(conditions, GlobalParams.Email, OrderType.ASC);
		}

		public static bool CheckSupervisorValidation(string employeeID, string supervisorID)
		{
			bool result = !string.IsNullOrEmpty(employeeID) && !employeeID.Equals(supervisorID);

			if (result)
			{
				List<SearchCondition> conditions = new List<SearchCondition>();
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKEmployeeID, employeeID, SearchComparator.Equal, SearchType.SearchString));
				Employee employee = CommonDAL<Employee>.GetSingleObject(conditions);

				result = null != employee;
				if (result)
				{
					List<string> pkIDList = new List<string>();
					GetAllLeaders(employee, pkIDList, null);
					result = null != pkIDList;
					if (result)
					{
						foreach (string pkID in pkIDList)
						{
							if (pkID.Equals(supervisorID))
							{
								result = false;
								break;
							}
						}
					}
				}
			}

			return result;
		}

		#endregion
	}
}
