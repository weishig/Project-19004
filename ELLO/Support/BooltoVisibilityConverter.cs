using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary>Boolena to Visibility converter</summary>
	public class BoolToVisibilityConverter : IValueConverter
	{

		/// <summary>Converts a boolena value to a visibility value.</summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			if (targetType == typeof(Visibility))
			{
				bool visible = System.Convert.ToBoolean(value, culture);
				if (InvertVisibility)
				{
					visible = !visible;
				}
				return visible ? Visibility.Visible : Visibility.Collapsed;
			}
			throw new InvalidOperationException("Converter can only convert to value of type Visibility.");
		}

		/// <summary>Converts a value.</summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			if(value is Visibility)
			{
				return (Visibility)value == Visibility.Visible;
			}
			throw new InvalidOperationException("Converter cannot convert back.");
		}

		/// <summary>Gets or sets a value indicating whether [invert visibility].</summary>
		/// <value>
		///   <c>true</c> if [invert visibility]; otherwise, <c>false</c>.
		/// </value>
		public Boolean InvertVisibility { get; set; }

	}
}
