using System;
using System.Collections.Generic;
using System.Reflection;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary>Generic class for maintaining user settings in common libraries</summary>
	public static class CommonUserSettings
	{

		private static object _settings;

		private static Type _settingsType;
		private static MethodInfo _save;
		private static readonly Dictionary<string, PropertyInfo> _propertyInfo = new Dictionary<string, PropertyInfo>();

		/// <summary>Initializes the common settings from a settings object.</summary>
		/// <param name="settings">The settings.</param>
		public static void InitializeCommonSettings(object settings)
		{
			_settings = settings;

			_settingsType = settings.GetType();
			_save = _settingsType.GetMethod("Save");
		}

		private static PropertyInfo GetPropertyInfo(string propertyName)
		{
			if (_settingsType == null)
			{
				return null;
			}
			if (_propertyInfo.ContainsKey(propertyName))
			{
				return _propertyInfo[propertyName];
			}
			PropertyInfo propertyInfo = _settingsType.GetProperty(propertyName);
			_propertyInfo.Add(propertyName, propertyInfo);
			return propertyInfo;
		}

		/// <summary>Gets the Setting value.</summary>
		/// <typeparam name="T">setting type</typeparam>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>the setting value</returns>
		public static T ReadUserSetting<T>(string propertyName, T defaultValue)
		{
			PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
			return (propertyInfo == null) ? default(T) : (T)propertyInfo.GetValue(_settings, null);
		}

		/// <summary>Sets the property value.</summary>
		/// <typeparam name="T">setting type</typeparam>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="value">The value.</param>
		public static void WriteUserSetting<T>(string propertyName, T value)
		{
			PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
			if (propertyInfo != null)
			{
				propertyInfo.SetValue(_settings, value, null);
				if (_save != null)
				{
					_save.Invoke(_settings, null);
				}
			}
		}

		/// <summary>Resets the user settings.</summary>
		public static void ResetConfig()
		{
		}
	}
}
