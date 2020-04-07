using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO.ViewModel.IFactoryInterfaces;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
	/// <summary> Main window view model. </summary>
	/// <seealso cref="BaseViewModel"/>
	/// <seealso cref="System.IDisposable"/>
	public sealed class MainWindowViewModel : BaseViewModel, IDisposable
	{
		/// <summary> The get about relay command. </summary>
		private ICommand _getAboutRelayCommand;
		/// <summary> The get history relay command. </summary>
		private ICommand _getHistoryRelayCommand;

		private readonly ELLDevicesViewModel _ellDevice = new ELLDevicesViewModel();

		public MainWindowViewModel(IEnumerable<string> args)
		{
			foreach (string s in args)
			{
				if (s.ToUpper().StartsWith("S:"))
				{
					char c;
					if(char.TryParse(s.Substring(2), out c))
					{
						if ((c >= '0') && (c <= 'F'))
						{
							_ellDevice.MinSearchLimit = c;
						}
					}
				}
				if (s.ToUpper().StartsWith("E:"))
				{
					char c;
					if (char.TryParse(s.Substring(2), out c))
					{
						if ((c >= '0') && (c <= 'F'))
						{
							_ellDevice.MaxSearchLimit = c;
						}
					}
				}
			}
		}

		#region IDisposable
		private bool disposed;

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				// Suppress finalization of this disposed instance. 
				if (disposing)
				{
					GC.SuppressFinalize(this);
				}
			}
		}

		/// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting
		/// unmanaged resources. </summary>
		/// <seealso cref="M:System.IDisposable.Dispose()"/>
		public void Dispose()
		{
			if (!disposed)
			{
				// Dispose of resources held by this instance.
				Dispose(true);
			}
		}

		// Disposable types implement a finalizer.
		~MainWindowViewModel()
		{
			Dispose(false);
		}
		#endregion

		/// <summary> Window loaded. </summary>
		public void WindowLoaded()
		{
		}

		/// <summary> Window closing. </summary>
		public void WindowClosing()
		{
		}

		/// <summary> Closeds this object. </summary>
		public void Closed()
		{
		}

		/// <summary> Determines whether this instance [can shut down]. </summary>
		/// <returns> <c>true</c> if this instance [can shut down]; otherwise, <c>false</c>. </returns>
		public bool CanShutDown()
		{
			_ellDevice.Disconnect();
			return true;
		}

		/// <summary> Gets or sets the about box factory. </summary>
		/// <value> The about box factory. </value>
		public IAboutBoxFactory AboutBoxFactory { get; set; }

		/// <summary> Gets or sets the history factory. </summary>
		/// <value> The history factory. </value>
		public IShowHistoryFactory ShowHistoryFactory { get; set; }

		/// <summary> Gets the click Run > Continue command handler. </summary>
		/// <value> The click about command. </value>
		public ICommand ClickAboutCommand { get { return _getAboutRelayCommand ?? (_getAboutRelayCommand = new RelayCommand(About)); } }

		/// <summary> Gets the click Run > Continue command handler. </summary>
		/// <value> The click history command. </value>
		public ICommand ClickHistoryCommand { get { return _getHistoryRelayCommand ?? (_getHistoryRelayCommand = new RelayCommand(ShowHistory)); } }

		/// <summary> Shows about for this application. </summary>
		private void About()
		{
			AboutBoxFactory.ShowAboutBox();
		}

		/// <summary> Shows the history. </summary>
		private void ShowHistory()
		{
			Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("Thorlabs.Elliptec.ELLO.History.txt");
			if (resource != null)
			{
				var textStreamReader = new StreamReader(resource);
				string history = textStreamReader.ReadToEnd();
				ShowHistoryFactory.ShowHistory(history);
				return;
			}
			MessageBox.Show("Error accessing resources!");
		}

		/// <summary> Gets the view mode. </summary>
		/// <value> The view mode. </value>
		public ELLDevicesViewModel Display
		{
			get { return _ellDevice; }
		}

	}
}
