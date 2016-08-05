using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CARS.Backend.DAL;
using CARS.Backend.Entity;
using CARS.Backend.BLL;
using CARSService.Email;
using System.IO;
using System.Reflection;

namespace CARSReminder
{
	class Program
	{
		private static string logFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Cars_Reminder_Log");
		private static string logFileName = "CARS_Remainder_{0}.log";
		private static string logFile = string.Empty;
		private static bool debugMode = false;

		private static void Log(string type, string log)
		{
			if (!Directory.Exists(logFilePath))
			{
				Directory.CreateDirectory(logFilePath);
			}

			if (string.IsNullOrEmpty(logFile))
			{
				logFile = Path.Combine(logFilePath, string.Format(logFileName, DateTime.Now.ToString("yyyy_MM_dd")));
			}

			log = string.Format("{0} [{1}] {2}\r\n", type, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), log);

			File.AppendAllText(logFile, log);
		}

		private static void Info(string info)
		{
			Log("= INFO =", info);
		}

		private static void Error(string error)
		{
			Log("**** ERROR ****", error);
		}

		private static EmailContent ec = null;

		static void Main(string[] args)
		{
			debugMode = File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "debug.txt"));
		
			EmailConfig.SetRootPath(Environment.CurrentDirectory);
			EmailConfig.RefreshData();
			ec = EmailConfig.EmailTemplates[EmailType.ReminderEmail.ToString()];

			List<Employee> managers = EmployeeBLL.GetManagers();

			foreach (Employee m in managers)
			{
#if DEBUG
				//if (m.Email.ToLower() != "kzhu@advent.com")
				//	continue;
#endif

				if (m.IsActive)
				{
					//Console.WriteLine(m.FirstName + " " + m.LastName);
					if (SendReminderEmail(m))
					{
						Info("Email sent out!");
#if DEBUG
						//break;
#endif
					}
				}
			}

#if DEBUG
			Console.WriteLine("Done");
			Console.ReadKey();
#endif
		}

		private static bool SendReminderEmail(Employee m)
		{
			try
			{
				DateTime now = DateTime.Now;
				DateTime start = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
				DateTime end = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59);

#if DEBUG
				start = new DateTime(2016, 7, 1, 0, 0, 0);
				end = new DateTime(2016, 7, 31, 23, 59, 59);
#endif

				List<LeaveInfo> leaves = LeaveBLL.GetMyTeamLeaves(m.PKEmployeeID.ToString(),
					start,
					end,
					false
					);

				if (leaves.Count == 0)
					return false; // do nothing

				#region Title
				string title = string.Empty;

				if (leaves.Count == 1)
				{
					title = "1 leave application";
				}
				else
				{
					title = string.Format("{0} leave applications", leaves.Count);
				}

				Info(string.Format("{0} has {1} to be handled.", m.FirstName + " " + m.LastName, title));

				title = string.Format(ec.Title, title);
				#endregion

				#region Table Rows
				StringBuilder sb = new StringBuilder();
				foreach (LeaveInfo info in leaves)
				{
					if (info.Status != CARS.Backend.Common.LeaveStatus.Applying)
					{
						continue;
					}

					string row = string.Empty;

					/*
					 * 0 - Employee Name
					 * 1 - Leave Hours
					 * 2 - Leave Type
					 * 3 - Reason
					 * 4 - Description
					 * 5 - Start
					 * 6 - End
					 */
					row = string.Format(ec.TableRow,
						(info.Submitter.FirstName + " " + info.Submitter.LastName),
						info.Hours,
						info.Type.Name,
						info.Reason,
						info.Description,
						info.FirstStartTime.ToString(EmailConfig.TimeFormatInEmail),
						info.LastEndTime.ToString(EmailConfig.TimeFormatInEmail));

					sb.Append(row);
				}
				#endregion

				#region Body

				/*
				 * 0 - Manager Name
				 * 1 - Table Rows
				 */
				string body = string.Format(ec.Body, m.FirstName, sb.ToString(), EmailConfig.Host);
				#endregion

				string managerEmail = m.Email;

#if DEBUG
				managerEmail = "szhao@advent.com";
#endif
				
				if(debugMode)
					managerEmail = "szhao@advent.com";

				SendMail sender = new SendMail(managerEmail, null, EmailConfig.EmailAccount, body, title, EmailConfig.Password, EmailConfig.Host);
				sender.Send();
				return true;
			}
			catch (Exception ex)
			{
				Error(ex.Message + Environment.NewLine + Environment.NewLine + ex.ToString());
				return false;
			}
		}
	}
}
