using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CARS.Backend.Common
{
    public class GlobalParams
    {
        public const string StartLogin = "Start login";
        public const string EndLogin = "End login";
        public const string StartMail = "Start send email";
        public const string EndMail = "End send email";
        public const string AdventMail = "@Advent.com";
        public const string MailError = "Failed to send email to the new employee since register failed or the email address is illeagal.";
        public const string StartAddEmpl = "Start add employee";
        public const string EndAddEmpl = "End add employee";
        public const string StartUpdateEmpl = "Start update employee";
        public const string EndUpdateEmpl = "End update employee";
        public const string StartGetPersonalLeaves = "Start get my leaves";
        public const string EndGetPersonalLeaves = "End get my leaves";
        public const string StartGetTeamLeaves = "Start get my team leaves";
        public const string EndGetTeamLeaves = "End get my team leaves";
        public const string StartGetManager = "Start get managers";
        public const string EndGetManager = "End get managers";
		public const string StartGetAllEmployees = "Start get all employees";
		public const string EndGetAllEmployees = "End get all employees";
        public const string StartApplyLeave = "Start apply leave";
        public const string EndApplyLeave = "End apply leave";
		public const string StartRecallLeave = "Start recall leave";
		public const string EndRecallLeave = "End recall leave";
        public const string StartApproveLeave = "Start approve leave";
        public const string EndApproveLeave = "End approve leave";
		public const string StartRejectLeave = "Start reject Leave";
		public const string EndRejectLeave = "End reject Leave";
		public const string StartFindLeaves = "Start find leaves";
		public const string EndFindLeaves = "End find leaves";
        public const string StartForgetPwd = "Start forget password";
        public const string EndForgetPwd = "End forget password";
        public const string PwdError = "Failed to send password by email since the employee doesn't exist or the email is illegal";
        public const string SelectSupervisorError = "Please do not set youself as your supervisor";
        
        public const string StartGetAnnualLeaveEarnedHours = "Start get annual leave earned hours";
        public const string EndGetAnnaulLeaveEarnedHours = "End get annual leave earned hours";
        public const string StartGetUsedAnnualLeaveHours = "Start get annual leave used hours";
        public const string EndGetUsedAnnualLeaveHours = "End get annual leave used hours";
        public const string AnnaulLeave = "Annual";

        public const string StartGetSickLeaveUsedHours = "Start get sick leave used hours";
        public const string EndGetSickLeaveUsedHours = "End get sick leave used hours";
        public const string SickLeave = "Sick";

        public const string StartGetLeaveTypes = "Start get leave types";
        public const string EndGetLeaveTypes = "End get leave types";

        public const string Advent = "Advent";
        public const string Gencos = "Gencos";
        public const string Email = "Email";
        public const string Password = "Password";
        public const string DefaultPwd = "1234";
        public const string Name = "Name";
        public const string IsActive = "IsActive";
        public const string IsDeleted = "IsDeleted";      

        public const string PKRoleID = "PKRoleID";
        public const string PKEmployeeID = "PKEmployeeID";
        public const string PKLeaveInfoID = "PKLeaveInfoID";
        public const string PKLeaveTypeID = "PKLeaveTypeID";
        public const string PKTDInfoID = "PKTDInfoID";
        public const string PKELSID = "PKELSID";
        public const string PKReportPeriodID = "PKReportPeriodID";
		public const string FirstStartTime = "FirstStartTime";
		public const string LastEndTime = "LastEndTime";
        
        public const string FKRoleID = "FKRoleID"; 
        public const string FKEmployeeID = "FKEmployeeID";
        public const string FKLeaveInfoID = "FKLeaveInfoID";
        public const string FKLeaveTypeID = "FKLeaveTypeID";
        public const string FKReportManagerID = "FKReportManagerID";
        public const string FKSubmitEmployeeID = "FKSubmitEmployeeID";
        public const string Status = "Status";
        public const string Year = "Year";
        
        public const string SelectTimeTokenQuery = "select TimeToken from {0} {1};";
        public const string UpdateQuery = "update {0} set {1} {2};";
        public const string UpdateTimeTokenClause = TimeTokenParameter + " = @" + TimeTokenParameter;
        public const string TimeTokenParameter = "TimeToken";
        public const string SelectQuery = "select * from {0} ";
        public const string InsertQuery = "insert into {0} ({1}) values ({2});";
        public const string DeleteQuery = "delete from {0} where {1} = '{2}'";
        public const string ConnNodeName = "ConnectString";
        public const string FrozenDayNodeName = "FrozenDay";
	}
}
