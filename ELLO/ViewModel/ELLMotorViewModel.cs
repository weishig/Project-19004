using System.Windows;
using System.Windows.Input;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO_DLL;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
	/// <summary> Elliptec motor view model. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.Support.ObservableObject"/>
	public class ELLMotorViewModel : ObservableObject
	{
		private readonly ELLBaseDevice _ellDevice;
		private readonly char _motorID;
		private readonly ELLDevicesViewModel _owner;
		private string _motorName;

		private decimal _fwdPeriod;
		private decimal _revPeriod;

		/// <summary> The get set forward period command. </summary>
		private ICommand _getSetFwdPeriodCommand;
		/// <summary> The get set reverse period command. </summary>
		private ICommand _getSetRevPeriodCommand;
		/// <summary> The get motor information command. </summary>
		private ICommand _getMotorInfoCommand;
		/// <summary> The get search period command. </summary>
		private ICommand _getSearchPeriodCommand;
		/// <summary> The get scan current command. </summary>
        private ICommand _getScanCurrentCommand;
        /// <summary>   The get fix frequency command. </summary>
	    private ICommand _getFixFrequencyCommand;

	    /// <summary> Gets the click status command. </summary>
		/// <value> The click status command. </value>
		public ICommand ClickGetMotorInfoCommand { get { return _getMotorInfoCommand ?? (_getMotorInfoCommand = new RelayCommand(GetMotorInfo)); } }
		/// <summary> Gets the click set forward period command. </summary>
		/// <value> The click set forward period command. </value>
		public ICommand ClickSetFwdPeriodCommand { get { return _getSetFwdPeriodCommand ?? (_getSetFwdPeriodCommand = new RelayCommand(SetFwdPeriod)); } }
		/// <summary> Gets the click set reverse period command. </summary>
		/// <value> The click set reverse period command. </value>
		public ICommand ClickSetRevPeriodCommand { get { return _getSetRevPeriodCommand ?? (_getSetRevPeriodCommand = new RelayCommand(SetRevPeriod)); } }

        /// <summary>   Gets the click fix frequency command. </summary>
        /// <value> The click fix frequency command. </value>
		public ICommand ClickFixFrequencyCommand { get { return _getFixFrequencyCommand ?? (_getFixFrequencyCommand = new RelayCommand(FixFrequency)); } }

	    /// <summary> Gets the click search period command. </summary>
	    /// <value> The click search period command. </value>
	    public ICommand ClickSearchPeriodCommand { get { return _getSearchPeriodCommand ?? (_getSearchPeriodCommand = new RelayCommand(SearchPeriod)); } }

        /// <summary> Gets the click scan current command. </summary>
        /// <value> The click scan current command. </value>
        public ICommand ClickScanCurrentCommand { get { return _getScanCurrentCommand ?? (_getScanCurrentCommand = new RelayCommand(ScanCurrent)); } }

		/// <summary> Constructor. </summary>
		/// <param name="owner">   The owner. </param>
		/// <param name="device">  The device. </param>
		/// <param name="motorID"> Identifier for the motor. </param>
		public ELLMotorViewModel(ELLDevicesViewModel owner, ELLBaseDevice device, char motorID)
		{
			_owner = owner;
			_ellDevice = device;
			_motorID = motorID;
			MotorName = "Motor " + motorID;
		}

		/// <summary> Gets the identifier of the motor. </summary>
		/// <value> The identifier of the motor. </value>
		public char MotorID
		{ get { return _motorID; }}

		/// <summary> Gets the owner. </summary>
		/// <value> The owner. </value>
		protected ELLDevicesViewModel Owner
		{ get { return _owner; }}

		/// <summary> Gets the device. </summary>
		/// <value> The device. </value>
		protected ELLBaseDevice Device
		{ get { return _ellDevice; }}

		public void GetMotorInfoDirect()
		{
			_ellDevice.GetMotorInfo(_motorID);
		}

		/// <summary> Gets the motor information. </summary>
		public void GetMotorInfo()
		{
			if (_owner.IsConnected)
			{
				_owner.BackgroundThreadManager.RunBackgroundFunction((s, e) =>
				{
					_ellDevice.GetMotorInfo(_motorID);
				});
			}
		}

		private bool IsValidPeriod(decimal period)
		{
			return (period >= 65 && period <= 120);
		}

		/// <summary> Gets or sets the name of the motor. </summary>
		/// <value> The name of the motor. </value>
		public string MotorName
		{
			get { return _motorName; }
			set
			{
				_motorName = value;
				RaisePropertyChanged(() => MotorName);
			}
		}

		/// <summary> Gets or sets the forward period. </summary>
		/// <value> The forward period. </value>
		public decimal FwdPeriod
		{
			get { return _fwdPeriod; }
			set
			{
				if (IsValidPeriod(value))
				{
					_fwdPeriod = value;
				}
				RaisePropertyChanged(() => FwdPeriod);
			}
		}

		/// <summary> Gets or sets the forward period. </summary>
		/// <value> The forward period. </value>
		public decimal RevPeriod
		{
			get { return _revPeriod; }
			set
			{
				if (IsValidPeriod(value))
				{
					_revPeriod = value;
				}
				RaisePropertyChanged(() => RevPeriod);
			}
		}

		/// <summary> Updates the information described by info. </summary>
		/// <param name="info"> The information. </param>
		public void UpdateInfo(MotorInfo info)
		{
			FwdPeriod = info.FwdFreq;
			RevPeriod = info.RevFreq;
		}

		private void SetFwdPeriod()
		{
			if (_owner.IsConnected && IsValidPeriod(FwdPeriod))
			{
				_owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _ellDevice.SetPeriod(_motorID, true, FwdPeriod, _ellDevice.AutoSave, false));
				FwdPeriod = _ellDevice.NormalizePeriod(FwdPeriod);
			}
		}

		private void SetRevPeriod()
		{
			if (_owner.IsConnected && IsValidPeriod(RevPeriod))
			{
				_owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _ellDevice.SetPeriod(_motorID, false, RevPeriod, _ellDevice.AutoSave, false));
				RevPeriod = _ellDevice.NormalizePeriod(RevPeriod);
			}
        }

	    private void FixFrequency()
	    {
	        if (_owner.IsConnected && IsValidPeriod(RevPeriod))
	        {
	            if (MessageBox.Show("Fixing the frequency search can cause malfunction!", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
	            {
	                return;
	            }

                _owner.BackgroundThreadManager.RunBackgroundFunction((s, e) =>
                {
                    _ellDevice.SetPeriod(_motorID, true, FwdPeriod, false, true);
                    _ellDevice.SetPeriod(_motorID, false, RevPeriod, true, true);
                });
	            RevPeriod = _ellDevice.NormalizePeriod(RevPeriod);
	            FwdPeriod = _ellDevice.NormalizePeriod(FwdPeriod);
	        }
        }

        private void SearchPeriod()
		{
			if (_owner.IsConnected)
			{
				_owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _ellDevice.SearchPeriod(_motorID, _ellDevice.AutoSave));
			}
		}

        private void ScanCurrent()
		{
			if (_owner.IsConnected)
			{
				_owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _ellDevice.ScanCurrentCurve(_motorID));
			}
		}
	}
}
