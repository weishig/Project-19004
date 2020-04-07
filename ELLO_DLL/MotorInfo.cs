using System.Collections.Generic;
using System.Runtime.InteropServices;
using Thorlabs.Elliptec.ELLO_DLL.Support;

// namespace: Thorlabs.Elliptec.ELLO.Model
//
// summary:	A Collection classes to drive the Elliptec Devices
namespace Thorlabs.Elliptec.ELLO_DLL
{
	/// <summary> The device MotorInfo structure. </summary>
	public class MotorInfo
	{
		/// <summary> Values that represent Switch States On/Off. </summary>
		public enum SwitchState
		{
			/// <summary> An enum constant representing the on option. </summary>
			On = '1',
			/// <summary> An enum constant representing the off option. </summary>
			Off = '0',
		}

		/// <summary> Structure containing stage axis parameters. </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		private struct MotorInfoStruct
		{
			/// <summary> device address. </summary>
			private readonly char address;
			/// <summary> The command name. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public readonly byte[] command;
			/// <summary> The loop state parameter. </summary>
			public readonly byte loopState;
			/// <summary> The motor state parameter. </summary>
			public readonly byte motorState;
			/// <summary> The motor current. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public readonly byte[] current;
			/// <summary> The ramp up parameter. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public readonly byte[] rampUp;
			/// <summary> The ramp down parameter. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public readonly byte[] rampoDown;
			/// <summary> The forward period. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public readonly byte[] fwdPeriod;
			/// <summary> The reverse period. </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public readonly byte[] revPeriod;
		};

		/// <summary>	Default constructor. </summary>
		/// <param name="motorID">	The identifier of the motor. </param>
		internal MotorInfo(char motorID)
		{
			MotorState = SwitchState.Off;
			MotorID = motorID;
		    IsValid = false;

		}

		/// <summary> Construct and Initialize. </summary>
		/// <param name="idString"> The initialization string. </param>
		internal MotorInfo(string idString)
		{
			MotorInfoStruct di = new MotorInfoStruct();
			MotorInfoStruct deviceInfo = Serialization.DeserializeMsg<MotorInfoStruct>(idString.ToBytes(Marshal.SizeOf(di)));
			MotorID = (char)deviceInfo.command[1];
			MotorState = (SwitchState)deviceInfo.motorState;
			LoopState = (SwitchState)deviceInfo.loopState;
			Current = deviceInfo.current.ToDecimal(1866, true);
			RampUp = deviceInfo.rampUp.ToInt(true);
			RampDown = deviceInfo.rampoDown.ToInt(true);
			FwdFreq = 1m / deviceInfo.fwdPeriod.ToDecimal(14740, true);
			RevFreq = 1m / deviceInfo.revPeriod.ToDecimal(14740, true);
		    IsValid = true;

		}

        /// <summary>   Gets a value indicating whether this object is valid. </summary>
        /// <value> True if this object is valid, false if not. </value>
        public bool IsValid { get; }

		/// <summary> Gets the loop state parameter. </summary>
		public SwitchState LoopState { get; }
		/// <summary> Gets the motor state parameter. </summary>
		public SwitchState MotorState { get; }
		/// <summary> The motor current parameter. </summary>
		public decimal Current { get; }
		/// <summary> The ramp up parameter. </summary>
		public int RampUp { get; }
		/// <summary> The rampo down parameter. </summary>
		public int RampDown { get; }
		/// <summary> The forward period. </summary>
		public decimal FwdFreq { get; }
		/// <summary> The reverse period. </summary>
		public decimal RevFreq { get; }
		/// <summary> Gets the identifier of the motor. </summary>
		/// <value> The identifier of the motor. </value>
		public char MotorID { get; }

		/// <summary> Gets the motor description. </summary>
		/// <returns> The motor description strings. </returns>
		public IEnumerable<string> Description()
		{
			List<string> list = new List<string>
			{
				string.Format("Motor ID {0}", MotorID),
				string.Format("Loop State {0}", LoopState),
				string.Format("Motor State {0}", MotorState),
				string.Format("Current {0:N2}A", Current),
				string.Format("Fwd Freqency {0:N1}kHz", FwdFreq),
				string.Format("Rev Frequency {0:N1}kHz", RevFreq),
				string.Format("RampUp {0}", RampUp),
				string.Format("RampDown {0}", RampDown)
			};
			return list;
		}
	}
}
