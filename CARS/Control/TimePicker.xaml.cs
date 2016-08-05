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
	public partial class TimePicker : UserControl
	{
		public delegate void DateChangeHandler(object sender, SelectionChangedEventArgs e);
		public delegate void TimeChangeHandler(object sender, SelectionChangedEventArgs e);

		public event DateChangeHandler DateChangeEvent;
		public event TimeChangeHandler TimeChangeEvent;

		public TimePicker()
		{
			InitializeComponent();

			date.SelectedDate = DateTime.Now;
		}

		public StartEnd Type
		{
			set
			{
				switch (value)
				{
					case StartEnd.Start:
						ComboBoxItem _9 = new ComboBoxItem();
						_9.Content = "09:00";
						time.Items.Add(_9);
						ComboBoxItem _13_1 = new ComboBoxItem();
						_13_1.Content = "13:00";
						time.Items.Add(_13_1);
						time.SelectedIndex = 0;
						break;
					case StartEnd.End:
						ComboBoxItem _13_2 = new ComboBoxItem();
						_13_2.Content = "13:00";
						time.Items.Add(_13_2);
						ComboBoxItem _17 = new ComboBoxItem();
						_17.Content = "17:00";
						time.Items.Add(_17);
						time.SelectedIndex = 1;
						break;
					default:
						break;
				}
			}
		}

		public void Clean()
		{
			date.SelectedDate = DateTime.Now;
			if (this.Name.ToLower().Contains("start"))
				time.SelectedIndex = 0;
			else
				time.SelectedIndex = time.Items.Count - 1;
		}

		public DateTime Date
		{
			set
			{
				date.SelectedDate = value;
			}

			get
			{
				if (date.SelectedDate.HasValue)
				{
					return (DateTime)date.SelectedDate;
				}
				return Convert.ToDateTime("2000-1-1");
			}
		}

		public int Time
		{
			set
			{
				if (value == 9 || value == 13 || value == 17)
				{
					for (int i = 0; i<time.Items.Count; i++ )
					{
						if (int.Parse(((ComboBoxItem)time.Items[i]).Content.ToString().Split(':')[0]) == value)
						{
							time.SelectedIndex = i;
							break;
						}
					}
				}
			}

			get
			{
				ComboBoxItem item = (ComboBoxItem)time.SelectedItem;
				return int.Parse(item.Content.ToString().Split(':')[0]);
			}
		}

		public DateTime SelectDateTime
		{
			get { return new DateTime(Date.Year, Date.Month, Date.Day, Time, 0, 0); }
			set {
				Date = value;
				Time = value.Hour;
			}
		}

		public bool HasValue
		{
			get { return date.SelectedDate.HasValue && time.SelectedIndex != -1; }
		}

		private void date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			DateChangeEvent(sender, e);
		}

		private void time_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TimeChangeEvent(sender, e);
		}
	}
}
