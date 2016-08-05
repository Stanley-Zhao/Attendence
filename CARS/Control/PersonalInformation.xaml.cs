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
using System.Text;

namespace CARS.Control
{
	public partial class PersonalInformation : UserControl
	{
		public delegate void UpdateListEventHandler(object sender, EventArgs e);
		public event UpdateListEventHandler UpdateListEvent;
		private Button updateButton;
		private BaseInfo baseInfoControl = null;
		private User mUser = null;
		private User currentUser = null;
		private bool stopFlag = false;

		public User CurrentUser
		{
			get { return currentUser; }
			set { currentUser = value; }
		}

		public PersonalInformation(User pUser, CARSPage page)
		{
			mUser = currentUser = pUser;

			InitializeComponent();

			ClientInstance.Get().UpdateEmployeeCompleted += new EventHandler<UpdateEmployeeCompletedEventArgs>(client_UpdateEmployeeCompleted);
			ClientInstance.Get().CheckSupervisorValidationCompleted += new EventHandler<CheckSupervisorValidationCompletedEventArgs>(PersonalInformation_CheckSupervisorValidationCompleted);

			baseInfoControl = new BaseInfo(pUser, page);
			baseInfoControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
			baseInfoControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
			sPanel.Children.Add(baseInfoControl);

			// not for now
			//MultipleSelection ms = new MultipleSelection();
			//ms.Width = 400;
			//ms.MaxHeight = 100;
			//ms.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
			//ms.VerticalAlignment = System.Windows.VerticalAlignment.Top;
			//sPanel.Children.Add(ms);
			updateButton = new Button();
			updateButton.Height = 23;
			updateButton.Width = 80;
			updateButton.Content = "Update";
			updateButton.Margin = new Thickness(0, 10, 0, 10);
			updateButton.Click += new RoutedEventHandler(update_Click);
			sPanel.Children.Add(updateButton);
		}

		void PersonalInformation_CheckSupervisorValidationCompleted(object sender, CheckSupervisorValidationCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Check Supervisor Validation Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			if (!e.Result)
			{
				string message = "You cannot selete one of {0} team members as {0} supervisor.";

				if (baseInfoControl.CurrentPgae == CARSPage.PersonalInfo)
					message = string.Format(message, "your");
				else if (baseInfoControl.CurrentPgae == CARSPage.EmployeeInfo)
					message = string.Format(message, currentUser.FirstName + "'s");

				Message.Warning(message);
				return;
			}

			currentUser.FirstName = baseInfoControl.firstName.Text.Trim();
			currentUser.MiddleName = baseInfoControl.middleName.Text.Trim();
			currentUser.LegalName = baseInfoControl.legalName.Text.Trim();
			currentUser.LastName = baseInfoControl.lastName.Text.Trim();
			currentUser.Gender = (Sex)Enum.Parse(typeof(Sex), ((ComboBoxItem)baseInfoControl.gender.SelectedItem).Content.ToString(), false);
			currentUser.ServiceYears = float.Parse(baseInfoControl.serviceYears.Text);
			currentUser.DateOfHire = baseInfoControl.dateOfHire.SelectedDate.HasValue ? baseInfoControl.dateOfHire.SelectedDate.Value : currentUser.DateOfHire;
			currentUser.Supervisor = baseInfoControl.supervisor.SelectedItem != null ? ((User)baseInfoControl.supervisor.SelectedItem).Employee : currentUser.Supervisor;
			if (baseInfoControl.password1.Password != "")
			{
				currentUser.Password = baseInfoControl.password1.Password;
			}
			currentUser.Phone = baseInfoControl.phone.Text.Trim();
			currentUser.Mobile = baseInfoControl.mobile.Text.Trim();
			currentUser.Employee.IsAdmin = baseInfoControl.isAdministrator.IsChecked.HasValue ? baseInfoControl.isAdministrator.IsChecked.Value : currentUser.Employee.IsAdmin;
			currentUser.Employee.IsActive = baseInfoControl.isActive.IsChecked.HasValue ? baseInfoControl.isActive.IsChecked.Value : currentUser.Employee.IsActive;
			currentUser.Employee.IsManager = baseInfoControl.isSupervisor.IsChecked.HasValue ? baseInfoControl.isSupervisor.IsChecked.Value : currentUser.Employee.IsManager;
			currentUser.Employee.CostCenter = baseInfoControl.costCenter.Text.Trim();
			currentUser.Employee.EmployeeNum = baseInfoControl.employeeID.Text == "" ? 0 : int.Parse(baseInfoControl.employeeID.Text.Trim());

			string supervisorEmail = currentUser.Supervisor != null ? currentUser.Supervisor.Email : "";
			ClientInstance.Get().UpdateEmployeeAsync(currentUser.Employee.PKEmployeeID.ToString(),
				currentUser.Email,
				currentUser.FirstName,
				currentUser.MiddleName,
				currentUser.LastName,
				currentUser.LegalName,
				currentUser.Gender,
				currentUser.ServiceYears,
				currentUser.DateOfHire,
				supervisorEmail,
				CryptographyStuff.AES_EncryptString(currentUser.Password),
				currentUser.Phone,
				currentUser.Mobile,
				currentUser.Employee.IsAdmin,
				currentUser.Employee.IsActive,
				currentUser.Employee.IsManager,
				currentUser.Employee.CostCenter,
				currentUser.Employee.EmployeeNum);
		}

		void update_Click(object sender, RoutedEventArgs e)
		{
			// email
			if (baseInfoControl.userName.Text == "")
			{
				Message.Warning("Please input email.");
				return;
			}

			// first name
			if (baseInfoControl.firstName.Text == "")
			{
				Message.Warning("Please input first name.");
				return;
			}

			// last name
			if (baseInfoControl.lastName.Text == "")
			{
				Message.Warning("Please input last name.");
				return;
			}

			// gender
			if (baseInfoControl.gender.SelectedIndex == -1)
			{
				Message.Warning("Please select gender.");
				return;
			}

			// service years
			float serviceYears = 0f;
			if (baseInfoControl.serviceYears.Text == "" || !float.TryParse(baseInfoControl.serviceYears.Text, out serviceYears) || !baseInfoControl.IsServiceYearValid)
			{
				Message.Warning("Please input service years. Like: 3 or 3.5");
				return;
			}
			else if (serviceYears < 0)
			{
				Message.Warning("Service years must be greater than 0.");
				return;
			}
			else if (serviceYears > 60f)
			{
				Message.Warning("Impossible, this guy is already retired? Service year is too big.");
				return;
			}

			// date of hire
			if (baseInfoControl.dateOfHire.SelectedDate == null || !baseInfoControl.dateOfHire.SelectedDate.HasValue)
			{
				Message.Warning("Please select date of hire.");
				return;
			}

			// supervisor
			if (baseInfoControl.supervisor.SelectedIndex == -1 && baseInfoControl.supervisorStar.Visibility == System.Windows.Visibility.Visible)
			{
				Message.Warning("Please select Supervisor");
				return;
			}

			if (baseInfoControl.password1.Password == string.Empty)
			{
				Message.Warning("Password cannot be empty.");
				return;
			}

			if (baseInfoControl.password1.Password != baseInfoControl.password2.Password)
			{
				Message.Warning("The two passwords you entered did not match.");
				return;
			}

			// select a team member as supervisor			
			if (baseInfoControl.supervisor.SelectedIndex != -1)
			{
				User tempUser = (User)baseInfoControl.supervisor.SelectedItem;
				// check if selecting himself/herself as supervisor
				if (tempUser.PKEmployeeID == currentUser.PKEmployeeID)
				{
					StringBuilder sb = new StringBuilder();
					if (baseInfoControl.CurrentPgae == CARSPage.PersonalInfo)
					{
						sb.Append("You cannot select yourself as your supervisor.");
					}
					else if (baseInfoControl.CurrentPgae == CARSPage.EmployeeInfo)
					{
						sb.Append("You cannot set ");
						sb.Append(currentUser.FirstName);
						sb.Append(currentUser.Gender == Sex.Male ? " himself as" : " herself as");
						sb.Append(currentUser.Gender == Sex.Male ? " his supervisor." : " her supervisor.");
					}

					Message.Warning(sb.ToString());
					return;
				}

				// check if employee id is not number				
				int employeeID = -1;
				if (baseInfoControl.employeeID.Text != "" && !int.TryParse(baseInfoControl.employeeID.Text.Trim(), out employeeID))
				{
					Message.Warning("Please input employee ID in integer.");
					return;
				}

				if (employeeID <= 0 && baseInfoControl.employeeID.Text != "")
				{
					Message.Warning("Please input employee ID in positive integer.");
					return;
				}

				// check if selecting a team member as supervisor		
				ClientInstance.Get().CheckSupervisorValidationAsync(currentUser.PKEmployeeID.ToString(), tempUser.PKEmployeeID.ToString());
			}
		}

		private void client_UpdateEmployeeCompleted(object sender, UpdateEmployeeCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Update Employee Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			if (currentUser.Email.ToLower() == mUser.Email.ToLower())
				Message.Information("Your personal information is updated.");
			else
				Message.Information(currentUser.FirstName + "'s information is updated.");
			this.UpdateListEvent(sender, e);
		}

		public void RefreshData(User user, CARSPage page)
		{
			currentUser = user;
			updateButton.IsEnabled = (user != null);
			baseInfoControl.RefreshData(user, page);
		}
	}
}
