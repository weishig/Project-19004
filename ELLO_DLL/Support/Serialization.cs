using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO_DLL.Support
{
	/// <summary> A Serialization extension class to manipulate byte arrays as part of the serialization process. </summary>
	public static class Serialization
	{
		/// <summary> Joins the the two byte arrays as a new byte array (Left to Right). </summary>
		/// <param name="first">  The first byte array to be added. </param>
		/// <param name="second"> The second byte array to be added. </param>
		/// <returns> return the combined byte array (first+second)  </returns>
		public static byte[] JoinTo(this byte[] second, byte[] first)
		{
			byte[] bytes = new byte[first.Length + second.Length];
			first.CopyTo(bytes, 0);
			second.CopyTo(bytes, first.Length);
			return bytes;
		}

		/// <summary> Joins the the two byte arrays as a new byte array (Right to Left). </summary>
		/// <param name="first">  The first byte array to be added. </param>
		/// <param name="second"> The second byte array to be added. </param>
		/// <returns> return the combined byte array (first+second)  </returns>
		public static byte[] JoinWith(this byte[] first, byte[] second)
		{
			byte[] bytes = new byte[first.Length + second.Length];
			first.CopyTo(bytes, 0);
			second.CopyTo(bytes, first.Length);
			return bytes;
		}

		/// <summary> Joins the the two byte arrays as a new byte array (Right to Left). </summary>
		/// <param name="first">  The first byte array to be added. </param>
		/// <param name="second"> The second byte array to be added. </param>
		/// <returns> return the combined byte array (first+second)  </returns>
		public static byte[] Join(byte[] first, byte[] second)
		{
			byte[] bytes = new byte[first.Length + second.Length];
			first.CopyTo(bytes, 0);
			second.CopyTo(bytes, first.Length);
			return bytes;
		}

		/// <summary> Extract a subset of the supplied byte array. </summary>
		/// <param name="source"> The source byte array. </param>
		/// <param name="index">  Zero-based index from which to copy. </param>
		/// <param name="length"> The of the byte array to be copied. </param>
		/// <returns> A subset of the source byte array. </returns>
		public static byte[] Subset(this byte[] source, int index, int length = -1)
		{
			if (length < 0)
			{
				length = source.Length - index;
			}
			if (length <= 0)
			{
				return new byte[0];
			}
			try
			{
				byte[] clone = new byte[length];
				Array.Copy(source, index, clone, 0, length);
				return clone;
			}
// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
			}
			return new byte[0];
		}

		/// <summary> Return the supplied object as a serialized byte array. </summary>
		/// <typeparam name="T"> Generic type parameter. </typeparam>
		/// <param name="t"> The object of type T to be serialized . </param>
		/// <returns> a byte array representing the object of type T. </returns>
		public static Byte[] SerializeMessage<T>(T t) where T : struct
		{
			int objsize = Marshal.SizeOf(typeof(T));
			Byte[] ret = new Byte[objsize];
			IntPtr buff = Marshal.AllocHGlobal(objsize);
			Marshal.StructureToPtr(t, buff, true);
			Marshal.Copy(buff, ret, 0, objsize);
			Marshal.FreeHGlobal(buff);
			return ret;
		}

		/// <summary> Extracts an object of type T from the supplied byte array. </summary>
		/// <typeparam name="T"> Generic type parameter. </typeparam>
		/// <param name="data"> The byte array representing the object. </param>
		/// <returns> returns an object of type T built from the supplied data. </returns>
		public static T DeserializeMsg<T>(Byte[] data) where T : struct
		{
			int objsize = Marshal.SizeOf(typeof(T));
			IntPtr buff = Marshal.AllocHGlobal(objsize);
			Marshal.Copy(data, 0, buff, objsize);
			T retStruct = (T)Marshal.PtrToStructure(buff, typeof(T));
			Marshal.FreeHGlobal(buff);
			return retStruct;
		}

		/// <summary> Extracts an object of type T from the supplied byte array. </summary>
		/// <typeparam name="T"> Generic type parameter. </typeparam>
		/// <param name="data"> The byte array representing the object. </param>
		/// <param name="index"> Zero-based index from which to start deserializing. </param>
		/// <returns> returns an object of type T built from the supplied data. </returns>
		public static T DeserializeMsg<T>(Byte[] data, int index) where T : struct
		{
			int objsize = Marshal.SizeOf(typeof(T));
			IntPtr buff = Marshal.AllocHGlobal(objsize);
			Marshal.Copy(data, index, buff, objsize);
			T retStruct = (T)Marshal.PtrToStructure(buff, typeof(T));
			Marshal.FreeHGlobal(buff);
			return retStruct;
		}

		/// <summary> A string extension method that converts the string to a byte array. </summary>
		/// <param name="str">		  The string to convert to a byte array. </param>
		/// <param name="bufferSize"> Size of the buffer.  will be null padded if greater than the string length </param>
		/// <returns> The given data converted to a byte[]. </returns>
		public static byte[] ToBytes(this string str, int bufferSize)
		{
			if (bufferSize <= 0)
			{
				return Encoding.UTF8.GetBytes(str);
			}
			byte[] buffer = new byte[bufferSize];
			if (str.Length > bufferSize)
			{
				str = str.Substring(0, bufferSize);
			}
			Array.Copy(Encoding.UTF8.GetBytes(str), buffer, str.Length);
			return buffer;
		}

		/// <summary> A byte[] extension method that converts the byte array to a string. </summary>
		/// <param name="bytes"> The source byte array. </param>
		/// <returns> The created string. </returns>
		public static string ToString(this byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		/// <summary> A byte[] extension method that converts the byte array to an int. </summary>
		/// <param name="bytes"> The source byte array. </param>
		/// <param name="hex">   true to use hexadecimal notation. </param>
		/// <returns> The created integer. </returns>
		public static int ToInt(this byte[] bytes, bool hex = false)
		{
			try
			{
				string s = Encoding.UTF8.GetString(bytes);
				if (hex)
				{
					return int.Parse(s, NumberStyles.AllowHexSpecifier);
				}
				return int.Parse(s);
			}
			catch (Exception)
			{
				return 0;
			}

		}

		/// <summary> A byte[] extension method that converts the byte array to a decimal. </summary>
		/// <param name="bytes"> The source byte array. </param>
		/// <param name="divisor"> The divisor to convert the int to a decimal. </param>
		/// <param name="hex">   true to use hexadecimal notation. </param>
		/// <returns> The created decimal. </returns>
		public static decimal  ToDecimal(this byte[] bytes, int divisor, bool hex = false)
		{
			int i = ToInt(bytes, hex);
			return ((decimal) i)/divisor;
		}
	}
}
