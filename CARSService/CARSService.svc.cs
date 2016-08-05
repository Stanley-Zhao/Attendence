using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CARS.Backend.Entity;
using CARS.Backend.Common;
using CARS.Backend.BLL;
using CARSService.Email;
using System.IO;
using System.Web;
using System.ServiceModel.Activation;
using System.Data.SqlClient;
using System.Configuration;

namespace CARSService
{
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class CARSService : ICARSService
	{
		#region Private methods
		private string GetEmail(string email)
		{
			email = email.Replace(" ", "");
            // Stanley on 2013-12-26
            // Now, we can use sfsmtp.advent.com to send email.
            // So, we don't need to replace @advent.com to @gencos.com
            return email;
		}

		private void AddEmailList(List<string> managers, string p)
		{
			string[] emails = p.Split(',');
			foreach (string email in emails)
				managers.Add(email.Trim().ToLower() + GlobalParams.AdventMail);
		}
		#endregion
		/// <summary>
		/// Login system by email and password
		/// </summary>
		/// <param name="email">This is the unique key in Employee table.</param>
		/// <param name="password">encrypted password from SL client</param>
		/// <returns>Retrun type is Employee.  Returns the employee who is got by email and password.</returns>
		public Employee Login(string email, string password)
		{
			Log.Info(GlobalParams.StartLogin);

			Employee employee = null;

			try
			{
				employee = EmployeeBLL.Login(email, password);

				if (!employee.IsActive)
					throw new Exception(email + " is inactive now." + Environment.NewLine + "Please ask help from Administrator.");
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}

			Log.Info(GlobalParams.EndLogin);

			return employee;
		}

		/// <summary>
		/// Register a new employee in system and send an email to hime/her with default password.
		/// </summary>
		/// <param name="email">This email should be a valid email address as system will send register email to it.</param>
		/// <param name="firstName">Not null in database.</param>
		/// <param name="middleName">Could be null in database.</param>
		/// <param name="lastName">Not null in database.</param>
		/// <param name="gender">Not null in database.</param>
		/// <param name="serviceYear">Count how many years the worker works.</param>
		/// <param name="dateOfHire">Hired date in our company.</param>
		/// <param name="supervisorEmail">Could be null in database.  The email of his/her manager.</param>
		/// <returns>Return type is Employee.  Returns the employee who is registered just now.</returns>
        public Employee AddEmployee(string email, string firstName, string middleName, string lastName, string legalName, Sex gender, float serviceYear, DateTime dateOfHire, string supervisorEmail, string costCenterCode, int employeeNum)
		{
			Log.Info(GlobalParams.StartAddEmpl);
			Employee newEmployee = null;

			try
			{
				newEmployee = EmployeeBLL.AddEmployee(email, firstName, middleName, lastName, gender, serviceYear, dateOfHire, supervisorEmail, costCenterCode, employeeNum, legalName);
				EmailConfig.SetRootPath(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath);
				// If add new employee, send a notification email to user.
				if (newEmployee != null && EmailConfig.EmailTemplates.ContainsKey(EmailType.Register_ToEmployeeSelf.ToString()))
				{
					Log.Info(GlobalParams.StartMail);

					string mEmail = GetEmail(newEmployee.Email);
					string firstNmae = newEmployee.FirstName;
					string userName = newEmployee.Email.Replace(GlobalParams.AdventMail, "").ToLower();
					EmailContent ec = EmailConfig.EmailTemplates[EmailType.Register_ToEmployeeSelf.ToString()];
					// {0} - First Name
					// {1} - Account (User Name)
					// {2} - Password
					// {3} - CARSAddress
					string emailBody = string.Format(ec.Body, firstName, userName, CryptographyStuff.AES_DecryptString(newEmployee.Password), EmailConfig.CARSAddress);
					SendMail sender = new SendMail(mEmail, null, EmailConfig.EmailAccount, emailBody, ec.Title, EmailConfig.Password, EmailConfig.Host);
					sender.Send();
					Log.Info(GlobalParams.EndMail);
				}
				else
				{
					Log.Warn(GlobalParams.MailError);
					throw new Exception(GlobalParams.MailError);
				}
			}
			catch (SqlException sqlEx)
			{
				if (sqlEx.Message.Contains("UniqueKeyEmail")) // try to add duplicate account
				{
					Log.Warn("Try to register a duplicate account." + Environment.NewLine + email);
					throw new Exception(email + " was registered already." + Environment.NewLine + "Try another email account or contact with Administrator.");
				}
				else
				{
					throw sqlEx;
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}

			Log.Info(GlobalParams.EndAddEmpl);

			return newEmployee;
		}

		/// <summary>
		/// Updates employee if the employeeID exists in database.
		/// </summary>
		/// <param name="employeeID">Get employee from database by employeeID.</param>
		/// <param name="email">Unique key in employee database.</param>
		/// <param name="firstName">Not null in database.</param>
		/// <param name="middleName">Could be null in database.</param>
		/// <param name="lastName">Not null in database.</param>
		/// <param name="gender">Not null in database.</param>
		/// <param name="serviceYear">Count how many years the worker works.</param>
		/// <param name="dateOfHire">Hired date in our company.</param>
		/// <param name="supervisorEmail">Could be null in database. The email of his/her manager.</param>
		/// <param name="password">Not null in database. Encrypted password.</param>
		/// <param name="phone">Could be null in database.</param>
		/// <param name="mobile">Could be null in database.</param>
		/// <param name="isAdmin">Check the role of the employee is admin or not.</param>
		/// <param name="isActive">Check the employee is active or terminated.</param>
		/// <returns>Return type is void.  Returns the updated employee.</returns>
        public Employee UpdateEmployee(string employeeID, string email, string firstName, string middleName, string lastName, string legalName, Sex gender, float serviceYear, DateTime dateOfHire, string supervisorEmail, string password, string phone, string mobile, bool isAdmin, bool isActive, bool isManager, string costCenterCode, int employeeNum)
		{
			Log.Info(GlobalParams.StartUpdateEmpl);
			Employee currentEmployee = null;

			try
			{
				currentEmployee = EmployeeBLL.GetEmployeeByID(employeeID);
				EmployeeBLL.UpdateEmployee(employeeID, email, firstName, middleName, lastName, gender, serviceYear, dateOfHire, supervisorEmail, password, phone, mobile, isAdmin, isActive, isManager, costCenterCode, employeeNum, legalName);
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndUpdateEmpl);

			return currentEmployee;
		}

		/// <summary>
		/// Gets all employees who are manager and active.
		/// </summary>
		/// <returns>Retrun type is List->Employee.  Returns employee entites list.</returns>
		public List<Employee> GetManagers()
		{
			Log.Info(GlobalParams.StartGetManager);
			List<Employee> employees = null;

			try
			{
				employees = EmployeeBLL.GetManagers();
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndGetManager);

			return employees;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="employeeID"></param>
		/// <param name="reason"></param>
		/// <param name="leaveType"></param>
		/// <param name="description"></param>
		/// <param name="durationList"></param>
		/// <returns></returns>
		public LeaveInfo ApplyLeave(string employeeID, string reason, string leaveType, string description, List<TimeDurationInfo> durationList)
		{
			Log.Info(GlobalParams.StartApplyLeave);
			LeaveInfo result = null;

			try
			{
				result = LeaveBLL.ApplyLeave(employeeID, reason, leaveType, description, durationList);
				EmailConfig.SetRootPath(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath);
				Employee employee = EmployeeBLL.GetEmployeeByID(employeeID);
				if (result != null && EmailConfig.EmailTemplates.ContainsKey(EmailType.ApplyLeave.ToString()))
				{
					Log.Info(GlobalParams.StartMail);
					EmailContent ec = EmailConfig.EmailTemplates[EmailType.ApplyLeave.ToString()];
					// {0} - Manager First Name
					// {1} - First Name
					// {2} - Hours
					// {3} - Leave Type
					// {4} - Reason
					// {5} - Description
					// {6} - Start
					// {7} - End
					// {8} - CARSAddress
					string emailBody = string.Format(ec.Body, employee.Manager.FirstName, employee.FirstName, result.Hours, result.Type.Name + " Leave", result.Reason, result.Description, result.FirstStartTime.ToString(EmailConfig.TimeFormatInEmail), result.LastEndTime.ToString(EmailConfig.TimeFormatInEmail), EmailConfig.CARSAddress);
					// {0} - First Name
					// {1} - Hours
					// {2} - Leave Type
					string emailTitle = string.Format(ec.Title, employee.FirstName, result.Hours, result.Type.Name + " Leave");
					string employeeEmail = GetEmail(employee.Email);
					string managerEmail = GetEmail(employee.Manager.Email);
					List<string> ccList = new List<string>();
					ccList.Add(employeeEmail);
#if DEBUG
					managerEmail = "szhao@advent.com";
#endif
					SendMail sender = new SendMail(managerEmail, ccList, EmailConfig.EmailAccount, emailBody, emailTitle, EmailConfig.Password, EmailConfig.Host);
					sender.Send();
					Log.Info(GlobalParams.EndMail);
				}
				else
				{
					Log.Warn(GlobalParams.MailError);
					throw new Exception(GlobalParams.MailError);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndApplyLeave);

			return result;
		}

		public bool RecallLeave(string pkleaveInfoID, string employeeID)
		{
			Log.Info(GlobalParams.StartRecallLeave);
			bool result = false;

			try
			{
				EmailConfig.SetRootPath(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath);

				LeaveInfo leaveInfo = LeaveBLL.GetLeaveInfoByID(pkleaveInfoID);
				result = LeaveBLL.RecallLeave(pkleaveInfoID);
				Employee employee = EmployeeBLL.GetEmployeeByID(employeeID);
				if (result && EmailConfig.EmailTemplates.ContainsKey(EmailType.ApplyLeave.ToString()))
				{
					Log.Info(GlobalParams.StartMail);
					EmailContent ec = EmailConfig.EmailTemplates[EmailType.RecallLeave.ToString()];
					// {0} - Manager First Name
					// {1} - First Name
					// {2} - Hours
					// {3} - Leave Type
					// {4} - Reason
					// {5} - Description
					// {6} - Start
					// {7} - End
					// {8} - CARSAddress
					string emailBody = string.Format(ec.Body, employee.Manager.FirstName, employee.FirstName, leaveInfo.Hours,
						leaveInfo.Type.Name + " Leave", leaveInfo.Reason, leaveInfo.Description,
						leaveInfo.FirstStartTime.ToString(EmailConfig.TimeFormatInEmail), 
						leaveInfo.LastEndTime.ToString(EmailConfig.TimeFormatInEmail), EmailConfig.CARSAddress);
					// {0} - First Name
					// {1} - Hours
					// {2} - Leave Type
					string emailTitle = string.Format(ec.Title, employee.FirstName, leaveInfo.Hours, leaveInfo.Type.Name + " Leave");
					string employeeEmail = GetEmail(employee.Email);
					string managerEmail = GetEmail(employee.Manager.Email);
					List<string> ccList = new List<string>();
					ccList.Add(employeeEmail);
#if DEBUG
					managerEmail = "szhao@advent.com";
#endif
					SendMail sender = new SendMail(managerEmail, ccList, EmailConfig.EmailAccount, emailBody, emailTitle, EmailConfig.Password, EmailConfig.Host);
					sender.Send();
					Log.Info(GlobalParams.EndMail);
				}
				else
				{
					Log.Warn(GlobalParams.MailError);
					throw new Exception(GlobalParams.MailError);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndRecallLeave);

			return result;
		}

		/// <summary>
		/// Get my leaves
		/// </summary>
		/// <param name="employeeID">GUID of current employee</param>
		/// <returns>List of leaves</returns>
		public List<LeaveInfo> GetMyLeaves(string employeeID)
		{
			Log.Info(GlobalParams.StartGetPersonalLeaves);
			List<LeaveInfo> leaves = null;

			try
			{
				leaves = LeaveBLL.GetMyLeaves(employeeID, new DateTime(DateTime.Now.Year, 1, 1), DateTime.MaxValue);
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndGetPersonalLeaves);

			return leaves;
		}

		/// <summary>
		/// Get my team members' leaves
		/// </summary>
		/// <param name="employeeID">GUID of current supervisor</param>
		/// <returns>List of leaves</returns>
        public List<LeaveInfo> GetMyTeamLeaves(string employeeID, bool showAllRecords)
		{
			Log.Info(GlobalParams.StartGetTeamLeaves);
			List<LeaveInfo> leaves = null;

			try
			{
				DateTime start;
				int month = DateTime.Now.Month;

				if (month == 1) start = new DateTime(DateTime.Now.Year, 1, 1);
				else start = new DateTime(DateTime.Now.Year, month - 1, 1);

                leaves = LeaveBLL.GetMyTeamLeaves(employeeID, start, DateTime.MaxValue, showAllRecords);
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndGetTeamLeaves);

			return leaves;
		}

		/// <summary>
		/// Approve leave
		/// </summary>
		/// <param name="managerID">GUID of supervisor</param>
		/// <param name="leaveID">GUID of leave</param>
		/// <param name="status">Leave status</param>
		/// <returns>Approved leave</returns>
		public LeaveInfo ApproveLeave(string managerID, string leaveID, LeaveStatus status)
		{
			Log.Info(GlobalParams.StartApproveLeave);
			LeaveInfo result = null;

			try
			{
				result = LeaveBLL.ApproveLeave(managerID, leaveID, status);
				Employee employee = EmployeeBLL.GetEmployeeByID(result.FKSubmitEmployeeID.ToString());
				Employee manager = EmployeeBLL.GetEmployeeByID(result.FKReportManagerID.ToString());
				if (result != null)
				{
					Log.Info(GlobalParams.StartMail);
					EmailConfig.SetRootPath(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath);
					EmailContent ec = EmailConfig.EmailTemplates[EmailType.ApproveLeave.ToString()];
					// {0} - First Name
					// {1} - Manager First Name
					// {2} - Hours
					// {3} - Leave Type
					// {4} - Reason
					// {5} - Description
					// {6} - Start
					// {7} - End
					// {8} - "Approved"
					// {9} - ""
					// {10} - CARSAddress
					string emailBody = string.Format(ec.Body, employee.FirstName, manager.FirstName, result.Hours, result.Type.Name + " Leave", result.Reason, result.Description, result.FirstStartTime.ToString(EmailConfig.TimeFormatInEmail), result.LastEndTime.ToString(EmailConfig.TimeFormatInEmail), "Approved", "", EmailConfig.CARSAddress);
					// {0} - First Name
					// {1} - Hours
					// {2} - Leave Type
					// {3} - Manager First Name
					// {4} - "Approved"
					string emailTitle = string.Format(ec.Title, employee.FirstName, result.Hours, result.Type.Name + " Leave", manager.FirstName, "Approved");
					string employeeEmail = GetEmail(employee.Email);
					string managerEmail = GetEmail(employee.Manager.Email);
#if DEBUG
					managerEmail = "szhao@advent.com";
#endif
					List<string> ccList = new List<string>();
					ccList.Add(managerEmail);
					SendMail sender = new SendMail(employeeEmail, ccList, EmailConfig.EmailAccount, emailBody, emailTitle, EmailConfig.Password, EmailConfig.Host);
					sender.Send();
					Log.Info(GlobalParams.EndMail);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndApproveLeave);

			return result;
		}

		/// <summary>
		/// Reject leave
		/// </summary>
		/// <param name="managerID">GUID of supervisor</param>
		/// <param name="leaveID">GUID of leave</param>
		/// <param name="status">Leave status</param>
		/// <param name="reason">Reason of rejection</param>
		/// <returns>Rejected Reason</returns>
		public LeaveInfo RejectLeave(string managerID, string leaveID, LeaveStatus status, string reason)
		{
			Log.Info(GlobalParams.StartRejectLeave);
			LeaveInfo result = null;

			try
			{
				result = LeaveBLL.ApproveLeave(managerID, leaveID, status);
				Employee employee = EmployeeBLL.GetEmployeeByID(result.FKSubmitEmployeeID.ToString());
				Employee manager = EmployeeBLL.GetEmployeeByID(result.FKReportManagerID.ToString());
				if (result != null)
				{
					Log.Info(GlobalParams.StartMail);
					EmailConfig.SetRootPath(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath);
					EmailContent ec = EmailConfig.EmailTemplates[EmailType.ApproveLeave.ToString()];
					// {0} - First Name
					// {1} - Manager First Name
					// {2} - Hours
					// {3} - Leave Type
					// {4} - Reason
					// {5} - Description
					// {6} - Start
					// {7} - End
					// {8} - "Rejected"
					// {9} - Reason
					// {10} - CARSAddress
					reason = "<br>Reason:<br>" + reason + "<br>";
					string emailBody = string.Format(ec.Body, employee.FirstName, manager.FirstName, result.Hours, result.Type.Name, result.Reason, result.Description, result.FirstStartTime.ToString(EmailConfig.TimeFormatInEmail), result.LastEndTime.ToString(EmailConfig.TimeFormatInEmail), "Rejected", reason, EmailConfig.CARSAddress);
					// {0} - First Name
					// {1} - Hours
					// {2} - Leave Type
					// {3} - Manager First Name
					// {4} - "Rejected"
					string emailTitle = string.Format(ec.Title, employee.FirstName, result.Hours, result.Type.Name, manager.FirstName, "Rejected");
					string employeeEmail = GetEmail(employee.Email);
					string managerEmail = GetEmail(employee.Manager.Email);
					List<string> ccList = new List<string>();
					ccList.Add(managerEmail);
					SendMail sender = new SendMail(employeeEmail, ccList, EmailConfig.EmailAccount, emailBody, emailTitle, EmailConfig.Password, EmailConfig.Host);
					sender.Send();
					Log.Info(GlobalParams.EndMail);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndRejectLeave);

			return result;
		}

		/// <summary>
		/// Forget password
		/// </summary>
		/// <param name="email">Email address to receive password</param>
		/// <returns>True or false</returns>
		public bool ForgetPassword(string email)
		{
			Log.Info(GlobalParams.StartForgetPwd);

			try
			{
				EmailConfig.SetRootPath(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath);

				Employee employee = EmployeeBLL.GetEmployeeByEmail(email);

				if (employee != null && EmailConfig.EmailTemplates.ContainsKey(EmailType.ForgetPassword.ToString()))
				{
					email = GetEmail(email);
					EmailContent emailContent = EmailConfig.EmailTemplates[EmailType.ForgetPassword.ToString()];
					// {0} - First Name
					// {1} - Password
					// {2} - CARSAddress
					string emailBody = string.Format(emailContent.Body, employee.FirstName, CryptographyStuff.AES_DecryptString(employee.Password), EmailConfig.CARSAddress);
					SendMail sender = new SendMail(email, null, EmailConfig.EmailAccount, emailBody, emailContent.Title, EmailConfig.Password, EmailConfig.Host);
					sender.Send();
				}
				else
				{
					Log.Warn(GlobalParams.PwdError);
					return false;
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndForgetPwd);

			return true;
		}

		/// <summary>
		/// Get employee's earned annual leave hours
		/// </summary>
		/// <param name="employeeID">GUID of employee</param>
		/// <returns>Hours</returns>
		public int GetAnnualLeaveEarnedHours(string employeeID)
		{
			Log.Info(GlobalParams.StartGetAnnualLeaveEarnedHours);
			int result = 0;

			try
			{
				result = (int)LeaveBLL.GetAnnualLeaveEarnedHours(employeeID);
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndGetAnnaulLeaveEarnedHours);

			return result;
		}

		/// <summary>
		/// Get employee's used annual leave hours
		/// </summary>
		/// <param name="employeeID">GUID of employee</param>
		/// <returns>Hours</returns>
		public int GetAnnualLeaveUsedHours(string employeeID)
		{
			Log.Info(GlobalParams.StartGetUsedAnnualLeaveHours);
			int result = 0;

			try
			{
				result = (int)LeaveBLL.GetUsedHours(employeeID, GlobalParams.AnnaulLeave);
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndGetUsedAnnualLeaveHours);

			return result;
		}

		/// <summary>
		/// Get employee's used sick leave hours
		/// </summary>
		/// <param name="employeeID">GUID of employee</param>
		/// <returns>Hours</returns>
		public int GetSickLeaveUsedHours(string employeeID)
		{
			Log.Info(GlobalParams.StartGetSickLeaveUsedHours);
			int result = 0;

			try
			{
				result = (int)LeaveBLL.GetUsedHours(employeeID, GlobalParams.SickLeave);
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndGetSickLeaveUsedHours);

			return result;
		}

		/// <summary>
		/// Get leave types
		/// </summary>
		/// <returns>List of leave types</returns>
		public List<LeaveType> GetLeaveTypes()
		{
			Log.Info(GlobalParams.StartGetLeaveTypes);
			List<LeaveType> leaveTypes = null;

			try
			{
				leaveTypes = LeaveBLL.GetLeaveTypes();
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndGetLeaveTypes);

			return leaveTypes;
		}

		/// <summary>
		/// Find leaves
		/// </summary>
		/// <param name="supervisor">Supervisor name</param>
		/// <param name="applicant">Applicant name</param>
		/// <param name="leaveTypeId">GUID of leave type</param>
		/// <param name="leaveStatus">Leave status</param>
		/// <param name="start">Start time</param>
		/// <param name="end">End time</param>
		/// <param name="supervisorIDs">GUIDs of supervisor</param>
		/// <returns>List of leaves</returns>
		public List<LeaveInfo> FindLeaves(string supervisor, string applicant, string leaveTypeId, string leaveStatus, DateTime start, DateTime end, string supervisorIDs)
		{
			Log.Info(GlobalParams.StartFindLeaves);
			List<LeaveInfo> result = null;
			try
			{
				if (leaveStatus == "All")
					leaveStatus = "None";
				LeaveStatus status = (LeaveStatus)Enum.Parse(typeof(LeaveStatus), leaveStatus);
				result = LeaveBLL.FindLeaves(supervisor, applicant, leaveTypeId, (int)status, start, end, supervisorIDs);
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndFindLeaves);
			return result;
		}

		/// <summary>
		/// Get all employees
		/// </summary>
		/// <returns>List of employees</returns>
		public List<Employee> GetAllEmployees()
		{
			Log.Info(GlobalParams.StartGetAllEmployees);
			List<Employee> employees = null;

			try
			{
				employees = EmployeeBLL.GetAllEmployees();
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
			Log.Info(GlobalParams.EndGetAllEmployees);

			return employees;
		}

		/// <summary>
		/// Get report periods
		/// </summary>
		/// <returns>List of report periods</returns>
		public List<ReportPeriod> GetReportPeriods()
		{
			try
			{
				return ReportBLL.GetReportPeriods();
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
		}

		/// <summary>
		/// Update report periods
		/// </summary>
		/// <param name="reportPeriods">List of report repiods</param>
		/// <returns>True or false</returns>
		public bool UpdateReportPeriods(List<ReportPeriod> reportPeriods)
		{
			try
			{
				ReportBLL.UpdateReportPeriods(reportPeriods);
				return true;
			}
			catch (Exception ex)
			{
				Log.Exception(ex.StackTrace);
				Log.Exception(ex.Message);
				throw ex;
			}
		}

		/// <summary>
		/// Get version information
		/// </summary>
		/// <returns>String of version</returns>
		public string GetVersion()
		{
			return ConfigurationManager.AppSettings["Version"];
		}

		/// <summary>
		/// Get frozen date
		/// </summary>
		/// <returns>Frozen date</returns>
		public DateTime GetFrozenDate()
		{
			return CommonMethods.GetCurrentFrozenDate();
		}

		/// <summary>
		/// Get URL of annual leave report for admin
		/// </summary>
		/// <returns>URL of annual leave report for admin</returns>
		public string GetAnnualLeaveReportForAdminAddress()
		{
			return ConfigurationManager.AppSettings["AnnualLeaveReportForAdmin"];
		}

		/// <summary>
		/// Get URL of sick leave report for admin
		/// </summary>
		/// <returns>URL of sick leave report for admin</returns>
		public string GetSickLeaveReportForAdminAddress()
		{
			return ConfigurationManager.AppSettings["SickLeaveReportForAdmin"];
		}

		/// <summary>
		/// Check supervisor validation
		/// </summary>
		/// <param name="employeeID">GUID of current employee</param>
		/// <param name="supervisorID">GUID of selected supervisor</param>
		/// <returns>True or false</returns>
		public bool CheckSupervisorValidation(string employeeID, string supervisorID)
		{
			return EmployeeBLL.CheckSupervisorValidation(employeeID, supervisorID);
		}

		/// <summary>
		/// Get URL of user manual address
		/// </summary>
		/// <returns>URL of user manual address</returns>
		public string GetUserManualAddress()
		{
			return ConfigurationManager.AppSettings["UserManual"];
		}

		/// <summary>
		/// Get URL of annual leave report for supervisor
		/// </summary>
		/// <returns>URL of annual leave report for supervisor</returns>
		public string GetAnnualLeaveReportForSupervisorAddress()
		{
			return ConfigurationManager.AppSettings["AnnualLeaveReportForSupervisor"];
		}

		/// <summary>
		/// Get URL of sick leave report for supervisor
		/// </summary>
		/// <returns>URL of sick leave report for supervisor</returns>
		public string GetSickLeaveReportForSupervisorAddress()
		{
			return ConfigurationManager.AppSettings["SickLeaveReportForSupervisor"];
		}

		/// <summary>
		/// Get URL of reporting bug address
		/// </summary>
		/// <returns>URL of reporting bug address</returns>
		public string GetReportBugAddress()
		{
			return ConfigurationManager.AppSettings["ReportBug"];
		}
	}
}
