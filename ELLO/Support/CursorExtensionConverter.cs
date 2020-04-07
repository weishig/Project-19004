using System;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary>Wait Cursor converter based on Boolean values</summary>
	public class CursorExtensionConverter : MarkupExtension, IValueConverter
	{
		/// <summary>Converts a value.</summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Cursor cursor = Cursors.Arrow;
			if (value is bool && (bool)value)
			{
				cursor = Cursors.Wait;
			}

			return cursor;
		}

		/// <summary>Converts a value.</summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		/// <summary>When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.</summary>
		/// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
		/// <returns>The object value to set on the property where the extension is applied.</returns>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Instance;
		}

		private static readonly CursorExtensionConverter Instance = new CursorExtensionConverter();
	}
}
