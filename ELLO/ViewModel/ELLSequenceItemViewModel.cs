using System;
using System.Collections.Generic;
using System.Linq;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO.Model;
using Thorlabs.Elliptec.ELLO_DLL;
using Thorlabs.Elliptec.ELLO_DLL.Support;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
	/// <summary> Interface for sequence root. </summary>
	public interface ISequenceRoot
	{
		void OnUpdate();

		Dictionary<char, DeviceID.DeviceTypes> DeviceTypes { get; }
	}

	/// <summary> Checkable address. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.ViewModel.CheckableItem{System.Char}"/>
// ReSharper disable once ClassNeverInstantiated.Global
	public class CheckableAddress : CheckableItem<char>
	{
		public CheckableAddress(ICheckableItemOwner<char> owner, char c)
			: base(owner, c)
		{
		}
	}

	/// <summary> Collection of checkable address. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.ViewModel.CheckableCollection{Thorlabs.Elliptec.ELLO.ViewModel.CheckableAddress,System.Char}"/>
	public class CheckableAddressCollection : CheckableCollection<CheckableAddress, char>
	{
		public CheckableAddressCollection(IEnumerable<char> list)
			: base(list)
		{
			
		}

		/// <summary> Updates the item value. </summary>
		/// <param name="oldValue"> The old value. </param>
		/// <param name="newValue"> The new value. </param>
		public void UpdateItemValue(char oldValue, char newValue)
		{
			foreach (CheckableAddress item in Items.Where(item => item.Value == oldValue))
			{
				item.UpdateValue(newValue);
			}
			UpdateSelectedAddressText();
		}
	}

	/// <summary> Elliptec sequence item. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.Support.ObservableObject"/>
	public class ELLSequenceItemViewModel : ObservableObject
	{
		private static readonly Dictionary<DeviceID.DeviceTypes, List<ELLDeviceSequence.ELLCommands>> _deviceCommands = new Dictionary<DeviceID.DeviceTypes, List<ELLDeviceSequence.ELLCommands>>
		{
		    {DeviceID.DeviceTypes.Paddle, new List<ELLDeviceSequence.ELLCommands>() },
			{DeviceID.DeviceTypes.RotaryStage, new List<ELLDeviceSequence.ELLCommands> {
					ELLDeviceSequence.ELLCommands.GetPosition,
					ELLDeviceSequence.ELLCommands.Home,
					ELLDeviceSequence.ELLCommands.MoveAbsolute,
					ELLDeviceSequence.ELLCommands.MoveRelative,
					ELLDeviceSequence.ELLCommands.SetJogstepSize,
					ELLDeviceSequence.ELLCommands.Forward,
					ELLDeviceSequence.ELLCommands.Backward,
				}},
		    {DeviceID.DeviceTypes.RotaryStage18, new List<ELLDeviceSequence.ELLCommands> {
		        ELLDeviceSequence.ELLCommands.GetPosition,
		        ELLDeviceSequence.ELLCommands.Home,
		        ELLDeviceSequence.ELLCommands.MoveAbsolute,
		        ELLDeviceSequence.ELLCommands.MoveRelative,
		        ELLDeviceSequence.ELLCommands.SetJogstepSize,
		        ELLDeviceSequence.ELLCommands.Forward,
		        ELLDeviceSequence.ELLCommands.Backward,
		    }},
			{DeviceID.DeviceTypes.Rotator, new List<ELLDeviceSequence.ELLCommands> {
					ELLDeviceSequence.ELLCommands.GetPosition,
					ELLDeviceSequence.ELLCommands.Home,
					ELLDeviceSequence.ELLCommands.MoveAbsolute,
					ELLDeviceSequence.ELLCommands.MoveRelative,
					ELLDeviceSequence.ELLCommands.SetJogstepSize,
					ELLDeviceSequence.ELLCommands.Forward,
					ELLDeviceSequence.ELLCommands.Backward,
				}},
		    {DeviceID.DeviceTypes.OpticsRotator, new List<ELLDeviceSequence.ELLCommands> {
		        ELLDeviceSequence.ELLCommands.GetPosition,
		        ELLDeviceSequence.ELLCommands.Home,
		        ELLDeviceSequence.ELLCommands.MoveAbsolute,
		        ELLDeviceSequence.ELLCommands.MoveRelative,
		        ELLDeviceSequence.ELLCommands.SetJogstepSize,
		        ELLDeviceSequence.ELLCommands.Forward,
		        ELLDeviceSequence.ELLCommands.Backward,
		    }},
			{DeviceID.DeviceTypes.LinearStage, new List<ELLDeviceSequence.ELLCommands> {
					ELLDeviceSequence.ELLCommands.GetPosition,
					ELLDeviceSequence.ELLCommands.Home,
					ELLDeviceSequence.ELLCommands.MoveAbsolute,
					ELLDeviceSequence.ELLCommands.MoveRelative,
					ELLDeviceSequence.ELLCommands.SetJogstepSize,
					ELLDeviceSequence.ELLCommands.Forward,
					ELLDeviceSequence.ELLCommands.Backward,
				}},
		    {DeviceID.DeviceTypes.LinearStage2, new List<ELLDeviceSequence.ELLCommands> {
		        ELLDeviceSequence.ELLCommands.GetPosition,
		        ELLDeviceSequence.ELLCommands.Home,
		        ELLDeviceSequence.ELLCommands.MoveAbsolute,
		        ELLDeviceSequence.ELLCommands.MoveRelative,
		        ELLDeviceSequence.ELLCommands.SetJogstepSize,
		        ELLDeviceSequence.ELLCommands.Forward,
		        ELLDeviceSequence.ELLCommands.Backward,
		    }},
		    {DeviceID.DeviceTypes.LinearStage17, new List<ELLDeviceSequence.ELLCommands> {
		        ELLDeviceSequence.ELLCommands.GetPosition,
		        ELLDeviceSequence.ELLCommands.Home,
		        ELLDeviceSequence.ELLCommands.MoveAbsolute,
		        ELLDeviceSequence.ELLCommands.MoveRelative,
		        ELLDeviceSequence.ELLCommands.SetJogstepSize,
		        ELLDeviceSequence.ELLCommands.Forward,
		        ELLDeviceSequence.ELLCommands.Backward,
		    }},
		    {DeviceID.DeviceTypes.LinearStage20, new List<ELLDeviceSequence.ELLCommands> {
		        ELLDeviceSequence.ELLCommands.GetPosition,
		        ELLDeviceSequence.ELLCommands.Home,
		        ELLDeviceSequence.ELLCommands.MoveAbsolute,
		        ELLDeviceSequence.ELLCommands.MoveRelative,
		        ELLDeviceSequence.ELLCommands.SetJogstepSize,
		        ELLDeviceSequence.ELLCommands.Forward,
		        ELLDeviceSequence.ELLCommands.Backward,
		    }},
			{DeviceID.DeviceTypes.Actuator, new List<ELLDeviceSequence.ELLCommands> {
					ELLDeviceSequence.ELLCommands.GetPosition,
					ELLDeviceSequence.ELLCommands.Forward,
					ELLDeviceSequence.ELLCommands.Backward,
				}},
			{DeviceID.DeviceTypes.Shutter, new List<ELLDeviceSequence.ELLCommands> {
					ELLDeviceSequence.ELLCommands.GetPosition,
					ELLDeviceSequence.ELLCommands.Forward,
					ELLDeviceSequence.ELLCommands.Backward,
				}},
		    {DeviceID.DeviceTypes.Shutter4, new List<ELLDeviceSequence.ELLCommands> {
		        ELLDeviceSequence.ELLCommands.GetPosition,
		        ELLDeviceSequence.ELLCommands.Forward,
		        ELLDeviceSequence.ELLCommands.Backward,
		        ELLDeviceSequence.ELLCommands.MoveAbsolute,
		    }},
			};

		private static readonly List<ELLDeviceSequence.ELLCommands> _synchroDeviceCommands = new List<ELLDeviceSequence.ELLCommands>
		{
				ELLDeviceSequence.ELLCommands.Home,
				ELLDeviceSequence.ELLCommands.MoveAbsolute,
				ELLDeviceSequence.ELLCommands.MoveRelative,
				ELLDeviceSequence.ELLCommands.Forward,
				ELLDeviceSequence.ELLCommands.Backward,
			};

		private static readonly Dictionary<DeviceID.DeviceTypes, Dictionary<ELLBaseDevice.DeviceDirection, string>> _deviceDirections = new Dictionary<DeviceID.DeviceTypes, Dictionary<ELLBaseDevice.DeviceDirection, string>>
		{
			{DeviceID.DeviceTypes.RotaryStage, new Dictionary<ELLBaseDevice.DeviceDirection, string> {
					{ ELLBaseDevice.DeviceDirection.Clockwise, "Clockwise" },
					{ ELLBaseDevice.DeviceDirection.AntiClockwise, "Anticlockwise" }
				}},
		    {DeviceID.DeviceTypes.RotaryStage18, new Dictionary<ELLBaseDevice.DeviceDirection, string> {
		        { ELLBaseDevice.DeviceDirection.Clockwise, "Clockwise" },
		        { ELLBaseDevice.DeviceDirection.AntiClockwise, "Anticlockwise" }
		    }},
			{DeviceID.DeviceTypes.Rotator, new Dictionary<ELLBaseDevice.DeviceDirection, string> {
					{ ELLBaseDevice.DeviceDirection.Clockwise, "Clockwise" },
					{ ELLBaseDevice.DeviceDirection.AntiClockwise, "Anticlockwise" }
				}},
		    {DeviceID.DeviceTypes.OpticsRotator, new Dictionary<ELLBaseDevice.DeviceDirection, string> {
		        { ELLBaseDevice.DeviceDirection.Clockwise, "Clockwise" },
		        { ELLBaseDevice.DeviceDirection.AntiClockwise, "Anticlockwise" }
		    }},
			{DeviceID.DeviceTypes.LinearStage, new Dictionary<ELLBaseDevice.DeviceDirection, string> {
					{ ELLBaseDevice.DeviceDirection.Linear, "Linear" },
				}},
		    {DeviceID.DeviceTypes.LinearStage2, new Dictionary<ELLBaseDevice.DeviceDirection, string> {
		            { ELLBaseDevice.DeviceDirection.Linear, "Linear" },
		        }},
		    {DeviceID.DeviceTypes.LinearStage17, new Dictionary<ELLBaseDevice.DeviceDirection, string> {
		        { ELLBaseDevice.DeviceDirection.Linear, "Linear" },
		    }},
		    {DeviceID.DeviceTypes.LinearStage20, new Dictionary<ELLBaseDevice.DeviceDirection, string> {
		        { ELLBaseDevice.DeviceDirection.Linear, "Linear" },
		    }},
			{DeviceID.DeviceTypes.Actuator, new Dictionary<ELLBaseDevice.DeviceDirection, string>()},
			{DeviceID.DeviceTypes.Shutter, new Dictionary<ELLBaseDevice.DeviceDirection, string>()},
		    {DeviceID.DeviceTypes.Shutter4, new Dictionary<ELLBaseDevice.DeviceDirection, string>()},
		    {DeviceID.DeviceTypes.Paddle, new Dictionary<ELLBaseDevice.DeviceDirection, string>()},
		};

		private readonly ISequenceRoot _owner;
		private readonly ELLSequenceItemXML _sequenceItem;

		private Dictionary<ELLBaseDevice.DeviceDirection, string> _directions;

		private Dictionary<ELLDeviceSequence.ELLCommands, string> _commands;

		private bool _allowZeroWait;
	    private bool _allowZeroParameter;

        private readonly CheckableAddressCollection _addressCollection;

		/// <summary> Default constructor. </summary>
		public ELLSequenceItemViewModel()
		{
			_sequenceItem = new ELLSequenceItemXML();
			_addressCollection = new CheckableAddressCollection(new List<char>());
			_addressCollection.ItemUpdate += AddressCollection_ItemUpdate;
		}

		/// <summary> Constructor. </summary>
		/// <param name="owner">	   The owner. </param>
		/// <param name="item">		   The item. </param>
		/// <param name="index">	   The index. </param>
		public ELLSequenceItemViewModel(ISequenceRoot owner, ELLSequenceItemXML item, int index)
		{
			_owner = owner;
			_sequenceItem = item;
			Index = index;
			_addressCollection = new CheckableAddressCollection(_owner.DeviceTypes.Keys.OrderBy(ch => ch));

			List<char> currentAddresses = new List<char>();
			foreach (char address in item.Addresses)
			{
				if (_owner.DeviceTypes.ContainsKey(address))
				{
					currentAddresses.Add(address);
					_addressCollection.Items.First(i => i.Value == address).IsChecked = true;
				}
			}
			if (currentAddresses.Count == 0)
			{
				currentAddresses.Add(_owner.DeviceTypes.Keys.First());
				_addressCollection.Items.First(i => i.Value == _owner.DeviceTypes.Keys.First()).IsChecked = true;
			}
			_sequenceItem.Addresses = currentAddresses;
			UpdateOptions();

			_addressCollection.ItemUpdate += AddressCollection_ItemUpdate;
			_owner = owner;
		}

		public void ReaddressSequenceItem(char oldAddress, char newAddress)
		{
			Addresses.UpdateItemValue(oldAddress, newAddress);
			_sequenceItem.Addresses = Addresses.CheckedItems.Select(i => i.Value).ToList();
			UpdateOptions();
		}

		/// <summary> Address collection item update. </summary>
		/// <exception cref="NotImplementedException"> Thrown when the requested operation is
		/// 										   unimplemented. </exception>
		/// <param name="sender"> Source of the event. </param>
		/// <param name="e">	  The CheckableItemStateChanged&lt;char&gt; to process. </param>
		private void AddressCollection_ItemUpdate(object sender, CheckableItemStateChanged<char> e)
		{
			_sequenceItem.Addresses = Addresses.CheckedItems.Select(i => i.Value).ToList();
			UpdateOptions();
		}

		/// <summary> Gets the directions. </summary>
		/// <value> The directions. </value>
		public Dictionary<ELLBaseDevice.DeviceDirection, string> Directions
		{
			get { return _directions; }
			set
			{
				_directions = value;
				RaisePropertyChanged(() => Directions);
			}
		}

		/// <summary> Gets a collection of checkable address. </summary>
		/// <value> . </value>
		public CheckableAddressCollection Addresses
		{
			get { return _addressCollection; }
		}

		/// <summary> Gets the commands. </summary>
		/// <value> The commands. </value>
		public Dictionary<ELLDeviceSequence.ELLCommands, string> Commands
		{
			get { return _commands; }
			set
			{
				_commands = value;
				RaisePropertyChanged(() => Commands);
			}
		}


		private void UpdateOptions()
		{
			int addressCount = _sequenceItem.Addresses.Count;
			if (addressCount == 0)
			{
				Commands = new Dictionary<ELLDeviceSequence.ELLCommands, string>();
				Directions = new Dictionary<ELLBaseDevice.DeviceDirection, string>();
			}
			else if (addressCount == 1)
			{
				char address = _sequenceItem.Addresses.First();
				DeviceID.DeviceTypes deviceType = (_owner.DeviceTypes.ContainsKey(address)) ? _owner.DeviceTypes[address] : DeviceID.DeviceTypes.Shutter;
				Commands = _deviceCommands[deviceType].ToDictionary(i => i, i => i.GetStringValue());
				Directions = _deviceDirections[deviceType];
			    if ((Commands.Count != 0) && (Directions.Count != 0))
			    {
			        if (!Commands.ContainsKey(_sequenceItem.Command))
			        {
			            _sequenceItem.Command = Commands.First().Key;
			        }
			        if (!Directions.ContainsKey(_sequenceItem.Parameter2))
			        {
			            _sequenceItem.Parameter2 = Directions.Any() ? Directions.First().Key : ELLBaseDevice.DeviceDirection.Linear;
			        }
			    }
            }
			else
			{
				List<ELLDeviceSequence.ELLCommands> synchCommands = new List<ELLDeviceSequence.ELLCommands>(_synchroDeviceCommands);
				List<DeviceID.DeviceTypes> deviceTypes = _sequenceItem.Addresses.Select(c => _owner.DeviceTypes[c]).Distinct().ToList();
				foreach (DeviceID.DeviceTypes deviceType in deviceTypes)
				{
					List<ELLDeviceSequence.ELLCommands> synchCommands2 = new List<ELLDeviceSequence.ELLCommands>(synchCommands);
					foreach (ELLDeviceSequence.ELLCommands cmd in synchCommands2)
					{
						if (!_deviceCommands[deviceType].Contains(cmd))
						{
							synchCommands.Remove(cmd);
						}
					}
				}
				Commands = synchCommands.ToDictionary(i => i, i => i.GetStringValue());
				Directions = _deviceDirections[deviceTypes[0]];
				if (!Commands.ContainsKey(_sequenceItem.Command))
				{
					_sequenceItem.Command = Commands.First().Key;
				}
				if (!Directions.ContainsKey(_sequenceItem.Parameter2))
				{
					_sequenceItem.Parameter2 = Directions.Any() ? Directions.First().Key : ELLBaseDevice.DeviceDirection.Linear;
				}
			}
			UpdateAllowZero();
			_owner?.OnUpdate();
		}

		private void UpdateAllowZero()
		{
			AllowZeroWait = _sequenceItem.Command == ELLDeviceSequence.ELLCommands.GetPosition;
			if (!AllowZeroWait && _sequenceItem.WaitTime.TotalSeconds < 1)
			{
				_sequenceItem.WaitTime = new TimeSpan(0,0,1);
			}
		    AllowZeroParameter = _sequenceItem.Command != ELLDeviceSequence.ELLCommands.SetJogstepSize;
		    if (!AllowZeroParameter && _sequenceItem.Parameter1 == 0)
		    {
		        _sequenceItem.Parameter1 = 1;
		    }
		}

        /// <summary> Gets the item. </summary>
        /// <value> The item. </value>
        public ELLSequenceItemXML Item
		{ get { return _sequenceItem; }}

		/// <summary> Gets or sets the time of the wait. </summary>
		/// <value> The time of the wait. </value>
		public ELLDeviceSequence.ELLCommands Command
		{
			get { return _sequenceItem.Command; }
			set
			{
				_sequenceItem.Command = value;
				RaisePropertyChanged(() => WaitTime);
				RaisePropertyChanged(() => ShowParam1);
				RaisePropertyChanged(() => ShowParam2);
				_owner?.OnUpdate();
				UpdateAllowZero();
			}
		}

		/// <summary> Gets or sets the time of the wait. </summary>
		/// <value> The time of the wait. </value>
		public decimal WaitTime
		{
			get { return (decimal)_sequenceItem.WaitTime.TotalMilliseconds / 1000; }
			set
			{
				_sequenceItem.WaitTime = TimeSpan.FromMilliseconds((double)(value * 1000));
				RaisePropertyChanged(() => WaitTime);
			}
		}

		/// <summary> Gets or sets a value indicating whether we allow zero. </summary>
		/// <value> true if allow zero, false if not. </value>
		public bool AllowZeroWait
		{
			get { return _allowZeroWait; }
			set
			{
				_allowZeroWait = value;
				RaisePropertyChanged(() => AllowZeroWait);
			}
		}

	    /// <summary> Gets or sets a value indicating whether we allow zero. </summary>
	    /// <value> true if allow zero, false if not. </value>
	    public bool AllowZeroParameter
	    {
	        get { return _allowZeroParameter; }
	        set
	        {
	            _allowZeroParameter = value;
	            RaisePropertyChanged(() => AllowZeroParameter);
	        }
	    }

        /// <summary> Gets or sets the parameter 1. </summary>
        /// <value> The parameter 1. </value>
        public decimal Parameter1
		{
			get { return _sequenceItem.Parameter1; }
			set
			{
			    if (!AllowZeroParameter && (value <= 0))
			    {
			        throw new Exception("Value must be greater than zero");
			    }
				_sequenceItem.Parameter1 = value;
				RaisePropertyChanged(() => WaitTime);
			}
		}

		/// <summary> Gets or sets a value indicating whether the parameter 1 is shown. </summary>
		/// <value> true if show parameter 1, false if not. </value>
		public bool ShowParam1
		{
			get { return ((_sequenceItem.Command == ELLDeviceSequence.ELLCommands.MoveAbsolute) || (_sequenceItem.Command == ELLDeviceSequence.ELLCommands.MoveRelative) || (_sequenceItem.Command == ELLDeviceSequence.ELLCommands.SetJogstepSize)); }
		}

		/// <summary> Gets or sets the parameter 2. </summary>
		/// <value> The parameter 2. </value>
		public ELLBaseDevice.DeviceDirection Parameter2
		{
			get { return _sequenceItem.Parameter2; }
			set
			{
				_sequenceItem.Parameter2 = value;
				RaisePropertyChanged(() => WaitTime);
			}
		}

		/// <summary> Gets or sets a value indicating whether the parameter 2 is shown. </summary>
		/// <value> true if show parameter 2, false if not. </value>
		public bool ShowParam2
		{
			get { return (_sequenceItem.Command == ELLDeviceSequence.ELLCommands.Home); }
		}

		/// <summary> Gets or sets zero-based index of this object. </summary>
		/// <value> The index. </value>
		public int Index { get; }
	}
}
