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
using System.Collections.ObjectModel;
using System.Collections;

namespace CARS.Control
{
	public partial class ApplyControl : UserControl
	{
		public delegate void UpdateListEventHandler(object sender, EventArgs e);
		public event UpdateListEventHandler UpdateListEvent;

		private int askedHours = 0;
		TimePicker startTime;
		TimePicker endTime;
		private LeaveItem leaveItem = new LeaveItem();
		private bool isViewMode = false;
		private User mUser;
		private User mUserHoursInfo;
		private User mUserRunAs;
		private CARSPage mPage;
		private Button rejectButton;
		private string sickLeaveString = "{0} had used {1} hours sick leave already";
		private string annualLeaveString = "{0} had earned {1} hours annual leave, left {2} hours";
		private int usedAnnualHours = 0;
		private int earnedAnnualHours = 0;
		private int usedSickHours = 0;
		private DateTime currentFrozenDate = DateTime.Now;

		public bool IsViewMode
		{
			get { return isViewMode; }
			set
			{
				if (CurrentPage == CARSPage.ApplyLeave)
				{
					if (!value) // not view mode
					{
						Clean();
						actButton.Content = "Apply";
						actButton.IsEnabled = false;
						startTime.IsEnabled = endTime.IsEnabled = addToListButton.IsEnabled = removeFromListButton.IsEnabled = true;
						leaveItem = new LeaveItem();

						this.recallButton.Visibility = System.Windows.Visibility.Collapsed;
					}
					else
					{
						// view mode? if current user is same as application's applier. Give him/her a chance can recall this application
						if (mUser.PKEmployeeID == leaveItem.LeaveInfo.FKSubmitEmployeeID 
							&& leaveItem.Status == LeaveStatus.Applying)
						{
							this.recallButton.Visibility = System.Windows.Visibility.Visible;
						}
						actButton.Content = "New Apply";
					}
					isViewMode = value;
				}
			}
		}

		private void Clean()
		{
			Clean(false);
		}

		public void Clean(bool full)
		{
			reason.Text = string.Empty;
			leaveType.SelectedIndex = -1;
			description.Text = string.Empty;
			datesList.ItemsSource = null;
			if (leaveItem != null)
			{
				leaveItem = new LeaveItem();
			}
			AskedHours = 0;
			startTime.Clean();
			endTime.Clean();
			if (CurrentPage == CARSPage.ApproveLeave)
			{
				actButton.IsEnabled = false;
				if (rejectButton != null)
					rejectButton.IsEnabled = false;
			}

			if (full)
			{
				annualLeaveLabel.Content = string.Empty;
				sickLeaveLabel.Content = string.Empty;
			}
		}

		public void LockControl(bool lockIt)
		{
			reason.IsReadOnly = description.IsReadOnly = lockIt;
			startTime.IsEnabled = endTime.IsEnabled = leaveType.IsEnabled = !lockIt;
		}

		public void SetLeaveItem(LeaveItem item)
		{
			if (item == null)
			{
				Clean(true);
				return;
			}

			leaveItem = item; // set current leave item
			if (mPage == CARSPage.ApproveLeave || mPage == CARSPage.LeaveHistory)
				mUserHoursInfo = new User(item.LeaveInfo.Submitter);
			else
				mUserHoursInfo = mUser;

			IsViewMode = true;
			actButton.IsEnabled = true;
			if (rejectButton != null)
				rejectButton.IsEnabled = true;

			reason.Text = item.Reason;
			description.Text = item.Description;
			for (int i = 0; i < leaveType.Items.Count; i++)
			{
				LeaveTypeValue lt = (LeaveTypeValue)leaveType.Items[i];
				if (lt.TypeValue.Name == leaveItem.LeaveInfo.Type.Name)
				{
					leaveType.SelectedIndex = i;
					break;
				}
			}
			startTime.SelectDateTime = item.LeaveInfo.FirstStartTime;
			endTime.SelectDateTime = item.LeaveInfo.LastEndTime;
			datesList.ItemsSource = null;
			datesList.ItemsSource = item.List;

			AskedHours = item.Hours;

			// disable some buttons
			startTime.IsEnabled = endTime.IsEnabled = addToListButton.IsEnabled = removeFromListButton.IsEnabled = false;

			GetHoursInfo();
		}

		private void GetHoursInfo()
		{
			ClientInstance.Get().GetAnnualLeaveEarnedHoursAsync(mUserHoursInfo.Employee.PKEmployeeID.ToString());
			ClientInstance.Get().GetAnnualLeaveUsedHoursAsync(mUserHoursInfo.Employee.PKEmployeeID.ToString());
			ClientInstance.Get().GetSickLeaveUsedHoursAsync(mUserHoursInfo.Employee.PKEmployeeID.ToString());
		}

		private void client_GetSickLeaveUsedHoursCompleted(object sender, GetSickLeaveUsedHoursCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Get Sick Leave Used Hours Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			SetHoursValue(ref usedSickHours, e.Result);
		}

		private void SetHoursValue(ref int intValue, int p)
		{
			intValue = p;

			// update hours info
			int leftAnnualHours = earnedAnnualHours - usedAnnualHours;
			string host = mUser.Employee.PKEmployeeID == mUserHoursInfo.Employee.PKEmployeeID ? "You" : mUserHoursInfo.Employee.FirstName;
			annualLeaveLabel.Content = string.Format(annualLeaveString, host, earnedAnnualHours, leftAnnualHours);
			sickLeaveLabel.Content = string.Format(sickLeaveString, host, usedSickHours);

			if (leftAnnualHours < 0)
			{
				annualLeaveLabel.FontWeight = FontWeights.Bold;
				annualLeaveLabel.Foreground = new SolidColorBrush(Colors.Yellow);
			}
			else
			{
				annualLeaveLabel.FontWeight = FontWeights.Normal;
				annualLeaveLabel.Foreground = new SolidColorBrush(Colors.Black);
			}

			if (usedSickHours > 80)
			{
				sickLeaveLabel.FontWeight = FontWeights.Bold;
				sickLeaveLabel.Foreground = new SolidColorBrush(Colors.Yellow);
			}
			else
			{
				sickLeaveLabel.FontWeight = FontWeights.Normal;
				sickLeaveLabel.Foreground = new SolidColorBrush(Colors.Black);
			}
		}

		private void client_GetAnnualLeaveUsedHoursCompleted(object sender, GetAnnualLeaveUsedHoursCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Get Annual Leave Used Hours Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			SetHoursValue(ref usedAnnualHours, e.Result);
		}

		private void client_GetAnnualLeaveEarnedHoursCompleted(object sender, GetAnnualLeaveEarnedHoursCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Get Annual Leave Earned Hours Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			SetHoursValue(ref earnedAnnualHours, e.Result);
		}

		public int AskedHours
		{
			get { return askedHours; }
			set
			{
				askedHours = value;
				askedHoursLabel.Content = "Asked " + askedHours.ToString("0") + (askedHours == 1 ? " Hour" : " Hours");
			}
		}

		public User UserRunAs
		{
			get { return mUserRunAs; }
			set { mUserRunAs = value; }
		}

		public DateTime CurrentFrozenDate
		{
			get { return currentFrozenDate; }
		}

		public ApplyControl() : this(CARSPage.PersonalInfo, null) { }

		public ApplyControl(CARSPage page, User pUser)
		{
			InitializeComponent();

			if (pUser != null)
			{
				mUser = mUserRunAs = pUser;
			}

			mPage = page;
			this.recallButton.Visibility = System.Windows.Visibility.Collapsed; // by default, hide this button

			ClientInstance.Get().GetAnnualLeaveEarnedHoursCompleted += new EventHandler<GetAnnualLeaveEarnedHoursCompletedEventArgs>(client_GetAnnualLeaveEarnedHoursCompleted);
			ClientInstance.Get().GetAnnualLeaveUsedHoursCompleted += new EventHandler<GetAnnualLeaveUsedHoursCompletedEventArgs>(client_GetAnnualLeaveUsedHoursCompleted);
			ClientInstance.Get().GetSickLeaveUsedHoursCompleted += new EventHandler<GetSickLeaveUsedHoursCompletedEventArgs>(client_GetSickLeaveUsedHoursCompleted);
			ClientInstance.Get().ApproveLeaveCompleted += new EventHandler<ApproveLeaveCompletedEventArgs>(client_ApproveLeaveCompleted);
			ClientInstance.Get().GetLeaveTypesCompleted += new EventHandler<GetLeaveTypesCompletedEventArgs>(client_GetLeaveTypesCompleted);
			ClientInstance.Get().GetFrozenDateCompleted += new EventHandler<GetFrozenDateCompletedEventArgs>(ApplyControl_GetFrozenDateCompleted);
			ClientInstance.Get().RecallLeaveCompleted += ApplyControl_RecallLeaveCompleted;

			ClientInstance.Get().GetLeaveTypesAsync();
			ClientInstance.Get().GetFrozenDateAsync();
		}

		void ApplyControl_RecallLeaveCompleted(object sender, RecallLeaveCompletedEventArgs e)
		{
			try
			{
				if (e.Result)
				{
					Message.Information("Done");

					// refresh list and page
					IsViewMode = false; // clean content of left area.
					this.UpdateListEvent(sender, e); // refresh list data
				}
			}
			catch
			{
				Message.Error(e.Error.Message);
			}
		}

		void ApplyControl_GetFrozenDateCompleted(object sender, GetFrozenDateCompletedEventArgs e)
		{
			currentFrozenDate = e.Result;
		}

		private void client_GetLeaveTypesCompleted(object sender, GetLeaveTypesCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Get LeaveTypes Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			List<LeaveType> types = new List<LeaveType>();
			if (e.Result != null)
				types = ((ObservableCollection<LeaveType>)e.Result).ToList<LeaveType>();
			else
				Logger.Instance().Log(MessageType.Error, "Result is NULL");

			foreach (LeaveType type in types)
			{
				leaveType.Items.Add(new LeaveTypeValue(type));
			}

			startTime = new TimePicker();
			startTime.Name = "startTime";
			startTime.DateChangeEvent += new TimePicker.DateChangeHandler(startTime_DateChangeEvent);
			startTime.TimeChangeEvent += new TimePicker.TimeChangeHandler(startTime_TimeChangeEvent);
			startTime.Type = SourceCode.StartEnd.Start;

			endTime = new TimePicker();
			endTime.Name = "endTime";
			endTime.DateChangeEvent += new TimePicker.DateChangeHandler(endTime_DateChangeEvent);
			endTime.TimeChangeEvent += new TimePicker.TimeChangeHandler(endTime_TimeChangeEvent);
			endTime.Type = SourceCode.StartEnd.End;

			Grid.SetColumn(startTime, 1);
			Grid.SetRow(startTime, 3);
			Grid.SetColumn(endTime, 1);
			Grid.SetRow(endTime, 4);

			basicGrid.Children.Add(startTime);
			basicGrid.Children.Add(endTime);

			RefreshUI();
		}

		void endTime_TimeChangeEvent(object sender, SelectionChangedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;

			if (startTime != null && endTime != null)
				if (startTime.Date != endTime.Date && endTime.Time == 13)
					endTime.Time = 9;
		}

		void endTime_DateChangeEvent(object sender, SelectionChangedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;

			if (startTime != null && endTime != null)
				if (startTime.date.SelectedDate.HasValue && endTime.date.SelectedDate.HasValue && startTime.Date > endTime.Date)
				{
					if (startTime.Time != endTime.Time)
						startTime.Date = endTime.Date;
					else
						startTime.Date = endTime.Date.AddDays(-1);
				}
		}

		void startTime_TimeChangeEvent(object sender, SelectionChangedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;

			if (startTime != null && endTime != null)
				if (startTime.Date != endTime.Date && startTime.Time == 13)
					endTime.Time = 17;
		}

		void startTime_DateChangeEvent(object sender, SelectionChangedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;

			if (startTime != null && endTime != null)
				if (startTime.date.SelectedDate.HasValue && endTime.date.SelectedDate.HasValue && startTime.Date > endTime.Date)
				{
					if (startTime.Time != endTime.Time)
						endTime.Date = startTime.Date;
					else
						endTime.Date = startTime.Date.AddDays(1);
				}
		}

		private void GetFirstLastTime()
		{
			if (leaveItem != null)
			{
				foreach (DateRecords dr in leaveItem.List)
				{
					if (leaveItem.Start > dr.StartTime)
						leaveItem.Start = dr.StartTime;
					if (leaveItem.End < dr.EndTime)
						leaveItem.End = dr.EndTime;
				}
			}
		}

		private void addToListButton_Click(object sender, RoutedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;

			if (startTime.HasValue && endTime.HasValue)
			{
				if (startTime.SelectDateTime >= endTime.SelectDateTime)
				{
					ShowError("End time must be greater than start time.");
					return;
				}
				if (startTime.SelectDateTime.Year != endTime.SelectDateTime.Year)
				{
					// show warning of leave is only allowed in one year.					
					ShowError("Please make sure your leave is in same year.");
					return;
				}
				else if (startTime.SelectDateTime < currentFrozenDate) // show warning of leave is before frozen date
				{
					ShowError("You cannot apply a leave which is before: " + currentFrozenDate.ToString("yyyy-MM-dd"));
					return;
				}

				int hours = CommonMethods.ComputeHours(startTime.SelectDateTime, endTime.SelectDateTime);
				string value = startTime.SelectDateTime.ToString("yyyy/MM/dd HH:00") + " - " + endTime.SelectDateTime.ToString("yyyy/MM/dd HH:00");

				DateRecords dr = new DateRecords();
				dr.Hours = hours;
				dr.Record = value;
				dr.StartTime = startTime.SelectDateTime;
				dr.EndTime = endTime.SelectDateTime;

				leaveItem.List.Add(dr);

				AskedHours += hours;

				actButton.IsEnabled = true;

				leaveItem.List.Sort(new DateRecordsComparision());
				datesList.ItemsSource = null;
				datesList.ItemsSource = leaveItem.List;
			}
		}

		private void removeFromListButton_Click(object sender, RoutedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;

			DateRecords item = (DateRecords)datesList.SelectedItem;

			for (int i = 0; i < leaveItem.List.Count; i++)
			{
				if (item == leaveItem.List[i])
				{
					AskedHours -= leaveItem.List[i].Hours;
					leaveItem.List.RemoveAt(i);
					break;
				}
			}

			datesList.ItemsSource = null;
			datesList.ItemsSource = leaveItem.List;

			if (leaveItem.List.Count == 0)
			{
				actButton.IsEnabled = false;
			}
		}

		private void actButton_Click(object sender, RoutedEventArgs e)
		{
			if (!actButton.IsEnabled)
			{
				return; // action is not finished
			}

			actButton.IsEnabled = false;

			if (CurrentPage == CARSPage.ApplyLeave)
			{
				if (!IsViewMode) // if it is view mode - apply button
				{
					// show warning of leave reason missing
					if (reason.Text == "")
					{
						ShowError("Please input leave reason.");
						return;
					}

					// show warning of leave type missing
					if (leaveType.SelectedIndex == -1)
					{
						ShowError("Please select leave type.");
						return;
					}

					leaveItem.LeaveInfo.TimeDurationInfoList.Clear();
					leaveItem.LeaveInfo.FirstStartTime = DateTime.MaxValue;
					leaveItem.LeaveInfo.LastEndTime = DateTime.MinValue;

					int currentYear = 0;
					foreach (DateRecords dr in leaveItem.List)
					{
						if (currentYear == 0)
							currentYear = dr.StartTime.Year;

						if (currentYear != dr.StartTime.Year || currentYear != dr.EndTime.Year)
						{
							// show warning of leave is only allowed in one year.					
							ShowError("Please make sure your leave is in same year.");
							return;
						}

						if (leaveItem.LeaveInfo.FirstStartTime > dr.StartTime)
							leaveItem.LeaveInfo.FirstStartTime = dr.StartTime;

						if (leaveItem.LeaveInfo.LastEndTime < dr.EndTime)
							leaveItem.LeaveInfo.LastEndTime = dr.EndTime;

						leaveItem.LeaveInfo.TimeDurationInfoList.Add(dr.ToTimeDurationInfo());
					}

					leaveItem.Reason = reason.Text;
					if (leaveType.SelectedIndex != -1)
					{
						leaveItem.Type = ((LeaveTypeValue)leaveType.SelectedItem).TypeValue;
					}
					leaveItem.Description = description.Text;
					leaveItem.Hours = (int)askedHours;

					string warningInfo = string.Empty;
					int leftAnnualLeave = earnedAnnualHours - usedAnnualHours;
					if (leaveItem.Hours > leftAnnualLeave && leaveItem.LeaveInfo.Type.Name == "Annual")
					{
						warningInfo = string.Format("You asked {0} hours annual leave, but you only have {1} hours available now. Click OK button to proceed if you insist, but talk to your manager immediately because you may not be paid for the {2} hours you have not earned.", askedHours, leftAnnualLeave, askedHours - leftAnnualLeave);
					}
					else if (leaveItem.Hours > (80 - usedSickHours) && leaveItem.LeaveInfo.Type.Name == "Sick" && leftAnnualLeave > 0)
					{
						warningInfo = string.Format("Your sick leave will be over than 80 hours. You can ask annual leave to instead. Otherwise you will not be paid fully for {0} hours leaving.", askedHours + usedSickHours - 80);
					}
					else if (leaveItem.Hours > (80 - usedSickHours) && leaveItem.LeaveInfo.Type.Name == "Sick" && leftAnnualLeave <= 0)
					{
						warningInfo = string.Format("Your sick leave will be over than 80 hours. Click OK button to ask sick leave insistently, you will not be paid fully for {0} hours leaving.", askedHours + usedSickHours - 80);
					}

					LeavePreview lp = new LeavePreview(mUser, leaveItem, warningInfo);
					lp.ApplyNewLeave += new LeavePreview.ApplyNewLeaveHandler(lp_ApplyNewLeave);
					lp.Show();
					noteLabel.Visibility = System.Windows.Visibility.Collapsed; // hide note information if a leave is created successfully
				}
				else
				{
					IsViewMode = false; // make it as apply mode
				}

				actButton.IsEnabled = true;
			}
			else // approve button
			{
				if (rejectButton != null)
					rejectButton.IsEnabled = false;

				// check if this application is before last frozen date
				if (leaveItem.Start < currentFrozenDate)
				{
					Message.Information("You cannot approve this application that its Start Time is before: " + currentFrozenDate.ToString("yyyy-MM-dd"));
				}
				else if (leaveItem.Status == LeaveStatus.Accepted)
				{
					Message.Information("This application is already approved.");
				}
				else
				{
					ClientInstance.Get().ApproveLeaveAsync(mUser.Employee.PKEmployeeID.ToString(), leaveItem.LeaveInfo.PKLeaveInfoID.ToString(), LeaveStatus.Accepted);
				}
			}
		}

		private void client_ApproveLeaveCompleted(object sender, ApproveLeaveCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Approve Leave Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			Message.Information("Done! A notification email will be sent to your team member. You are in the CC list.");
			this.UpdateListEvent(sender, e); // refresh list data
			GetHoursInfo();

			actButton.IsEnabled = true;

			if (rejectButton != null)
			{
				rejectButton.IsEnabled = true;
			}
		}

		void lp_ApplyNewLeave(object sender, EventArgs e)
		{
			IsViewMode = false; // clean content of left area.
			this.UpdateListEvent(sender, e); // refresh list data
		}

		private void ShowError(string value)
		{
			noteLabel.Content = value;
			noteLabel.Visibility = System.Windows.Visibility.Visible;
		}

		public CARSPage CurrentPage
		{
			get { return mPage; }
			set
			{
				mPage = value;
				RefreshUI();
			}
		}

		private void RefreshUI()
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;

			if (mPage == CARSPage.ApproveLeave)
			{
				Clean();
				LockControl(true);
				addToListButton.Visibility = removeFromListButton.Visibility = System.Windows.Visibility.Collapsed;
				actButton.Content = "Approve";
				actButton.IsEnabled = false;
				actButton.Visibility = System.Windows.Visibility.Visible;
				if (rejectButton == null)
				{
					rejectButton = new Button();
					rejectButton.Content = "Reject";
					rejectButton.Margin = new Thickness(20, 0, 0, 0);
					rejectButton.Width = 80;
					rejectButton.Height = 23;
					rejectButton.IsEnabled = false;
					rejectButton.Click += new RoutedEventHandler(rejectButton_Click);
				}
				if (!buttonBar.Children.Contains(rejectButton))
					buttonBar.Children.Add(rejectButton);
				annualLeaveLabel.Content = sickLeaveLabel.Content = string.Empty;
			}
			else if (mPage == CARSPage.ApplyLeave)
			{
				Clean();
				LockControl(false);
				mUserHoursInfo = new User(mUser.Employee);
				GetHoursInfo();
				actButton.Visibility = addToListButton.Visibility = removeFromListButton.Visibility = System.Windows.Visibility.Visible;
				addToListButton.IsEnabled = removeFromListButton.IsEnabled = true;
				actButton.Content = "Apply";
				actButton.IsEnabled = false;
				if (buttonBar.Children.Contains(rejectButton))
					buttonBar.Children.Remove(rejectButton);
			}
			else if (mPage == CARSPage.LeaveHistory)
			{
				Clean(true);
				LockControl(true);
				actButton.Visibility = addToListButton.Visibility = removeFromListButton.Visibility = System.Windows.Visibility.Collapsed;
				if (buttonBar.Children.Contains(rejectButton))
					buttonBar.Children.Remove(rejectButton);
				annualLeaveLabel.Content = sickLeaveLabel.Content = string.Empty;
			}
		}

		void rejectButton_Click(object sender, RoutedEventArgs e)
		{
			if (leaveItem.Start < currentFrozenDate && leaveItem.Status != LeaveStatus.Applying)
			{
				Message.Information("You cannot reject this application that its Start Time is before: " + currentFrozenDate.ToString("yyyy-MM-dd"));
			}
			else if (leaveItem.Status == LeaveStatus.Rejected)
			{
				Message.Information("This application is already rejected.");
			}
			else
			{
				rejectButton.IsEnabled = actButton.IsEnabled = false;

				RejectReason rr = new RejectReason(mUser, new List<LeaveItem>() { leaveItem });
				rr.RejectedLeaveEvent += new EventHandler(rr_RejectedLeaveEvent);
				rr.Show();
			}
		}

		void rr_RejectedLeaveEvent(object sender, EventArgs e)
		{
			this.UpdateListEvent(sender, e);
			GetHoursInfo();
			rejectButton.IsEnabled = actButton.IsEnabled = true;
		}

		public void SetRunAsUser(User user)
		{
			mUserRunAs = user;
		}

		private void reason_TextChanged(object sender, TextChangedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void leaveType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void description_TextChanged(object sender, TextChangedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void recallButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (MessageBoxResult.OK == MessageBox.Show("Sure to recall this leave application?", "Confirm", MessageBoxButton.OKCancel))
				{
					ClientInstance.Get().RecallLeaveAsync(leaveItem.LeaveInfo.PKLeaveInfoID.ToString(), mUser.PKEmployeeID.ToString());
				}
			}
			catch (Exception ex)
			{
				Message.Error(ex.Message);
			}
		}
	}
}
