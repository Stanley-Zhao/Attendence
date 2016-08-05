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
using CARS.Control;
using System.ServiceModel;
using System.Windows.Browser;

namespace CARS.Pages
{
	public partial class Login : Page
	{
		bool isLoading = false;

		public Login(bool firstTime)
		{
			InitializeComponent();

			isLoading = true;

			if (firstTime)
			{
				ClientInstance.Get().LoginCompleted += new EventHandler<LoginCompletedEventArgs>(client_LoginCompleted);
				ClientInstance.Get().ForgetPasswordCompleted += new EventHandler<ForgetPasswordCompletedEventArgs>(client_ForgetPasswordCompleted);
				ClientInstance.Get().GetUserManualAddressCompleted += new EventHandler<GetUserManualAddressCompletedEventArgs>(Login_GetUserManualAddressCompleted);
				ClientInstance.Get().GetReportBugAddressCompleted += new EventHandler<GetReportBugAddressCompletedEventArgs>(Login_GetReportBugAddressCompleted);
				//animblur.Begin();
			}

			savePW.IsChecked = CarsConfig.Instance().DoSavePW;
			if (savePW.IsChecked == true)
			{
				autoLogin.IsEnabled = true;
				autoLogin.IsChecked = CarsConfig.Instance().DoAutoLogin;
			}
			else
				autoLogin.IsEnabled = false;

			//forgetButton.Height = 
			loginButton.Height = registryButton.Height = 30;
			//forgetButton.Width = 
			loginButton.Width = registryButton.Width = 100;
			registryButton.ActiveColor = Colors.Black;
            loginButton.ActiveColor = Colors.Black;

			CARSServiceClient client = CARSServiceClientFactory.CreateCARSServiceClient();
			client.GetVersionCompleted += new EventHandler<GetVersionCompletedEventArgs>(client_GetVersionCompleted);
			client.GetVersionAsync();

			if (ClientInstance.UserManualAddress != string.Empty)
				userManualButton.Visibility = System.Windows.Visibility.Visible;
			else
				ClientInstance.Get().GetUserManualAddressAsync();

			ClientInstance.Get().GetReportBugAddressAsync();
		}

		void Login_GetReportBugAddressCompleted(object sender, GetReportBugAddressCompletedEventArgs e)
		{
			ClientInstance.ReportBugAddress = e.Result;

			isLoading = false;

			if (CarsConfig.Instance().DoSavePW)
			{
				LoginInfo loginInfo = CarsConfig.Instance().LoadLoginInfo();
				userNameTxtBox.Text = loginInfo.UserName.Replace(emailSuffix.Content.ToString(), "");
				passwordTxtBox.Password = loginInfo.Password;

				if (CarsConfig.Instance().DoAutoLogin)
					ClientInstance.Get().LoginAsync(loginInfo.UserName, CryptographyStuff.AES_EncryptString(loginInfo.Password));
			}
		}

		void Login_GetUserManualAddressCompleted(object sender, GetUserManualAddressCompletedEventArgs e)
		{
			ClientInstance.UserManualAddress = e.Result;

			if (ClientInstance.UserManualAddress != string.Empty)
				userManualButton.Visibility = System.Windows.Visibility.Visible;
			else
				userManualButton.Visibility = System.Windows.Visibility.Collapsed;
		}

		void client_GetVersionCompleted(object sender, GetVersionCompletedEventArgs e)
		{
#if DEBUG
			version.Text = "Version: " + e.Result + " - DEBUG";
#elif DEV
			version.Text = "Version: " + e.Result + " - DEV";
#else
			version.Text = "Version: " + e.Result;
#endif
		}

		private void ClickLogin(object sender, RoutedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;
			if (userNameTxtBox.Text == "")
			{
				ShowMessage("Please input your name.", MessageType.Error);
			}
			else if (passwordTxtBox.Password == "")
			{
				ShowMessage("Please input your password.", MessageType.Error);
			}
			else
			{
				string email = userNameTxtBox.Text.ToLower().Trim() + emailSuffix.Content.ToString();
				ClientInstance.ShowSpinner();
				if (CarsConfig.Instance().DoSavePW)
				{
					LoginInfo loginInfo = new LoginInfo();
					loginInfo.UserName = email;
					loginInfo.Password = passwordTxtBox.Password;
					CarsConfig.Instance().SaveLoginInfo(loginInfo);
				}
				ClientInstance.Get().LoginAsync(email, CryptographyStuff.AES_EncryptString(passwordTxtBox.Password));
			}
		}

		private void client_LoginCompleted(object sender, LoginCompletedEventArgs e)
		{
			ClientInstance.HideSpinner();

			Logger.Instance().Log(MessageType.Information, "Login Completed");
			if (ErrorHandler.Handle(e.Error))
			{
				passwordTxtBox.Password = string.Empty;
				userNameTxtBox.Focus();
				userNameTxtBox.SelectAll();
				return;
			}

			Employee employee = new Employee();

			if (e.Result != null)
			{
				employee = (Employee)e.Result;
				string aesPassword = CryptographyStuff.AES_DecryptString(employee.Password);
				employee.Password = aesPassword;
				User currentUser = new User(employee);
				this.Content = new Main(currentUser);
			}
			else
			{
				Logger.Instance().Log(MessageType.Error, "Result is NULL");
			}
		}

		private void ClickRegister(object sender, RoutedEventArgs e)
		{
			this.Content = new Registry(ClientInstance.FirstTimeToRegisterPage);
		}

		private void ClickSendEmail(object sender, RoutedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;
			if (userNameTxtBox.Text.Trim() != string.Empty)
			{
				ClientInstance.Get().ForgetPasswordAsync(userNameTxtBox.Text.Trim() + emailSuffix.Content.ToString());
			}
			else
				ShowMessage("Input your email account.", MessageType.Error);
		}

		private void client_ForgetPasswordCompleted(object sender, ForgetPasswordCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Forget Password Completed");
			if (ErrorHandler.Handle(e.Error))
				return;

			if (e.Result)
				ShowMessage("Your password was sent to your Advent email.", MessageType.Information);
			else
				ShowMessage("Wrong email address.", MessageType.Error);
		}

		private void ShowMessage(string value, MessageType type)
		{
			if (type == MessageType.Information)
				noteLabel.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x64, 0x95, 0xED)); //CornflowerBlue
			else
				noteLabel.Foreground = new SolidColorBrush(Colors.Red); //Red

			noteLabel.Content = value;
			noteLabel.Visibility = System.Windows.Visibility.Visible;
		}

		private void Login_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				ClickLogin(null, null);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			ClientInstance.SetSpinner(loginSpinner);
		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			loginSpinner.Width = e.NewSize.Width;
			loginSpinner.Height = e.NewSize.Height;
		}

		private void userNameTxtBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void passwordTxtBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			noteLabel.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void userManualButton_Click(object sender, RoutedEventArgs e)
		{
			HtmlWindow userManualWindow = HtmlPage.Window;
			userManualWindow.Navigate(new Uri(ClientInstance.UserManualAddress), "_blank");
		}

		private void autoLogin_Checked(object sender, RoutedEventArgs e)
		{
			if (isLoading)
				return;

			CarsConfig.Instance().DoAutoLogin = autoLogin.IsChecked ?? false;
		}

		private void autoLogin_Unchecked(object sender, RoutedEventArgs e)
		{
			if (isLoading)
				return;

			CarsConfig.Instance().DoAutoLogin = autoLogin.IsChecked ?? false;
		}

		private void savePW_Checked(object sender, RoutedEventArgs e)
		{
			autoLogin.IsEnabled = savePW.IsChecked ?? false;

			if (autoLogin.IsEnabled == false)
				autoLogin.IsChecked = false;

			if (isLoading)
				return;

			CarsConfig.Instance().DoSavePW = savePW.IsChecked ?? false;
		}

		private void savePW_Unchecked(object sender, RoutedEventArgs e)
		{
			autoLogin.IsEnabled = savePW.IsChecked ?? false;

			if (autoLogin.IsEnabled == false)
				autoLogin.IsChecked = false;

			if (isLoading)
				return;

			CarsConfig.Instance().DoSavePW = savePW.IsChecked ?? false;
		}
	}
}
