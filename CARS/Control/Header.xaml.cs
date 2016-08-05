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
using System.Windows.Threading;
using CARS.Pages;
using CARS.SourceCode;
using System.Windows.Browser;

namespace CARS.Control
{
	public partial class Header : UserControl
	{
		bool isLoading = false;

		public Header()
		{
			InitializeComponent();

			isLoading = true;

			savePW.IsChecked = CarsConfig.Instance().DoSavePW;
			if (savePW.IsChecked == true)
			{
				autoLogin.IsEnabled = true;
				autoLogin.IsChecked = CarsConfig.Instance().DoAutoLogin;
			}
			else
				autoLogin.IsEnabled = false;


			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = new TimeSpan(0, 0, 1);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

			if (ClientInstance.UserManualAddress != string.Empty)
				userManualButton.Visibility = System.Windows.Visibility.Visible;
			else
				userManualButton.Visibility = System.Windows.Visibility.Collapsed;

			isLoading = false;
		}

		// update timer
		void timer_Tick(object sender, EventArgs e)
		{
			headerTime.Text = "Today is " + DateTime.Now.ToString("MM/dd/yyyy") + ", Now is " + DateTime.Now.ToString("HH:mm:ss");
		}

		private void exitButton_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Browser.HtmlPage.Window.Navigate(System.Windows.Browser.HtmlPage.Document.DocumentUri, "_self");
		}

		private void userManualButton_Click(object sender, RoutedEventArgs e)
		{
			HtmlWindow userManualWindow = HtmlPage.Window;
			userManualWindow.Navigate(new Uri(ClientInstance.UserManualAddress), "_blank");
		}

		private void reportBugButton_Click(object sender, RoutedEventArgs e)
		{
			HtmlWindow userManualWindow = HtmlPage.Window;
			userManualWindow.Navigate(new Uri(ClientInstance.ReportBugAddress), "_blank");
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
