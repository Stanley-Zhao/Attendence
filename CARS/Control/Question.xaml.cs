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

namespace CARS.Control
{
	public partial class Question : ChildWindow
	{
		private string mQuestion;
		public string QuestionValue
		{
			get { return mQuestion; }
			set
			{
				mQuestion = value;
				question.Text = value;
			}
		}

		public Question(string pQuestion)
		{
			InitializeComponent();

			QuestionValue = pQuestion;
		}

		private void YesButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Do something here.");
			this.DialogResult = true;
		}

		private void NoButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		public static void ShowQuestion(string value)
		{
			new Question(value).Show();
		}
	}
}

