﻿using System;
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
using CARS.Pages;
using CARS.Control;
using System.Net.Browser;
using CARS.SourceCode;

namespace CARS
{
	public partial class App : Application
	{
		public App()
		{
			this.Startup += this.Application_Startup;
			this.Exit += this.Application_Exit;
			this.UnhandledException += this.Application_UnhandledException;

			InitializeComponent();
			Logger.Instance().Start();

			bool registerResult = WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
			bool httpsResult = WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
		}

		private ScrollViewer GoToPage(Page page)
		{
			ScrollViewer svRoot = new ScrollViewer();
			svRoot.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			svRoot.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			svRoot.Content = page;
			return svRoot;
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			try
			{
				//CARS.Control.CARSButton cbutton = new Control.CARSButton("goodButton", "Good", new Size(120, 40), Colors.Red, HorizontalAlignment.Left);
				//CARS.Control.Menu menu = new Control.Menu();                
				//Main main = new Main();				
				this.RootVisual = GoToPage(new Login(true));
			}
			catch (Exception ex)
			{
				Message.Error(ex.ToString());
			}
		}

		private void Application_Exit(object sender, EventArgs e)
		{

		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!System.Diagnostics.Debugger.IsAttached)
			{

				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}

		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}
