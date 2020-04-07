using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
// namespace: Thorlabs.Elliptec.ELLO.Model
// 
// summary:	A Collection classes to drive the Elliptec Devices

namespace Thorlabs.Elliptec.ELLO_DLL
{
	/// <summary> Elliptec devices collection class. </summary>
	public class ELLDevices
	{

		/// <summary> The device. </summary>
		private ELLBaseDevice _selectedDevice;

		/// <summary> The devices. </summary>
		private readonly Dictionary<char, ELLBaseDevice> _devices = new Dictionary<char, ELLBaseDevice>(); 

		private readonly MessageUpdater _messageUpdater = new MessageUpdater();

		/// <summary> Default constructor. </summary>
		public ELLDevices()
		{
			IsConnected = false;
		}

		/// <summary> Gets the devices connection state. </summary>
		/// <value> True if any device is connected. </value>
		public bool IsConnected { get; private set; }

		/// <summary> Sends a free command. </summary>
		/// <param name="freeCommand"> The free command to send to the port. </param>
		public void SendFreeCommand(string freeCommand)
		{
			_messageUpdater.UpdateOutput("Sending free command...");
			ELLDevicePort.SendFreeString(freeCommand);
		}

		/// <summary>	Scans the addresses. </summary>
		/// <param name="minDeviceAddress">	The minimum device address. </param>
		/// <param name="maxDeviceAddress">	The maximum device address. </param>
		/// <returns>	Returns a list of available device addresses. </returns>
		public List<string> ScanAddresses(char minDeviceAddress, char maxDeviceAddress)
		{
			List<string> connections = new List<string>();
			if(!ELLDevicePort.IsConnected)
			{
				return connections;
			}

			_messageUpdater.UpdateOutput("Scanning for devices");
			foreach (char address in ELLBaseDevice.ValidAddresses.Where(item => item >= minDeviceAddress && item <= maxDeviceAddress ))
			{
				ELLDevicePort.SendString(address, "in");
				try
				{
					string msg = ELLDevicePort.WaitForResponse(address, "IN", ELLBaseDevice.StatusTimeout);
					if (!string.IsNullOrEmpty(msg))
					{
						connections.Add(msg);
					}
				}
				catch (ELLException e)
				{
					Debug.WriteLine(e.Message);
					// device address not used
				}
			}
			_messageUpdater.UpdateOutput(connections.Count < 2
				? string.Format("{0} device found", connections.Count)
				: string.Format("{0} devices found", connections.Count));
			return connections;
		}

		/// <summary> Clears the devices collection. </summary>
		public void ClearDevices()
		{
			List<char> addresses = _devices.Keys.ToList();
			foreach (char address in addresses)
			{
				_devices.Remove(address);
			}
		}

		/// <summary> Configures the given device address. </summary>
		/// <param name="deviceID"> Identifier for the device. </param>
		/// <returns> Success. </returns>
		public bool Configure(string deviceID)
		{
			if (string.IsNullOrEmpty(deviceID))
			{
				return false;
			}
			char address = deviceID[0];
			if(ELLBaseDevice.ValidAddresses.Contains(address))
			{
			    DeviceID di = ELLBaseDevice.Configure(deviceID, _messageUpdater);
			    if (di.DeviceType == DeviceID.DeviceTypes.Paddle)
			    {
			        _devices[address] = new ELLPaddlePolariser(di, _messageUpdater);
			        _selectedDevice = _devices[address];
			    }
                else
			    {
			        _devices[address] = new ELLDevice(di, _messageUpdater);
			        _selectedDevice = _devices[address];
			    }
            }
			return true;
		}

		/// <summary> Gets the selected device. </summary>
		/// <value> The selected device. </value>
		public ELLBaseDevice SelectedDevice
		{ get { return _selectedDevice; }}

		/// <summary> Readdress a device. </summary>
		/// <param name="oldAddress"> The old address. </param>
		/// <param name="newAddress"> The new address. </param>
		/// <returns> True if the device were reeaddressed succesfully. </returns>
		public bool ReaddressDevice(char oldAddress, char newAddress)
		{
		    ELLBaseDevice device = AddressedDevice(oldAddress);
			if (device == null)
			{
				return false;
			}
			if (!device.SetAddress(newAddress))
			{
				return false;
			}
			_devices.Remove(oldAddress);
			_devices[newAddress] = device;
			return true;
		}

		/// <summary>	Get the Addressed device. </summary>
		/// <param name="address">	The device address. </param>
		/// <returns>	returns the <see cref="ELLDevice"/> at the given address. </returns>
		public ELLBaseDevice AddressedDevice(char address)
		{
			return _devices.ContainsKey(address) ? _devices[address] : null;
		}

		/// <summary> Gets a list of Valid device addresses. </summary>
		/// <param name="addresses"> The addresses. </param>
		/// <returns> . </returns>
		public List<char> ValidAddresses(IEnumerable<char> addresses)
		{
			return addresses.Where(c => _devices.ContainsKey(c)).ToList();
		}

		/// <summary> Gets the message update handler. </summary>
		/// <value> The current <see cref="MessageUpdater"/> message handler. </value>
		public MessageUpdater MessageUpdates
		{ get { return _messageUpdater; }}

		/// <summary> Connects the selected device. </summary>
		/// <returns> True if the connection succeeds. </returns>
		public bool Connect()
		{
			if (SelectedDevice.IsValidAddress())
			{
				IsConnected = true;
			}
			return IsConnected;
		}

		/// <summary> Disconnects the connected connection. </summary>
		/// <returns> True if disconnect suceeds. </returns>
		public bool Disconnect()
		{
			IsConnected = false;
			return IsConnected;
		}

		/// <summary> Gets the valid address for this device. </summary>
		/// <value> The valid address. </value>
		public List<char> ValidAddress
		{ get { return ELLBaseDevice.ValidAddresses; } }
	}
}
