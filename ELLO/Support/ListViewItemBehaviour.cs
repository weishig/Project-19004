using System;
using System.Windows;
using System.Windows.Controls;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary> Tree view item behavior. </summary>
	public static class ListViewItemBehavior
	{
		#region IsBroughtIntoViewWhenSelected
		/// <summary> Gets the is brought into view when selected. </summary>
		/// <param name="listViewItem"> The tree view item. </param>
		/// <returns> Success. </returns>
		public static bool GetIsBroughtIntoViewWhenSelected(ListViewItem listViewItem)
		{
			return (bool)listViewItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);
		}

		/// <summary> Sets the is brought into view when selected. </summary>
		/// <param name="listViewItem"> The list view item. </param>
		/// <param name="value">	    <c>true</c> to value. </param>
		public static void SetIsBroughtIntoViewWhenSelected(ListViewItem listViewItem, bool value)
		{
			listViewItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
		}

		/// <summary> The is brought into view when selected property. </summary>
		public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty = DependencyProperty.RegisterAttached("IsBroughtIntoViewWhenSelected", typeof(bool), typeof(ListViewItemBehavior), new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

		/// <summary> Raises the dependency property changed event. </summary>
		/// <param name="depObj"> The dep object. </param>
		/// <param name="e">	  Event information to send to registered event handlers. </param>
		public static void OnIsBroughtIntoViewWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			ListViewItem item = depObj as ListViewItem;
			if (item != null)
			{
				if (e.NewValue is bool)
				{
					if ((bool)e.NewValue)
					{
						item.Selected += OnListViewItemSelected;
					}
					else
					{
						item.Selected -= OnListViewItemSelected;
					}
				}
			}
		}

		/// <summary> Raises the routed event. </summary>
		/// <param name="sender"> Source of the event. </param>
		/// <param name="e">	  Event information to send to registered event handlers. </param>
		public static void OnListViewItemSelected(object sender, RoutedEventArgs e)
		{
			// Only react to the Selected event raised by the TreeViewItem
			// whose IsSelected property was modified. Ignore all ancestors
			// who are merely reporting that a descendant's Selected fired.
			if (!Object.ReferenceEquals(sender, e.OriginalSource))
			{
				return;
			}

			ListViewItem item = e.OriginalSource as ListViewItem;
			if (item != null)
			{
				item.BringIntoView();
			}
		}

		#endregion // IsBroughtIntoViewWhenSelected
	}
}
