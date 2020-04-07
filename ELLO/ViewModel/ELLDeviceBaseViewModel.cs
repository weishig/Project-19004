using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO_DLL;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
    /// <summary> Elliptec device base view model. </summary>
    /// <seealso cref="T:Thorlabs.Elliptec.ELLO.Support.ObservableObject"/>
    public abstract class ELLDeviceBaseViewModel : ObservableObject
    {
        private List<ELLMotorViewModel> _motors = new List<ELLMotorViewModel>();

        private List<string> _description;
        private string _deviceName;
        private string _fullDeviceName;
        private string _units;

        private decimal _currentPosition;

        private readonly ELLBaseDevice _device;

        /// <summary> The get position command. </summary>
        private ICommand _getGetPositionCommand;

        /// <summary> The get jog forward command. </summary>
        private ICommand _getJogForwardCommand;

        /// <summary> The get jog reverse command. </summary>
        private ICommand _getJogReverseCommand;

        /// <summary> The get save user data command. </summary>
        private ICommand _getSaveUserDataCommand;

        /// <summary> The get search period command. </summary>
        private ICommand _getResetPeriodCommand;

        /// <summary> Gets the click save user data command. </summary>
        /// <value> The click save user data command. </value>
        public ICommand ClickSaveUserDataCommand
        {
            get { return _getSaveUserDataCommand ?? (_getSaveUserDataCommand = new RelayCommand(SaveUserData)); }
        }

        /// <summary> Gets the click get position command. </summary>
        /// <value> The click get position command. </value>
        public ICommand ClickGetPositionCommand
        {
            get { return _getGetPositionCommand ?? (_getGetPositionCommand = new RelayCommand(GetPosition)); }
        }

        /// <summary> Gets the click jog forward command. </summary>
        /// <value> The click jog forward command. </value>
        public ICommand ClickJogForwardCommand
        {
            get { return _getJogForwardCommand ?? (_getJogForwardCommand = new RelayCommand(JogForward)); }
        }

        /// <summary> Gets the click jog reverse command. </summary>
        /// <value> The click jog reverse command. </value>
        public ICommand ClickJogReverseCommand
        {
            get { return _getJogReverseCommand ?? (_getJogReverseCommand = new RelayCommand(JogReverse)); }
        }
        /// <summary>   Gets the click reset period command. </summary>
        /// <value> The click reset period command. </value>
        public ICommand ClickResetPeriodCommand { get { return _getResetPeriodCommand ?? (_getResetPeriodCommand = new RelayCommand(ResetPeriod)); } }

        protected ELLDeviceBaseViewModel(ELLDevicesViewModel owner, string deviceName, ELLBaseDevice device, int motorCount)
        {
            Owner = owner;
            _device = device;
            _deviceName = deviceName;
            _fullDeviceName = string.Format("{0}:{1}", deviceName, _device.Address);
            for (int i = 0; i < motorCount; i++)
            {
                Motors.Add(new ELLMotorViewModel(owner, device, (char) ('1' + i)));
            }
            _units = _device.DeviceInfo.Units;
            Description = _device.DeviceInfo.Description();
        }

        /// <summary>   Initializes the view model. </summary>
        public virtual void InitializeViewModel()
        { }

        /// <summary> Gets the owner. </summary>
        /// <value> The owner. </value>
        public ELLDevicesViewModel Owner { get; }

        /// <summary>   Gets a value indicating whether we can run sequence. </summary>
        /// <value> True if we can run sequence, false if not. </value>
        public virtual bool CanRunSequence { get { return true; } }

        /// <summary> Gets the device. </summary>
		/// <value> The device. </value>
		public ELLBaseDevice Device
		{ get { return _device; } }

        /// <summary>   Gets the format string. </summary>
        /// <value> The format string. </value>
        public string FormatStr
        {  get { return _device.DeviceInfo.FormatStr; } }

		/// <summary> Gets or sets the motors. </summary>
		/// <value> The motors. </value>
		public List<ELLMotorViewModel> Motors
		{
			get { return _motors; }
			set
			{
				_motors = value;
				RaisePropertyChanged(() => Motors);
			}
		}

		/// <summary> Gets or sets the name of the motor. </summary>
		/// <value> The name of the motor. </value>
		public string DeviceName
		{
			get { return _deviceName; }
			set
			{
				_deviceName = value;
				RaisePropertyChanged(() => DeviceName);
			}
		}

		/// <summary> Gets or sets the name of the motor. </summary>
		/// <value> The name of the motor. </value>
		public string FullDeviceName
		{
			get { return _fullDeviceName; }
			set
			{
				_fullDeviceName = value;
				RaisePropertyChanged(() => FullDeviceName);
			}
		}

		/// <summary> Gets or sets the name of the motor. </summary>
		/// <value> The name of the motor. </value>
		public string Units
		{
			get { return _units; }
			set
			{
				_units = value;
				RaisePropertyChanged(() => Units);
			}
		}

		/// <summary> Gets or sets the description. </summary>
		/// <value> The description. </value>
		public List<string> Description
		{
			get { return _description; }
			set
			{
				_description = value;
				RaisePropertyChanged(() => Description);
			}
		}

		/// <summary> Gets or sets the current position. </summary>
		/// <value> The current position. </value>
		public decimal CurrentPosition
		{
			get { return _currentPosition; }
			set
			{
				_currentPosition = value;
				RaisePropertyChanged(() => CurrentPosition);
				RaisePropertyChanged(() => CurrentPositionStr);
			}
		}

		/// <summary> Gets the current position string. </summary>
		/// <value> The current position string. </value>
		public string CurrentPositionStr
		{
			get
			{
				return _device.DeviceInfo.FormatPosition(_currentPosition);
			}
		}

		private void GetPosition()
		{
            ELLDevice ellDevice = Device as ELLDevice;
			if (Owner.IsConnected && (ellDevice != null))
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ellDevice.GetPosition());
			}
		}

		private void JogForward()
		{
		    ELLDevice ellDevice = Device as ELLDevice;
			if (Owner.IsConnected && (ellDevice != null))
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ellDevice.JogForward());
			}
		}

		private void JogReverse()
		{
		    ELLDevice ellDevice = Device as ELLDevice;
			if (Owner.IsConnected && (ellDevice != null))
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ellDevice.JogBackward());
			}
		}

		/// <summary> Updates the position described by position. </summary>
		/// <param name="position"> The position. </param>
		public void UpdatePosition(decimal position)
		{
			CurrentPosition = position;
		}

		/// <summary> Gets or sets a value indicating whether the automatic save. </summary>
		/// <value> true if automatic save, false if not. </value>
		public bool AutoSave
		{
			get { return _device.AutoSave; }
			set
			{
				_device.AutoSave = value;
				RaisePropertyChanged(() => AutoSave);
			}
		}

		/// <summary> Gets the device status. </summary>
		private void SaveUserData()
		{
			if (Owner.IsConnected)
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _device.SaveUserData());
			}
		}

        private void ResetPeriod()
        {
            if (Owner.IsConnected)
            {
                 if (MessageBox.Show(@"This option will reset the Motor Frequencies." + Environment.NewLine + @"Power cycle the device or re-search the frequencies on completion.", @"Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                 {
                    Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) =>
                    {
                        _device.ResetPeriod();
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            MessageBox.Show(@"Now power cycle the device or re-search the frequencies.", @"Confirmation", MessageBoxButton.OK);
                        });

                    });
                }
            }
        }
		
        /// <summary> Updates the home offset described by homeOffset. </summary>
        /// <param name="homeOffset"> The home offset. </param>
        public virtual void UpdateHomeOffset(decimal homeOffset) { }

		/// <summary> Updates the jogstep size described by jogStepSize. </summary>
		/// <param name="jogStepSize"> Size of the jog step. </param>
		public virtual void UpdateJogstepSize(decimal jogStepSize) { }

		/// <summary> Sets the address. </summary>
		/// <returns> Success. </returns>
		public void UpdateAddress()
		{
			FullDeviceName = string.Format("{0}:{1}", _deviceName, _device.Address);
		}
	}
}
