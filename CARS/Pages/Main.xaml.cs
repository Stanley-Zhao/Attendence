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
using System.Windows.Navigation;
using CARS.SourceCode;
using CARS.Control;

namespace CARS.Pages
{
	public partial class Main : Page
	{
		#region Members
		private CARSPage currentPage = CARSPage.ApplyLeave;
		private CARSPage nextPage = CARSPage.ApplyLeave;
		private User user = null;
		private ApplyControl applyControl;
		private LeaveRecords leaveRecords;
		private PersonalInformation personnalInfomation;
		private EmployeeRecords employeeRecords;
		private Menu menu;
		#endregion

		#region Constructor
		public Main(User userPara)
		{
			InitializeComponent();
			user = userPara;
			// Create menu according to user's type. (Admin, Employee, Manager, Admin+Manager)
			menu = new Menu(user);
			menu.ClickApplyLeaveButton += new EventHandler(menu_ClickApplyLeaveButton);
			menu.ClickPersonalInfoButton += new EventHandler(menu_ClickPersonalInfoButton);
			menu.ClickApproveLeaveButton += new EventHandler(menu_ClickApprovalButton);
			menu.ClickEmployeeInfoButton += new EventHandler(menu_ClickEmployeeInfoButton);
			menu.ClickVacationRulesButton += new EventHandler(menu_ClickVacationRulesButton);
			menu.ClickLeaveHistoryButton += new EventHandler(menu_ClickLeaveHistoryButton);
			menu.ClickLeaveReportButton += new EventHandler(menu_ClickLeaveReportButton);
			menuPanel.Children.Add(menu);

			header.headerContent.Text = "Hello, " + user.FirstName;

			if (applyControl == null)
			{
				applyControl = new ApplyControl(CARSPage.ApplyLeave, user);
				applyControl.UpdateListEvent += new ApplyControl.UpdateListEventHandler(applyControl_ApplyNewLeave);
			}
			leftArea.Children.Clear();
			leftArea.Children.Add(applyControl);

			if (leaveRecords == null)
			{
				leaveRecords = new LeaveRecords(CARSPage.ApplyLeave, user);
				leaveRecords.SelectionChanged += new SelectionChangedEventHandler(leaveRecords_LeaveSelectionChanged);
				leaveRecords.UpdateLeftUI += new EventHandler(leaveRecords_UpdateLeftUI);
				leaveRecords.RunAsEvent += new EventHandler(leaveRecords_RunAsEvent);
			}
			rightArea.Children.Clear();
			rightArea.Children.Add(leaveRecords);
			header.headerLabel.Content = "Apply Leave";
		}

		void leaveRecords_RunAsEvent(object sender, EventArgs e)
		{
			applyControl.SetRunAsUser(leaveRecords.RunAsUser);
		}

		void leaveRecords_UpdateLeftUI(object sender, EventArgs e)
		{
			applyControl.Clean(true);
		}

		void applyControl_ApplyNewLeave(object sender, EventArgs e)
		{
			leaveRecords.CurrentPage = currentPage;
			leaveRecords.RefreshData(false); // stanley: no change
		}

		private void leaveRecords_LeaveSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			applyControl.SetLeaveItem(leaveRecords.SelectedItem);
		}

		private void menu_ClickLeaveReportButton(object sender, EventArgs e)
		{
			menu.CloseMenu();
			GenerateReport gr = new GenerateReport(user);
			gr.Show();
		}
		#endregion

		#region Event Handler
		private void menu_ClickEmployeeInfoButton(object sender, EventArgs e)
		{
			ChangePageTo(CARSPage.EmployeeInfo);
			header.headerLabel.Content = "Employee Information";
		}

		private void menu_ClickApprovalButton(object sender, EventArgs e)
		{
			ChangePageTo(CARSPage.ApproveLeave);
			header.headerLabel.Content = "Approve Leave";            
		}

		private void menu_ClickPersonalInfoButton(object sender, EventArgs e)
		{
			ChangePageTo(CARSPage.PersonalInfo);
			header.headerLabel.Content = "Personal Information";
		}

		private void menu_ClickApplyLeaveButton(object sender, EventArgs e)
		{
			ChangePageTo(CARSPage.ApplyLeave);
			header.headerLabel.Content = "Apply Leave";
		}

		private void menu_ClickVacationRulesButton(object sender, EventArgs e)
		{
			ChangePageTo(CARSPage.VacationRules);
			header.headerLabel.Content = "Vacation Rules";
		}

		private void menu_ClickLeaveHistoryButton(object sender, EventArgs e)
		{
			ChangePageTo(CARSPage.LeaveHistory);
			header.headerLabel.Content = "Leave History";
		}
		#endregion

		#region Private Methods
		private Color PageToColor(CARSPage page)
		{
			Color color = ColorUtil.WhiteSmoke;
			switch (page)
			{
				case CARSPage.ApplyLeave:
					color = ColorUtil.LightBrightBlue; //(use blue now)
					break;
				case CARSPage.PersonalInfo:
					//color = ColorUtil.LightBrightBlue; //(use blue now)
                    color = ColorUtil.LightBrightGreen; // use green
					break;
				case CARSPage.ApproveLeave:
					//color = ColorUtil.LightBrightBlue; //(use blue now)
                    color = ColorUtil.LightBrightRed; // use red
					break;
				case CARSPage.EmployeeInfo:
					//color = ColorUtil.LightBrightBlue; //(use blue now)
                    color = ColorUtil.LightBrightGreen; // use green
					break;
				case CARSPage.VacationRules:
					//color = ColorUtil.LightBrightBlue; //(use blue now)
                    // TODO
					break;
				case CARSPage.LeaveHistory:
					//color = ColorUtil.LightBrightBlue; //(use blue now)
                    color = ColorUtil.LightBrightYellow; // use yellow
					break;
				default:
					break;
			}
			return color;
		}

		private LinearGradientBrush CreateBackColor(CARSPage page)
		{
			GradientStopCollection gsc = new GradientStopCollection();
			GradientStop color1 = new GradientStop();
			GradientStop color2 = new GradientStop();
			color1.Color = ColorUtil.WhiteSmoke;
			color2.Color = PageToColor(page);
			color2.Offset = 1;
			gsc.Add(color1);
			gsc.Add(color2);
			return new LinearGradientBrush(gsc, 45);
		}

		private void ChangePageTo(CARSPage page)
		{
            if (currentPage == page)
                return;
            else
                nextPage = page;

            // following four lines are used to remove whole page vertical scrollbar on Approve leave page.
            if (nextPage == CARSPage.ApproveLeave)
                this.Height = this.ActualHeight - 2d;
            else if (currentPage == CARSPage.ApproveLeave)
                this.Height = this.ActualHeight + 2d;

			leftArea.Background = CreateBackColor(currentPage);
			hideOldPage.Begin();
		}

		private void DoubleAnimation_Completed(object sender, EventArgs e)
		{
			leftArea.Background = CreateBackColor(nextPage);
			currentPage = nextPage;

			// Do page content updating
			UpdatePageContent(nextPage);

			showNewPage.Begin();
		}

		private void UpdatePageContent(CARSPage page)
		{
			leftArea.Children.Clear();

			switch (page)
			{
				case CARSPage.ApplyLeave:
					rightArea.Children.Clear();
					applyControl.IsViewMode = false; // update data
					applyControl.CurrentPage = CARSPage.ApplyLeave;
					leaveRecords.CurrentPage = CARSPage.ApplyLeave;
					leaveRecords.RefreshData(); // update data, current user's
					leftArea.Children.Add(applyControl);
					rightArea.Children.Add(leaveRecords);
					break;
				case CARSPage.PersonalInfo:
					rightArea.Children.Clear();
					if (personnalInfomation == null)
					{
						personnalInfomation = new PersonalInformation(user, CARSPage.PersonalInfo);
						personnalInfomation.UpdateListEvent += new PersonalInformation.UpdateListEventHandler(personnalInfomation_UpdateListEvent);
					}
					personnalInfomation.RefreshData(user, CARSPage.PersonalInfo); // update data
					leaveRecords.CurrentPage = CARSPage.PersonalInfo;
					leaveRecords.RefreshData(); // update data, current user's
					leftArea.Children.Add(personnalInfomation);
					rightArea.Children.Add(leaveRecords);
					break;
				case CARSPage.ApproveLeave:
					rightArea.Children.Clear();
					applyControl.CurrentPage = CARSPage.ApproveLeave;
					if (leaveRecords.CurrentFrozenDate == DateTime.MinValue)
						leaveRecords.CurrentFrozenDate = applyControl.CurrentFrozenDate;
					leaveRecords.CurrentPage = CARSPage.ApproveLeave;                    
					leaveRecords.RefreshData(CarsConfig.Instance().ShowAllRecords); // update data, by default, don't show other leaves, only applying leaves.
					leftArea.Children.Add(applyControl);
					rightArea.Children.Add(leaveRecords);
					break;
				case CARSPage.EmployeeInfo:
					rightArea.Children.Clear();
					if (personnalInfomation == null)
					{
						personnalInfomation = new PersonalInformation(user, CARSPage.PersonalInfo);
						personnalInfomation.UpdateListEvent += new PersonalInformation.UpdateListEventHandler(personnalInfomation_UpdateListEvent);
					}
					personnalInfomation.RefreshData(null, CARSPage.EmployeeInfo); // update data
					if (employeeRecords == null)
					{
						employeeRecords = new EmployeeRecords();
						employeeRecords.SelectionChanged += new SelectionChangedEventHandler(employeeRecords_EmployeeSelectionChanged);
					}
					leftArea.Children.Add(personnalInfomation);
					rightArea.Children.Add(employeeRecords);
					break;
				case CARSPage.VacationRules:
					//TODO Don't need for now.
					//StackPanel sp = new StackPanel();
					//sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
					//sp.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
					//sp.Width = double.NaN;
					//sp.Height = double.NaN;
					//TextBox tb = new TextBox();
					//tb.Width = 450;
					//tb.Height = 480;
					//tb.Margin = new Thickness(20, 20, 20, 20);
					//tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					//tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
					//tb.Text = "Here is vacation rules";
					//sp.Children.Add(tb);
					//if (user.Type == UserType.Employee || user.Type == UserType.Manager)
					//{
					//    tb.IsReadOnly = true;
					//    tb.Background = new SolidColorBrush(Colors.Transparent);
					//}
					//else
					//{
					//    Button ub = new Button();
					//    ub.Content = "Update";
					//    ub.Width = 80;
					//    ub.Height = 30;
					//    ub.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					//    ub.Click += new RoutedEventHandler(ub_Click);
					//    sp.Children.Add(ub);
					//}
					//leftArea.Children.Add(sp);
					//rightArea.Children.Clear();
					//rightArea.Children.Add(new HolidayConfigControl(user));
					break;
				case CARSPage.LeaveHistory:
					rightArea.Children.Clear();
					applyControl.CurrentPage = CARSPage.LeaveHistory;
					leaveRecords.CurrentPage = CARSPage.LeaveHistory;
					leaveRecords.RefreshData(CarsConfig.Instance().ShowAllRecords); // update data, show all
					leftArea.Children.Add(applyControl);
					rightArea.Children.Add(leaveRecords);
					break;
				default:
					break;
			}
		}

		void employeeRecords_EmployeeSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			personnalInfomation.RefreshData(employeeRecords.SelectedItem, CARSPage.EmployeeInfo);
		}

		void personnalInfomation_UpdateListEvent(object sender, EventArgs e)
		{
			if (currentPage == CARSPage.EmployeeInfo)
				employeeRecords.GetData();
		}
		#endregion

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			ClientInstance.SetSpinner(mainSpinner);
		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			mainSpinner.Width = e.NewSize.Width;
			mainSpinner.Height = e.NewSize.Height;
		}

		private void menuPanel_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!menu.IsOpen())
			{
				open.Begin();
			}
		}

		private void menuPanel_MouseLeave(object sender, MouseEventArgs e)
		{
			if (menu.IsOpen())
			{
				close.Begin();
			}
		}
	}
}
