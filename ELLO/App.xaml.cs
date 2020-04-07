using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Shell;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO.ViewModel;
using Thorlabs.Elliptec.ELLO.Views;

namespace Thorlabs.Elliptec.ELLO
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application, IDisposable
	{
		private MainWindow _mainWindow;
		private MainWindowViewModel _mainWindowViewModel;

		#region IDisposable
		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				// Dispose of resources held by this instance. 
				_mainWindowViewModel.Dispose();

				disposed = true;
				// Suppress finalization of this disposed instance. 
				if (disposing)
				{
					GC.SuppressFinalize(this);
				}
			}
		}

		public void Dispose()
		{
			if (!disposed)
			{
				// Dispose of resources held by this instance.
				Dispose(true);
			}
		}

		// Disposable types implement a finalizer.
		~App()
		{
			Dispose(false);
		}
		#endregion

		/// <summary>Gets or sets a value indicating whether [application busy].</summary>
		/// <value><c>true</c> if [application busy]; otherwise, <c>false</c>.</value>
		public static bool ApplicationBusy
		{
			get
			{
				MainWindow wnd = Current.MainWindow as MainWindow;
				if (wnd != null)
				{
					MainWindowViewModel view = wnd.DataContext as MainWindowViewModel;
					if (view != null)
					{
					}
				}
				return false;
			}
			set
			{
				MainWindow wnd = Current.MainWindow as MainWindow;
				if (wnd != null)
				{
					MainWindowViewModel view = wnd.DataContext as MainWindowViewModel;
					if (view != null)
					{
					}
				}
			}
		}

		/// <summary> Gets the company. </summary>
		/// <value> The company. </value>
		public static string ApplicationName
		{
			get { return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title; }
		}

		/// <summary> Gets the company. </summary>
		/// <value> The company. </value>
		public static string Company
		{
			get { return ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company; }
		}

		/// <summary> Gets the full pathname of the setting directory. </summary>
		/// <value> The full pathname of the setting file. </value>
		public static string SettingPath(string filename, string subDirectory)
		{
			if (string.IsNullOrEmpty(subDirectory))
			{
				return Path.Combine(SettingsPath, filename);
			}
			return Path.Combine(SettingsPath, subDirectory, filename);
		}

		/// <summary> Gets the full pathname of the setting directory. </summary>
		/// <value> The full pathname of the setting file. </value>
		public static string SettingPath(string filename)
		{
			return Path.Combine(SettingsPath, filename);
		}

		/// <summary> Gets the full pathname of the settings file. </summary>
		/// <value> The full pathname of the settings file. </value>
		public static string SettingsPath
		{
			get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Company); }
		}

		/// <summary> Gets the full pathname of the logging directory. </summary>
		/// <value> The full pathname of the log file. </value>
		public static string LogPath(string filename)
		{
			return Path.Combine(SettingsPath, "Logs", filename);
		}

		private void ApplicationStartup(object sender, StartupEventArgs e)
		{
			DoHandleUnexpectedExceptions = true;
			try
			{
				//#if DEBUG
				//#else
				AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
			    //#endif

			    FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

			    CommonUserSettings.InitializeCommonSettings(ELLO.Properties.Settings.Default);
				// setup the JumpList features before testing for single application as otherwise the JumpList can be reset
				JumpList jumpList = JumpList.GetJumpList(Current);

				JumpTask jt = new JumpTask
				{
					Title = "Edit application settings",
					ApplicationPath = SettingPath(ApplicationName + ".Config", App.ApplicationName),
					IconResourcePath = @"c:\windows\notepad.exe",
					Description = "Edit the application configuration file to customise settings",
					CustomCategory = "Customise"
				};
				jumpList.JumpItems.Add(jt);

				jt = new JumpTask
				{
					Arguments = LogPath(ApplicationName + ".Log"),
					Title = "Open MotionControlManager log",
					ApplicationPath = @"c:\windows\notepad.exe",
					IconResourcePath = @"c:\windows\notepad.exe",
					Description = "Open the diagnostics log",
					CustomCategory = "Logging"
				};
				jumpList.JumpItems.Add(jt);

				jumpList.Apply();

				// Create the View Model for the Main Window
				_mainWindowViewModel = new MainWindowViewModel(e.Args);

				// Set it as the DataContext for Binding and Commanding
				_mainWindow = new MainWindow(_mainWindowViewModel);

				_mainWindow.Show();
				Thread.Sleep(500);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Caught exception: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private void ApplicationExit(object sender, ExitEventArgs e)
		{
			Dispose();
		}

		/// <summary>Gets or sets a value indicating whether [do handle].</summary>
		/// <value><c>true</c> if [do handle]; otherwise, <c>false</c>.</value>
		public bool DoHandleUnexpectedExceptions { get; set; }

		private void ApplicationDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			//#if DEBUG
			//            //If you do not set e.Handled to true, the application will close due to crash.
			//            MessageBox.Show("Application is going to close!\r\nPlease review error log", "Uncaught Exception");
			//            e.Handled = false;
			//#else
			if (DoHandleUnexpectedExceptions)
			{
				//Handling the exception within the UnhandledException handler.
			    if (e.Exception.InnerException != null)
			    {
			        MessageBox.Show( e.Exception.InnerException.Message, "Unhandled Error Detected");
			    }
			    else
			    {
			        MessageBox.Show(e.Exception.Message, "Unhandled Error Detected");
			    }
                e.Handled = true;
			}
			else
			{
				//If you do not set e.Handled to true, the application will close due to crash.
				MessageBox.Show("Application is going to close!\r\nPlease revied error log", "Uncaught Exception");
				e.Handled = false;
			}
			//#endif
		}

		private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
			{
				MessageBox.Show(ex.Message, "Uncaught Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				MessageBox.Show(e.ToString(), "Uncaught Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

	}
}
