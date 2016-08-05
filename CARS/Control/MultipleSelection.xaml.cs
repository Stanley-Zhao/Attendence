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
	public partial class MultipleSelection : UserControl
	{
		public MultipleSelection()
		{
			InitializeComponent();
		}

		private void selectButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (ListBoxItem item in sourceList.SelectedItems)
			{
				ListBoxItem newItem = new ListBoxItem();
				newItem.Content = item.Content;
				selectedList.Items.Add(newItem);
				sourceList.Items.Remove(item);
			}
		}

		private void unSelectButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (ListBoxItem item in selectedList.SelectedItems)
			{
				ListBoxItem newItem = new ListBoxItem();
				newItem.Content = item.Content;
				sourceList.Items.Add(newItem);
				selectedList.Items.Remove(item);
			}
		}
	}
}
