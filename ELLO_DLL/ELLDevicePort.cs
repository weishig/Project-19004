using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
#pragma warning disable 1570
#pragma warning disable 1574
#pragma warning disable 1584, 1711, 1572, 1581, 1580

// namespace: Thorlabs.Elliptec.ELLO.Model
//
// summary:	A Collection classes to drive the Elliptec Devices
namespace Thorlabs.Elliptec.ELLO_DLL
{
	/// <summary> Exception for signalling Elliptec errors. </summary>
	/// <seealso cref="T:System.Exception"/>
	[Serializable]
	public class ELLException : Exception
	{
		/// <summary> Constructor. </summary>
		/// <param name="message"> The message. </param>
		public ELLException(string message)
			: base(message)
		{
			
		}
	}

	/// <summary> Elliptec device port. </summary>
	public static class ELLDevicePort
	{
		private static readonly Object _lockObject = new object(); 
		
		private static readonly SerialPort _serialPort;
		private static Thread _readThread;
		private static bool _backgroundRunning;
		private static readonly List<string> _responses = new List<string>();

		/// <summary> Delegate to define the DataRecieved function callback definition. </summary>
		/// <param name="str"> The received data. </param>
		public delegate void DataReceivedDelegate(string str);

		/// <summary> The data received callback function. </summary>
		/// <remarks> Connect to this delegate to view the received data</remarks>
		public static DataReceivedDelegate DataReceived;

		/// <summary> Delegate to define the DataSent function callback definition. </summary>
		/// <param name="str"> The sent data. </param>
		public delegate void DataSentDelegate(string str);

		/// <summary> The data sent callback function. </summary>
		/// <remarks> Connect to this delegate to view the data sent to the device</remarks>
		public static DataSentDelegate DataSent;


		/// <summary> Static constructor used to initialize the static fields. </summary>
		static ELLDevicePort()
		{
			_serialPort = new SerialPort
			{
				NewLine = "\a",
				BaudRate = 9600,
				Parity = Parity.None,
				StopBits = StopBits.One,
				DataBits = 8,
				Handshake = Handshake.None,
				ReadTimeout = 2000,
				WriteTimeout = 500
			};
		}

		/// <summary> Connect to the specified port. </summary>
		/// <param name="portName"> The port name to connect to. </param>
		/// <returns> True if a connection was established. </returns>
		/// <seealso cref="Disconnect()"/>
		/// <seealso cref="IsConnected"/>
		public static bool Connect(string portName)
		{
			if (_serialPort.IsOpen)
			{
				return true;
			}
			try
			{
				_serialPort.PortName = portName;
				_serialPort.Open();
				if (!_serialPort.IsOpen)
				{
					return false;
				}
				_serialPort.DiscardInBuffer();
			}
			catch (Exception)
			{
				if (_serialPort.IsOpen)
				{
					_serialPort.Close();
					_backgroundRunning = false;
				}
				throw new ELLException(string.Format("{0} not available available", _serialPort.PortName));
			}

			try
			{
				_backgroundRunning = true;
				_readThread = new Thread(ReadThread);
				_readThread.Start();
			}
			catch (Exception)
			{
				_serialPort.Close();
				_backgroundRunning = false;
				throw new ELLException("Failed to start server");
			}
			return true;
		}

		/// <summary> Disconnects from the current port. </summary>
		/// <returns> True when disconnection succesfull. </returns>
		/// <seealso cref="Connect(string)"/>
		/// <seealso cref="IsConnected"/>
		public static bool Disconnect()
		{
			if (!_serialPort.IsOpen)
			{
				return false;
			}
			_backgroundRunning = false;
			if (_readThread.IsAlive)
			{
				_readThread.Join();
			}
			_serialPort.Close();
			return false;
		}

		/// <summary> Gets a value indicating whether a connection is established. </summary>
		/// <value> True if a connection is established. </value>
		/// <seealso cref="Disconnect()"/>
		/// <seealso cref="Connect(string)"/>
		public static bool IsConnected
		{ get { return _serialPort.IsOpen; } }

		/// <summary> Reads the thread. </summary>
		private static void ReadThread()
		{
			string data = "";
			while (_backgroundRunning)
			{
				try
				{
					if (_serialPort.BytesToRead > 0)
					{
						data += _serialPort.ReadExisting();
					}
					int eol = data.IndexOf('\n');
					while (eol >= 0)
					{
						string message = data.Substring(0, eol + 1).Trim(' ', '\r', '\n');
						data = data.Remove(0, eol + 1);
						eol = data.IndexOf('\n');

						if (!string.IsNullOrEmpty(message))
						{
							DataReceived?.Invoke(message);
							StoreResponse(message);
						}
					}
					Thread.Sleep(50);
				}
				catch (TimeoutException)
				{
					
				}
			}
		}

		/// <summary> Stores a response. </summary>
		/// <param name="response"> The response. </param>
		private static void StoreResponse(string response)
		{
			lock (_lockObject)
			{
				if (!string.IsNullOrEmpty(response))
				{
					_responses.Add(response);
				}
			}
		}

		/// <summary> Query if responses have beed received. </summary>
		/// <remarks> Use the parameterless version if only 1 device is used.</remarks>
		/// <returns> True if response are available. </returns>
		/// <seealso cref="GetNextResponse()"/>
		/// <seealso cref="ClearResponses()"/>
		public static bool HasResponse()
		{
			lock (_lockObject)
			{
				return (_responses.Count > 0);
			}
		}

		/// <summary>	Query if responses have beed received. </summary>
		/// <remarks> Use this version if multiple devices are being used.</remarks>
		/// <param name="address">	The device address. </param>
		/// <returns>	True if response are available. </returns>
		/// <seealso cref="GetNextResponse(char)"/>
		/// <seealso cref="ClearResponses(char)"/>
		public static bool HasResponse(char address)
		{
			lock (_lockObject)
			{
				return _responses.Any(s => s[0] == address);
			}
		}

		/// <summary>	Query if responses have beed received. </summary>
		/// <remarks> Use this version if multiple devices are being used.</remarks>
		/// <param name="addresses">	The device address. </param>
		/// <returns>	True if response are available. </returns>
		/// <seealso cref="GetNextResponse(List<char>)"/>
		/// <seealso cref="ClearResponses(List<char>)"/>
		public static bool HasResponse(List<char> addresses)
		{
			lock (_lockObject)
			{
				return _responses.Any(s => addresses.Contains(s[0]));
			}
		}

		/// <summary>	Gets the next response from the queue. </summary>
		/// <remarks> Use the parameterless version if only 1 device is used.</remarks>
		/// <exception cref="ELLException">	Thrown when no responses are available. Check <see cref="HasResponse()" /> before use </exception>
		/// <returns>	The next response. </returns>
		/// <seealso cref="HasResponse()"/>
		/// <seealso cref="ClearResponses()"/>
		public static string GetNextResponse()
		{
			lock (_lockObject)
			{
				if (!HasResponse())
				{
					throw new ELLException("No Responses available");
				}

				string response = _responses[0];
				_responses.RemoveAt(0);
				return response;
			}
		}

		/// <summary>	Gets the next response from the queue. </summary>
		/// <remarks> Use this version if multiple devices are being used.</remarks>
		/// <exception cref="ELLException">	Thrown when no responses are available. Check <see cref="HasResponse()" /> before use</exception>
		/// <param name="address">	The address. </param>
		/// <returns>	The next response. </returns>
		/// <seealso cref="HasResponse(char)"/>
		/// <seealso cref="ClearResponses(char)"/>
		public static string GetNextResponse(char address)
		{
			lock (_lockObject)
			{
				if (!HasResponse(address))
				{
					throw new ELLException("No Responses available");
				}

				int index = _responses.FindIndex(s => s[0] == address);
				if (index < 0)
				{
					throw new ELLException("No Responses available");
				}
				string response = _responses[index];
				_responses.RemoveAt(index);
				return response;
			}
		}

		/// <summary>	Gets the next response from the queue. </summary>
		/// <remarks> Use this version if multiple devices are being used.</remarks>
		/// <exception cref="ELLException">	Thrown when no responses are available. Check <see cref="HasResponse()" /> before use</exception>
		/// <param name="addresses">	The device address. </param>
		/// <returns>	The next response. </returns>
		/// <seealso cref="HasResponse(List<char>)"/>
		/// <seealso cref="ClearResponses(List<char>)"/>
		public static string GetNextResponse(List<char> addresses)
		{
			lock (_lockObject)
			{
				if (!HasResponse(addresses))
				{
					throw new ELLException("No Responses available");
				}

				int index = _responses.FindIndex(s => addresses.Contains(s[0]));
				if (index < 0)
				{
					throw new ELLException("No Responses available");
				}
				string response = _responses[index];
				_responses.RemoveAt(index);
				return response;
			}
		}

		/// <summary>	Clears the responses queue. </summary>
		/// <remarks> Use this version if multiple devices are being used.</remarks>
		/// <param name="address">	The device address. </param>
		/// <seealso cref="HasResponse(char)"/>
		/// <seealso cref="GetNextResponse(char)"/>
		public static void ClearResponses(char address)
		{
			lock (_lockObject)
			{
				_responses.RemoveAll(s => s[0] == address);
			}
		}

		/// <summary>	Clears the responses queue. </summary>
		/// <remarks> Use the parameterless version if only 1 device is used.</remarks>
		/// <seealso cref="HasResponse()"/>
		/// <seealso cref="GetNextResponse()"/>
		public static void ClearResponses()
		{
			lock (_lockObject)
			{
				_responses.Clear();
			}
		}

		/// <summary>	Clears the responses queue. </summary>
		/// <remarks> Use this version if multiple devices are being used.</remarks>
		/// <param name="addresses">	The device address. </param>
		/// <seealso cref="HasResponse(List<char>)"/>
		/// <seealso cref="GetNextResponse(List<char>)"/>
		public static void ClearResponses(List<char> addresses)
		{
			lock (_lockObject)
			{
				_responses.RemoveAll(s => addresses.Contains(s[0]));
			}
		}

		/// <summary> Sends free text to the port. </summary>
		/// <param name="text"> The text to be sent . </param>
		/// <seealso cref="SendString(char, string)"/>
		public static void SendFreeString(string text)
		{
			lock (_lockObject)
			{
				_serialPort.WriteLine(text);
			}
			DataSent?.Invoke(text);
		}

		private static void SendCommand(string command, bool clearResponses)
		{
			lock (_lockObject)
			{
				if (clearResponses)
				{
					_responses.Clear();
				}
				_serialPort.WriteLine(command);
			}
			DataSent?.Invoke(command);
		}


		/// <summary> Sends a specifi command to device at the given address. </summary>
		/// <param name="address"> The device address. </param>
		/// <param name="command"> The command to be sent. </param>
		/// <seealso cref="SendFreeString(string)"/>
		/// <seealso cref="SendStringI16(char, string, Int16)"/>
		/// <seealso cref="SendStringI32(char, string, Int32)"/>
		/// <seealso cref="SendStringB(char, string, byte)"/>
		public static void SendString(char address, string command)
		{
			SendCommand(string.Format("{0}{1}", address, command), true);
		}

		/// <summary> Sends a command with an Int16 parameter. </summary>
		/// <param name="address"> The address. </param>
		/// <param name="command"> The command. </param>
		/// <param name="i">	   The int16 parameter. </param>
		/// <seealso cref="SendFreeString(string)"/>
		/// <seealso cref="SendString(char, string)"/>
		/// <seealso cref="SendStringI32(char, string, Int32)"/>
		/// <seealso cref="SendStringB(char, string, byte)"/>
		public static void SendStringI16(char address, string command, Int16 i)
		{
			SendCommand(string.Format("{0}{1}{2:X4}", address, command, i), true);
		}

	    /// <summary> Sends a command with an Int16 parameter. </summary>
	    /// <param name="address"> The address. </param>
	    /// <param name="command"> The command. </param>
	    /// <param name="i">	   The int16 parameter. </param>
	    /// <seealso cref="SendFreeString(string)"/>
	    /// <seealso cref="SendString(char, string)"/>
	    /// <seealso cref="SendStringI32(char, string, Int32)"/>
	    /// <seealso cref="SendStringB(char, string, byte)"/>
	    public static void SendStringI16(char address, string command, UInt16 i)
	    {
	        SendCommand(string.Format("{0}{1}{2:X4}", address, command, i), true);
	    }
		
        /// <summary> Sends a command with an Int32 parameter. </summary>
        /// <param name="address"> The address. </param>
        /// <param name="command"> The command. </param>
        /// <param name="i">	   The int32 parameter. </param>
        /// <seealso cref="SendFreeString(string)"/>
        /// <seealso cref="SendString(char, string)"/>
        /// <seealso cref="SendStringI16(char, string, Int16)"/>
        /// <seealso cref="SendStringB(char, string, byte)"/>
        public static void SendStringI32(char address, string command, Int32 i)
		{
			SendCommand(string.Format("{0}{1}{2:X8}", address, command, i), true);
		}

		/// <summary> Sends a command with a byte parameter. </summary>
		/// <param name="address"> The address. </param>
		/// <param name="command"> The command. </param>
		/// <param name="b">	   The byte value. </param>
		/// <seealso cref="SendFreeString(string)"/>
		/// <seealso cref="SendString(char, string)"/>
		/// <seealso cref="SendStringI16(char, string, Int16)"/>
		/// <seealso cref="SendStringI32(char, string, Int32)"/>
		public static void SendStringB(char address, string command, byte b)
		{
			SendCommand(string.Format("{0}{1}{2}", address, command, (char)b), true);
		}

		/// <summary> Wait for response from the device. </summary>
		/// <exception cref="ELLException"> Thrown when an Elliptec error condition occurs. </exception>
		/// <param name="address"> The address. </param>
		/// <param name="response"> The expected response. </param>
		/// <param name="timeout"> The timeout. </param>
		/// <returns> Returns the response. </returns>
		/// <seealso cref="WaitForResponse(char,IEnumerable<string>,int)"/>
		/// <seealso cref="WaitForResponse(IEnumerable<char>,IEnumerable<string>,int)"/>
		public static string WaitForResponse(char address, string response, int timeout)
		{
			DateTime timeoutTime = DateTime.Now + new TimeSpan(timeout * 10000L);
			string header = string.Format("{0}{1}", address, response);
			while (DateTime.Now < timeoutTime)
			{
				if (HasResponse(address))
				{
					string message = GetNextResponse(address);
					if (message.Substring(0, header.Length) == header)
					{
						return message;
					}
				}
				Thread.Sleep(100);
			}
			throw new ELLException("Response timeout");
		}

		/// <summary> Wait for response from the device. </summary>
		/// <exception cref="ELLException"> Thrown when an Elliptec error condition occurs. </exception>
		/// <param name="address">   The address. </param>
		/// <param name="responses"> The list of permitted responses. </param>
		/// <param name="timeout">   The timeout. </param>
		/// <returns> Returns the response. </returns>
		/// <seealso cref="WaitForResponse(char,string,int)"/>
		/// <seealso cref="WaitForResponse(IEnumerable<char>,IEnumerable<string>,int)"/>
		public static string WaitForResponse(char address, IEnumerable<string> responses, int timeout)
		{
			return WaitForResponse(new List<char> {address}, responses, timeout);
		}

		/// <summary> Wait for response from the device. </summary>
		/// <exception cref="ELLException"> Thrown when an Elliptec error condition occurs. </exception>
		/// <param name="addresses">   The address. </param>
		/// <param name="responses"> The list of permitted responses. </param>
		/// <param name="timeout">   The timeout. </param>
		/// <returns> Returns the response. </returns>
		/// <seealso cref="WaitForResponse(char,string,int)"/>
		/// <seealso cref="WaitForResponse(char,IEnumerable<string>,int)"/>
		public static string WaitForResponse(List<char> addresses, IEnumerable<string> responses, int timeout)
		{
			DateTime timeoutTime = DateTime.Now + new TimeSpan(timeout * 10000);
			List<string> headers = addresses.Aggregate(new List<string>(), (list, address) =>
			{
				list.AddRange(responses.Select(s => string.Format("{0}{1}", address, s)).ToList());
				return list;
			});
			while (DateTime.Now < timeoutTime)
			{
				if (HasResponse(addresses))
				{
					string message = GetNextResponse(addresses);
					if (headers.Contains(message.Substring(0, 3)))
					{
						return message;
					}
				}
				Thread.Sleep(100);
			}
			throw new ELLException("Response timeout");
		}
	}
}
