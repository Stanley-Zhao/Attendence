using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CARS.Backend.Entity;
using CARS.Backend.Common;

namespace CARSService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface ICARSService
	{
		[OperationContract]
        Employee Login(string email, string password);

        [OperationContract]
        Employee AddEmployee(string email, string firstName, string middleName, string lastName, string legalName, Sex gender, float serviceYear, DateTime dateOfHire, string supervisorEmail, string costCenterCode, int employeeNum);

        [OperationContract]
        Employee UpdateEmployee(string employeeID, string email, string firstName, string middleName, string lastName, string legalName, Sex gender, float serviceYear, DateTime dateOfHire, string supervisorEmail, string password, string phone, string mobile, bool isAdmin, bool isActive, bool isManager, string costCenterCode, int employeeNum);

        [OperationContract]
		List<Employee> GetManagers();

        [OperationContract]
        LeaveInfo ApplyLeave(string employeeID, string reason, string leaveType, string description, List<TimeDurationInfo> durationList);

        [OperationContract]
        List<LeaveInfo> GetMyLeaves(string employeeID);

        [OperationContract]
        List<LeaveInfo> GetMyTeamLeaves(string employeeID, bool onlyApplyingRecords);

        [OperationContract]
		LeaveInfo ApproveLeave(string managerID, string leaveID, LeaveStatus status);

		[OperationContract]
		LeaveInfo RejectLeave(string managerID, string leaveID, LeaveStatus status, string reason);

        [OperationContract]
        int GetAnnualLeaveEarnedHours(string employeeID);

        [OperationContract]
        int GetAnnualLeaveUsedHours(string employeeID);

        [OperationContract]
        int GetSickLeaveUsedHours(string employeeID);

		[OperationContract]
		bool ForgetPassword(string email);

        [OperationContract]
        List<LeaveType> GetLeaveTypes();

		[OperationContract]
        List<LeaveInfo> FindLeaves(string supervisors, string applicants, string leaveTypeId, string leaveStatus, DateTime start, DateTime end, string supervisorIDs);

		[OperationContract]
        List<Employee> GetAllEmployees();

        [OperationContract]
        List<ReportPeriod> GetReportPeriods();

        [OperationContract]
        bool UpdateReportPeriods(List<ReportPeriod> reportPeriods);

		[OperationContract]
		string GetVersion();

		[OperationContract]
		DateTime GetFrozenDate();

		[OperationContract]
		string GetAnnualLeaveReportForAdminAddress();

		[OperationContract]
		string GetSickLeaveReportForAdminAddress();

		[OperationContract]
		string GetAnnualLeaveReportForSupervisorAddress();

		[OperationContract]
		string GetSickLeaveReportForSupervisorAddress();

		[OperationContract]
		bool CheckSupervisorValidation(string employeeID, string supervisorID);

		[OperationContract]
		string GetUserManualAddress();

		[OperationContract]
		string GetReportBugAddress();

		[OperationContract]
		bool RecallLeave(string pkleaveInfoID, string employeeID);
	}
}
