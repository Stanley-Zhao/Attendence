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
using System.Text;
using System.Windows.Browser;
using CARS.CARSService;
using CARS.SourceCode;
using System.Collections.ObjectModel;

namespace CARS.Control
{
	public partial class GenerateReport : ChildWindow
	{
		private StringBuilder urlSB;
		private bool lockingFlag = false;
		private string urlSRAnnual = string.Empty;
		private string urlSRSick = string.Empty;
		private string urlARAnnual = string.Empty;
		private string urlARSick = string.Empty;
		private User user;

		public GenerateReport(User pUser)
		{
			InitializeComponent();

			InitializeUI();

			user = pUser;

			if (pUser.Employee.IsAdmin && pUser.Employee.IsManager) // admin and supervisor
			{
				reportTypeCom.SelectedIndex = 0;
				reportTypeCom.IsEnabled = true;
			}
			else if (pUser.Employee.IsAdmin && !pUser.Employee.IsManager) // admin only
			{
				reportTypeCom.SelectedIndex = 0;
				reportTypeCom.IsEnabled = false;
			}
			else // supervisor only
			{
				reportTypeCom.SelectedIndex = 1;
				reportTypeCom.IsEnabled = false;
			}

			CARSServiceClient client = CARSServiceClientFactory.CreateCARSServiceClient();
			client.GetReportPeriodsCompleted += new EventHandler<GetReportPeriodsCompletedEventArgs>(GenerateReport_GetReportPeriodsCompleted);
			client.GetReportPeriodsAsync();
		}

		void GenerateReport_GetReportPeriodsCompleted(object sender, GetReportPeriodsCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Get Report Periods Completed");
			if (ErrorHandler.Handle(e.Error))
			{
				return;
			}

			lockingFlag = true;

			List<ReportPeriod> list = e.Result.ToList<ReportPeriod>();
			if (list != null)
				for (int i = 0; i < list.Count; i++)
				{
					int index = (int)list[i].Month;
					DatePicker start = null;
					DatePicker end = null;
					//if (index > 1)
					//{
						start = (DatePicker)dateGrid.FindName("month" + index.ToString() + "Start");
						end = (DatePicker)dateGrid.FindName("month" + index.ToString() + "End");

						start.SelectedDate = list[i].StartTime;
						end.SelectedDate = list[i].EndTime;
					//}
					//else if (index == 1)
					//{
					//    end = (DatePicker)dateGrid.FindName("month" + index.ToString() + "End");
					//    end.SelectedDate = list[i].EndTime;
					//}
				}

			CARSServiceClient client = CARSServiceClientFactory.CreateCARSServiceClient();

			if (user.Employee.IsManager)
			{
				client.GetAnnualLeaveReportForSupervisorAddressCompleted += new EventHandler<GetAnnualLeaveReportForSupervisorAddressCompletedEventArgs>(client_GetAnnualLeaveReportForSupervisorAddressCompleted);
				client.GetSickLeaveReportForSupervisorAddressCompleted += new EventHandler<GetSickLeaveReportForSupervisorAddressCompletedEventArgs>(client_GetSickLeaveReportForSupervisorAddressCompleted);

				client.GetAnnualLeaveReportForSupervisorAddressAsync();
				client.GetSickLeaveReportForSupervisorAddressAsync();
			}

			client.GetAnnualLeaveReportForAdminAddressCompleted += new EventHandler<GetAnnualLeaveReportForAdminAddressCompletedEventArgs>(client_GetAnnualLeaveReportForAdminAddressCompleted);
			client.GetAnnualLeaveReportForAdminAddressAsync();

			client.GetSickLeaveReportForAdminAddressCompleted += new EventHandler<GetSickLeaveReportForAdminAddressCompletedEventArgs>(client_GetSickLeaveReportForAdminAddressCompleted);
			client.GetSickLeaveReportForAdminAddressAsync();
		}

		void client_GetSickLeaveReportForSupervisorAddressCompleted(object sender, GetSickLeaveReportForSupervisorAddressCompletedEventArgs e)
		{
			urlSRSick = e.Result;
		}

		void client_GetAnnualLeaveReportForSupervisorAddressCompleted(object sender, GetAnnualLeaveReportForSupervisorAddressCompletedEventArgs e)
		{
			urlSRAnnual = e.Result;
		}

		private void InitializeUI()
		{
			for (int i = 1; i <= 12; i++)
			{
				DatePicker start = (DatePicker)dateGrid.FindName("month" + i.ToString() + "Start");
				DatePicker end = (DatePicker)dateGrid.FindName("month" + i.ToString() + "End");
				Label label = (Label)dateGrid.FindName("month" + i.ToString() + "Label");

				//if (i > DateTime.Now.Month) // hide the comming months
				//{
				//    start.Height = 0;
				//    end.Height = 0;
				//    label.Height = 0;
				//}

				//if (i == 1)
				//{
				//    start.IsEnabled = false;
				//    start.SelectedDate = DateTime.Parse("1/1/" + DateTime.Now.Year.ToString());
				//}

				//if (i == 12 && DateTime.Now.Month == 12)
				//{
				//    end.IsEnabled = false;
				//    end.SelectedDate = DateTime.Parse("12/31/" + DateTime.Now.Year.ToString());
				//}
			}

			error.Height = 0;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckValidation())
				return;

			urlSB = new StringBuilder();
			ObservableCollection<ReportPeriod> list = new ObservableCollection<ReportPeriod>();
			for (int i = 1; i <= 12; i++)
			{
				DatePicker start = (DatePicker)dateGrid.FindName("month" + i.ToString() + "Start");
				DatePicker end = (DatePicker)dateGrid.FindName("month" + i.ToString() + "End");
				if (start.SelectedDate.HasValue && end.SelectedDate.HasValue)
				{
					ReportPeriod rp = new ReportPeriod();
					rp.Month = (MonthRank)i;
					rp.StartTime = new DateTime(start.SelectedDate.Value.Year, start.SelectedDate.Value.Month, start.SelectedDate.Value.Day, 0, 0, 0);
					rp.EndTime = new DateTime(end.SelectedDate.Value.Year, end.SelectedDate.Value.Month, end.SelectedDate.Value.Day, 23, 59, 59);
					list.Add(rp);

					urlSB.Append("&month");
					urlSB.Append(i.ToString());
					urlSB.Append("start=");
					urlSB.Append(rp.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
					urlSB.Append("&month");
					urlSB.Append(i.ToString());
					urlSB.Append("end=");
					urlSB.Append(rp.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
				}
				else
				{
					ReportPeriod rp = new ReportPeriod();
					rp.Month = (MonthRank)i;
					rp.StartTime = null;
					rp.EndTime = null;
					list.Add(rp);
				}
			}

			urlARAnnual += urlSB.ToString();
			urlARSick += urlSB.ToString();

			// Add parameter supervisorID
			urlSB.Append(string.Format("&supervisorID={0}", user.PKEmployeeID.ToString()));

			if (reportTypeCom.SelectedIndex == 0) // administrator report, save data to DB.
			{
				CARSServiceClient client = CARSServiceClientFactory.CreateCARSServiceClient();
				client.UpdateReportPeriodsCompleted += new EventHandler<UpdateReportPeriodsCompletedEventArgs>(GenerateReport_UpdateReportPeriodsCompleted);
				client.UpdateReportPeriodsAsync(list);
			}
			else // supervisor report, do NOT save data to DB.
			{
				HtmlWindow srAnnual = HtmlPage.Window;
				HtmlWindow srSick = HtmlPage.Window;
				if (urlSRAnnual.ToLower().Contains("commingsoon"))
				{
					srAnnual.Navigate(new Uri(urlSRAnnual), "_blank");
					srSick.Navigate(new Uri(urlSRSick), "_blank");
				}
				else
				{
					urlSRAnnual += urlSB.ToString();
					urlSRSick += urlSB.ToString();
					srAnnual.Navigate(new Uri(urlSRAnnual), "_blank");
					srSick.Navigate(new Uri(urlSRSick), "_blank");
				}
			}
			this.DialogResult = true;
		}

		void GenerateReport_UpdateReportPeriodsCompleted(object sender, UpdateReportPeriodsCompletedEventArgs e)
		{
			Logger.Instance().Log(MessageType.Information, "Update Report Periods Completed");
			if (ErrorHandler.Handle(e.Error))
			{
				return;
			}

			HtmlWindow sickReportForAdmin = HtmlPage.Window;
			sickReportForAdmin.Navigate(new Uri(urlARSick), "_blank");

			HtmlWindow annualReportForAdmin = HtmlPage.Window;
			annualReportForAdmin.Navigate(new Uri(urlARAnnual), "_blank");
		}

		void client_GetSickLeaveReportForAdminAddressCompleted(object sender, GetSickLeaveReportForAdminAddressCompletedEventArgs e)
		{
			urlARSick = e.Result;
			lockingFlag = false;
			//HtmlWindow sickReportForAdmin = HtmlPage.Window;
			//sickReportForAdmin.Navigate(new Uri(e.Result), "_blank");
		}

		void client_GetAnnualLeaveReportForAdminAddressCompleted(object sender, GetAnnualLeaveReportForAdminAddressCompletedEventArgs e)
		{
			urlARAnnual = e.Result;

			//HtmlWindow annualReportForAdmin = HtmlPage.Window;
			//annualReportForAdmin.Navigate(new Uri(e.Result), "_blank");
		}

		private bool CheckValidation()
		{
			// >>>> start to check null
			int firstNull = 13;
			for (int i = 1; i <= 12; i++)
			{
				// find first null setting (this null value must be a start picker, if it's a end picker, show errors)
				DatePicker dpCurrentStart = (DatePicker)dateGrid.FindName("month" + i.ToString() + "Start");
				DatePicker dpCurrentEnd = (DatePicker)dateGrid.FindName("month" + i.ToString() + "End");

				if (i == 1 && (dpCurrentStart.SelectedDate.Value.Month != 1 || dpCurrentStart.SelectedDate.Value.Day != 1))
				{
					ShowError("First day is not 1/1.");
					return false;
				}

				if (i == 12 && (dpCurrentEnd.SelectedDate.Value.Month != 12 || dpCurrentEnd.SelectedDate.Value.Day != 31))
				{
					ShowError("Last day is not 12/31");
					return false;
				}

				if (dpCurrentStart.SelectedDate.HasValue && !dpCurrentEnd.SelectedDate.HasValue)
				{
					ShowError("You didn't set end date of Month " + i.ToString());
					return false;
				}
				else if (!dpCurrentStart.SelectedDate.HasValue && dpCurrentEnd.SelectedDate.HasValue)
				{
					ShowError("You didn't set start date of Month " + i.ToString());
					return false;
				}

				if (!dpCurrentStart.SelectedDate.HasValue)
				{
					firstNull = i;
					break;
				}
			}

			for (int i = firstNull + 1; i <= 12; i++)
			{
				// After firstNull, all of the picker should be NULL value
				DatePicker dpCurrentStart = (DatePicker)dateGrid.FindName("month" + i.ToString() + "Start");
				DatePicker dpCurrentEnd = (DatePicker)dateGrid.FindName("month" + i.ToString() + "End");

				if (dpCurrentStart.SelectedDate.HasValue || dpCurrentEnd.SelectedDate.HasValue)
				{
					ShowError("You didn't set start date of Month " + firstNull.ToString());
					return false;
				}
			}
			// <<<< end for checking null

			for (int i = 1; i <= firstNull - 2; i++)
			{
				DatePicker dpCurrentStart = (DatePicker)dateGrid.FindName("month" + i.ToString() + "Start");
				DatePicker dpCurrentEnd = (DatePicker)dateGrid.FindName("month" + i.ToString() + "End");
				DatePicker dpNextStart = (DatePicker)dateGrid.FindName("month" + (i + 1).ToString() + "Start");
				DatePicker dpNextEnd = (DatePicker)dateGrid.FindName("month" + (i + 1).ToString() + "End");

				int year = dpCurrentStart.SelectedDate.Value.Year;
				if (dpCurrentEnd.SelectedDate.Value.Year != year ||
					dpNextStart.SelectedDate.Value.Year != year || dpNextEnd.SelectedDate.Value.Year != year)
				{
					ShowError("All start dates and end dates should be in same year.");
					return false;
				}				

				//if (!CheckDatePicker(dpCurrentStart, i) ||
				//    !CheckDatePicker(dpCurrentEnd, i) ||
				//    !CheckDatePicker(dpNextStart, i + 1) ||
				//    !CheckDatePicker(dpNextEnd, i + 1))
				//    return false;

				if (!CheckDaysLosingOrDuplicating(dpCurrentEnd, dpNextStart))
					return false;
			}

			return true;
		}

		private bool CheckDaysLosingOrDuplicating(DatePicker dpCurrentEnd, DatePicker dpNextStart)
		{
			string nextMonth = ((Label)dateGrid.FindName(dpNextStart.Name.Replace("Start", "Label"))).Content.ToString();
			string currentMonth = ((Label)dateGrid.FindName(dpCurrentEnd.Name.Replace("End", "Label"))).Content.ToString();

			int days = (dpNextStart.SelectedDate.Value.DayOfYear - dpCurrentEnd.SelectedDate.Value.DayOfYear);
			if (days > 1)
			{
				days--;
				string isDays = (days) == 1 ? " day" : " days";
				ShowError("Lost " + days.ToString() + " days between " + currentMonth + " end date and " + nextMonth + " start date.");
				return false; // wrong
			}

			if (days < 1)
			{
				days--;
				string isDays = (-days) == 1 ? " day" : " days";
				ShowError((-days).ToString() + isDays + " are duplicated between " + currentMonth + " end date and " + nextMonth + " start date.");
				return false; // wrong
			}

			return true; // OK
		}

		private bool CheckDatePicker(DatePicker dp, int i)
		{
			string temp = dp.Name.Contains("Start") ? "Start" : "End" + " day of " + ((Label)dateGrid.FindName("month" + i.ToString() + "Label")).Content.ToString();
			if (!dp.SelectedDate.HasValue)
			{
				ShowError(temp + " is not selected.");
				return false; // wrong
			}
			//else if (dp.SelectedDate.Value.Year != DateTime.Now.Year)
			//{
			//    ShowError(temp + " is not in this year.");
			//    return false; // wrong
			//}

			return true; // OK
		}

		private void ShowError(string p)
		{
			error.Height = double.NaN;
			error.Text = p;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void month_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lockingFlag)
				return;

			DatePicker current = (DatePicker)sender;
			int currentMonthIndex = 0;
			if (current.Name.Contains("End")) // end
			{
				currentMonthIndex = Convert.ToInt32(current.Name.ToLower().Replace("month", "").Replace("end", ""));

				if (DateTime.Now.Month > currentMonthIndex && current.SelectedDate.HasValue)
				{
					DatePicker nextStart = (DatePicker)dateGrid.FindName("month" + (++currentMonthIndex) + "Start");
					nextStart.SelectedDate = current.SelectedDate.Value.AddDays(1);
				}
			}
			else
			{
				currentMonthIndex = Convert.ToInt32(current.Name.ToLower().Replace("month", "").Replace("start", ""));

				if (currentMonthIndex > 1 && current.SelectedDate.HasValue)
				{
					DatePicker lastEnd = (DatePicker)dateGrid.FindName("month" + (--currentMonthIndex) + "End");
					lastEnd.SelectedDate = current.SelectedDate.Value.AddDays(-1);
				}
			}
		}
	}
}

