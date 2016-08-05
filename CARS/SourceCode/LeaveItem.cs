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
using CARS.Control;
using System.Collections.Generic;
using System.ComponentModel;
using CARS.CARSService;

namespace CARS.SourceCode
{
	public class LeaveItem : INotifyPropertyChanged
	{
		private List<DateRecords> list;
		private LeaveInfo mLI;

		public LeaveInfo @LeaveInfo
		{
			get { return mLI; }
			set
			{
				mLI = value;

				foreach (TimeDurationInfo tdi in mLI.TimeDurationInfoList)
				{
					DateRecords dr = new DateRecords(tdi);
					list.Add(dr);
				}
			}
		}

		public int Index { get; set; }
		public LeaveType Type { get { return mLI.Type; } set { mLI.Type = value; } }
		public DateTime Start { get { return mLI.FirstStartTime; } set { mLI.FirstStartTime = value; } }
		public DateTime End { get { return mLI.LastEndTime; } set { mLI.LastEndTime = value; } }
		public string Reason { get { return mLI.Reason; } set { mLI.Reason = value; } }
		public int Hours { get { return mLI.Hours; } set { mLI.Hours = value; } }
		public LeaveStatus Status { get { return mLI.Status; } set { mLI.Status = value; } }
		public string Submitter { get; set; }
		public string ApprovedBy { get { return mLI.Manager.FirstName + " " + mLI.Manager.LastName ; } }
		private bool m_IsSelect;
        public bool IsSelect 
        { 
            get { return m_IsSelect; }
            set
            {
                m_IsSelect = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsSelect"));
            }
        }
		public string Description { get { return mLI.Description; } set { mLI.Description = value; } }

		public string IndexValue { get { return Index.ToString("000"); } }
		public string StartValue { get { return mLI.FirstStartTime.ToString("yyyy-MM-dd"); } }
		public string EndValue { get { return mLI.LastEndTime.ToString("yyyy-MM-dd"); } }
		public string TypeValue
		{
			get
			{
				if (Type == null)
				{
					return "";
				}
				else
				{
					return Type.Name;
				}
			}
		}
		public string StatusValue
		{
			get
			{
				string result = Status.ToString();
				//switch (Status)
				//{
				//    case WebService.LeaveStatus.Applying:
				//    case WebService.LeaveStatus.Cancelling:
				//        result = "Pending";
				//        break;
				//}
				return result;
			}
		}

		public SolidColorBrush StatusColor
		{
			get 
			{
				if (Status == LeaveStatus.Applying)
					return new SolidColorBrush(Colors.Blue);
				return new SolidColorBrush(Colors.Black);
			}
		}

		public Visibility IsButtonVisiable
		{
			get
			{
				// Don't need Cancel button anymore.
				//Visibility result = Visibility.Collapsed;

				//switch (Status)
				//{
				//    case CARS.WebService.LeaveStatus.Approved:
				//    case CARS.WebService.LeaveStatus.PendingForResumption:
				//    case CARS.WebService.LeaveStatus.PendingForApply:
				//        result = Visibility.Visible;
				//        break;
				//}

				return Visibility.Collapsed;
			}
		}

		public List<DateRecords> List
		{
			get { return list; }
			set { list = value; }
		}

		public LeaveItem()
		{
			mLI = new LeaveInfo();
			mLI.TimeDurationInfoList = new System.Collections.ObjectModel.ObservableCollection<TimeDurationInfo>();
			mLI.FirstStartTime = DateTime.MaxValue;
			mLI.LastEndTime = DateTime.MinValue;
			list = new List<DateRecords>();
		}

		public LeaveItem(LeaveInfo li)
		{
			list = new List<DateRecords>();
			if (li.TimeDurationInfoList != null)
			{
				foreach (TimeDurationInfo tdi in li.TimeDurationInfoList)
				{
					DateRecords dr = new DateRecords(tdi);
					list.Add(dr);
				}

				mLI = li;
			}
		}
	
#region INotifyPropertyChanged Members

public event PropertyChangedEventHandler  PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, e);
        }

#endregion
}
}
