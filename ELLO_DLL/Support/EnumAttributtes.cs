using System;
using System.Collections.Generic;
using System.Reflection;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO_DLL.Support
{
	/// <summary>A general purpose String Value attribute which can be applied to any method, property or field</summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class StringValueAttribute : Attribute
	{
		#region Properties
		/// <summary>The Attribute string value</summary>
		public string StringValue { get; private set; }
		#endregion

		#region Constructor

		/// <summary>	Constructor. </summary>
		/// <param name="value">	The applied Attribute string value. </param>
		public StringValueAttribute(string value)
		{
			StringValue = value;
		}
		#endregion
	}

	/// <summary>Adds a named attribute to an enum</summary>
	public static class EnumAttribute
	{
		/// <summary>Get the String Value from an enumeration</summary>
		/// <param name="value">The enum value to be named</param>
		/// <returns>enum's attribute</returns>
		public static string GetStringValue(this Enum value)
		{
			Type type = value.GetType();

			FieldInfo fieldInfo = type.GetField(value.ToString());
			if (fieldInfo == null)
			{
				return value.ToString();
			}

			StringValueAttribute[] a = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

			return (a != null) && (a.Length > 0) ? a[0].StringValue : string.Empty;
		}

		/// <summary>Extract the list of Descriptions from the given enumeration</summary>
		/// <param name="type">The type of enumeration to be listed</param>
		/// <returns>returns array of valid enum value descriptions</returns>
		public static string[] EnumDescriptions(Type type)
		{
			List<string> names = new List<string>();
			if (type.BaseType == typeof(Enum))
			{
				foreach (string name in Enum.GetNames(type))
				{
					FieldInfo fieldInfo = type.GetField(name);

					StringValueAttribute[] a = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

					names.Add((a != null) && (a.Length > 0) ? a[0].StringValue : string.Empty);
				}
			}
			return names.ToArray();
		}

		/// <summary>Extract the enum values as a Dictionary of Value, Description pairs</summary>
		/// <returns>returns array of names</returns>
		public static Dictionary<T, string> EnumDescriptions<T>()
		{
			Dictionary<T, string> names = new Dictionary<T, string>();
			Type t = typeof(T);
			if (t.BaseType == typeof(Enum))
			{
				foreach (T t1 in t.GetEnumValues())
				{
					string s = t1.ToString();
					FieldInfo fieldInfo = t.GetField(t1.ToString());
					if (fieldInfo != null)
					{
						StringValueAttribute[] a = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
						if ((a != null) && (a.Length > 0))
						{
							s = a[0].StringValue;
						}
					}
					if (!names.ContainsKey(t1))
					{
						names.Add(t1, s);
					}
				}
			}
			return names;
		}

		/// <summary>Parses an enum value from it's string value.</summary>
		/// <param name="type">The type.</param>
		/// <param name="description">The description.</param>
		/// <returns></returns>
		public static object ParseFromDescription(Type type, string description)
		{
			foreach (var e in Enum.GetValues(type))
			{
				FieldInfo fieldInfo = type.GetField(e.ToString());
				StringValueAttribute[] a = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
				string d = (a != null) && (a.Length > 0) ? a[0].StringValue : string.Empty;

				if (!string.IsNullOrEmpty(d) && (d == description))
				{
					return e;
				}
			}
			return null;
		}

		/// <summary> A T extension method that validate value. </summary>
		/// <typeparam name="T"> Generic type parameter. </typeparam>
		/// <param name="value">	    enum to be named. </param>
		/// <param name="defaultValue"> The default value. </param>
		/// <returns> . </returns>
		public static T ValidateValue<T>(this T value, T defaultValue)
		{
			return Enum.IsDefined(typeof(T), value) ? value : defaultValue;
		}
	}
}
