using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Thorlabs.Elliptec.ELLO.ViewModel;
using Thorlabs.Elliptec.ELLO.Views.Factories;

// namespace: Thorlabs.Elliptec.ELLO.Views
//
// summary:	Provides the UI for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainWindowViewModel _viewModel;

		/// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
		/// <param name="viewModel">MotionControlManager View Model</param>
		public MainWindow(MainWindowViewModel viewModel)
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

			_viewModel = viewModel;

			DataContext = _viewModel;
			viewModel.AboutBoxFactory = new AboutBoxFactory(this);
			viewModel.ShowHistoryFactory = new ShowHistoryFactory(this);
		}

		private void WindowClosed(object sender, System.EventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}
			_viewModel.Closed();
		}

		private void WindowClosing(object sender, CancelEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}
			if (!_viewModel.CanShutDown())
			{
				MessageBox.Show(this, "Please stop all logging before shutting down", "Application Closing", MessageBoxButton.OK, MessageBoxImage.Information);
				e.Cancel = true;
			}
		}

		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}
			_viewModel.WindowLoaded();
		}
	}
}
