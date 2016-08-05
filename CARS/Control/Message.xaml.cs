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
using System.Windows.Media.Imaging;

namespace CARS.Control
{
	public partial class Message : ChildWindow
	{
		private MessageType mType;
		private string mMessage;
		private ImageSource information;
		private ImageSource warning;
		private ImageSource error;

		public MessageType Type
		{
			get { return mType; }
			set
			{
				mType = value;
				UpdateICON();
			}
		}

		private void UpdateICON()
		{
			icon.Source = mType == MessageType.Information ? information : (mType == MessageType.Error ? error : warning);
		}

		public string MessageValue
		{
			get { return mMessage; }
			set
			{
				mMessage = value;
				message.Text = value;
			}
		}

		public Message()
			: this(MessageType.Information, "Message Box")
		{

		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		public Message(MessageType pType, string pMessage)
		{
			InitializeComponent();

			information = new BitmapImage(new Uri("/CARS;component/Images/Info32x32.png", UriKind.Relative));
			error = new BitmapImage(new Uri("/CARS;component/Images/Error32x32.png", UriKind.Relative));
			warning = new BitmapImage(new Uri("/CARS;component/Images/Warning32x32.png", UriKind.Relative));

			MessageValue = pMessage;
			mType = pType;

			this.Title = mType.ToString();
			UpdateICON();
		}

		public static void Error(string value)
		{
			new Message(MessageType.Error, value).Show();
		}

		public static void Information(string value)
		{
			new Message(MessageType.Information, value).Show();
		}

		public static void Warning(string value)
		{
			new Message(MessageType.Warning, value).Show();
		}
	}
}

