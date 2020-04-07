using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary> Provides a button with a bindable IsPressed property. It's a shame this isn't provided by WPF
	/// as standard! </summary>
	/// <seealso cref="T:System.Windows.Controls.Button"/>
	public class BindableButton : Button
	{
		/// <summary> The is pressed property. </summary>
		public new static readonly DependencyProperty IsPressedProperty =
		  DependencyProperty.Register("IsPressed", typeof(bool), typeof(BindableButton),
		  new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		/// <summary> Gets or sets a value indicating whether this object is pressed. </summary>
		/// <value> true if this object is pressed, false if not. </value>
		public new bool IsPressed
		{
			get { return (bool)GetValue(IsPressedProperty); }
			set { SetValue(IsPressedProperty, value); }
		}

		/// <summary> Called when the
		/// <see cref="P:System.Windows.Controls.Primitives.ButtonBase.IsPressed" /> property changes. </summary>
		/// <param name="e"> The data for
		/// 				 <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />. </param>
		/// <seealso cref="M:System.Windows.Controls.Primitives.ButtonBase.OnIsPressedChanged(DependencyPropertyChangedEventArgs)"/>
		protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnIsPressedChanged(e);

			IsPressed = (bool)e.NewValue;
		}
	}
}
