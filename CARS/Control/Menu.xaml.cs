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

namespace CARS.Control
{
	public partial class Menu : UserControl
	{
		#region Click Event
		public event EventHandler ClickApplyLeaveButton;
		public event EventHandler ClickPersonalInfoButton;
		public event EventHandler ClickApproveLeaveButton;
		public event EventHandler ClickEmployeeInfoButton;
		public event EventHandler ClickVacationRulesButton;
		public event EventHandler ClickLeaveReportButton;
		public event EventHandler ClickLeaveHistoryButton;
		#endregion

		#region Members
		private bool isOpen = false;
		private static readonly Size DEFAULT_SIZE = new Size(155, 40);
		#endregion

		#region Constructor
		public Menu()
			: this(null)
		{
		}

		public Menu(User user)
		{
			InitializeComponent();
			// blue
			CARSButton applyLeaveButton = new CARSButton("applyLeaveButton", "Apply Leave", Colors.White, ColorUtil.BrightBlue, ColorUtil.SlateGary, DEFAULT_SIZE, System.Windows.HorizontalAlignment.Left);
			applyLeaveButton.BorderColor = new SolidColorBrush(Colors.White);
			applyLeaveButton.Click += new MouseButtonEventHandler(applyLeaveButton_Click);
			buttonList.Children.Add(applyLeaveButton);

			// green
			CARSButton personalInfoButton = new CARSButton("personalInfoButton", "Personal Information", Colors.White, ColorUtil.BrightGreen, ColorUtil.SlateGary, DEFAULT_SIZE, System.Windows.HorizontalAlignment.Left);
			personalInfoButton.BorderColor = new SolidColorBrush(Colors.White);
			personalInfoButton.Click += new MouseButtonEventHandler(personalInfoButton_Click);
			buttonList.Children.Add(personalInfoButton);

#if DEBUG
			if (user == null || user.Type == UserType.Manager || user.Type == UserType.ManagerAndAdmin)
#else
            if (user.Type == UserType.Manager || user.Type == UserType.ManagerAndAdmin)
#endif
			{
				// red
				CARSButton approveLeaveButton = new CARSButton("approveLeaveButton", "Approve Leave", Colors.White, ColorUtil.BrightRed, ColorUtil.SlateGary, DEFAULT_SIZE, System.Windows.HorizontalAlignment.Left);
				approveLeaveButton.BorderColor = new SolidColorBrush(Colors.White);
				approveLeaveButton.Click += new MouseButtonEventHandler(approveButton_Click);
				buttonList.Children.Add(approveLeaveButton);
			}

			// green (use blue now, not for now)
			//CARSButton vacationRulesButton = new CARSButton("vacationRulesButton", "Vacation Rules", Colors.White, ColorUtil.BrightBlue, ColorUtil.SlateGary, DEFAULT_SIZE, System.Windows.HorizontalAlignment.Left);
			//vacationRulesButton.BorderColor = new SolidColorBrush(Colors.White);
			//vacationRulesButton.Click += new MouseButtonEventHandler(configButton_Click);
			//buttonList.Children.Add(vacationRulesButton);

#if DEBUG
			if (user == null || user.Type == UserType.Administrator || user.Type == UserType.ManagerAndAdmin)
#else
            if (user.Type == UserType.Administrator || user.Type == UserType.ManagerAndAdmin)
#endif
			{
				// green
				CARSButton employeeInfoButton = new CARSButton("employeeInfoButton", "Employee Information", Colors.White, ColorUtil.BrightGreen, ColorUtil.SlateGary, DEFAULT_SIZE, System.Windows.HorizontalAlignment.Left);
				employeeInfoButton.BorderColor = new SolidColorBrush(Colors.White);
				employeeInfoButton.Click += new MouseButtonEventHandler(employeeInfoButton_Click);
				buttonList.Children.Add(employeeInfoButton);
			}

#if DEBUG
			if (user == null || user.Type == UserType.Administrator || user.Type == UserType.ManagerAndAdmin || user.Type == UserType.Manager)
#else
            if (user.Type == UserType.Administrator || user.Type == UserType.ManagerAndAdmin || user.Type == UserType.Manager)
#endif
			{
				// yellow
				CARSButton leaveReportButton = new CARSButton("leaveReportButton", "Leave Report", Colors.White, ColorUtil.BrightYellow, ColorUtil.SlateGary, DEFAULT_SIZE, System.Windows.HorizontalAlignment.Left);
				leaveReportButton.BorderColor = new SolidColorBrush(Colors.White);
				leaveReportButton.Click += new MouseButtonEventHandler(reportButton_Click);
				buttonList.Children.Add(leaveReportButton);
			}

#if DEBUG
			if (user == null || user.Type == UserType.Manager || user.Type == UserType.ManagerAndAdmin)
#else
			if (user.Type == UserType.Manager || user.Type == UserType.ManagerAndAdmin)
#endif
			{
				// yellow
				CARSButton leaveHistoryButton = new CARSButton("leaveHistoryButton", "Leave History", Colors.White, ColorUtil.BrightYellow, ColorUtil.SlateGary, DEFAULT_SIZE, System.Windows.HorizontalAlignment.Left);
				leaveHistoryButton.BorderColor = new SolidColorBrush(Colors.White);
				leaveHistoryButton.Click += new MouseButtonEventHandler(historyButton_Click);
				buttonList.Children.Add(leaveHistoryButton);
			}
			// adjust the height
			menuLayoutRoot.Height = (buttonList.Children.Count + 1) * DEFAULT_SIZE.Height;
			mainGrid.Height = (buttonList.Children.Count + 1) * DEFAULT_SIZE.Height;
		}
		#endregion

		#region Private methods
		// click control button
		private void employeeInfoButton_Click(object sender, MouseButtonEventArgs e)
		{
			if (ClickEmployeeInfoButton != null)
			{
				ClickEmployeeInfoButton(sender, (EventArgs)e);
			}
		}

		// click approval button
		private void approveButton_Click(object sender, MouseButtonEventArgs e)
		{
			if (ClickApproveLeaveButton != null)
			{
				ClickApproveLeaveButton(sender, (EventArgs)e);
			}
		}

		// click personal information button
		private void personalInfoButton_Click(object sender, MouseButtonEventArgs e)
		{
			if (ClickPersonalInfoButton != null)
			{
				ClickPersonalInfoButton(sender, (EventArgs)e);
			}
		}

		// click appliy leave button
		void applyLeaveButton_Click(object sender, MouseButtonEventArgs e)
		{
			if (ClickApplyLeaveButton != null)
			{
				ClickApplyLeaveButton(sender, (EventArgs)e);
			}
		}

		// click config button
		void configButton_Click(object sender, MouseButtonEventArgs e)
		{
			if (ClickVacationRulesButton != null)
			{
				ClickVacationRulesButton(sender, (EventArgs)e);
			}
		}

		// click report button
		void reportButton_Click(object sender, MouseButtonEventArgs e)
		{
			if (ClickLeaveReportButton != null)
			{
				ClickLeaveReportButton(sender, (EventArgs)e);
			}
		}

		// click history button
		void historyButton_Click(object sender, MouseButtonEventArgs e)
		{
			if (ClickLeaveHistoryButton != null)
			{
				ClickLeaveHistoryButton(sender, (EventArgs)e);
			}
		}
		#endregion

		public bool IsOpen()
		{
			return isOpen;
		}

		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!isOpen)
			{
				open.Begin();
			}
		}

		public void CloseMenu()
		{
			Grid_MouseLeave(null, null);
		}

		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			if (isOpen)
			{
				close.Begin();
			}
		}

		private void open_Completed(object sender, EventArgs e)
		{
			isOpen = true;
		}

		private void close_Completed(object sender, EventArgs e)
		{
			isOpen = false;
		}
	}
}
