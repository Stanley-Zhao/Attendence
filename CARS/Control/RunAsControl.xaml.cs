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
using CARS.CARSService;

namespace CARS.Control
{
	public partial class RunAsControl : ChildWindow
	{
		public event EventHandler SelectRunAsEvent;

		public User SelectedUser
		{
			get { return new User((Employee)((TreeViewItem)treeView.SelectedItem).DataContext); }
		}

		public RunAsControl(User rootManager)
		{
			InitializeComponent();

			treeView.Items.Clear();
			treeView.Items.Add(MakeTree(rootManager));
		}

		private TreeViewItem MakeTree(User rootManager)
		{
			TreeViewItem item = new TreeViewItem();
			item.Header = rootManager.FirstName + " " + rootManager.LastName;
			item.DataContext = rootManager.Employee;
			foreach (Employee leader in rootManager.Employee.Leaders)
			{
				item.Items.Add(MakeTree(new User(leader)));
			}

			return item;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (SelectRunAsEvent != null)
			{
				SelectRunAsEvent(sender, e);
			}
			this.DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		internal void SetRootManager(User rootManager)
		{
			treeView.Items.Clear();
			treeView.Items.Add(MakeTree(rootManager));
		}
	}
}