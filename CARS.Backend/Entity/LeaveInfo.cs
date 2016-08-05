using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;
using CARS.Backend.DAL;
using System.Transactions;

namespace CARS.Backend.Entity
{
	public class LeaveInfo : BaseEntity
	{

		#region Private Fields

		private Guid pkLeaveInfoID;
		private Guid fkSubmitEmployeeID;
		private Guid fkReportManagerID;
		private Guid fkLeaveTypeID;
		private LeaveStatus status;
		private String reason;
		private DateTime firstStartTime;
		private DateTime lastEndTime;
		private String description;
		private DateTime knowledgeDate;
		private DateTime createdTime;

		private List<TimeDurationInfo> timeDurationList;
		private LeaveType leaveCategory;
		private Employee manager;
		private Employee submitter;

		#endregion

		#region Public Properties

		public Guid PKLeaveInfoID
		{
			get { return pkLeaveInfoID; }
			set { pkLeaveInfoID = value; }
		}

		public Guid FKSubmitEmployeeID
		{
			get { return fkSubmitEmployeeID; }
			set { fkSubmitEmployeeID = value; }
		}

		public Guid FKReportManagerID
		{
			get { return fkReportManagerID; }
			set { fkReportManagerID = value; }
		}

		public Guid FKLeaveTypeID
		{
			get { return fkLeaveTypeID; }
			set { fkLeaveTypeID = value; }
		}

		public DateTime CreatedTime
		{
			get { return createdTime; }
			set { createdTime = value; }
		}

		[Custom]
		public LeaveStatus PreStatus
		{
			get;
			set;
		}

		public LeaveStatus Status
		{
			get { return status; }
			set { status = value; }
		}

		public String Reason
		{
			get { return reason; }
			set { reason = value; }
		}

		public DateTime FirstStartTime
		{
			get { return firstStartTime; }
			set { firstStartTime = value; }
		}

		public DateTime LastEndTime
		{
			get { return lastEndTime; }
			set { lastEndTime = value; }
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

		[Custom]
		public Employee Manager
		{
			get { return manager; }
			set { }
		}

		[Custom]
		public int Hours
		{
			get
			{
				return CommonMethods.ComputeHours(timeDurationList);
			}
			set { }
		}

		[Custom]
		public LeaveType Type
		{
			get { return leaveCategory; }
			set { leaveCategory = value; }
		}

		[Custom]
		public List<TimeDurationInfo> TimeDurationInfoList
		{
			get { return timeDurationList; }
			set { timeDurationList = value; }
		}

		[Custom]
		public Employee Submitter
		{
			get { return submitter; }
			set { }
		}
		#endregion

		#region Override Fields

		public override Guid GetPKID()
		{
			return pkLeaveInfoID;
		}

		public override string GetPKIDName()
		{
			return GlobalParams.PKLeaveInfoID;
		}

		public override void SetPKID(Guid pkID)
		{
			pkLeaveInfoID = pkID;
		}

		public override void Init(DataRow row)
		{
			FillEntity(row);
			InitChildren();
		}

		public override void FillEntity(DataRow row)
		{
			pkLeaveInfoID = row["PKLeaveInfoID"] != DBNull.Value ? (Guid)row["PKLeaveInfoID"] : Guid.Empty;
			fkSubmitEmployeeID = row["FKSubmitEmployeeID"] != DBNull.Value ? (Guid)row["FKSubmitEmployeeID"] : Guid.Empty;
			fkReportManagerID = row["FKReportManagerID"] != DBNull.Value ? (Guid)row["FKReportManagerID"] : Guid.Empty;
			fkLeaveTypeID = row["FKLeaveTypeID"] != DBNull.Value ? (Guid)row["FKLeaveTypeID"] : Guid.Empty;
			status = row["Status"] != DBNull.Value ? (LeaveStatus)Enum.Parse(typeof(LeaveStatus), row["Status"].ToString()) : (LeaveStatus)Enum.Parse(typeof(LeaveStatus), "None");
			reason = row["Reason"] != DBNull.Value ? (String)row["Reason"] : null;
			firstStartTime = row["FirstStartTime"] != DBNull.Value ? (DateTime)row["FirstStartTime"] : DateTime.MinValue;
			lastEndTime = row["LastEndTime"] != DBNull.Value ? (DateTime)row["LastEndTime"] : DateTime.MinValue;
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
			using (TransactionScope ts = new TransactionScope())
			{
				List<SearchCondition> conditions = new List<SearchCondition>();
				conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKLeaveInfoID, PKLeaveInfoID.ToString(), SearchComparator.Equal, SearchType.SearchString));
				CommonDAL<LeaveInfo>.Update(this, conditions);

				foreach (TimeDurationInfo item in timeDurationList)
				{
					if (item.IsNew) item.FKLeaveInfoID = this.PKLeaveInfoID;
					item.Save();
				}

				//#region For transact unit test
				//throw new Exception();
				//#endregion

				if (this.PreStatus != LeaveStatus.Accepted && this.Status == LeaveStatus.Accepted
					|| this.status == LeaveStatus.Rejected && this.PreStatus == LeaveStatus.Accepted)
				{
					int year;
					double usedHours = GetDurationHours(timeDurationList, out year);

					if (usedHours != 0 && year != 0)
					{
						conditions.Clear();
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKEmployeeID, FKSubmitEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKLeaveTypeID, FKLeaveTypeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Year, year.ToString(), SearchComparator.Equal, SearchType.SearchNotString));
						EmployeeLeaveSummary leaveSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);
						if (null == leaveSummary) leaveSummary = EmployeeLeaveSummary.CreateEmployeeLeaveSummary(FKSubmitEmployeeID, FKLeaveTypeID, year);
						if (this.status == LeaveStatus.Rejected) usedHours = -usedHours;
						
						leaveSummary.UsedHours += usedHours;

						if (leaveSummary.UsedHours < 0)
							leaveSummary.UsedHours = 0; // used hours cannot be a negative number

						leaveSummary.Save();
					}
				}

				ts.Complete();
			}
		}

		protected override void Insert()
		{
			using (TransactionScope ts = new TransactionScope())
			{
				CommonDAL<LeaveInfo>.Insert(this);

				foreach (TimeDurationInfo item in timeDurationList)
				{
					item.FKLeaveInfoID = this.PKLeaveInfoID;
					item.Save();
				}

				//#region For transact unit test
				//throw new Exception();
				//#endregion

				// When the leave is accepted, update the employee leave summary.
				if (this.Status == LeaveStatus.Accepted)
				{
					int year;
					double usedHours = GetDurationHours(timeDurationList, out year);

					if (usedHours != 0 && year != 0)
					{
						List<SearchCondition> conditions = new List<SearchCondition>();
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKEmployeeID, FKSubmitEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKLeaveTypeID, FKLeaveTypeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Year, year.ToString(), SearchComparator.Equal, SearchType.SearchNotString));
						EmployeeLeaveSummary leaveSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);
						if (null == leaveSummary) leaveSummary = EmployeeLeaveSummary.CreateEmployeeLeaveSummary(FKSubmitEmployeeID, FKLeaveTypeID, year);
						leaveSummary.UsedHours += usedHours;
						leaveSummary.Save();
					}
				}

				ts.Complete();
			}
		}

		public override void Delete()
		{
			using (TransactionScope ts = new TransactionScope())
			{
				// If the leave record is accepted we should update the leave summary when we want remove the leave record.
				// This case only happened in Unit Test as delete method is not allowed to invoke in real business logic.
				if (this.Status == LeaveStatus.Accepted)
				{
					int year;
					double usedHours = GetDurationHours(timeDurationList, out year);

					if (usedHours != 0 && year != 0)
					{
						List<SearchCondition> conditions = new List<SearchCondition>();
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKEmployeeID, FKSubmitEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKLeaveTypeID, FKLeaveTypeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
						conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.Year, year.ToString(), SearchComparator.Equal, SearchType.SearchNotString));
						EmployeeLeaveSummary leaveSummary = CommonDAL<EmployeeLeaveSummary>.GetSingleObject(conditions);
						leaveSummary.UsedHours -= usedHours;
						leaveSummary.Save();
					}
				}

				foreach (TimeDurationInfo item in timeDurationList)
				{
					item.Delete();
				}

				CommonDAL<LeaveInfo>.Delete(this);

				ts.Complete();
			}
		}

		#endregion

		#region Private Methods

		private void InitChildren()
		{
			List<SearchCondition> conditions = new List<SearchCondition>();
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.FKLeaveInfoID, PKLeaveInfoID.ToString(), SearchComparator.Equal, SearchType.SearchString));
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.IsDeleted, Convert.ToString(0), SearchComparator.Equal, SearchType.SearchNotString));
			timeDurationList = CommonDAL<TimeDurationInfo>.GetObjects(conditions);

			conditions.Clear();
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKLeaveTypeID, FKLeaveTypeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
			leaveCategory = CommonDAL<LeaveType>.GetSingleObject(conditions);

			conditions.Clear();
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKEmployeeID, FKReportManagerID.ToString(), SearchComparator.Equal, SearchType.SearchString));
			manager = CommonDAL<Employee>.GetSingleObject(conditions);

			conditions.Clear();
			conditions.Add(SearchCondition.CreateSearchCondition(GlobalParams.PKEmployeeID, FKSubmitEmployeeID.ToString(), SearchComparator.Equal, SearchType.SearchString));
			submitter = CommonDAL<Employee>.GetSingleObject(conditions);
		}

		private double GetDurationHours(List<TimeDurationInfo> timeList, out int year)
		{
			double result = 0;
			year = 0;

			if (null != timeList && timeList.Count > 0)
			{
				year = timeList[0].StartTime.Year; // Since the timeList in one leave info must be in the same year, I could get the year from the first one.
				result = CommonMethods.ComputeHours(timeList);
			}

			return result;
		}

		#endregion

		#region Public Methods

		public void SetTimeDurationList(List<TimeDurationInfo> timeDurationList)
		{
			this.timeDurationList = timeDurationList;
		}

		#endregion

		#region Constructures

		private LeaveInfo() { timeDurationList = new List<TimeDurationInfo>(); }

		public LeaveInfo(int key) { timeDurationList = new List<TimeDurationInfo>(); }

		public static LeaveInfo CreateLeaveInfo(Guid fkSubmitEmployeeID, Guid fkReportManagerID, Guid fkLeaveTypeID, LeaveStatus status, string reason, String description, List<TimeDurationInfo> timeDurationList)
		{
			LeaveInfo leaveInfo = null;

			if (Guid.Empty != fkSubmitEmployeeID && Guid.Empty != fkReportManagerID && Guid.Empty != fkLeaveTypeID &&
				!string.IsNullOrEmpty(reason) && null != timeDurationList)
			{
				leaveInfo = new LeaveInfo();

				leaveInfo.FKSubmitEmployeeID = fkSubmitEmployeeID;
				leaveInfo.FKReportManagerID = fkReportManagerID;
				leaveInfo.FKLeaveTypeID = fkLeaveTypeID;
				leaveInfo.Status = status;
				leaveInfo.Reason = reason;
				leaveInfo.Description = description;
				leaveInfo.SetTimeDurationList(timeDurationList);

				DateTime firstStartTime = DateTime.MaxValue;
				DateTime lastEndTime = DateTime.MinValue;
				foreach (TimeDurationInfo item in timeDurationList)
				{
					if (item.StartTime < firstStartTime) firstStartTime = item.StartTime;
					if (item.EndTime > lastEndTime) lastEndTime = item.EndTime;
				}
				leaveInfo.FirstStartTime = firstStartTime;
				leaveInfo.LastEndTime = lastEndTime;
			}

			return leaveInfo;
		}

		#endregion
	}
}
