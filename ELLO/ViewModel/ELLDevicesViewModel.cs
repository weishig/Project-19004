using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO_DLL;
using Thorlabs.Elliptec.ELLO_DLL.Support;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
	/// <summary> Output item. </summary>
	public class OutputItem
	{
		/// <summary> Values that represent OutputItemType. </summary>
		public enum OutputItemType
		{
			/// <summary> An enum constant representing the normal option. </summary>
			Normal,
			/// <summary> An enum constant representing the error option. </summary>
			Error,
			/// <summary> An enum constant representing the receive option. </summary>
			Rx,
			/// <summary> An enum constant representing the transmit option. </summary>
			Tx,
		}

		private static readonly Dictionary<OutputItemType, Brush> _brushes = new Dictionary<OutputItemType, Brush>
		{
			{ OutputItemType.Normal, Brushes.Black }, { OutputItemType.Error, Brushes.Red }, { OutputItemType.Rx, Brushes.ForestGreen}, {OutputItemType.Tx, Brushes.Blue}
		};

		/// <summary> Constructor. </summary>
		/// <param name="text">   The text. </param>
		/// <param name="type">   The type. </param>
		/// <param name="indent"> The indent. </param>
		public OutputItem(string text, OutputItemType type, int indent = 0)
		{
			Text = text;
			ItemType = type;
			DisplayColour = _brushes[type];
			Margin = new Thickness(0 + (indent * 10), 0, 0, 0);
		}

		/// <summary> Gets or sets the text. </summary>
		/// <value> The text. </value>
		public string Text { get; private set; }

		/// <summary> Gets or sets the colour of the display. </summary>
		/// <value> The colour of the display. </value>
		public Brush DisplayColour { get; private set; }

		/// <summary> Gets or sets the type. </summary>
		/// <value> The type. </value>
		public OutputItemType ItemType { get; private set; }

		/// <summary> Gets the margin. </summary>
		/// <value> The margin. </value>
		public Thickness Margin { get; }
	}

	/// <summary> Display view model. </summary>
	/// <seealso cref="T:ELLO.Support.ObservableObject"/>
	public class ELLDevicesViewModel : ObservableObject
	{
		private List<string> _availablePorts;
		private string _selectedPort;
		private bool _isConnected;
		private readonly ELLDevices _ellDevice = new ELLDevices();
		private char _oldAddress;
		private char _newAddress;
		private bool _canReaddress;
		private List<char> _usedAddress;
		private List<char> _freeAddress;
		private List<char> _validAddress;
		private readonly Dictionary<char, ELLDeviceBaseViewModel> _deviceViewModels = new Dictionary<char, ELLDeviceBaseViewModel>();
		private ObservableCollection<ELLDeviceBaseViewModel> _observableDeviceViewModels = new ObservableCollection<ELLDeviceBaseViewModel>();
		private ELLDeviceBaseViewModel _selectedViewModel;
		private ELLSequenceViewModel _sequenceViewModel;
		private string _freeCommandText;
		private ObservableCollection<string> _freeCommandStore = new ObservableCollection<string>();
	    private bool _showSequncer;

		/// <summary> Gets or sets the manager for background thread. </summary>
		/// <value> The background thread manager. </value>
		public BackgroundThreadManager BackgroundThreadManager { get; private set; }

		/// <summary> The dispatcher. </summary>
		private readonly ViewModelDispatcher _dispatcher = new ViewModelDispatcher();

		private ObservableCollection<OutputItem> _commsLog;
		private readonly List<OutputItem> _commsLogStore;
		private readonly List<OutputItem.OutputItemType> _commsLogTypes = new List<OutputItem.OutputItemType>
			{
				OutputItem.OutputItemType.Normal,
				OutputItem.OutputItemType.Error,
				OutputItem.OutputItemType.Rx,
				OutputItem.OutputItemType.Tx
			}; 
 
		private OutputItem _commsLogSelected;

		/// <summary> The get connect command. </summary>
		private ICommand _getConnectCommand;
		/// <summary> The get set address command. </summary>
		private ICommand _getSetAddressCommand;
		/// <summary> The get send free command. </summary>
		private ICommand _getSendFreeCommandCommand;
		/// <summary> The get add free command. </summary>
		private ICommand _getAddFreeCommandCommand;

		private char _maxSearchLimit;
		private char _minSearchLimit;

		/// <summary> Gets the click connect command. </summary>
		/// <value> The click connect command. </value>
		public ICommand ClickConnectCommand { get { return _getConnectCommand ?? (_getConnectCommand = new RelayCommand(Connect)); } }


		/// <summary> Gets the click set address command. </summary>
		/// <value> The click set address command. </value>
		public ICommand ClickSetAddressCommand { get { return _getSetAddressCommand ?? (_getSetAddressCommand = new RelayCommand(SetAddress)); } }

		/// <summary> Gets the click send free command. </summary>
		/// <value> The click send free command. </value>
		public ICommand ClickSendFreeCommandCommand { get { return _getSendFreeCommandCommand ?? (_getSendFreeCommandCommand = new RelayCommand(SendFreeCommand)); } }

		/// <summary> Gets the click add free command. </summary>
		/// <value> The click add free command. </value>
		public ICommand ClickAddFreeCommandCommand { get { return _getAddFreeCommandCommand ?? (_getAddFreeCommandCommand = new RelayCommand(AddFreeCommand)); } }

		/// <summary> Default constructor. </summary>
		public ELLDevicesViewModel()
		{
			MaxSearchLimit = 'F';
			MinSearchLimit = '0';
			SelectedPort = CommonUserSettings.ReadUserSetting("DevicePort", "Com1");
			ShowTxLoggedCmds = CommonUserSettings.ReadUserSetting("ShowTxOutput", true);
			ShowRxLoggedCmds = CommonUserSettings.ReadUserSetting("ShowRxOutput", true);
			ValidAddress = _ellDevice.ValidAddress;
			AvailablePorts = SerialPort.GetPortNames().ToList();
			if (!AvailablePorts.Contains(SelectedPort))
			{
				SelectedPort = AvailablePorts.Count > 0 ? AvailablePorts[0] : "";
			}
			IsConnected = false;
			_commsLogStore = new List<OutputItem>();
			CommsLog = new ObservableCollection<OutputItem>(_commsLogStore);
			BackgroundThreadManager = new BackgroundThreadManager();
			_ellDevice.MessageUpdates.OutputUpdate += OutputUpdate;
			_ellDevice.MessageUpdates.ParameterUpdate += ParameterUpdate;
			_sequenceViewModel = new ELLSequenceViewModel(this, _ellDevice);
			UpdateUI();
		}

		/// <summary> Gets or sets the available ports. </summary>
		/// <value> The available ports. </value>
		public List<string> AvailablePorts
		{
			get { return _availablePorts; }
			set
			{
				_availablePorts = value;
				RaisePropertyChanged(() => AvailablePorts);
			}
		}

		/// <summary> Gets or sets the selected port. </summary>
		/// <value> The selected port. </value>
		public string SelectedPort
		{
			get { return _selectedPort; }
			set
			{
				_selectedPort = value;
				RaisePropertyChanged(() => SelectedPort);
				if (CommonUserSettings.ReadUserSetting("DevicePort", "Com1") != _selectedPort)
				{
					CommonUserSettings.WriteUserSetting("DevicePort",_selectedPort);
				}
			}
		}

		/// <summary> Gets a list of maximum device counts. </summary>
		/// <value> A List of maximum device counts. </value>
		public List<char> SearchRangeValues
		{
			get { return ELLBaseDevice.ValidAddresses; } 
		}

		/// <summary> Gets or sets the number of maximum devices. </summary>
		/// <value> The number of maximum devices. </value>
		public char MaxSearchLimit
		{
			get { return _maxSearchLimit; }
			set
			{
				_maxSearchLimit = value;
				RaisePropertyChanged(() => MaxSearchLimit);
			}
		}

		/// <summary> Gets or sets the number of maximum devices. </summary>
		/// <value> The number of maximum devices. </value>
		public char MinSearchLimit
		{
			get { return _minSearchLimit; }
			set
			{
				_minSearchLimit = value;
				RaisePropertyChanged(() => MinSearchLimit);
			}
		}

		/// <summary> Gets or sets a value indicating whether this object is connected. </summary>
		/// <value> true if this object is connected, false if not. </value>
		public bool IsConnected
		{
			get { return _isConnected; }
			set
			{
				_isConnected = value;
				RaisePropertyChanged(() => IsConnected);
			}
		}

		private void DisconnectEx()
		{
			ELLDevicePort.DataSent = null;
			ELLDevicePort.DataReceived = null;
			ELLDevicePort.Disconnect();
			IsConnected = ELLDevicePort.IsConnected;
			_deviceViewModels.Clear();
		}

		private void ConnectEx()
		{
			if (ELLDevicePort.IsConnected)
			{
				DisconnectEx();
			}
			else
			{
				bool connected;
				try
				{
					connected = ELLDevicePort.Connect(SelectedPort);
				}
				catch (Exception ex)
				{
					OutputUpdate(ex.Message, OutputItem.OutputItemType.Error);
					connected = false;
				}
				IsConnected = ELLDevicePort.IsConnected;
				if (!connected)
				{
					return;
				}
				if (MinSearchLimit > MaxSearchLimit)
				{
					MaxSearchLimit = MinSearchLimit;
				}
				ELLDevicePort.DataSent = DataSent;
				ELLDevicePort.DataReceived = DataReceived;
				_deviceViewModels.Clear();
				_ellDevice.ClearDevices();
				List<string> devices = _ellDevice.ScanAddresses(MinSearchLimit, MaxSearchLimit);
				if (devices.Count > 0)
				{
					foreach (string deviceID in devices)
					{
						_ellDevice.Configure(deviceID);
						_ellDevice.Connect();
						char address = deviceID[0];
						ELLDeviceBaseViewModel vm = null;
					    DeviceID.DeviceTypes deviceType = _ellDevice.AddressedDevice(address).DeviceInfo.DeviceType;
					    if (deviceType == DeviceID.DeviceTypes.Paddle)
					    {
					        ELLPaddlePolariser device = _ellDevice.AddressedDevice(address) as ELLPaddlePolariser;
					        vm = new ELLPaddlePolariserViewModel(this, device);
					    }
                        else
					    {
                            ELLDevice device = _ellDevice.AddressedDevice(address) as ELLDevice;
					        switch (deviceType)
					        {
					            case DeviceID.DeviceTypes.Actuator:
					                vm = new ELLActuatorViewModel(this, device);
					                break;
					            case DeviceID.DeviceTypes.Shutter:
					                vm = new ELLShutterViewModel(this, device, 2);
					                break;
					            case DeviceID.DeviceTypes.Shutter4:
					                vm = new ELLShutterViewModel(this, device, 4);
					                break;
					            case DeviceID.DeviceTypes.Rotator:
					                vm = new ELLStageViewModel(this, device, 2, true, true, false);
					                break;
					            case DeviceID.DeviceTypes.OpticsRotator:
					                vm = new ELLStageViewModel(this, device, 2, true, true, true);
					                break;
					            case DeviceID.DeviceTypes.RotaryStage18:
					                vm = new ELLStageViewModel(this, device, 2, true, false, true);
					                break;
					            case DeviceID.DeviceTypes.RotaryStage:
					                vm = new ELLStageViewModel(this, device, 2, true, false, false);
					                break;
					            case DeviceID.DeviceTypes.LinearStage:
					            case DeviceID.DeviceTypes.LinearStage2:
					                vm = new ELLStageViewModel(this, device, 2, false, false, false);
					                break;
					            case DeviceID.DeviceTypes.LinearStage17:
					            case DeviceID.DeviceTypes.LinearStage20:
					                vm = new ELLStageViewModel(this, device, 2, false, false, true);
					                break;
					        }
					    }
                        if (vm != null)
						{
							_deviceViewModels[address] = vm;
							foreach (ELLMotorViewModel motor in vm.Motors)
							{
								motor.GetMotorInfoDirect();
							}
                            vm.InitializeViewModel();
						}
						_sequenceViewModel.DeviceTypes = _deviceViewModels.ToDictionary(item => item.Key, item => item.Value.Device.DeviceInfo.DeviceType);
					}

				}
				else
				{
					DisconnectEx();
				}
			}
		}

		private void Connect()
		{
			if (ELLDevicePort.IsConnected)
			{
				OutputUpdate("Disconnecting...", OutputItem.OutputItemType.Normal);
				BackgroundThreadManager.RunBackgroundFunction((s, e) => DisconnectEx(), delegate
				{
					UpdateUI();
					OutputUpdate("Disconnected", OutputItem.OutputItemType.Normal);
				});
			}
			else
			{
				_commsLogStore.Clear();
				CommsLog.Clear();
				OutputUpdate("Connecting...", OutputItem.OutputItemType.Normal);
				BackgroundThreadManager.RunBackgroundFunction((s, e) => ConnectEx(), (e) => UpdateUI());
			}
		}

		/// <summary> Disconnects this object. </summary>
		public void Disconnect()
		{
			if (IsConnected)
			{
				DisconnectEx();
			}
		}

		private void UpdateUI()
		{
			ObservableDeviceViewModels = new ObservableCollection<ELLDeviceBaseViewModel>(_deviceViewModels.Values.OrderBy(i => i.FullDeviceName));
			SelectedViewModel = ObservableDeviceViewModels.FirstOrDefault();
			UsedAddress = _deviceViewModels.Keys.OrderBy(ch => ch).ToList();
			FreeAddress = ValidAddress.Where(ch => !UsedAddress.Contains(ch)).OrderBy(ch => ch).ToList();
			CanReaddress = UsedAddress.Any() && FreeAddress.Any();
			if (CanReaddress)
			{
				NewAddress = FreeAddress.First();
				OldAddress = UsedAddress.First();
			}
			else
			{
				NewAddress = '\0';
				OldAddress = '\0';
			}
            UpdateSequence();
		}

	    public bool ShowSequencer
	    {
	        get { return _showSequncer; }
	        set
	        {
	            _showSequncer = value;
	            RaisePropertyChanged(() => ShowSequencer);
	        }
	    }

        /// <summary>   Updates the sequence. </summary>
	    public void UpdateSequence()
        {
            _sequenceViewModel.CanRun = _deviceViewModels.Values.All(i => i.CanRunSequence);
        }

		/// <summary> Gets the device status. </summary>
		private void GetDeviceStatus()
		{
			if (IsConnected)
			{
				BackgroundThreadManager.RunBackgroundFunction((s, e) => _ellDevice.SelectedDevice.GetDeviceStatus());
			}
		}

		/// <summary> Gets or sets the selected view model. </summary>
		/// <value> The selected view model. </value>
		public ELLDeviceBaseViewModel SelectedViewModel
		{
			get { return _selectedViewModel; }
			set
			{
				_selectedViewModel = value;
				RaisePropertyChanged(() => SelectedViewModel);
				if ((SelectedViewModel != null) && (UsedAddress != null) && UsedAddress.Contains(SelectedViewModel.Device.Address))
				{
					OldAddress = SelectedViewModel.Device.Address;
				}
			    ShowSequencer = (SelectedViewModel != null) && !(SelectedViewModel is ELLPaddlePolariserViewModel);
			}
		}

		/// <summary> Gets or sets the device view model. </summary>
		/// <value> The device view model. </value>
		public ObservableCollection<ELLDeviceBaseViewModel> ObservableDeviceViewModels
		{
			get { return _observableDeviceViewModels; }
			set
			{
				_observableDeviceViewModels = value;
				RaisePropertyChanged(() => ObservableDeviceViewModels);
			}
		}

		/// <summary> Gets or sets the sequence view model. </summary>
		/// <value> The sequence view model. </value>
		public ELLSequenceViewModel SequenceViewModel
		{
			get { return _sequenceViewModel; }
			set
			{
				_sequenceViewModel = value;
				RaisePropertyChanged(() => SequenceViewModel);
			}
		}

		/// <summary> Gets or sets the new address. </summary>
		/// <value> The new address. </value>
		public char NewAddress
		{
			get { return _newAddress; }
			set
			{
				_newAddress = value;
				RaisePropertyChanged(() => NewAddress);
			}
		}

		/// <summary> Gets or sets the old address. </summary>
		/// <value> The old address. </value>
		public char OldAddress
		{
			get { return _oldAddress; }
			set
			{
				_oldAddress = value;
				RaisePropertyChanged(() => OldAddress);
			}
		}

		/// <summary> Gets or sets the old address. </summary>
		/// <value> The old address. </value>
		public bool CanReaddress
		{
			get { return _canReaddress; }
			set
			{
				_canReaddress = value;
				RaisePropertyChanged(() => CanReaddress);
			}
		}

		/// <summary> Gets or sets the used address. </summary>
		/// <value> The used address. </value>
		public List<char> UsedAddress
		{
			get { return _usedAddress; }
			set
			{
				_usedAddress = value;
				RaisePropertyChanged(() => UsedAddress);
			}
		}

		/// <summary> Gets or sets the free address. </summary>
		/// <value> The free address. </value>
		public List<char> FreeAddress
		{
			get { return _freeAddress; }
			set
			{
				_freeAddress = value;
				RaisePropertyChanged(() => FreeAddress);
			}
		}

		/// <summary> Gets or sets the available ports. </summary>
		/// <value> The available ports. </value>
		public List<char> ValidAddress
		{
			get { return _validAddress; }
			set
			{
				_validAddress = value;
				RaisePropertyChanged(() => ValidAddress);
			}
		}

		/// <summary> Sets the address. </summary>
		private void SetAddress()
		{
			if (IsConnected && CanReaddress)
			{
				BackgroundThreadManager.RunBackgroundFunction((s, e) => _ellDevice.ReaddressDevice(OldAddress, NewAddress),
					e =>
					{
						ELLDeviceBaseViewModel vm = _deviceViewModels[OldAddress];
						vm.UpdateAddress();
						_deviceViewModels.Remove(OldAddress);
						_deviceViewModels[NewAddress] = vm;
						_sequenceViewModel.ReaddressSequence(OldAddress, NewAddress);
						UpdateUI();
					});
			}
		}

		private void DataSent(string str)
		{
			OutputUpdate("Tx: " + str, OutputItem.OutputItemType.Tx);
		}

		private void DataReceived(string str)
		{
			if (str.Length < 35)
			{
				OutputUpdate("Rx: " + str, OutputItem.OutputItemType.Rx);
			}
			else
			{
				List<string> list = new List<string> {"Rx: " + str.Substring(0, 35)};
				while (!string.IsNullOrEmpty(str))
				{
					int len = Math.Min(str.Length, 35);
					list.Add("    " + str.Substring(0, len));
					str = str.Substring(len);
				}
				OutputUpdate(list, OutputItem.OutputItemType.Rx);
			}
		}

		private void OutputUpdate(string item, OutputItem.OutputItemType type)
		{
			_dispatcher.BeginInvoke(() =>
			{
				OutputItem outputItem = new OutputItem(item, type);
				_commsLogStore.Add(outputItem);
				if (_commsLogTypes.Contains(type))
				{
					while (CommsLog.Count >= 1000)
					{
						CommsLog.RemoveAt(0);
					}
					CommsLog.Add(outputItem);
					CommsLogSelected = CommsLog.Last();
				}
			});
		}

		private void OutputUpdate(List<string> list, OutputItem.OutputItemType type)
		{
			if ((list == null) || !list.Any())
			{
				return;
			}
			_dispatcher.BeginInvoke(() =>
			{
				List<OutputItem> newItems = new List<OutputItem> { new OutputItem(list[0], type) };
				if (list.Count > 1)
				{
					list.RemoveAt(0);
					newItems.AddRange(list.Select(item => new OutputItem(item, type, 1)).ToList());
				}
				_commsLogStore.AddRange(newItems);
				if (_commsLogTypes.Contains(type))
				{
					List<OutputItem> currentList = CommsLog.ToList();
					currentList.AddRange(newItems);
					while (currentList.Count > 1000)
					{
						currentList.RemoveAt(0);
					}
					CommsLog = new ObservableCollection<OutputItem>(currentList);
					CommsLogSelected = CommsLog.Last();
				}
			});
		}

		private void RebuildOutputList()
		{
			if (_commsLogStore == null)
			{
				return;
			}
			List<OutputItem> newItems = _commsLogStore.Where(item => _commsLogTypes.Contains(item.ItemType)).ToList();
			int count = newItems.Count;
			if (count > 1000)
			{
				newItems = newItems.Skip(Math.Max(0, count - 1000)).ToList();
			}
			_dispatcher.BeginInvoke(() =>
			{
				CommsLog = new ObservableCollection<OutputItem>(newItems);
				CommsLogSelected = CommsLog.Last();
			});
		}

		private void OutputUpdate(List<string> list, bool error)
		{
			if (list.Count == 1)
			{
				OutputUpdate(list[0], error ? OutputItem.OutputItemType.Error : OutputItem.OutputItemType.Normal);
			}
			else
			{
				OutputUpdate(list, error ? OutputItem.OutputItemType.Error : OutputItem.OutputItemType.Normal);
			}
		}

		private void ParameterUpdate(MessageUpdater.UpdateTypes type, char address, object data)
		{
			switch (type)
			{
				case MessageUpdater.UpdateTypes.DeviceInfo:
				{
					DeviceID info = data as DeviceID;
					if ((info != null) && _deviceViewModels.ContainsKey(address))
					{
						_dispatcher.BeginInvoke(() => _deviceViewModels[address].Description = info.Description());
					}
				}
				break;
				case MessageUpdater.UpdateTypes.MotorInfo:
				{
					MotorInfo info = data as MotorInfo;
					if ((info != null) && _deviceViewModels.ContainsKey(address))
					{
						ELLMotorViewModel first = _deviceViewModels[address].Motors.FirstOrDefault(motor => motor.MotorID == info.MotorID);
						if (first != null)
						{
							_dispatcher.BeginInvoke(() => first.UpdateInfo(info));
						}
					}
				}
				break;
				case MessageUpdater.UpdateTypes.Status:
				{
					DeviceStatus status = data as DeviceStatus;
					if (status != null)
					{
					    switch (status.Status)
					    {
                            case DeviceStatus.DeviceStatusValues.OK:
                                break;
					        case DeviceStatus.DeviceStatusValues.Busy:
					            break;
                            default:
                                MessageBox.Show(string.Format("Device error: {0}", status.Status.GetStringValue()), "Device Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
					    }
					}
				}
				break;
				case MessageUpdater.UpdateTypes.Position:
				{
					try
					{
						decimal position = (decimal)data;
						if (_deviceViewModels.ContainsKey(address))
						{
							_dispatcher.BeginInvoke(() => _deviceViewModels[address].UpdatePosition(position));
						}
					}
					catch (Exception e)
					{
						MessageBox.Show(string.Format("Device error: status {0}", e.Message), "Device Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}

				}
				break;
			    case MessageUpdater.UpdateTypes.PolarizerPositions:
			    {
			        try
			        {
			            ELLPaddlePolariser.PolarizerPaddlePositions positions = (ELLPaddlePolariser.PolarizerPaddlePositions)data;
			            if (_deviceViewModels.ContainsKey(address))
			            {
                            ELLPaddlePolariserViewModel paddleViewModel = _deviceViewModels[address] as ELLPaddlePolariserViewModel;
			                if (paddleViewModel != null)
			                {
			                    _dispatcher.BeginInvoke(() => paddleViewModel.UpdatePaddlePosition(positions));
			                }
                        }
			        }
			        catch (Exception e)
			        {
			            MessageBox.Show(string.Format("Device error: status {0}", e.Message), "Device Error", MessageBoxButton.OK, MessageBoxImage.Error);
			        }

			    }
			        break;
			    case MessageUpdater.UpdateTypes.PaddlePosition:
			    {
			        try
			        {
			            ELLPaddlePolariser.PaddlePosition position = (ELLPaddlePolariser.PaddlePosition)data;
			            if (_deviceViewModels.ContainsKey(address))
			            {
			                ELLPaddlePolariserViewModel paddleViewModel = _deviceViewModels[address] as ELLPaddlePolariserViewModel;
			                if (paddleViewModel != null)
			                {
			                    _dispatcher.BeginInvoke(() => paddleViewModel.UpdatePaddlePosition(position));
			                }
			            }
			        }
			        catch (Exception e)
			        {
			            MessageBox.Show(string.Format("Device error: status {0}", e.Message), "Device Error", MessageBoxButton.OK, MessageBoxImage.Error);
			        }

			    }
			        break;
				case MessageUpdater.UpdateTypes.HomeOffset:
				{
					try
					{
						decimal homeOffset = (decimal)data;
						if (_deviceViewModels.ContainsKey(address))
						{
							_dispatcher.BeginInvoke(() => _deviceViewModels[address].UpdateHomeOffset(homeOffset));
						}
					}
					catch (Exception e)
					{
						MessageBox.Show(string.Format("Device error: status {0}", e.Message), "Device Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}

				}
				break;
				case MessageUpdater.UpdateTypes.JogstepSize:
				{
					try
					{
						decimal jogStep = (decimal)data;
						if (_deviceViewModels.ContainsKey(address))
						{
							_dispatcher.BeginInvoke(() => _deviceViewModels[address].UpdateJogstepSize(jogStep));
						}
					}
					catch (Exception e)
					{
						MessageBox.Show(string.Format("Device error: status {0}", e.Message), "Device Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}

				}
				break;
			}
		}

		/// <summary> Gets or sets the communications log selected. </summary>
		/// <value> The communications log selected. </value>
		public OutputItem CommsLogSelected
		{
			get { return _commsLogSelected; }
			set
			{
				_commsLogSelected = value;
				RaisePropertyChanged(() => CommsLogSelected);
			}
		}

		/// <summary> Gets or sets the communications log. </summary>
		/// <value> The communications log. </value>
		public ObservableCollection<OutputItem> CommsLog
		{
			get { return _commsLog; }
			set
			{
				_commsLog = value; 
				RaisePropertyChanged(() => CommsLog);
			}
		}

		public bool ShowTxLoggedCmds
		{
			get { return _commsLogTypes.Contains(OutputItem.OutputItemType.Tx); }
			set
			{
				bool updated = false;
				if (value && !_commsLogTypes.Contains(OutputItem.OutputItemType.Tx))
				{
					_commsLogTypes.Add(OutputItem.OutputItemType.Tx);
					updated = true;
				}
				else if (!value && _commsLogTypes.Contains(OutputItem.OutputItemType.Tx))
				{
					_commsLogTypes.Remove(OutputItem.OutputItemType.Tx);
					updated = true;
				}
				if (updated)
				{
					new Thread(RebuildOutputList).Start();
				}
				if (CommonUserSettings.ReadUserSetting("ShowTxOutput", true) != value)
				{
					CommonUserSettings.WriteUserSetting("ShowTxOutput", value);
				}
			}
		}

		public bool ShowRxLoggedCmds
		{
			get { return _commsLogTypes.Contains(OutputItem.OutputItemType.Rx); }
			set
			{
				bool updated = false;
				if (value && !_commsLogTypes.Contains(OutputItem.OutputItemType.Rx))
				{
					_commsLogTypes.Add(OutputItem.OutputItemType.Rx);
					updated = true;
				}
				else if (!value && _commsLogTypes.Contains(OutputItem.OutputItemType.Rx))
				{
					_commsLogTypes.Remove(OutputItem.OutputItemType.Rx);
					updated = true;
				}
				if (updated)
				{
					new Thread(RebuildOutputList).Start();
				}
				if (CommonUserSettings.ReadUserSetting("ShowRxOutput", true) != value)
				{
					CommonUserSettings.WriteUserSetting("ShowRxOutput", value);
				}
			}
		}

		/// <summary> Gets or sets the free command text. </summary>
		/// <value> The free command text. </value>
		public string FreeCommandText
		{
			get { return _freeCommandText; }
			set
			{
				_freeCommandText = value;
				RaisePropertyChanged(() => FreeCommandText);
			}
		}

		/// <summary> Gets or sets the free command text. </summary>
		/// <value> The free command text. </value>
		public ObservableCollection<string> FreeCommandStore
		{
			get { return _freeCommandStore; }
			set
			{
				_freeCommandStore = value;
				RaisePropertyChanged(() => FreeCommandStore);
			}
		}

		private void AddFreeCommand()
		{
			if (IsConnected && !string.IsNullOrEmpty(FreeCommandText) && !FreeCommandStore.Contains(FreeCommandText))
			{
				FreeCommandStore.Add(FreeCommandText);
			}
		}
		private void SendFreeCommand()
		{
			if (IsConnected && !string.IsNullOrEmpty(FreeCommandText))
			{
				BackgroundThreadManager.RunBackgroundFunction((s, e) => _ellDevice.SendFreeCommand(FreeCommandText));
			}
		}
	}
}
