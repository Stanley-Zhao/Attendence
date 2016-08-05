using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CARSService.Email
{
	public enum EmailType
	{
		Register_ToEmployeeSelf,
		Register_ToAdminAndManager,
		ForgetPassword,
		ApplyLeave,
		ApproveLeave,
		RecallLeave,
		ReminderEmail
	}
}