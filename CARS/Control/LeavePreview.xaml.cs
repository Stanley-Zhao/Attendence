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
using CARS.SourceCode;
using CARS.CARSService;

namespace CARS.Control
{
	public partial class LeavePreview : ChildWindow
	{
		private User user;
		private LeaveItem leaveItem;
		private bool isClickingOKButton = false;

		public delegate void ApplyNewLeaveHandler(object sender, EventArgs e);
		public event ApplyNewLeaveHandler ApplyNewLeave;

		public LeavePreview()
			: this(null, null, "")
		{
			// nothing
		}

		public LeavePreview(User pUser, LeaveItem item, string warning)
		{
			InitializeComponent();

			ClientInstance.Get().ApplyLeaveCompleted += new EventHandler<ApplyLeaveCompletedEventArgs>(client_ApplyLeaveCompleted);

			SetLeaveValue(item);
			if (warning != "")
			{
				warningInfo.Height = double.NaN;
				warningInfo.Visibility = System.Windows.Visibility.Visible;
				warningInfo.Text = warning;
			}
			else
			{
				warningInfo.Height = 0;
				warningInfo.Visibility = System.Windows.Visibility.Collapsed;
			}

			user = pUser;
			leaveItem = item;
		}

		public LeavePreview(User pUser, LeaveItem item)
			: this(pUser, item, "")
		{
			// nothing
		}

		private void SetLeaveValue(LeaveItem item)
		{
			if (item != null)
			{
				baseApplyInfo.reason.Content = item.Reason;
				baseApplyInfo.description.Text = item.Description;
				baseApplyInfo.leaveType.Content = item.TypeValue;

				foreach (DateRecords dr in item.List)
				{
					baseApplyInfo.datesList.Items.Add(dr.Record);
				}

				baseApplyInfo.note.Content = "Asked: " + item.Hours.ToString() + " Hours";
			}
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (!isClickingOKButton)
			{
				isClickingOKButton = true;
				ClientInstance.Get().ApplyLeaveAsync(user.Employee.PKEmployeeID.ToString(), leaveItem.Reason, leaveItem.Type.Name, leaveItem.Description, leaveItem.LeaveInfo.TimeDurationInfoList);				
			}
		}

		private void client_ApplyLeaveCompleted(object sender, ApplyLeaveCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Apply Leave Completed");
			if (ErrorHandler.Handle(e.Error))
				return;			
			this.ApplyNewLeave(this, new EventArgs());
			this.Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}

