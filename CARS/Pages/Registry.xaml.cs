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
using CARS.CARSService;
using CARS.Control;

namespace CARS.Pages
{
	public partial class Registry : Page
	{
		public Registry(bool firstTime)
		{
			InitializeComponent();

			if (firstTime)
			{
				ClientInstance.Get().AddEmployeeCompleted += new EventHandler<AddEmployeeCompletedEventArgs>(client_AddEmployeeCompleted);
				ClientInstance.FirstTimeToRegisterPage = false;
			}

			createButton.Height = 30;
			createButton.Width = 100;
			createButton.TextHorizontalAligment = HorizontalAlignment.Center;
            createButton.ActiveColor = Colors.Black;
		}

		// Executes when the user navigates to this page.
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
		}

		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			this.Content = new Login(false);
		}

		private void ShowError(string value)
		{
			noteLabel.Visibility = System.Windows.Visibility.Visible;
			noteLabel.Content = value;
		}

		private void createButton_Click(object sender, MouseButtonEventArgs e)
		{
			// email
			if (baseInfo.userName.Text == "")
			{
				ShowError("Need input your email.");
				return;
			}

			// email's validation 1
			if (!baseInfo.IsEmailValid)
			{
				ShowError("Email is not valid. Email address only allows English, numbers or underscore.");
				return;
			}

            // email's validation 2
            if (baseInfo.userName.Text.ToLower().Contains("@"))
            {
                ShowError("You don't need to input \"@Advent.com\", CARS will add email suffix on it.");
                return;
            }

			// first name
			if (baseInfo.firstName.Text == "")
			{
				ShowError("Need your first name.");
				return;
			}

			// last name
			if (baseInfo.lastName.Text == "")
			{
				ShowError("Need your last name.");
				return;
			}

			// gender
			if (baseInfo.gender.SelectedIndex == -1)
			{
				ShowError("Select your Gender.");
				return;
			}

			// service years
			float serviceYears = 0f;
			if (baseInfo.serviceYears.Text == "" || !float.TryParse(baseInfo.serviceYears.Text, out serviceYears) || !baseInfo.IsServiceYearValid)
			{
				ShowError("Need input your service years. Like: 3 or 3.5");
				return;
			}
			else if (serviceYears < 0)
			{
				ShowError("Service years must be greater than 0.");
				return;
			}
			else if (serviceYears >60f )
			{
				ShowError("Impossible, you are already retired? Service year is too big.");
				return;
			}

			// date of hire
			if (baseInfo.dateOfHire.SelectedDate == null || !baseInfo.dateOfHire.SelectedDate.HasValue)
			{
				ShowError("Select your date of hire.");
				return;
			}

			// supervisor
			if (baseInfo.supervisor.SelectedIndex == -1)
			{
				ShowError("Who will approval your leave application? Select \"Supervisor\"");
				return;
			}

			User manager = (User)baseInfo.supervisor.SelectedItem;
			ClientInstance.ShowSpinner();
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;
			ClientInstance.Get().AddEmployeeAsync(baseInfo.userName.Text.ToLower().Trim() + baseInfo.emailSuffix.Content.ToString(),
				baseInfo.firstName.Text.Trim(),
				baseInfo.middleName.Text.Trim(),
				baseInfo.lastName.Text.Trim(),
				baseInfo.legalName.Text.Trim(),
				baseInfo.gender.SelectedIndex == 0 ? Sex.Female : Sex.Male,
				serviceYears,
				baseInfo.dateOfHire.SelectedDate.Value, manager.Email, "5030"
                , 0);
		}

		private void client_AddEmployeeCompleted(object sender, AddEmployeeCompletedEventArgs e)
		{
			ClientInstance.HideSpinner();

			Logger.Instance().Log(MessageType.Information, "Add Employee Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			Message.Information("Done! Check your email to get your default password. Don't forget to change your default password when you login CARS.");
			backButton_Click(backButton, new RoutedEventArgs());
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			ClientInstance.SetSpinner(regSpinner);
		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			regSpinner.Width = e.NewSize.Width;
			regSpinner.Height = e.NewSize.Height;
		}
	}
}
