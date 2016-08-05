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
using CARS.SourceCode;
using CARS.CARSService;

namespace CARS.Control
{
	public partial class NumberColorBar : UserControl
	{
		// hours
		private int totalLeave = 120;
		// hours
		private int askedLeave = 0;
		// hours
		private int left = 120;
		//private string text = "";
		private LeaveType type;

		public LeaveType Type
		{
			get { return type; }
			set
			{
				type = value;
				UpdateContent();
			}
		}

		public int TotalLeave
		{
			get { return totalLeave; }
			set
			{
				if (value > askedLeave)
				{
					totalLeave = value;
					DoUpdate();
				}
			}
		}

		public int AskedLeave
		{
			get { return askedLeave; }
			set
			{
				if (value < totalLeave)
				{
					askedLeave = value;
					DoUpdate();
				}
			}
		}

		public string Text
		{
			get { return (string)label.Content; }
			set { label.Content = value; }
		}

		private void DoUpdate()
		{
			left = totalLeave - askedLeave;
			UpdateBarWidth();
			UpdateContent();
		}

		private void UpdateBarWidth()
		{
			bar.Width = ((float)askedLeave / (float)totalLeave) * 200;
		}

		private void UpdateContent()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(type.ToString());
			sb.Append(" Leave: ");
			sb.Append(left.ToString());
			sb.Append(left == 1 ? " hour (" : " hours (");
			sb.Append(((float)left / 8f).ToString("0.0"));
			sb.Append(left == 8 ? " day) left" : " days) left");
			label.Content = sb.ToString();
		}

		public NumberColorBar(LeaveType typePara, int totalPara)
		{
			InitializeComponent();

			type = typePara;
			totalLeave = totalPara;

			DoUpdate();
		}
	}
}
