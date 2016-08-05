using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CARS.CARSService;
using CARS.Control;
using CARS.Pages;

namespace CARS.SourceCode
{
	public static class ClientInstance
	{
		private static CARSServiceClient client;
		private static Spinner spinner;
		private static bool firstTimeToRegisterPage = true;
		private static string userManualAddress = string.Empty;
		private static string reportBugAddress = string.Empty;

		public static CARSServiceClient Get()
		{
			if (client == null)
				client = CARSServiceClientFactory.CreateCARSServiceClient();
			return client;
		}

		public static void SetSpinner(Spinner s)
		{
			spinner = s;
		}

		public static void ShowSpinner(string value)
		{
			if (spinner == null)
				throw new Exception("Spinner is not set.");
			if (!string.IsNullOrEmpty(value))
				spinner.TextValue = value;
			spinner.Visibility = Visibility.Visible;
		}

		public static void ShowSpinner()
		{
			ShowSpinner(string.Empty);
		}

		public static void HideSpinner()
		{
			if (spinner == null)
				throw new Exception("Spinner is not set.");
			spinner.Visibility = Visibility.Collapsed;
		}

		public static bool FirstTimeToRegisterPage { get { return firstTimeToRegisterPage; } set { firstTimeToRegisterPage = value; } }

		public static string UserManualAddress
		{
			get { return userManualAddress; }
			set { userManualAddress = value; }
		}

		public static string ReportBugAddress
		{
			get { return reportBugAddress; }
			set { reportBugAddress = value; }
		}
	}
}
