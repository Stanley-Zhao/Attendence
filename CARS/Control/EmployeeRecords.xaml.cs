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
using CARS.Pages;
using CARS.CARSService;

namespace CARS.Control
{
	public partial class EmployeeRecords : UserControl
	{
		public event SelectionChangedEventHandler SelectionChanged;
		List<User> source = new List<User>();
		private int currentSelectIndex = -1;

		public User SelectedItem
		{
			get { return (User)records.SelectedItem; }
		}

		public EmployeeRecords()
		{
			InitializeComponent();

			ClientInstance.Get().GetAllEmployeesCompleted += new EventHandler<GetAllEmployeesCompletedEventArgs>(client_GetAllEmployeesCompleted);

			GetData();

			records.SelectionChanged += new SelectionChangedEventHandler(records_SelectionChanged);
		}

		void records_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SelectionChanged != null)
			{
				SelectionChanged(sender, e);
			}
		}

		public void GetData()
		{
			loadingRow = 0;
			currentSelectIndex = records.SelectedIndex;
			ClientInstance.ShowSpinner("Loading Employees");
			ClientInstance.Get().GetAllEmployeesAsync();
		}

		private int loadingRow = 0;
		private void client_GetAllEmployeesCompleted(object sender, GetAllEmployeesCompletedEventArgs e)
		{	
			Logger.Instance().Log(MessageType.Information, "Get All Employee Completed");
			if (ErrorHandler.Handle(e.Error))
				return;
			List<Employee> employees = new List<Employee>();
			
			if (e.Result != null)
				employees = e.Result.ToList<Employee>();
			else
				Logger.Instance().Log(MessageType.Error, "Result is NULL");

			source.Clear();
			for (int i = 0; i < employees.Count; i++)
			{
				employees[i].Password = CryptographyStuff.AES_DecryptString(employees[i].Password);
				User user = new User(employees[i]);
				user.Index = i + 1;
				source.Add(user);
			}

			records.ItemsSource = null;
			records.ItemsSource = source;

			records.SelectedIndex = currentSelectIndex;

			records.LoadingRow += new EventHandler<DataGridRowEventArgs>(records_LoadingRow);

			//ClientInstance.HideSpinner();
		}

		void records_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			loadingRow++;
			if (loadingRow == source.Count)
			{
				ClientInstance.HideSpinner();
				records.LoadingRow -= new EventHandler<DataGridRowEventArgs>(records_LoadingRow);
			}
		}

		private void gridPanel_LayoutUpdated(object sender, EventArgs e)
		{
			this.Width = Application.Current.Host.Content.ActualWidth - 600;
			this.MaxHeight = Application.Current.Host.Content.ActualHeight - 110;
		}
	}
}
