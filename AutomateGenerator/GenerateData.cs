using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CARS.Backend.BLL;
using CARS.Backend.Common;
using CARS.Backend.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;

namespace AutomateGenerator
{
	public partial class GenerateData : Form
	{
		#region Members
		private const string emplyeeName = "Email({0})@Advent.com";
		private const string firstName = "FirstName({0})";
		private const string lastName = "LastName({0})";
		private const string leaveTypeName = "LeaveType({0})";
		private const string newFirstName = "f{0}";
		private const string newLastName = "l{0}";
		private string logFile = string.Empty;
		#endregion

		#region Constructor
		public GenerateData()
		{
			InitializeComponent();

			logFile = Path.Combine(Environment.CurrentDirectory, "TestData.txt");
			if (File.Exists(logFile))
				File.Delete(logFile);
			File.Create(logFile).Close();
		}
		#endregion

		#region Private methods
		private void GenerateStartEndTime(ref DateTime start, ref DateTime end)
		{
			do
			{
				Random random = new Random((int)DateTime.Now.Ticks);
				int startTimeSeed = random.Next(1);
				int startTime = 0;
				switch (startTimeSeed % 2)
				{
					case 0:
						startTime = 9;
						break;
					case 1:
						startTime = 13;
						break;
				}

				int endTimeSeed = random.Next(1);
				int endTime = 0;
				switch (endTimeSeed % 2)
				{
					case 0:
						endTime = 13;
						break;
					case 1:
						endTime = 17;
						break;
				}

				int temp = random.Next(30);
				start = DateTime.Now.AddDays(temp);
				start = new DateTime(start.Year, start.Month, start.Day, startTime, 0, 0);
				end = DateTime.Now.AddDays(temp + random.Next(10));
				end = new DateTime(end.Year, end.Month, end.Day, endTime, 0, 0);
			} while (start >= end);
		}

		private void DeleteData()
		{
			string query = @"delete from TimeDurationInfo;
                             delete from LeaveInfo;
                             delete from EmployeeLeaveSummary;
                             delete from LeaveType;
                             delete from EmployeeRoleRL;
                             delete from Employee;
                             delete from Role;";

			using (SqlConnection conn = new SqlConnection("server=COSAPX2;uid=sa;pwd=Advent.sa;database=CARS"))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.ExecuteNonQuery();
				}
				conn.Close();
			}
		}

		private void WriteLog(string msg)
		{
			StreamWriter sw = File.AppendText(logFile);
			sw.Write(msg);
			sw.Flush();
			sw.Close();
		}
		#endregion

		#region Click Buttons
		private void btnGenerateData_Click(object sender, EventArgs e)
		{
			int countEmployee = Int32.Parse(txtCountEmployee.Text);
			int countLeave = Int32.Parse(txtCountLeave.Text);

			string roleName1 = RoleRank.Admin.ToString();
			Role role1 = Role.GetRoleByName(roleName1);
			if (null == role1)
			{
				role1 = Role.CreateRole(roleName1);
				role1.Save();
			}

			string roleName2 = RoleRank.Manager.ToString();
			Role role2 = Role.GetRoleByName(roleName2);
			if (null == role2)
			{
				role2 = Role.CreateRole(roleName2);
				role2.Save();
			}

			string roleName3 = RoleRank.Employee.ToString();
			Role role3 = Role.GetRoleByName(roleName3);
			if (null == role3)
			{
				role3 = Role.CreateRole(roleName3);
				role3.Save();
			}

			// Add a default manager.
			Employee manager = Employee.CreateEmployee(string.Format(emplyeeName, Guid.NewGuid().ToString().Substring(0, 5)),
														CryptographyStuff.AES_EncryptString("1234"),
														Sex.Male,
														string.Format(firstName, Guid.NewGuid().ToString().Substring(0, 5)),
														Convert.ToDateTime("1/1/2011"),
														string.Format(lastName, Guid.NewGuid().ToString().Substring(0, 5)));
			List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
			employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role2.PKRoleID));
			manager.SetRoleList(employeeRoleList);
			manager.IsActive = true;
			manager.Save();
			Guid managerID = manager.PKEmployeeID;

			string leaveTypeName1 = string.Format(leaveTypeName, Guid.NewGuid().ToString().Substring(0, 5));
			LeaveType leaveType1 = LeaveType.CreateLeaveType(leaveTypeName1,
										100,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType1.Save();

			string leaveTypeName2 = string.Format(leaveTypeName, Guid.NewGuid().ToString().Substring(0, 5));
			LeaveType leaveType2 = LeaveType.CreateLeaveType(leaveTypeName2,
										100,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType2.Save();

			string leaveTypeName3 = string.Format(leaveTypeName, Guid.NewGuid().ToString().Substring(0, 5));
			LeaveType leaveType3 = LeaveType.CreateLeaveType(leaveTypeName3,
										100,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType3.Save();

			DateTime durationStartTime = Convert.ToDateTime("02/02/2001");
			DateTime durationEndTime = Convert.ToDateTime("02/05/2001");
			DateTime durationStartTime2 = Convert.ToDateTime("02/02/2011");
			DateTime durationEndTime2 = Convert.ToDateTime("02/05/2011");

			while (countEmployee > 0)
			{
				employeeRoleList = new List<EmployeeRoleRL>();
				switch (countEmployee % 3)
				{
					case 0:
						// Admin
						employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role1.PKRoleID));
						break;
					case 1:
						// Manager
						employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role2.PKRoleID));
						break;
					case 2:
						// Employee
						employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role3.PKRoleID));
						break;
					default:
						// Employee
						employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, role3.PKRoleID));
						break;
				}

				Employee employee = Employee.CreateEmployee(string.Format(emplyeeName, Guid.NewGuid().ToString().Substring(0, 5)),
															CryptographyStuff.AES_EncryptString("1234"),
															Sex.Male,
															string.Format(firstName, Guid.NewGuid().ToString().Substring(0, 5)),
															Convert.ToDateTime("1/1/2011"),
															string.Format(lastName, Guid.NewGuid().ToString().Substring(0, 5)));
				employee.SetRoleList(employeeRoleList);
				employee.IsActive = true;
				employee.FKReportManagerID = managerID;
				employee.Save();

				// Set the manager id as the employee who is manager.
				if (countEmployee % 3 == 1)
				{
					managerID = employee.PKEmployeeID;
				}

				int count = countLeave;
				while (count > 0)
				{
					string name = string.Empty;
					switch (countLeave % 3)
					{
						case 0:
							name = leaveTypeName1;
							break;
						case 1:
							name = leaveTypeName2;
							break;
						case 2:
							name = leaveTypeName3;
							break;
						default:
							name = leaveTypeName1;
							break;
					}

					List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
					TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
					TimeDurationInfo timeDurationInfo2 = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime2, durationEndTime2);
					timeDurationList.Add(timeDurationInfo);
					timeDurationList.Add(timeDurationInfo2);

					LeaveBLL.ApplyLeave(employee.PKEmployeeID.ToString(), "generated automatically", name, "generated automatically", timeDurationList);

					count--;
				}

				countEmployee--;
			}

			MessageBox.Show("Finished!");
		}

		private void btnClearDatabase_Click(object sender, EventArgs e)
		{
			DeleteData();

			MessageBox.Show("Finished!");
		}

		private void btnGenerateTestData_Click(object sender, EventArgs e)
		{
			#region Delete Data First
			DeleteData();
			#endregion

			int countEmployee = 0;
			int countLeave = 0;
			List<Guid> managerIDs = new List<Guid>();

			#region Check Input
			if (!Int32.TryParse(txtCountEmployee.Text, out countEmployee))
			{
				MessageBox.Show("Count of Employees must be a number.");
				return;
			}

			if (!Int32.TryParse(txtCountLeave.Text, out countLeave))
			{
				MessageBox.Show("Count of level records for each employee must be a number.");
				return;
			}

			// we don't have more than 200 employees in BJ office.
			if (countEmployee > 200 || countEmployee < 1)
			{
				MessageBox.Show("We don't have more than 200 employees in BJ office.\r\nCount of Employees must >=1 and <201.");
				return;
			}

			// each employee can ask 150 leaves for a year? don't believe so
			if (countLeave > 150 || countLeave < 0)
			{
				MessageBox.Show("Each employee can ask 150 leaves for a year? Don't believe so.\r\nCount of level records for each employee must >=0 and <151.");
				return;
			}
			#endregion

			#region Add Role - Manager/Admin/Employee
			WriteLog("<< TEST DATA >>\r\n");
			WriteLog("\r\n==Roles==\r\n");
			// Stanley add: Our Real data is "Admin"/"Manager" and "Employee"
			string strManagerRole = "Manager";
			Role roleManager = Role.CreateRole(strManagerRole);
			roleManager.Save();
			//List<EmployeeRoleRL> employeeRoleList_Manager = new List<EmployeeRoleRL>();
			//employeeRoleList_Manager.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleManager.PKRoleID));
			WriteLog("Role: Manager\r\n");

			// Stanley add: Our Real data is "Admin"/"Manager" and "Employee"
			string strAdminRole = "Admin";
			Role roleAdmin = Role.CreateRole(strAdminRole);
			roleAdmin.Save();
			//List<EmployeeRoleRL> employeeRoleList_Admin = new List<EmployeeRoleRL>();
			//employeeRoleList_Admin.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleAdmin.PKRoleID));
			WriteLog("Role: Admin\r\n");

			// Stanley add: Our Real data is "Admin"/"Manager" and "Employee"
			string strEmployeeRole = "Employee";
			Role roleEmployee = Role.CreateRole(strEmployeeRole);
			roleEmployee.Save();
			//List<EmployeeRoleRL> employeeRoleList_Employee = new List<EmployeeRoleRL>();
			//employeeRoleList_Employee.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleEmployee.PKRoleID));
			WriteLog("Role: Employee\r\n");
			#endregion

			#region Add Leave Type, 7 types actually
			/**
			 * Annual Leave
			 * Bereavement Leave
			 * Marriage Leave
			 * Maternity Leave
			 * Paternity Leave
			 * Regular Check
			 * Sick Leave
			 */
			WriteLog("\r\n==Leave Types==\r\n");
			string leave_Annual = "Annual Leave";
			LeaveType leaveType_Annual = LeaveType.CreateLeaveType(leave_Annual,
										120,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType_Annual.Save();
			WriteLog("\r\nLeave Type: Annual Leave\r\n");

			string leave_Sick = "Sick Leave";
			LeaveType leaveType_Sick = LeaveType.CreateLeaveType(leave_Sick,
										80,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType_Sick.Save();
			WriteLog("Leave Type: Sick Leave\r\n");

			string leave_Marriage = "Marriage Leave";
			LeaveType leaveType_Marriage = LeaveType.CreateLeaveType(leave_Marriage,
										80,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType_Marriage.Save();
			WriteLog("Leave Type: Marriage Leave\r\n");

			// only for female
			string leave_Maternity = "Maternity Leave";
			LeaveType leaveType_Maternity = LeaveType.CreateLeaveType(leave_Maternity,
										960,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType_Maternity.Save();
			WriteLog("Leave Type: Maternity Leave\r\n");

			// only for male
			string leave_Paternity = "Paternity Leave";
			LeaveType leaveType_Paternity = LeaveType.CreateLeaveType(leave_Paternity,
										40,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType_Paternity.Save();
			WriteLog("Leave Type: Paternity Leave\r\n");

			// only for female
			string leave_RegularCheck = "Regular Check Leave";
			LeaveType leaveType_RegularCheck = LeaveType.CreateLeaveType(leave_RegularCheck,
										80,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType_RegularCheck.Save();
			WriteLog("Leave Type: Regular Check Leave\r\n");

			string leave_Bereavement = "Bereavement Leave";
			LeaveType leaveType_Bereavement = LeaveType.CreateLeaveType(leave_Bereavement,
										80,
										Convert.ToDateTime("1/1/2010"),
										Convert.ToDateTime("1/1/2011"));
			leaveType_Bereavement.Save();
			WriteLog("Leave Type: Bereavement Leave\r\n");
			#endregion

			#region Add a default (root) manager, it's rchou or hniu actually, here using rchou as example.
			Employee rootManager = Employee.CreateEmployee("rchou@Advent.com",
													 CryptographyStuff.AES_EncryptString("1234"),
														Sex.Male,
														"Richard",
														Convert.ToDateTime("1/1/2001"),
														"Chou");
			List<EmployeeRoleRL> employeeRoleList_RootManager = new List<EmployeeRoleRL>();
			employeeRoleList_RootManager.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleManager.PKRoleID));
			employeeRoleList_RootManager.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleAdmin.PKRoleID));
			employeeRoleList_RootManager.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleEmployee.PKRoleID));
			rootManager.SetRoleList(employeeRoleList_RootManager); // manager & admin & employee
			rootManager.IsActive = true;
			rootManager.Save();
			rootManager.IsAdmin = rootManager.IsManager = true;
			managerIDs.Add(rootManager.PKEmployeeID); // managerIDs[0] is rchou's GUID

			WriteLog("\r\n==Test data of Users==\r\n");
			WriteLog("=====================================================================\r\n");
			WriteLog("Name\t\tIsManager\tIsAdmin\tBelong to\tLeave Counts\r\n");
			WriteLog("=====================================================================\r\n");
			WriteLog(rootManager.Email.Replace("@Advent.com", "") + "\t\t" + rootManager.IsManager.ToString() + "\t\t" + rootManager.IsAdmin.ToString() + "\t \t\t0\r\n");
			WriteLog("---------------------------------------------------------------------\r\n");
			#endregion

			#region As Shan said, create two test accounts, Admin(super-user) and Manager.
			// test account employee@advent.com (admin and manager)
			Employee adminAccount = Employee.CreateEmployee("admin@Advent.com",
													 CryptographyStuff.AES_EncryptString("1234"),
														Sex.Female,
														"Admin",
														Convert.ToDateTime("1/1/2001"),
														"Test");
			List<EmployeeRoleRL> adminRoleList_EmployeeAccount = new List<EmployeeRoleRL>();
			adminRoleList_EmployeeAccount.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleEmployee.PKRoleID));
			adminRoleList_EmployeeAccount.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleAdmin.PKRoleID));
			adminRoleList_EmployeeAccount.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleManager.PKRoleID));
			adminAccount.SetRoleList(adminRoleList_EmployeeAccount); // admin, manager and employee
			adminAccount.IsActive = true;
			adminAccount.FKReportManagerID = rootManager.PKEmployeeID; // report to Richard			
			adminAccount.Save();
			adminAccount.IsAdmin = adminAccount.IsManager = true;
			managerIDs.Add(adminAccount.PKEmployeeID); // managerIDs[1] is testAdmin's GUID
			WriteLog(adminAccount.Email.Replace("@Advent.com", "") + "\t\t" + adminAccount.IsManager.ToString() + "\t\t" + adminAccount.IsAdmin.ToString() + "\t" + rootManager.Email.Replace("@Advent.com", "") + "\t\t");
			WriteLog("0\r\n");
			WriteLog("---------------------------------------------------------------------\r\n");

			// test account manager@advent.com
			Employee managerAccount = Employee.CreateEmployee("manager@Advent.com",
													 CryptographyStuff.AES_EncryptString("1234"),
														Sex.Male,
														"Manager",
														Convert.ToDateTime("1/1/2001"),
														"Test");
			List<EmployeeRoleRL> employeeRoleList_ManagerAccount = new List<EmployeeRoleRL>();
			employeeRoleList_ManagerAccount.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleManager.PKRoleID));
			employeeRoleList_ManagerAccount.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleEmployee.PKRoleID));
			managerAccount.SetRoleList(employeeRoleList_ManagerAccount); // manager & employee
			managerAccount.IsActive = true;
			managerAccount.FKReportManagerID = adminAccount.PKEmployeeID; // report to admin
			managerAccount.Save();
			managerAccount.IsManager = true;
			managerIDs.Add(managerAccount.PKEmployeeID); // managerIDs[2] is testManager's GUID
			WriteLog(managerAccount.Email.Replace("@Advent.com", "") + "\t\t" + managerAccount.IsManager.ToString() + "\t\t" + managerAccount.IsAdmin.ToString() + "\t" + adminAccount.Email.Replace("@Advent.com", "") + "\t\t");
			WriteLog("0\r\n");
			WriteLog("---------------------------------------------------------------------\r\n");
			#endregion

			#region Create an employee randomlly
			int index = 0;
			string employeeName = "";
			Sex sex = Sex.Male;
			// create employees (admins and managers)
			while (countEmployee > 0)
			{
				bool isAdmin = false;
				bool isManager = false;
				Random random = new Random((int)DateTime.Now.Ticks);
				index++;
				List<EmployeeRoleRL> employeeRoleList = new List<EmployeeRoleRL>();
				switch ((index - 1) % 6) // many employees, few admins and managers
				{
					case 0:
						employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleManager.PKRoleID));
						employeeName = "m" + index.ToString("000") + "@Advent.com";
						isManager = true;
						break;
					case 1:
						employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleAdmin.PKRoleID));
						employeeName = "a" + index.ToString("000") + "@Advent.com";
						isAdmin = true;
						break;
					case 2:
						employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleEmployee.PKRoleID));
						employeeName = "e" + index.ToString("000") + "@Advent.com";
						break;
					default:
						employeeRoleList.Add(EmployeeRoleRL.CreateEmployeeRoleRL(Guid.Empty, roleEmployee.PKRoleID));
						employeeName = "e" + index.ToString("000") + "@Advent.com";
						break;
				}

				if (random.Next(100) % 3 == 0) sex = Sex.Female;  // 1/3 people are female

				int tempDay = -random.Next(30, 720);
				Employee employee = Employee.CreateEmployee(employeeName,
															CryptographyStuff.AES_EncryptString("1234"),
															sex, //Sex.Male,
															string.Format(newFirstName, index.ToString("000")) + employeeName[0],
															DateTime.Now.AddDays(tempDay),
															string.Format(newLastName, index.ToString("000")) + employeeName[0]);
				employee.SetRoleList(employeeRoleList);
				employee.IsActive = true;
				if (employeeName.StartsWith("m")) // a manager
				{
					employee.FKReportManagerID = managerIDs[0]; // manager reports to root manager					
				}
				else if (employeeName.StartsWith("a")) // a admin
				{
					employee.FKReportManagerID = managerIDs[0]; // admin reports to root manager
				}
				else
				{
					Random randomTemp = new Random((int)DateTime.Now.Ticks);
					if (managerIDs.Count == 1)
						employee.FKReportManagerID = managerIDs[0]; // only have one manager
					else if (managerIDs.Count == 2)
						employee.FKReportManagerID = managerIDs[1]; // normally, others employees belong to a normal manager, not root manager
					else
					{
						int tempIndex = randomTemp.Next(1, managerIDs.Count - 1);
						employee.FKReportManagerID = managerIDs[tempIndex];
					}
				}
				employee.Save();
				employee.IsManager = isManager;
				employee.IsAdmin = isAdmin;
				Employee tempManager = EmployeeBLL.GetEmployeeByID(employee.FKReportManagerID.ToString());
				WriteLog(employee.Email.Replace("@Advent.com", "") + "\t\t" + employee.IsManager.ToString() + "\t\t" + employee.IsAdmin.ToString() + "\t" + tempManager.Email.Replace("@Advent.com", "") + "\t\t");

				// add manager to management team 
				if (employeeName.StartsWith("m"))
					managerIDs.Add(employee.PKEmployeeID);

				int count = countLeave;
				int leaves = 0;
				while (count > 0)
				{
					string name = string.Empty;
					switch (random.Next(100) % 7)
					{
						case 0:
							name = leave_Annual;
							break;
						case 1:
							name = leave_Sick;
							break;
						case 2:
							name = leave_Marriage;
							break;
						case 3:
							name = leave_Bereavement;
							break;
						case 4:
							name = leave_Maternity;
							break;
						case 5:
							name = leave_Paternity;
							break;
						case 6:
							name = leave_RegularCheck;
							break;
						default:
							name = leave_Annual;
							break;
					}

					#region Create Random TimeDurationInfo
					Random randomForTimeDurationInfo = new Random((int)DateTime.Now.Ticks);
					List<TimeDurationInfo> timeDurationList = new List<TimeDurationInfo>();
					for (int i = 0; i < randomForTimeDurationInfo.Next(5); i++)
					{
						DateTime durationStartTime = DateTime.Now;
						DateTime durationEndTime = DateTime.Now;
						GenerateStartEndTime(ref durationStartTime, ref durationEndTime);
						TimeDurationInfo timeDurationInfo = TimeDurationInfo.CreateTimeDurationInfo(durationStartTime, durationEndTime);
						timeDurationList.Add(timeDurationInfo);
					}
					#endregion

					if (timeDurationList.Count > 0)
					{
						LeaveBLL.ApplyLeave(employee.PKEmployeeID.ToString(), "generated automatically", name, "generated automatically", timeDurationList);
						leaves++;
					}

					count--;
				}
				WriteLog(leaves.ToString() + "\r\n");
				WriteLog("---------------------------------------------------------------------\r\n");

				countEmployee--;
			}
			#endregion

			if (MessageBox.Show("Finished!") == System.Windows.Forms.DialogResult.OK)
				Process.Start(logFile);
		}
		#endregion
	}
}
