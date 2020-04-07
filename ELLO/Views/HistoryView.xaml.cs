using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Thorlabs.Elliptec.ELLO.ViewModel;

// namespace: Thorlabs.Elliptec.ELLO.Views
//
// summary:	Provides the UI for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.Views
{
	/// <summary>Interaction logic for History.xaml</summary>
	public partial class HistoryView : Window
	{
		/// <summary>Initializes a new instance of the <see cref="HistoryView"/> class.</summary>
		/// <param name="parent">The parent.</param>
		/// <param name="historyViewModel">The history view model.</param>
		public HistoryView(Window parent, HistoryViewModel historyViewModel)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				InitializeComponent();
				return;
			}
			try
			{
				InitializeComponent();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}

			DataContext = historyViewModel;
			Owner = parent;
		}
	}
}
