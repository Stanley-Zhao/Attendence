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
using System.Threading;
using System.Windows.Threading;

namespace CARS.Control
{
	public partial class Spinner : UserControl
	{
		private double hourAngle = 0d;
		private double minAngle = 0d;
		private double secAngle = 0d;
		private RotateTransform hourRt;
		private RotateTransform minRt;
		private RotateTransform secRt;
		private SolidColorBrush scb;
		private string textValue = "Loading Data";

		public string TextValue
		{
			get { return textValue; }
			set { textValue = value; }
		}

		public Spinner()
		{
			InitializeComponent();

			SetClock();
			SetColorLine(true);

			// start animation
			story1.Begin();
			story2.Begin();
			story3.Begin();
			story4.Begin();
			story5.Begin();
			story6.Begin();
			story7.Begin();
			story8.Begin();
			story9.Begin();
			story10.Begin();

			DispatcherTimer dt = new DispatcherTimer();
			dt.Interval = new TimeSpan(1000);
			dt.Tick += new EventHandler(dt_Tick);
			dt.Start();
		}

		void dt_Tick(object sender, EventArgs e)
		{
			SetClock();
			SetColorLine(false);
		}

		private void SetColorLine(bool firstTime)
		{
			if (firstTime)
			{
				if (scb == null)
					scb = new SolidColorBrush();
			}

			switch (DateTime.Now.Second % 4)
			{
				case 0:
					scb.Color = Color.FromArgb(255, 198, 6, 33); // red
					break;
				case 1:
					scb.Color = Color.FromArgb(255, 57, 132, 49); // green
					break;
				case 2:
					scb.Color = Color.FromArgb(255, 247, 165, 41); // yellow
					break;
				case 3:
					scb.Color = Color.FromArgb(255, 8, 115, 181); // blue
					break;
				default:
					break;
			}

			if (firstTime)
			{
				rec1.Fill = scb;
				rec2.Fill = scb;
				rec3.Fill = scb;
				rec4.Fill = scb;
				rec5.Fill = scb;
				rec6.Fill = scb;
				rec7.Fill = scb;
				rec8.Fill = scb;
				rec9.Fill = scb;
				rec10.Fill = scb;
				loadData.Foreground = scb;
			}
		}

		private void SetClock()
		{
			DateTime now = DateTime.Now;
			hourAngle = (now.Hour % 12) * 30 + now.Minute / 2;
			minAngle = now.Minute * 6;
			secAngle = now.Second * 6;

			if (hourRt == null)
			{
				hourRt = new RotateTransform();
				hourRt.CenterX = 3;
				hourRt.CenterY = 25;
				hourHand.RenderTransform = hourRt;
			}
			hourRt.Angle = hourAngle;

			if (minRt == null)
			{
				minRt = new RotateTransform();
				minRt.CenterX = 2;
				minRt.CenterY = 34;
				minHand.RenderTransform = minRt;
			}
			minRt.Angle = minAngle;

			if (secRt == null)
			{
				secRt = new RotateTransform();
				secRt.CenterX = 2;
				secRt.CenterY = 49;
				secondHand.RenderTransform = secRt;
			}
			secRt.Angle = secAngle;

			loadData.Text = textValue + " ." + GotDots(now);
		}

		private string GotDots(DateTime now)
		{
			string result = string.Empty;

			for (int i = 0; i < (now.Second % 3); i++)
				result += ".";

			return result;
		}
	}
}
