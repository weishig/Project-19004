using System;
using System.Windows.Threading;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary> View model dispatcher. </summary>
	public class ViewModelDispatcher
	{
		private readonly Dispatcher _dispatcher;

		/// <summary> Initializes a new instance of the <see cref="ViewModelDispatcher"/> class.</summary>
		public ViewModelDispatcher()
		{
			_dispatcher = Dispatcher.CurrentDispatcher;
		}

		/// <summary>Gets the dispatcher used by this view model to execute actions on the thread it is associated with.</summary>
		/// <value>
		/// The <see cref="System.Windows.Threading.Dispatcher"/> used by this view model to 
		/// execute actions on the thread it is associated with. 
		/// The default value is the <see cref="System.Windows.Threading.Dispatcher.CurrentDispatcher"/>.
		/// </value>
		public Dispatcher Dispatcher
		{
			get { return _dispatcher; }
		}

		/// <summary> Executes the specified <paramref name="action"/> synchronously on the thread 
		/// the <see cref="ViewModelDispatcher"/> is associated with. </summary>
		/// <param name="action">The <see cref="Action"/> to execute.</param>
		public void Execute(Action action)
		{
			if (Dispatcher.CheckAccess())
			{
				action.Invoke();
			}
			else
			{
				Dispatcher.Invoke(DispatcherPriority.DataBind, action);
			}
		}
		/// <summary> Executes the specified <paramref name="action"/> synchronously on the thread 
		/// the <see cref="ViewModelDispatcher"/> is associated with. </summary>
		/// <param name="action">The <see cref="Action"/> to execute.</param>
		public void BeginInvoke(Action action)
		{
			if (Dispatcher.CheckAccess())
			{
				action.Invoke();
			}
			else
			{
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, action);
			}
		}
	}
}
