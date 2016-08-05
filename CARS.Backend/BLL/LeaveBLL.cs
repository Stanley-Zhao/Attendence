using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CARS.Backend.Entity;
using CARS.Backend.Common;
using CARS.Backend.DAL;
using System.Data;
using System.Data.SqlClient;

namespace CARS.Backend.BLL
{
	public class LeaveBLL
	{

		#region Public Methods

		public static LeaveInfo GetLeaveInfoByID(string leaveID)
		{
			LeaveInfo leave = null;

			if (!string.IsNullOrEmpty(leaveID))
			{
				List<SearchCondition> conditions = new List<SearchCondition>();
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKLeaveInfoID, leaveID, SearchComparator.Equal, SearchType.SearchString));
				leave = CommonDAL<LeaveInfo>.GetSingleObject(conditions);
			}

			return leave;
		}

		public static LeaveType GetLeaveTypeByName(string leaveTypeName)
		{
			LeaveType leaveType = null;

			if (!string.IsNullOrEmpty(leaveTypeName))
			{
				List<SearchCondition> conditions = new List<SearchCondition>();
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Name, leaveTypeName, SearchComparator.Equal, SearchType.SearchString));
				leaveType = CommonDAL<LeaveType>.GetSingleObject(conditions);
			}

			return leaveType;
		}

		#endregion

		#region Service Methods
		public static bool RecallLeave(string leaveID)
		{
			bool result = false;

			LeaveInfo leave = null;

			try
			{
				if (!string.IsNullOrEmpty(leaveID))
				{
					List<SearchCondition> conditions = new List<SearchCondition>();
					conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKLeaveInfoID, leaveID, SearchComparator.Equal, SearchType.SearchString));
					leave = CommonDAL<LeaveInfo>.GetSingleObject(conditions);
				}

				if (leave == null)
				{
					throw new DataException("Record not found in DB");
				}
				else if(leave.Status != LeaveStatus.Applying)
				{
					throw new DataException(string.Format("Record has been locked since it's {0} already.", leave.Status.ToString()));
				}

				// delete FK time duration first
				List<SearchCondition> conditionsTimeDuration = new List<SearchCondition>();
				conditionsTimeDuration.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKLeaveInfoID, leaveID, SearchComparator.Equal, SearchType.SearchString));

				List<TimeDurationInfo> timeDurations = CommonDAL<TimeDurationInfo>.GetObjects(conditionsTimeDuration);

				for (int i = timeDurations.Count - 1; i >= 0; i--)
				{
					CommonDAL<TimeDurationInfo>.Delete(timeDurations[i]);
				}

				// delete leave info
				CommonDAL<LeaveInfo>.Delete(leave);
				result = true;
			}
			catch(Exception ex)
			{
				result = false;
				throw new Exception(ex.Message);
			}

			return result;
		}

		public static LeaveInfo ApplyLeave(string employeeID, string reason, string leaveType, string description, List<TimeDurationInfo> durationList)
		{
			LeaveInfo leaveInfo = null;

			if (!string.IsNullOrEmpty(employeeID) && !string.IsNullOrEmpty(reason) && !string.IsNullOrEmpty(leaveType) && null != durationList)
			{
				Employee employee = EmployeeBLL.GetEmployeeByID(employeeID);
				if (null != employee)
				{
					if (employee.Manager == null)
						throw new Exception("You didn't set supervisor to approve your leave application.");

					List<SearchCondition> conditions = new List<SearchCondition>();
					conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Name, leaveType, SearchComparator.Equal, SearchType.SearchString));
					LeaveType lType = CommonDAL<LeaveType>.GetSingleObject(conditions);
					leaveInfo =
						LeaveInfo.CreateLeaveInfo(employee.PKEmployeeID, employee.FKReportManagerID, lType.PKLeaveTypeID,
												  LeaveStatus.Applying, reason, description, durationList);
					leaveInfo.Save();

					conditions.Clear();
					conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKLeaveTypeID, leaveInfo.FKLeaveTypeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
					leaveInfo.Type = CommonDAL<LeaveType>.GetSingleObject(conditions);
				}
			}

			return leaveInfo;
		}

		public static List<LeaveInfo> GetMyLeaves(string employeeID)
		{
			return GetMyLeaves(employeeID, DateTime.MinValue, DateTime.MaxValue);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="employeeID"></param>
		/// <param name="start">If start = DateTime.MinValue, it will not be regarded as a where parameter.</param>
		/// <param name="end">If end = DateTime.MinValue, it will not be regarded as a where parameter.</param>
		/// <returns></returns>
		public static List<LeaveInfo> GetMyLeaves(string employeeID, DateTime start, DateTime end)
		{
			List<LeaveInfo> leaveList = null;

			if (!string.IsNullOrEmpty(employeeID))
			{
				List<SearchCondition> conditions = new List<SearchCondition>();
				conditions.Add(SearchCondition.CreateSearchCondition("FKSubmitEmployeeID", employeeID, SearchComparator.Equal, SearchType.SearchString));

				if (start != DateTime.MinValue)
				{
					conditions.Add(SearchCondition.CreateSearchCondition("FirstStartTime", start.ToString(), SearchComparator.Greater, SearchType.SearchString));
				}

				if (end != DateTime.MaxValue)
				{
					conditions.Add(SearchCondition.CreateSearchCondition("LastEndTime", end.ToString(), SearchComparator.Less, SearchType.SearchString));
				}

				leaveList = CommonDAL<LeaveInfo>.GetObjects(conditions, GlobalParams.Status + "," + GlobalParams.FirstStartTime, OrderType.ASC);
			}

			return leaveList;
		}

		/// <summary>
		/// Get all of the leave records of the manager's team members.  Also includes the leave record that the report manager is not the manager.
		/// All the active team members.
		/// </summary>
		/// <param name="manager"></param>
		/// <returns></returns>
		public static List<LeaveInfo> GetMyTeamLeaves(string employeeID, bool showAllRecords)
		{
			return GetMyTeamLeaves(employeeID, CommonMethods.GetCurrentFrozenDate(), DateTime.MinValue, showAllRecords);
		}

		public static List<LeaveInfo> GetMyTeamLeaves(string employeeID, DateTime start, DateTime end, bool showAllRecords)
		{
			List<LeaveInfo> leaveList = null;

			if (!string.IsNullOrEmpty(employeeID))
			{
				leaveList = new List<LeaveInfo>();
				List<SearchCondition> conditions = new List<SearchCondition>();
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKReportManagerID, employeeID, SearchComparator.Equal, SearchType.SearchString));
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.IsActive, Convert.ToString(1), SearchComparator.Equal, SearchType.SearchNotString));

				List<Employee> employees = CommonDAL<Employee>.GetObjects(conditions);

				if (null != employees)
				{
					foreach (Employee employee in employees)
					{
						conditions.Clear();
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKSubmitEmployeeID, employee.PKEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));

						if (start != DateTime.MinValue)
						{
							conditions.Add(SearchCondition.CreateSearchCondition("FirstStartTime", start.ToString(), SearchComparator.Greater, SearchType.SearchString));
						}

						if (end != DateTime.MaxValue)
						{
							conditions.Add(SearchCondition.CreateSearchCondition("LastEndTime", end.ToString(), SearchComparator.Less, SearchType.SearchString));
						}

						if (!showAllRecords) // !showAllRecords means only show applying records
							conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Status, ((int)LeaveStatus.Applying).ToString(), SearchComparator.Equal, SearchType.SearchNotString));

						List<LeaveInfo> leaves = CommonDAL<LeaveInfo>.GetObjects(conditions, GlobalParams.Status + "," + GlobalParams.FirstStartTime, OrderType.ASC);

						if (null != leaves)
						{
							foreach (LeaveInfo leave in leaves)
							{
								leaveList.Add(leave);
							}
						}
					}
				}
			}

			leaveList.Sort(new LeaveInfoComparision());

			return leaveList;
		}

		public static LeaveInfo ApproveLeave(string managerID, string leaveID, LeaveStatus status)
		{
			LeaveInfo leave = null;

			if (!string.IsNullOrEmpty(managerID) && !string.IsNullOrEmpty(leaveID) && status != LeaveStatus.None)
			{
				leave = LeaveBLL.GetLeaveInfoByID(leaveID);
				leave.FKReportManagerID = Guid.Parse(managerID);
				leave.PreStatus = leave.Status; // get previous status
				leave.Status = status;
				leave.Save();
			}

			return leave;
		}

		/// <summary>
		/// All the employee earn the same hours every month.
		/// </summary>
		/// <returns></returns>
		public static double GetAnnualLeaveEarnedHours(string employeeID)
		{
			double result = 0.0d;
			Employee employee = EmployeeBLL.GetEmployeeByID(employeeID);
			if (employee.HiredDate.Year == DateTime.Now.Year)
			{
				for (int i = 1; i <= DateTime.Now.Month; i++)
				{
					// If the employee is hired after 15th, he/she doesn't earn the annual leave hours that month.
					if (i == employee.HiredDate.Month && employee.HiredDate.Day > 15)
					{
						continue;
					}
					else if (i == employee.HiredDate.Month && employee.HiredDate.Day <= 15)
					{
						result += 10d;
						continue;
					}

					// Employee will earn 10 hours annual leave if current date is equal or after 15th in current month.
					if (i == DateTime.Now.Month && DateTime.Now.Day >= 15) result += 10d;

					// Employee will earn 10 hours annual leave in past monthes between his/her joined date and now
					if (i > employee.HiredDate.Month && i < DateTime.Now.Month) result += 10d;
				}
			}
			else
			{
				// Employee will earn 10 hours annual leave if current date is equal or after 15th.
				if (DateTime.Now.Day >= 15)
					result = DateTime.Now.Month * 10;
				else
					result = (DateTime.Now.Month - 1) * 10;
			}
			return result;
		}

		/// <summary>
		/// If the employeeID or leaveTypeName doesn't exist, return 0.
		/// </summary>
		/// <param name="employeeID"></param>
		/// <param name="leaveTypeName"></param>
		/// <returns></returns>
		public static double GetUsedHours(string employeeID, string leaveTypeName)
		{
			double hours = 0;

			if (!string.IsNullOrEmpty(employeeID) && !string.IsNullOrEmpty(leaveTypeName))
			{
				Employee employee = EmployeeBLL.GetEmployeeByID(employeeID);
				LeaveType leaveType = GetLeaveTypeByName(leaveTypeName);

				if (null != employee && null != leaveType)
				{
					List<SearchCondition> conditions = new List<SearchCondition>();
					conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKEmployeeID, employee.PKEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
					conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKLeaveTypeID, leaveType.PKLeaveTypeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
					conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Year, DateTime.Now.Year.ToString(), SearchComparator.Equal, SearchType.SearchNotString));
					EmployeeLeaveSummary leaveSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);
					if (null != leaveSummary) hours = leaveSummary.UsedHours;
				}
			}

			return hours;
		}

		public static List<LeaveType> GetLeaveTypes()
		{
			return CommonDAL<LeaveType>.GetObjects(null, GlobalParams.Name, OrderType.ASC);
		}

		public static List<LeaveInfo> FindLeaves(string manager, string employee, string leaveTypeId, int leaveStatus, DateTime start, DateTime end, string supervisorIDs)
		{
			List<LeaveInfo> leaves = new List<LeaveInfo>();
			DataTable table = new DataTable();

			using (SqlConnection conn = CommonConnection.Conn)
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand("FindLeaves", conn))
				{
					cmd.Parameters.Add(new SqlParameter("@SubmitEmployeeID", employee));
					cmd.Parameters.Add(new SqlParameter("@ReportManagerID", manager));
					cmd.Parameters.Add(new SqlParameter("@LeaveTypeID", leaveTypeId));
					cmd.Parameters.Add(new SqlParameter("@LeaveStatus", leaveStatus));
					cmd.Parameters.Add(new SqlParameter("@StartTime", start));
					cmd.Parameters.Add(new SqlParameter("@EndTime", end));
					cmd.Parameters.Add(new SqlParameter("@SupervisorIDs", supervisorIDs));
					cmd.CommandType = CommandType.StoredProcedure;
					using (SqlDataAdapter adapter = new SqlDataAdapter())
					{
						adapter.SelectCommand = cmd;
						adapter.Fill(table);
					}
				}
				conn.Close();
			}

			if (null != table.Rows)
			{
				foreach (DataRow row in table.Rows)
				{
					LeaveInfo info = new LeaveInfo(523);
					info.SetIsNewFlag(false);
					info.Init(row);
					leaves.Add(info);
				}
			}

			return leaves;
		}

		#endregion
	}
}
