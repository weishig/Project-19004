using System.Collections.Generic;

namespace Thorlabs.Elliptec.ELLO_DLL
{
	/// <summary> Message update handler class. </summary>
	/// <remarks> This class is used to broadcast state and parameter changes within the DLL</remarks>
	public class MessageUpdater
	{
		/// <summary> Values that represent Update Types. </summary>
		public enum UpdateTypes
		{
			/// <summary> The Elliptec Device status has changed. </summary>
			Status,
			/// <summary> New Elliptec <see cref="ELLO_DLL.MotorInfo"/> data is available. </summary>
			MotorInfo,
			/// <summary> New Elliptec <see cref="DeviceID"/> data is available. </summary>
			DeviceInfo,
			/// <summary> The Elliptec Devices position has changed. </summary>
			Position,
			/// <summary> The Elliptec Devices Home Offset has been changed. </summary>
			HomeOffset,
			/// <summary> The Elliptec devices Jog Step Size has been changed. </summary>
			JogstepSize,
            /// <summary>  The Elliptec Polarizer Paddle positions have changed. </summary>
            PolarizerPositions,
		    /// <summary>  An Elliptec Paddle position has changed. </summary>
		    PaddlePosition,
		}


        /// <summary> The output Update. </summary>
        /// <remarks> Notification event used to update the world of new outputs available</remarks>
        public ELLBaseDevice.OutputUpdateDelegate OutputUpdate;

        /// <summary> The Parameter update delegate. </summary>
        /// <param name="type"> The parameter type updated. </param>
        /// <param name="data"> The new parameter value. </param>
        /// <param name="address"> The parameter address. </param>
        public delegate void ParameterUpdateDelegate(UpdateTypes type, char address, object data);

		/// <summary> The parameter update. </summary>
		/// <remarks> Called whenever a device parameter changes. </remarks>
		public ParameterUpdateDelegate ParameterUpdate;

		/// <summary> Fires the <see cref="OutputUpdate"/> event with the supplied output string. </summary>
		/// <param name="message">	The message. </param>
		/// <param name="error">  	true to error. </param>
		internal void UpdateOutput(string message, bool error = false)
		{
			if (OutputUpdate != null)
			{
				List<string> list = new List<string> { message };
				OutputUpdate(list, error);
			}
		}

		/// <summary> Fires the <see cref="OutputUpdate"/> event with the supplied output list. </summary>
		/// <param name="list">  The list. </param>
		/// <param name="error"> true to error. </param>
		internal void UpdateOutput(List<string> list, bool error = false)
		{
			OutputUpdate?.Invoke(list, error);
		}

		/// <summary> Fires the <see cref="ParameterUpdate"/> event with the supplied update parameters. </summary>
		/// <param name="updateType"> The parameter update type, <see cref="UpdateTypes"/>. </param>
		/// <param name="address">    The Elliptec device bus address. </param>
		/// <param name="data">		  The new data applied to the appropriate parameter. </param>
		internal void UpdateParameter(UpdateTypes updateType, char address, object data)
		{
			ParameterUpdate?.Invoke(updateType, address, data);
		}

		/// <summary> Fires the <see cref="ParameterUpdate"/> event with the supplied update parameters. </summary>
		/// <param name="message"> The update message. </param>
		/// <param name="list">    The list of update strings. </param>
		internal void UpdateOutput(string message, IEnumerable<string> list)
		{
			if (OutputUpdate != null)
			{

				List<string> list1 = new List<string> { message };
				list1.AddRange(list);
				OutputUpdate(list1, false);
			}
		}
	}
}
