using System;
using System.Collections.Generic;
using System.Linq;
using Thorlabs.Elliptec.ELLO_DLL;
using Thorlabs.Elliptec.ELLO_DLL.Support;

// namespace: Thorlabs.Elliptec.ELLO.Model
//
// summary:	A Collection classes to drive the Elliptec Devices
namespace Thorlabs.Elliptec.ELLO.Model
{
	/// <summary> Elliptec device sequence. </summary>
	public class ELLDeviceSequence
	{
		/// <summary> Values that represent ELLCommands. </summary>
		public enum ELLCommands
		{
			/// <summary> An enum constant representing the get position option. </summary>
			[StringValue("Get Position")]
			GetPosition,
			/// <summary> An enum constant representing the forward option. </summary>
			[StringValue("Jog Forward")]
			Forward,
			/// <summary> An enum constant representing the backward option. </summary>
			[StringValue("Jog Backward")]
			Backward,
			/// <summary> An enum constant representing the move relative option. </summary>
			[StringValue("Move Relative")]
			MoveRelative,
			/// <summary> An enum constant representing the move absolute option. </summary>
			[StringValue("Move Absolute")]
			MoveAbsolute,
			/// <summary> An enum constant representing the home option. </summary>
			[StringValue("Home")]
			Home,
			/// <summary> An enum constant representing the home option. </summary>
			[StringValue("Set Jog Step Size")]
			SetJogstepSize,
		}

		/// <summary> Sequence item. </summary>
		public class SequenceItem
		{
			/// <summary> Constructor. </summary>
			/// <param name="command">	   The command. </param>
			/// <param name="address">	   The address. </param>
			/// <param name="func">		   The function. </param>
			/// <param name="description"> The description. </param>
			/// <param name="wait">		   The wait. </param>
			public SequenceItem(ELLCommands command, char address, Func<bool> func, string description, TimeSpan wait)
			{
				Address = address;
				WaitTime = wait;
				ELLFunction = func;
				Description = description;
				Command = command;
			}
			
			/// <summary> Gets or sets the command. </summary>
			/// <value> The command. </value>
			public ELLCommands Command { get; private set; }

			/// <summary> Gets or sets the time of the wait. </summary>
			/// <value> The time of the wait. </value>
			public TimeSpan WaitTime { get; private set; }

			/// <summary> Gets or sets the Elliptec function. </summary>
			/// <value> The Elliptec function. </value>
			public Func<bool> ELLFunction { get; }

			/// <summary> Gets or sets the address. </summary>
			/// <value> The address. </value>
			public char Address { get; }

			/// <summary> Gets or sets the description. </summary>
			/// <value> The description. </value>
			public string Description { get; }

			/// <summary> Executes this object. </summary>
			/// <returns> Success. </returns>
			public bool Execute()
			{
				return ELLFunction.Invoke();
			}
		}

		/// <summary> Creates a command. </summary>
		/// <param name="devices">	  The devices. </param>
		/// <param name="addresses">  The addresses. </param>
		/// <param name="command">    The command. </param>
		/// <param name="wait">		  The wait. </param>
		/// <param name="parameter1"> The first parameter. </param>
		/// <param name="parameter2"> The second parameter. </param>
		/// <returns> The new command. </returns>
		public SequenceItem CreateCommand(ELLDevices devices, List<char> addresses, ELLCommands command, TimeSpan wait, decimal parameter1, ELLBaseDevice.DeviceDirection parameter2)
		{
			if (!addresses.Any())
			{
				return null;
			}
			ELLDevice device = devices.AddressedDevice(addresses.First()) as ELLDevice;
		    if (device == null)
		    {
		        return null;
		    }
			if (addresses.Count == 1)
			{
				switch (command)
				{
					case ELLCommands.GetPosition:
						return new SequenceItem(command, device.Address, device.GetPosition, command.GetStringValue(), wait);
					case ELLCommands.Forward:
						return new SequenceItem(command, device.Address, device.JogForward, command.GetStringValue(), wait);
					case ELLCommands.Backward:
						return new SequenceItem(command, device.Address, device.JogBackward, command.GetStringValue(), wait);
					case ELLCommands.Home:
						return new SequenceItem(command, device.Address, () => device.Home(parameter2), command.GetStringValue(), wait);
					case ELLCommands.MoveRelative:
						return new SequenceItem(command, device.Address, () => device.MoveRelative(parameter1), command.GetStringValue(), wait);
					case ELLCommands.MoveAbsolute:
						return new SequenceItem(command, device.Address, () => device.MoveAbsolute(parameter1), command.GetStringValue(), wait);
					case ELLCommands.SetJogstepSize:
						return new SequenceItem(command, device.Address, () => device.SetJogstepSize(parameter1), command.GetStringValue(), wait);
				}
			}
			else
			{
				switch (command)
				{
					case ELLCommands.Forward:
						return new SequenceItem(command, device.Address, () => device.JogForward(addresses), command.GetStringValue(), wait);
					case ELLCommands.Backward:
						return new SequenceItem(command, device.Address, () => device.JogBackward(addresses), command.GetStringValue(), wait);
					case ELLCommands.Home:
						return new SequenceItem(command, device.Address, () => device.Home(addresses, parameter2), command.GetStringValue(), wait);
					case ELLCommands.MoveRelative:
						return new SequenceItem(command, device.Address, () => device.MoveRelative(addresses, parameter1), command.GetStringValue(), wait);
					case ELLCommands.MoveAbsolute:
						return new SequenceItem(command, device.Address, () => device.MoveAbsolute(addresses, parameter1), command.GetStringValue(), wait);
				}
			}
			return null;
		}

		/// <summary> Gets or sets the sequence. </summary>
		/// <value> The sequence. </value>
		public List<SequenceItem> Sequence { get; set; }
	}
}
