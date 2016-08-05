using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CARS.CARSService;

namespace CARS.SourceCode
{
	public enum UserType
	{
		Employee,
		Manager,
		Administrator,
		ManagerAndAdmin
	}

	public class User
	{
		private Employee employee;
		private Employee manager;

		public bool IsActive { get { return employee.IsActive; } }
		public Employee @Employee { get { return employee; } }
		public UserType Type { get; set; }
        public Guid PKEmployeeID { get { return employee.PKEmployeeID; } }
		public string UserName { get { return employee.Email.Replace("@Advent.com", ""); } set { employee.Email = value + "@Advent.com"; } }
		public string Email { get { return employee.Email; } set { employee.Email = value; } }
		public string Password { get { return employee.Password; } set { employee.Password = value; } }
		public Sex Gender { get { return employee.Gender; } set { employee.Gender = value; } }
		public string FirstName { get { return employee.FirstName == null ? "" : employee.FirstName; } set { employee.FirstName = value; } }
		public string LastName { get { return employee.LastName == null ? "" : employee.LastName; } set { employee.LastName = value; } }
		public float ServiceYears { get { return employee.ServiceYears; } set { employee.ServiceYears = value; } }
		public string MiddleName { get { return employee.MiddleName == null ? "" : employee.MiddleName; } set { employee.MiddleName = value; } }
		public string LegalName { get { return employee.LegalName == null ? "" : employee.LegalName; } set { employee.LegalName = value; } }
		public string Phone { get { return employee.Phone == null ? "" : employee.Phone; } set { employee.Phone = value; } }
		public string Mobile { get { return employee.Mobile == null ? "" : employee.Mobile; } set { employee.Mobile = value; } }
		public DateTime DateOfHire { get { return employee.HiredDate; } set { employee.HiredDate = value; } }
		public string GenderValue { get { return employee.Gender.ToString(); } }
		public string ServiceYearsValue { get { return employee.ServiceYears.ToString("0.0"); } }
		public Employee Supervisor { get { return manager; } set { manager = value; } }
		public string SupervisorValue { get { return manager == null ? "" : manager.FirstName + " " + manager.LastName; } }
		public int Index { get; set; }
		public string IndexValue { get { return Index.ToString(); } }

		public User() : this("demo", "", UserType.Employee) { }

		public User(string name, string password, UserType type)
		{
			if (employee == null)
				employee = new Employee();
			name = name.Trim().ToLower();

			UserName = name;
			employee.Password = password;
			Type = type;
		}

		public User(Employee pEmployee)
		{
			employee = pEmployee;
			manager = employee.Manager;

			UserType type = UserType.Employee;
			if (employee.IsAdmin && employee.IsManager)
				type = UserType.ManagerAndAdmin;
			else if (employee.IsAdmin)
				type = UserType.Administrator;
			else if (employee.IsManager)
				type = UserType.Manager;

			Type = type;
			UserName = employee.Email.ToLower().Replace("@advent.com", "");
			//Question 1: How to get Supervisors for the drop-down list? Maybe need a new API in web service.			
		}

		public override string ToString()
		{
			return FirstName + " " + LastName;
		}
	}
}
