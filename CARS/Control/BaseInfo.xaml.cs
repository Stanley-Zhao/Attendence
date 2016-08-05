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
using System.Text.RegularExpressions;

namespace CARS.Control
{
	public partial class BaseInfo : UserControl
	{
		private User mUser = null;
		private CARSPage currentPage = CARSPage.ApplyLeave;
		private string previousInputs = string.Empty;

		private void SetVisibleValue(System.Windows.Visibility value)
		{
			passwordLabel1.Visibility = password1.Visibility =
							passwordLabel2.Visibility = password2.Visibility = phoneLabel.Visibility = phone.Visibility =
							mobileLabel.Visibility = mobile.Visibility = administratorLabel.Visibility = isAdministrator.Visibility =
							activeLabel.Visibility = isActive.Visibility = costCenterLabel.Visibility = costCenter.Visibility = idLabel.Visibility = employeeID.Visibility = supervisorLabel.Visibility = isSupervisor.Visibility = value;
		}

		private void SetNameInfoVisible(System.Windows.Visibility value)
		{
			passwordLabel1.Visibility = password1.Visibility = passwordLabel2.Visibility = password2.Visibility = phoneLabel.Visibility = phone.Visibility = mobile.Visibility = mobileLabel.Visibility = value;
			isAdministrator.Visibility = administratorLabel.Visibility = isActive.Visibility = activeLabel.Visibility = supervisorLabel.Visibility = isSupervisor.Visibility = System.Windows.Visibility.Collapsed;
		}

		public CARSPage CurrentPgae
		{
			get { return currentPage; }
			set
			{
				currentPage = value;
				if (currentPage == CARSPage.EmployeeInfo)				
					Clean();
			}
		}

		private void Clean()
		{
			userName.Text = string.Empty;
			firstName.Text = string.Empty;
			middleName.Text = string.Empty;
			lastName.Text = string.Empty;
			gender.IsEnabled = false;
			gender.SelectedIndex = -1;
			serviceYears.Text = string.Empty;
			dateOfHire.SelectedDate = null;

			supervisor.SelectedIndex = -1;
			supervisor.IsEnabled = false;

			phone.Text = string.Empty;
			mobile.Text = string.Empty;
			isSupervisor.IsChecked = false;
			isAdministrator.IsChecked = false;
			isActive.IsChecked = false;

			password1.Password = password2.Password = string.Empty;
			password1.IsEnabled = password2.IsEnabled = false;
		}

		public BaseInfo() : this(CARSPage.Register) { }

		public BaseInfo(CARSPage page) : this(null, page) { }

		public BaseInfo(User pUser, CARSPage page)
		{
			mUser = pUser;
			currentPage = page;
			InitializeComponent();

			SetPage();

			ClientInstance.Get().GetManagersCompleted += new EventHandler<GetManagersCompletedEventArgs>(client_GetManagersCompleted);
			ClientInstance.Get().GetManagersAsync();
		}

		private void SetPage()
		{
			if (currentPage == CARSPage.PersonalInfo)
			{
				snLabel.Foreground = new SolidColorBrush(Colors.Yellow);
				SetNameInfoVisible(System.Windows.Visibility.Visible);
				this.userName.IsEnabled = false;
				SetEnable(false);
				this.Height = 396; // 12 rows show * 33
			}
			else if (currentPage == CARSPage.EmployeeInfo)
			{
				snLabel.Foreground = new SolidColorBrush(Colors.Yellow);
				SetVisibleValue(System.Windows.Visibility.Visible);
				this.userName.IsEnabled = false;
				SetEnable(true);
				this.Height = 561;  // 17 rows show * 33
			}
			else // regester page
			{
				snLabel.Foreground = new SolidColorBrush(Colors.Blue);

				SetVisibleValue(System.Windows.Visibility.Collapsed);
				this.userName.IsEnabled = true;
				SetEnable(true);
				this.Height = 264; // 8 rows show * 33
			}
		}

		private void client_GetManagersCompleted(object sender, GetManagersCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Get Managers Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			List<Employee> managers = new List<Employee>();

			if (e.Result != null)
				managers = ((ObservableCollection<Employee>)e.Result).ToList<Employee>();
			else
				Logger.Instance().Log(MessageType.Error, "Result is NULL");

			supervisor.Items.Clear();
			foreach (Employee manager in managers)
			{
				User userManager = new User(manager);
				if(userManager.IsActive)
					supervisor.Items.Add(userManager);
			}

			SetPage();

			if (currentPage == CARSPage.PersonalInfo)
				UpdateUserInfomation(true);
			else if (currentPage == CARSPage.EmployeeInfo)
				UpdateUserInfomation(false);

			ClientInstance.HideSpinner();
		}

		private void UpdateUserInfomation(bool isPasswordCanBeChanged)
		{
			if (mUser == null)
			{
				Clean();
				return;
			}

			userName.Text = mUser.UserName;
			firstName.Text = mUser.FirstName;
			middleName.Text = mUser.MiddleName;
			legalName.Text = mUser.LegalName;
			lastName.Text = mUser.LastName;
			gender.SelectedIndex = mUser.Gender == Sex.Female ? 0 : 1;
			serviceYears.Text = mUser.ServiceYearsValue;
			dateOfHire.SelectedDate = mUser.DateOfHire;

			supervisor.SelectedIndex = -1;
			supervisor.IsEnabled = true;
			for (int i = 0; i < supervisor.Items.Count; i++)
			{
				Employee temp = ((User)supervisor.Items[i]).Employee;
				Employee currentUserManager = mUser.Supervisor;
				if (currentUserManager == null)
				{
					supervisor.SelectedIndex = -1;
					supervisor.IsEnabled = false;
					supervisorStar.Visibility = System.Windows.Visibility.Collapsed;
					break;
				}
				else if (temp.PKEmployeeID == currentUserManager.PKEmployeeID)
				{
					supervisor.SelectedIndex = i;
					supervisor.IsEnabled = true;
					supervisorStar.Visibility = System.Windows.Visibility.Visible;
					break;
				}
			}

			phone.Text = mUser.Phone;
			mobile.Text = mUser.Mobile;
			isSupervisor.IsChecked = mUser.Employee.IsManager;
			isAdministrator.IsChecked = mUser.Employee.IsAdmin;
			isActive.IsChecked = mUser.Employee.IsActive;

			password1.Password = password2.Password = mUser.Password;
			password1.IsEnabled = password2.IsEnabled = isPasswordCanBeChanged;

			costCenter.Text = mUser.Employee.CostCenter == null ? string.Empty : mUser.Employee.CostCenter;
			employeeID.Text = mUser.Employee.EmployeeNum == 0 ? string.Empty : mUser.Employee.EmployeeNum.ToString();
		}

		private void SetEnable(bool value)
		{
			this.firstName.IsEnabled = value;
			this.middleName.IsEnabled = value;
			this.lastName.IsEnabled = value;
			this.gender.IsEnabled = value;
			this.serviceYears.IsEnabled = value;
			this.dateOfHire.IsEnabled = value;
			this.legalName.IsEnabled = value;
		}

		public void RefreshData(User user, CARSPage page)
		{
			mUser = user;
			currentPage = page;
			ClientInstance.ShowSpinner("Get Managers");
			ClientInstance.Get().GetManagersAsync();
		}

		public bool IsServiceYearValid
		{
			get
			{
				Match m = Regex.Match(this.serviceYears.Text, @"(^\d{1,2}.[0-9]$)|(^\d{1,2}$)");
				return m.Success;
			}
		}

		public bool IsEmailValid
		{
			get
			{
				Match m = Regex.Match(this.userName.Text, @"[a-zA-Z0-9_]$");
				return m.Success;
			}
		}
	}
}
