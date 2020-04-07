using System.Runtime.InteropServices;
using Thorlabs.Elliptec.ELLO_DLL.Support;

// namespace: Thorlabs.Elliptec.ELLO.Model
//
// summary:	A Collection of classes to drive the Elliptec Devices
namespace Thorlabs.Elliptec.ELLO_DLL
{
	/// <summary> The ELLO Device status. </summary>
	public class DeviceStatus
	{
		/// <summary> Values that represent Device Status. </summary>
		public enum DeviceStatusValues
		{
			/// <summary> An enum constant representing the ok option. </summary>
			[StringValue("Ok")]
			OK = 0,
			/// <summary> An enum constant representing the communication error option. </summary>
			[StringValue("Communication Error")]
			CommunicationError = 1,
			/// <summary> An enum constant representing the mechanical time out option. </summary>
			[StringValue("Mechanical Time Out")]
			MechanicalTimeOut = 2,
			/// <summary> An enum constant representing the command error option. </summary>
			[StringValue("Command Error")]
			CommandError = 3,
			/// <summary> An enum constant representing the value out of range option. </summary>
			[StringValue("Value Out Of Range")]
			ValueOutOfRange = 4,
			/// <summary> An enum constant representing the module isolated option. </summary>
			[StringValue("Module Isolated")]
			ModuleIsolated = 5,
			/// <summary> An enum constant representing the module out of isolation option. </summary>
			[StringValue("Module Out Of Isolation")]
			ModuleOutOfIsolation = 6,
			/// <summary> An enum constant representing the initialization error option. </summary>
			[StringValue("Initialization Error")]
			InitializationError = 7,
			/// <summary> An enum constant representing the thermal error option. </summary>
			[StringValue("Thermal Error")]
			ThermalError = 8,
		    /// <summary> An enum constant representing the Busy option. </summary>
		    [StringValue("Busy")]
		    Busy = 9,
		    /// <summary> An enum constant representing the Sensor error option. </summary>
		    [StringValue("Sensor Error")]
		    SensorError = 10,
		    /// <summary> An enum constant representing the Motor error option. </summary>
		    [StringValue("Motor Error")]
		    MotorError = 11,
		    /// <summary> An enum constant representing the Out of range error option. </summary>
		    [StringValue("Out of range Error")]
		    OutOfRangeError = 12,
		    /// <summary> An enum constant representing the Over current error option. </summary>
		    [StringValue("Over current Error")]
		    OverCurrentError = 13,
		    /// <summary> An enum constant representing the General error option. </summary>
		    [StringValue("General Error")]
		    GeneralError = 14,
		}

		/// <summary> Structure containing device stage axis parameters. </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		private struct DeviceStatusStruct
		{
			/// <summary> The device address. </summary>
			private readonly char address;
			/// <summary> The device command as a 2 byte command. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			private readonly byte[] command;
			/// <summary> The status as a 2 byte status. <see cref="DeviceStatusValues"/></summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public readonly byte[] status;
		};

		/// <summary> Default constructor. </summary>
		internal DeviceStatus()
		{
			Status = DeviceStatusValues.OK;
		}

		/// <summary> Constructor from incoming status message. </summary>
		internal DeviceStatus(string message)
		{
			DeviceStatusStruct di = new DeviceStatusStruct();
			DeviceStatusStruct deviceStatusStruct = Serialization.DeserializeMsg<DeviceStatusStruct> (message.ToBytes(Marshal.SizeOf(di)));
			Status = (DeviceStatusValues)(deviceStatusStruct.status.ToInt(true));
		}

		/// <summary> Gets the status value. </summary>
		/// <value> The status as a <see cref="DeviceStatusValues"/> structure. </value>
		public DeviceStatusValues Status { get; private set; }
	}
}
