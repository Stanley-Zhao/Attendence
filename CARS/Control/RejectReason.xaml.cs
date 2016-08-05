using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CARS.CARSService;
using CARS.SourceCode;

namespace CARS.Control
{
	public partial class RejectReason : ChildWindow
	{
		public event EventHandler RejectedLeaveEvent;

		private User mUser;
		private List<LeaveItem> leaves;
		private int count = 0;

		public RejectReason(User manager, List<LeaveItem> items)
		{
			InitializeComponent();

			mUser = manager;
			leaves = items;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			CARSServiceClient client = CARSServiceClientFactory.CreateCARSServiceClient();// leave this line, we cannot use a public static object here. Because this reject reason window will be created may times.
			client.RejectLeaveCompleted += new EventHandler<RejectLeaveCompletedEventArgs>(client_RejectLeaveCompleted);
			string reasonValue = reason.Text.Replace("&", "&amp;")
				.Replace(" ", "&nbsp;")
				.Replace("\"", "&quot;")
				.Replace(">", "&gt;")
				.Replace("<", "&lt;");
			foreach (LeaveItem leave in leaves)
			{
				client.RejectLeaveAsync(mUser.Employee.PKEmployeeID.ToString(), leave.LeaveInfo.PKLeaveInfoID.ToString(), LeaveStatus.Rejected, reasonValue);
				count++;
			}

			this.DialogResult = true;
		}

		private void client_RejectLeaveCompleted(object sender, RejectLeaveCompletedEventArgs e)
		{
			count--;
			if (count == 0 && RejectedLeaveEvent != null)
			{
				Logger.Instance().Log(MessageType.Information, "Reject Leave Completed");
				if (ErrorHandler.Handle(e.Error))
					return;

				RejectedLeaveEvent(sender, e);
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}

