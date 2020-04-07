using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Thorlabs.Elliptec.ELLO_DLL.Support;


// namespace: Thorlabs.Elliptec.ELLO_DLL
//
// summary:	.
namespace Thorlabs.Elliptec.ELLO_DLL
{
	/// <summary> ELLO Device identifier. </summary>
	public class DeviceID
	{
		/// <summary>	Values that represent units in the Elliptec devices. </summary>
		public enum UnitTypes
		{
			/// <summary>	An enum constant representing the millimetres option. </summary>
			[StringValue("mm")]
			MM,
			/// <summary>	An enum constant representing the inches option. </summary>
			[StringValue("in")]
			Inches,
			/// <summary>	An enum constant representing the degrees option. </summary>
			[StringValue("deg")]
			Degrees,
		}

		/// <summary> Values that represent Elliptec Device Types. </summary>
		public enum DeviceTypes
		{
		    /// <summary> An enum constant representing the Elliptec Rotator4. </summary>
		    [StringValue("Paddle")]
		    Paddle = 3,
			/// <summary> An enum constant representing the Elliptec Rotator4. </summary>
            [StringValue("Rotator4")]
			Rotator = 4,
			/// <summary> An enum constant representing the Elliptec Actuator. </summary>
			[StringValue("Actuator")]
			Actuator = 5,
			/// <summary> An enum constant representing the Elliptec Shutter. </summary>
			[StringValue("Shutter")]
			Shutter = 6,
			/// <summary> An enum constant representing the Elliptec Linear Stage. </summary>
			[StringValue("Linear Stage")]
			LinearStage = 7,
			/// <summary> An enum constant representing the Elliptec Rotary Stage. </summary>
			[StringValue("Rotary Stage")]
			RotaryStage = 8,
		    /// <summary> An enum constant representing the Elliptec Shutter. </summary>
		    [StringValue("Shutter")]
		    Shutter4 = 9,
		    /// <summary> An enum constant representing the Elliptec Linear Stage. </summary>
            [StringValue("Linear Stage")]
		    LinearStage2 = 10,
            /// <summary> An enum constant representing the optics rotator option. </summary>
		    [StringValue("Optics Rotator")]
		    OpticsRotator = 14,
            /// <summary> An enum constant representing the linear stage 17 option. </summary>
		    [StringValue("Linear Stage")]
		    LinearStage17 = 17,
            /// <summary> An enum constant representing the rotary stage 18 option. </summary>
		    [StringValue("Rotarty Stage")]
		    RotaryStage18 = 18,
            /// <summary> An enum constant representing the linear stage 20 option. </summary>
		    [StringValue("Linear Stage")]
		    LinearStage20 = 20,
		}

        /// <summary> Structure containing ELLO stage axis parameters. </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		private struct DeviceInfo
		{
			/// <summary> The device address. </summary>
			public readonly char Address;
			/// <summary> The command name. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			private readonly byte[] command;
			/// <summary> The type of the device. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public readonly byte[] deviceType;
			/// <summary> The device serial number. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public readonly byte[] serialNo;
			/// <summary> The device year. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public readonly byte[] year;
			/// <summary> The firmware version. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public readonly byte[] firmware;
			/// <summary> The hardware version. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public readonly byte[] hardware;
			/// <summary> The travel in mm. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public readonly byte[] travel;
			/// <summary> The pulse per position. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public readonly byte[] pulsePerPosition;
		};

		private readonly UnitTypes _unitType;
		private readonly decimal _unitFactor;

		/// <summary> Default DeviceID constructor. </summary>
		public DeviceID()
		{
		}

		/// <summary> The DeviceID Constructor. </summary>
		/// <param name="idString"> The identifier string defining the device. <Br />This is obtained as an output from the <see cref="ELLDevices.ScanAddresses(char, char)"/> function.</param>
		/// \include ScanDevices_snippet.txt
		internal DeviceID(string idString )
		{
			DeviceInfo di = new DeviceInfo();
			DeviceInfo deviceInfo = Serialization.DeserializeMsg<DeviceInfo>(idString.ToBytes(Marshal.SizeOf(di)));
			Address = deviceInfo.Address;
			SerialNo = Serialization.ToString(deviceInfo.serialNo);
			Year = deviceInfo.year.ToInt();
			Firmware = deviceInfo.firmware.ToDecimal(10);
			byte hw = (byte)deviceInfo.hardware.ToInt(true);
			Hardware = hw & 0x7F;
			Imperial = hw >= 0x80;
			Travel = deviceInfo.travel.ToInt(true);
			PulsePerPosition = deviceInfo.pulsePerPosition.ToInt(true);
			DeviceType = (DeviceTypes)(deviceInfo.deviceType.ToInt(true));
		    switch (DeviceType)
		    {
		        case DeviceTypes.Shutter:
		        case DeviceTypes.Actuator:
		            MotorCount = 1;
		            break;

		        case DeviceTypes.Shutter4:
		        case DeviceTypes.Rotator:
		        case DeviceTypes.RotaryStage:
		        case DeviceTypes.LinearStage:
		        case DeviceTypes.LinearStage2:
		        case DeviceTypes.OpticsRotator:
		        case DeviceTypes.RotaryStage18:
		        case DeviceTypes.LinearStage17:
		        case DeviceTypes.LinearStage20:
		            MotorCount = 2;
		            break;
		        case DeviceTypes.Paddle:
		            MotorCount = 3;
		            break;
		        default:
		            MotorCount = 1;
		            break;
		    }
		    switch (DeviceType)
		    {
		        case DeviceTypes.Paddle:
		            _unitFactor = (Decimal)(PulsePerPosition-1) / Travel;
		            _unitType = UnitTypes.Degrees;
		            FormatStr = "{0:0.0##}";
		            break;
		        case DeviceTypes.Rotator:
		        case DeviceTypes.OpticsRotator:
		        case DeviceTypes.RotaryStage:
		        case DeviceTypes.RotaryStage18:
		            _unitFactor = PulsePerPosition / 360.0m;
		            _unitType = UnitTypes.Degrees;
		            FormatStr = "{0:0.0##}";
		            break;
                default:
                    if(Imperial)
                    {
                        _unitType = UnitTypes.Inches;
                        _unitFactor = 25.4m * PulsePerPosition;
                        FormatStr = "{0:0.0###}";
                    }
                        else
                    {
                        _unitType = UnitTypes.MM;
                        _unitFactor = PulsePerPosition;
                        FormatStr = "{0:0.0##}";
                    }
                    break;
		    }
			Units = _unitType.GetStringValue();
		}

        /// <summary>   Gets the format string. </summary>
        /// <value> The format string. </value>
	    public string FormatStr { get; }
		
        /// <summary>	Convert Pulses to Units (mm). </summary>
        /// <param name="pulses">	The number of pulses to translate. </param>
        /// <returns> The equivalent Units (mm) </returns>
        public decimal PulseToUnit(decimal pulses)
		{
			if (_unitFactor == 0)
			{
				return pulses;
			}
			return pulses / _unitFactor;
		}

		/// <summary>	Convert Units (mm) to pulses. </summary>
		/// <param name="units">	The number of Units (mm). </param>
		/// <returns>	The equivalent number of pulses. </returns>
		public int UnitToPulse(decimal units)
		{
			if (_unitFactor == 0)
			{
				return (int)units;
			}
			return (int)Math.Round(units * _unitFactor, 0, MidpointRounding.AwayFromZero);
		}

		/// <summary> Convert mm to inches. </summary>
		/// <param name="mm"> The number of millimetres to convert. </param>
		/// <returns> The equivalent number of inches. </returns>
		private decimal MMtoInches(decimal mm)
		{
			return mm / 25.4m;
		}

		/// <summary> Convert Inches to millimetres. </summary>
		/// <param name="mm"> The number of millimetres to convert. </param>
		/// <returns> The equivalent number of Inches. </returns>
		private decimal InchestoMM(decimal mm)
		{
			return mm * 25.4m;
		}

		/// <summary> Format the position as a string. </summary>
		/// <param name="position">	 The position to be formatted. </param>
		/// <param name="showUnits"> True to show units or False to hide the units. </param>
		/// <param name="showSpace"> True to include spaces, False to remove the spaces. </param>
		/// <returns> The formatted position. </returns>
		public string FormatPosition(decimal position, bool showUnits = false, bool showSpace = false)
		{
			string output = string.Format(FormatStr, position);
			if (showUnits)
			{
				if (showSpace)
				{
					output += " ";
				}
				output += Units;
			}
			return output;
		}

		/// <summary> Gets the units string. </summary>
		/// <value> The units string, "mm", "inches" or "deg". </value>
		public string Units { get; }

		/// <summary> Gets the serial number of the device. </summary>
		/// <value> The serial number as a string. </value>
		private string SerialNo { get; }

		/// <summary> Gets the bus address of the device '0' to 'F'. </summary>
		/// <value> The device bus address '0' to 'F'. </value>
		public char Address { get; private set; }

		/// <summary> Gets the year of manufacture. </summary>
		/// <value> The year of manufacture. </value>
		private int Year { get; }

		/// <summary> Gets the firmware version number. </summary>
		/// <value> The firmware version number (major.minor). </value>
		private Decimal Firmware { get; }

		/// <summary> Gets the hardware version number. </summary>
		/// <value> The hardware version number (major.minor). </value>
		private int Hardware { get; }

		/// <summary> Gets the travel in mm of the device. </summary>
		/// <value> The travel in mm of the device. Only applicable to Linear and Rotary stages</value>
		public int Travel { get; }

		/// <summary> Gets a flag indicating whether the device is metric or imperial. </summary>
		/// <remarks> Used by the application to determine which units to use.</remarks>
		/// <value> True if the device is imperial, False if the device is metric. </value>
		public bool Imperial { get; }

		/// <summary> Gets the number of pulse per position (mm). </summary>
		/// <value> The pulses per position in mm. </value>
		public Int32 PulsePerPosition { get; }

		/// <summary> Gets or sets the number of motors. </summary>
		/// <remarks> Linear and Rotary stages have 2 motors otherwise 1 motor.</remarks>
		/// <value> The number of motors. </value>
		private int MotorCount { get; }

		/// <summary> Gets the Elliptec Device Type. </summary>
		/// <value> The type of Elliptec Device, see <see cref="DeviceTypes" />. </value>
		public DeviceTypes DeviceType { get; }

		/// <summary> Gets the description as a list of named strings. </summary>
		/// <returns> A list of strings holding the device description. </returns>
		public List<string> Description()
		{
			List<string> list = new List<string>
			{
				string.Format("Address: {0}", Address),
				string.Format("Serial Number: {0}", SerialNo),
				string.Format("Device Type: {0}", DeviceType.GetStringValue()),
				string.Format("Motors: {0}", MotorCount),
				string.Format("Firmware: {0}", Firmware),
				string.Format("Hardware: {0}", Hardware),
			    _unitType == UnitTypes.Degrees ? "" : string.Format("Variant: {0}", Imperial ? "Imperial" : "Metric"),
				string.Format("Year: {0}", Year),
			};
			switch (_unitType)
			{
				case UnitTypes.MM:
					list.Add(string.Format("Travel: {0} {1}", Travel, Units));
					list.Add(string.Format("Pulses Per {1}: {0}", PulsePerPosition == 0 ? "NA" : UnitToPulse(1.0m).ToString(CultureInfo.InvariantCulture), Units));
					break;
				case UnitTypes.Inches:
					list.Add(string.Format("Travel: {0:F3} {1}", MMtoInches(Travel), Units));
					list.Add(string.Format("Pulses Per {1}: {0}", PulsePerPosition == 0 ? "NA" : UnitToPulse(1.0m).ToString(CultureInfo.InvariantCulture), Units));
					break;
				case UnitTypes.Degrees:
					list.Add(string.Format("Travel: {0} {1}", Travel, Units));
					list.Add(string.Format("Pulses Per {1}: {0}", PulsePerPosition == 0 ? "NA" : UnitToPulse(1.0m).ToString(CultureInfo.InvariantCulture), Units));
					break;
			}

			return list;
		}

	}
}
