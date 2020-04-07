using System;
using System.Diagnostics;
using System.Windows.Input;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary>
	/// A command whose sole purpose is to 
	/// relay its functionality to other
	/// objects by invoking delegates. The
	/// default return value for the CanExecute
	/// method is 'true'.
	/// </summary>
	public class RelayCommand<T> : ICommand
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="execute">The execute.</param>
		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}

			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion // Constructors

		#region ICommand Members

		/// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		/// <returns>true if this command can be executed; otherwise, false.</returns>
		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute((T)parameter);
		}

		/// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (_canExecute != null)
				{
					CommandManager.RequerySuggested += value;
				}
			}
			remove
			{
				if (_canExecute != null)
				{
					CommandManager.RequerySuggested -= value;
				}
			}
		}

		/// <summary>Defines the method to be called when the command is invoked.</summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		public void Execute(object parameter)
		{
			_execute((T)parameter);
		}

		#endregion // ICommand Members

		#region Fields

		private readonly Action<T> _execute;
		private readonly Predicate<T> _canExecute;

		#endregion // Fields
	}

	/// <summary>
	/// A command whose sole purpose is to 
	/// relay its functionality to other
	/// objects by invoking delegates. The
	/// default return value for the CanExecute
	/// method is 'true'.
	/// </summary>
	public class RelayCommand : ICommand
	{
		#region Constructors

		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		public RelayCommand(Action execute)
			: this(execute, null)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}

			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion // Constructors

		#region ICommand Members

		/// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		/// <returns>true if this command can be executed; otherwise, false.</returns>
		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute();
		}

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (_canExecute != null)
				{
					CommandManager.RequerySuggested += value;
				}
			}
			remove
			{
				if (_canExecute != null)
				{
					CommandManager.RequerySuggested -= value;
				}
			}
		}

		/// <summary>Defines the method to be called when the command is invoked.</summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		public void Execute(object parameter)
		{
			_execute();
		}

		#endregion // ICommand Members

		#region Fields

		private readonly Action _execute;
		private readonly Func<bool> _canExecute;

		#endregion // Fields
	}
}
