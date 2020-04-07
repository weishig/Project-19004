using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary>
	/// This is the abstract base class for any object that provides property change notifications.  
	/// </summary>
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		#region Constructor

		#endregion // Constructor

		#region RaisePropertyChanged

		/// <summary>
		/// Raises this object's property changed.
		/// </summary>
		/// <typeparam name="T">The type of the action</typeparam>
		/// <param name="action">The action.</param>
		protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
		{
			string propertyName = GetPropertyName(action);
			RaisePropertyChanged(propertyName);
		}

		/// <summary> Gets a property name. </summary>
		/// <typeparam name="T"> Generic type parameter. </typeparam>
		/// <param name="action"> The action. </param>
		/// <returns> The property name&lt; t&gt; </returns>
		protected static string GetPropertyName<T>(Expression<Func<T>> action)
		{
			MemberExpression expression = (MemberExpression)action.Body;
			string propertyName = expression.Member.Name;
			return propertyName;
		}

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value.</param>
		protected void RaisePropertyChanged(string propertyName)
		{
			VerifyPropertyName(propertyName);

			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
				handler(this, e);
			}
		}

		#endregion // RaisePropertyChanged

		#region Debugging Aides

		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This 
		/// method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// If you raise PropertyChanged and do not specify a property name,
			// all properties on the object are considered to be changed by the binding system.
			if (string.IsNullOrEmpty(propertyName))
				return;

			// Verify that the property name matches a real,  
			// public, instance property on this object.
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				string msg = "Invalid property name: " + propertyName;

				if (ThrowOnInvalidPropertyName)
				{
					throw new ArgumentException(msg);
				}
				Debug.Fail(msg);
			}
		}

		/// <summary>
		/// Returns whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might 
		/// override this property's getter to return true.
		/// </summary>
		protected virtual bool ThrowOnInvalidPropertyName { get; set; }

		#endregion // Debugging Aides

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Raised when a property on this object has a new value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion // INotifyPropertyChanged Members
	}
}
