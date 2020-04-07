using System;
using System.Windows;
using System.Windows.Input;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary>Base View Model to handle common window functions</summary>
	[Serializable]
	public class BaseViewModel : ObservableObject
	{
		[NonSerialized]
		private readonly CommandBindingCollection _commandBindings = new CommandBindingCollection();

		#region WindowPropertys
		/// <summary>Shows the message box.</summary>
		/// <param name="message">The message.</param>
		public void ShowMessageBox(string message)
		{
			MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		/// <summary>Gets the close handler.</summary>
		public ICommand Close
		{
			get { return new RelayCommand(CloseApplication); }
		}

		/// <summary>Gets the Maximize handler.</summary>
		public ICommand Maximize
		{
			get { return new RelayCommand(MaximizeApplication); }
		}

		/// <summary>Gets the Minimize handler.</summary>
		public ICommand Minimize
		{
			get { return new RelayCommand(MinimizeApplication); }
		}

		/// <summary>Gets the DragMove handler.</summary>
		public ICommand DragMove
		{
			get { return new RelayCommand(DragMoveCommand); }
		}

		/// <summary>Gets the Restart handler.</summary>
		public ICommand Restart
		{
			get { return new RelayCommand(RestartCommand); }
		}

		private static void RestartCommand()
		{
			Application.Current.Shutdown();
		}

		private static void DragMoveCommand()
		{
			Application.Current.MainWindow.DragMove();
		}

		private static void CloseApplication()
		{
			Application.Current.Shutdown();
		}

		private static void MaximizeApplication()
		{
			Application.Current.MainWindow.WindowState = (Application.Current.MainWindow.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
		}

		private static void MinimizeApplication()
		{
			if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
			{
				Application.Current.MainWindow.Opacity = 1;
				Application.Current.MainWindow.WindowState = WindowState.Normal;
			}
			else
			{
				Application.Current.MainWindow.Opacity = 0;
				Application.Current.MainWindow.WindowState = WindowState.Minimized;
			}
		}

		/// <summary>Gets the command bindings.</summary>
		public CommandBindingCollection CommandBindings
		{
			get { return _commandBindings; }
		}
		#endregion
	}
}
