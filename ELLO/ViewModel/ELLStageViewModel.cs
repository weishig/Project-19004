using System;
using System.Collections.Generic;
using System.Windows.Input;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO_DLL;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
	/// <summary> Ell rotary stage view model. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.ViewModel.ELLMotorBaseViewModel"/>
	public class ELLStageViewModel : ELLDeviceBaseViewModel
	{
		private ELLBaseDevice.DeviceDirection _homeDirection;
		private Dictionary<ELLBaseDevice.DeviceDirection, string> _homeDirections;
		private bool _useHomeDirection;
		private bool _allowContinuousJog;
		private bool _isContinuousJog;
	    private bool _allowCleaning;

		private decimal _targetAbsoluteMove;
		private decimal _targetRelativeMove;
		private decimal _targetHomeOffset;
		private decimal _targetJogStepSize;

		/// <summary> The get home command. </summary>
		private ICommand _getHomeCommand;
	    /// <summary> The get CleanMachine command. </summary>
	    private ICommand _getCleanMachineCommand;
	    /// <summary> The get CleanAndOptimize command. </summary>
	    private ICommand _getCleanAndOptimizeCommand;
	    /// <summary> The get StopCleaning command. </summary>
	    private ICommand _getStopCleaningCommand;
		/// <summary> The get move absolute command. </summary>
		private ICommand _getMoveAbsoluteCommand;
		/// <summary> The get move relative command. </summary>
		private ICommand _getMoveRelativeCommand;
		/// <summary> The get home offset command. </summary>
		private ICommand _getGetHomeOffsetCommand;
		/// <summary> The get set home offset command. </summary>
		private ICommand _getSetHomeOffsetCommand;
		/// <summary> The get jog size command. </summary>
		private ICommand _getGetJogSizeCommand;
		/// <summary> The get set jog size command. </summary>
		private ICommand _getSetJogSizeCommand;

		private bool _forwardButtonPressed;
		private bool _reverseButtonPressed;
        private bool _isCleaning;

        /// <summary> Gets the click home command. </summary>
        /// <value> The click home command. </value>
        public ICommand ClickHomeCommand { get { return _getHomeCommand ?? (_getHomeCommand = new RelayCommand(Home)); } }
	    /// <summary> Gets the click home command. </summary>
	    /// <value> The click home command. </value>
	    public ICommand ClickCleanMachineCommand { get { return _getCleanMachineCommand ?? (_getCleanMachineCommand = new RelayCommand(SendCleanMachine)); } }
	    /// <summary> Gets the click home command. </summary>
	    /// <value> The click home command. </value>
	    public ICommand ClickCleanAndOptimizeCommand { get { return _getCleanAndOptimizeCommand ?? (_getCleanAndOptimizeCommand = new RelayCommand(SendCleanAndOptimize)); } }

		/// <summary> Gets the click home command. </summary>
		/// <value> The click home command. </value>
		public ICommand ClickStopCleaningCommand { get { return _getStopCleaningCommand ?? (_getStopCleaningCommand = new RelayCommand(SendStopCleaning)); } }

		/// <summary> Gets the click move relative command. </summary>
		/// <value> The click move relative command. </value>
		public ICommand ClickMoveRelativeCommand { get { return _getMoveRelativeCommand ?? (_getMoveRelativeCommand = new RelayCommand(MoveRelative)); } }

		/// <summary> Gets the click move absolute command. </summary>
		/// <value> The click move absolute command. </value>
		public ICommand ClickMoveAbsoluteCommand { get { return _getMoveAbsoluteCommand ?? (_getMoveAbsoluteCommand = new RelayCommand(MoveAbsolute)); } }

		/// <summary> Gets the click get home offset command. </summary>
		/// <value> The click get home offset command. </value>
		public ICommand ClickGetHomeOffsetCommand { get { return _getGetHomeOffsetCommand ?? (_getGetHomeOffsetCommand = new RelayCommand(GetHomeOffset)); } }

		/// <summary> Gets the click set home offset command. </summary>
		/// <value> The click set home offset command. </value>
		public ICommand ClickSetHomeOffsetCommand { get { return _getSetHomeOffsetCommand ?? (_getSetHomeOffsetCommand = new RelayCommand(SetHomeOffset)); } }

		/// <summary> Gets the click get jog size command. </summary>
		/// <value> The click get jog size command. </value>
		public ICommand ClickGetJogSizeCommand { get { return _getGetJogSizeCommand ?? (_getGetJogSizeCommand = new RelayCommand(GetJogSize)); } }

		/// <summary> Gets the click set jog size command. </summary>
		/// <value> The click set jog size command. </value>
		public ICommand ClickSetJogSizeCommand { get { return _getSetJogSizeCommand ?? (_getSetJogSizeCommand = new RelayCommand(SetJogSize)); } }

        public ELLDevice ELLDeviceStage { get; private set; }

        /// <summary> Constructor. </summary>
        /// <param name="owner">         The owner. </param>
        /// <param name="device">        The device. </param>
        /// <param name="motorCount">    Number of motors. </param>
        /// <param name="rotary">        true to rotary. </param>
        /// <param name="continuousJog"> true to continuous jog. </param>
        /// <param name="allowCleaning">      True if this object can clean. </param>
        public ELLStageViewModel(ELLDevicesViewModel owner, ELLDevice device, int motorCount, bool rotary, bool continuousJog, bool allowCleaning)
			: base(owner, rotary ? "Rotary Stage" : "Linear Stage", device, motorCount)
        {
            _allowCleaning = allowCleaning;
            ELLDeviceStage = device;
            UseHomeDirection = rotary;
			if (rotary)
			{
				AllowContinuousJog = continuousJog;
				IsContinuousJog = false;
				HomeDirections = new Dictionary<ELLBaseDevice.DeviceDirection, string>
				{
					{ELLBaseDevice.DeviceDirection.Clockwise, "Clockwise"},
					{ELLBaseDevice.DeviceDirection.AntiClockwise, "Anticlockwise"}
				};
				HomeDirection = ELLBaseDevice.DeviceDirection.AntiClockwise;
				Units = "deg";
			}
			else
			{
				AllowContinuousJog = false;
				IsContinuousJog = false;
				HomeDirections = new Dictionary<ELLBaseDevice.DeviceDirection, string>
				{
					{ELLBaseDevice.DeviceDirection.Linear, "Linear"},
				};
				HomeDirection = ELLBaseDevice.DeviceDirection.Linear;
				Units = Device.DeviceInfo.Units;
			}
			TargetAbsoluteMove = 0;
			TargetRelativeMove = 0.1m;
            ELLDeviceStage.CleaningUpdate += (sender, b) => IsCleaning = b;
        }

	    public bool IsCleaning
	    {
	        get { return _isCleaning; }
	        set
	        {
	            _isCleaning = value;
	            RaisePropertyChanged(() => IsCleaning);
	        }
	    }

	    /// <summary>   Initializes the view model. </summary>
	    public override void InitializeViewModel()
	    {
	        ELLDeviceStage.GetHomeOffset();
	        ELLDeviceStage.GetJogstepSize();
	        ELLDeviceStage.GetPosition();
	    }

        /// <summary>	Gets or sets a value indicating whether this object is continuous jog. </summary>
        /// <value>	true if this object is continuous jog, false if not. </value>
        public bool IsContinuousJog
		{
			get { return _isContinuousJog; }
			set
			{
				_isContinuousJog = value;
				RaisePropertyChanged(() => IsContinuousJog);
			}
		}

		/// <summary>	Gets or sets a value indicating whether we allow continuous jog. </summary>
		/// <value>	true if allow continuous jog, false if not. </value>
		public bool AllowContinuousJog
		{
			get { return _allowContinuousJog; }
			set
			{
				_allowContinuousJog = value;
				RaisePropertyChanged(() => AllowContinuousJog);
			}
		}

        /// <summary> Gets or sets a value indicating whether we allow cleaning. </summary>
        /// <value> True if allow cleaning, false if not. </value>
	    public bool AllowCleaning
	    {
	        get { return _allowCleaning; }
	        set
	        {
	            _allowCleaning = value;
	            RaisePropertyChanged(() => AllowContinuousJog);
	        }
	    }

		/// <summary> Gets or sets the home direction. </summary>
		/// <value> The home direction. </value>
		public bool UseHomeDirection
		{
			get { return _useHomeDirection; }
			set
			{
				_useHomeDirection = value;
				RaisePropertyChanged(() => UseHomeDirection);
			}
		}

		/// <summary> Gets or sets the home direction. </summary>
		/// <value> The home direction. </value>
		public ELLBaseDevice.DeviceDirection HomeDirection
		{
			get { return _homeDirection; }
			set
			{
				_homeDirection = value;
				RaisePropertyChanged(() => HomeDirection);
			}
		}
		
		/// <summary> Gets or sets the home directions. </summary>
		/// <value> The home directions. </value>
		public Dictionary<ELLBaseDevice.DeviceDirection, string> HomeDirections
		{
			get { return _homeDirections; }
			set
			{
				_homeDirections = value;
				RaisePropertyChanged(() => HomeDirections);
			}
		}

        /// <summary>   Gets a value indicating whether we can run sequence. </summary>
        /// <value> True if we can run sequence, false if not. </value>
        /// <seealso cref="P:Thorlabs.Elliptec.ELLO.ViewModel.ELLDeviceBaseViewModel.CanRunSequence"/>
	    public override bool CanRunSequence
	    {
	        get { return !IsContinuousJog; }
	    }

		/// <summary> Gets or sets target absolute move. </summary>
		/// <value> The target absolute move. </value>
		public decimal TargetAbsoluteMove
		{
			get { return _targetAbsoluteMove; }
			set
			{
				_targetAbsoluteMove = value;
				RaisePropertyChanged(() => TargetAbsoluteMove);
			    RaisePropertyChanged(() => TargetAbsoluteMoveStr);
			    if ((value < 0) || (value > ELLDeviceStage.DeviceInfo.Travel))
			    {
			        throw new ArgumentOutOfRangeException($"Range 0 to {ELLDeviceStage.DeviceInfo.Travel}");
			    }
			}
		}

	    /// <summary> Gets or sets target absolute move. </summary>
	    /// <value> The target absolute move. </value>
	    public string TargetAbsoluteMoveStr
	    {
	        get
	        {
	            return Device.DeviceInfo.FormatPosition(_targetAbsoluteMove);
	        }
	        set
	        {
	            decimal d;
	            bool result = decimal.TryParse(value, out d);
               TargetAbsoluteMove = d;
	        }
	    }

		/// <summary> Gets or sets target relative move. </summary>
        /// <value> The target relative move. </value>
        public decimal TargetRelativeMove
		{
			get { return _targetRelativeMove; }
			set
			{
				_targetRelativeMove = value;
				RaisePropertyChanged(() => TargetRelativeMove);
			    RaisePropertyChanged(() => TargetRelativeMoveStr);
			    if ((Math.Abs(value) < 0.001m) || (Math.Abs(value) > ELLDeviceStage.DeviceInfo.Travel))
			    {
			        throw new ArgumentOutOfRangeException($"Range 0.001 to {ELLDeviceStage.DeviceInfo.Travel}");
			    }
			}
        }

	    /// <summary> Gets or sets target absolute move. </summary>
	    /// <value> The target absolute move. </value>
	    public string TargetRelativeMoveStr
        {
	        get
	        {
	            return Device.DeviceInfo.FormatPosition(_targetRelativeMove);
	        }
	        set
	        {
	            decimal d;
	            bool result = decimal.TryParse(value, out d);
	            TargetRelativeMove = d;
	        }
	    }
		
        /// <summary> Gets or sets target home offset. </summary>
        /// <value> The target home offset. </value>
        public decimal TargetHomeOffset
		{
			get { return _targetHomeOffset; }
			set
			{
			    if ((value < 0) || (value > ELLDeviceStage.DeviceInfo.Travel))
			    {
			        throw new ArgumentOutOfRangeException($"Range 0 to {ELLDeviceStage.DeviceInfo.Travel}");
			    }
				_targetHomeOffset = value;
				RaisePropertyChanged(() => TargetHomeOffset);
	            RaisePropertyChanged(() => TargetHomeOffsetStr);
			}
		}

	    /// <summary> Gets or sets target absolute move. </summary>
	    /// <value> The target absolute move. </value>
	    public string TargetHomeOffsetStr
        {
	        get
	        {
	            return Device.DeviceInfo.FormatPosition(_targetHomeOffset);
	        }
	        set
	        {
	            decimal d;
	            bool result = decimal.TryParse(value, out d);
	            TargetHomeOffset = d;
	        }
        }
		
        /// <summary> Gets or sets the size of the target jog step. </summary>
        /// <value> The size of the target jog step. </value>
        public decimal TargetJogStepSize
		{
			get { return _targetJogStepSize; }
			set
			{
			    if (AllowContinuousJog)
			    {
			        if ((value < 0) || (value > ELLDeviceStage.DeviceInfo.Travel))
			        {
			            throw new ArgumentOutOfRangeException($"Range +/- (0.001 to {ELLDeviceStage.DeviceInfo.Travel})");

                    }
			    }
                else
			    {
			        if ((value < 0.001m) || (value > ELLDeviceStage.DeviceInfo.Travel))
			        {
			            throw new ArgumentOutOfRangeException($"Range +/- (0.001 to {ELLDeviceStage.DeviceInfo.Travel})");
			        }
			    }
                _targetJogStepSize = value;
				RaisePropertyChanged(() => TargetJogStepSize);
			    RaisePropertyChanged(() => TargetJogStepSizeStr);
				IsContinuousJog = AllowContinuousJog && (_targetJogStepSize == 0);
			    Owner.UpdateSequence();
			}
        }
	    /// <summary> Gets or sets target absolute move. </summary>
	    /// <value> The target absolute move. </value>
	    public string TargetJogStepSizeStr
        {
	        get
	        {
	            return Device.DeviceInfo.FormatPosition(_targetJogStepSize);
	        }
	        set
	        {
	            decimal d;
	            bool result = decimal.TryParse(value, out d);
	            TargetJogStepSize = d;
	        }
        }

        private void Home()
		{
			if (Owner.IsConnected)
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.Home(HomeDirection));
			}
		}

		private void MoveAbsolute()
		{
			if (Owner.IsConnected)
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.MoveAbsolute(TargetAbsoluteMove));
			}
		}

		private void MoveRelative()
		{
			if (Owner.IsConnected)
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.MoveRelative(TargetRelativeMove));
			}
		}

		private void GetHomeOffset()
		{
			if (Owner.IsConnected)
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.GetHomeOffset());
			}
		}

		private void SetHomeOffset()
		{
			if (Owner.IsConnected)
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.SetHomeOffset(TargetHomeOffset));
			}
		}

		private void GetJogSize()
		{
			if (Owner.IsConnected)
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.GetJogstepSize());
			}
		}

		private void SetJogSize()
		{
			if (Owner.IsConnected)
			{
				Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.SetJogstepSize(TargetJogStepSize));
			}
		}

		/// <summary> Updates the home offset described by homeOffset. </summary>
		/// <param name="homeOffset"> The home offset. </param>
		public override void UpdateHomeOffset(decimal homeOffset)
		{
			TargetHomeOffset = homeOffset;
		}

		/// <summary> Updates the jogstep size described by jogStepSize. </summary>
		/// <param name="jogstepSize"> Size of the jog step. </param>
		public override void UpdateJogstepSize(decimal jogstepSize)
		{
			TargetJogStepSize = jogstepSize;
		}

		/// <summary>	Gets or sets a value indicating whether the forward button pressed. </summary>
		/// <value>	true if forward button pressed, false if not. </value>
		public bool ForwardButtonPressed
		{
			get { return _forwardButtonPressed; }
			set
			{
				if (_forwardButtonPressed != value)
				{
					_forwardButtonPressed = value;
					RaisePropertyChanged(() => ForwardButtonPressed);
					if (_forwardButtonPressed)
					{
						Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.JogForwardStart());
					}
					else
					{
						Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.JogStop());
					}
				}
			}
		}

		/// <summary>	Gets or sets a value indicating whether the reverse button pressed. </summary>
		/// <value>	true if reverse button pressed, false if not. </value>
		public bool ReverseButtonPressed
		{
			get { return _reverseButtonPressed; }
			set
			{
				if (_reverseButtonPressed != value)
				{
					_reverseButtonPressed = value;
					RaisePropertyChanged(() => ReverseButtonPressed);
					if (_reverseButtonPressed)
					{
						Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.JogBackwardStart());
					}
					else
					{
						Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.JogStop());
					}
				}
			}
		}
	    /// <summary> Gets the device status. </summary>
	    private void SendCleanMachine()
	    {
	        if (Owner.IsConnected)
	        {
	            Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.SendCleanMachine());
	        }
	    }
	    /// <summary> Gets the device status. </summary>
	    private void SendCleanAndOptimize()
	    {
	        if (Owner.IsConnected)
	        {
	            Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceStage.SendCleanAndOptimize());
	        }
	    }
	    /// <summary> Gets the device status. </summary>
	    private void SendStopCleaning()
	    {
	        if (Owner.IsConnected)
	        {
	            ELLDeviceStage.SendStopCleaning();
	        }
	    }
	}
}
