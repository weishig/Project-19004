using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.Win32;
using Thorlabs.Elliptec.ELLO.Model;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO_DLL;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
	/// <summary> Elliptec sequence view model. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.Support.ObservableObject"/>
	public class ELLSequenceViewModel : ObservableObject, ISequenceRoot
	{
		private ELLSequenceXML _sequence;

		private ObservableCollection<ELLSequenceItemViewModel> _items;
		private ELLSequenceItemViewModel _selectedItem;
		private readonly ELLDevices _ellDevices;
		private readonly ELLDevicesViewModel _owner;
		private bool _runningSequence;
		private char _runningAddress;
		private int _runningIndex;
		private string _runningCmd;
		private string _runningTime;
		private int _runningCycleCount;
		private int _cycleCount;
		private readonly object _lockableObject = new object();
		private ICommand _getLoadCommand;
		private ICommand _getSaveCommand;
		private ICommand _getAddCommand;
		private ICommand _getInsertCommand;
		private ICommand _getRemoveCommand;
		private ICommand _getClearCommand;
		private ICommand _getRunCommand;
		private ICommand _getStopCommand;

		/// <summary> Gets the click load command. </summary>
		/// <value> The click load command. </value>
		public ICommand ClickLoadCommand { get { return _getLoadCommand ?? (_getLoadCommand = new RelayCommand(Load)); } }

		/// <summary> Gets the click save command. </summary>
		/// <value> The click save command. </value>
		public ICommand ClickSaveCommand { get { return _getSaveCommand ?? (_getSaveCommand = new RelayCommand(Save)); } }

		/// <summary> Gets the click add command. </summary>
		/// <value> The click add command. </value>
		public ICommand ClickAddCommand { get { return _getAddCommand ?? (_getAddCommand = new RelayCommand(AddItem)); } }

		/// <summary> Gets the click insert command. </summary>
		/// <value> The click insert command. </value>
		public ICommand ClickInsertCommand { get { return _getInsertCommand ?? (_getInsertCommand = new RelayCommand(InsertItem)); } }

		/// <summary> Gets the click remove command. </summary>
		/// <value> The click remove command. </value>
		public ICommand ClickRemoveCommand { get { return _getRemoveCommand ?? (_getRemoveCommand = new RelayCommand(Remove)); } }

		/// <summary> Gets the click clear command. </summary>
		/// <value> The click clear command. </value>
		public ICommand ClickClearCommand { get { return _getClearCommand ?? (_getClearCommand = new RelayCommand(Clear)); } }

		/// <summary> Gets the click run command. </summary>
		/// <value> The click run command. </value>
		public ICommand ClickRunCommand { get { return _getRunCommand ?? (_getRunCommand = new RelayCommand(Run)); } }

		/// <summary> Gets the click stop command. </summary>
		/// <value> The click stop command. </value>
		public ICommand ClickStopCommand { get { return _getStopCommand ?? (_getStopCommand = new RelayCommand(Stop)); } }

		/// <summary> The dispatcher. </summary>
		private readonly ViewModelDispatcher _dispatcher = new ViewModelDispatcher();

		private int _repeatCount;
		private bool _repeatContinuously;
		private bool _repeatRun;
	    private bool _canRun;

	    /// <summary> Constructor. </summary>
		/// <param name="owner">	 The owner. </param>
		/// <param name="ellDevices"> The Elliptec device. </param>
		public ELLSequenceViewModel(ELLDevicesViewModel owner, ELLDevices ellDevices)
		{
			_owner = owner;
			_ellDevices = ellDevices;
			_sequence = new ELLSequenceXML();
			Items = new ObservableCollection<ELLSequenceItemViewModel>();
		}

		/// <summary> Gets or sets the type of the device. </summary>
		/// <value> The type of the device. </value>
		public Dictionary<char,DeviceID.DeviceTypes> DeviceTypes { get; set; }

		/// <summary> Readdress sequence. </summary>
		/// <param name="oldAddress"> The old address. </param>
		/// <param name="newAddress"> The new address. </param>
		public void ReaddressSequence(char oldAddress, char newAddress)
		{
			if (DeviceTypes.ContainsKey(oldAddress))
			{
				DeviceTypes[newAddress] = DeviceTypes[oldAddress];
				DeviceTypes.Remove(oldAddress);
			}
			foreach (ELLSequenceItemViewModel ellSequenceItemViewModel in Items)
			{
				ellSequenceItemViewModel.ReaddressSequenceItem(oldAddress, newAddress);
			}
		}

		/// <summary> Gets or sets the items. </summary>
		/// <value> The items. </value>
		public ObservableCollection<ELLSequenceItemViewModel> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				RaisePropertyChanged(() => Items);
			}
		}

		/// <summary> Gets or sets the items. </summary>
		/// <value> The items. </value>
		public ELLSequenceItemViewModel SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				RaisePropertyChanged(() => SelectedItem);
			}
		}

		/// <summary> Executes the update action. </summary>
		public void OnUpdate()
		{
			RaisePropertyChanged(() => ShowParam);
		}

		private void BuildList(ELLSequenceItemXML selected)
		{
			if (!_sequence.Items.Any())
			{
				Items = new ObservableCollection<ELLSequenceItemViewModel>();
				SelectedItem = null;
				RaisePropertyChanged(() => ShowParam);
				return;
			}
			int index = 1;
			if (selected == null)
			{
				selected = SelectedItem?.Item;
			}
			Items = new ObservableCollection<ELLSequenceItemViewModel>(_sequence.Items.Select(item => new ELLSequenceItemViewModel(this, item, index++)));
			SelectedItem = (selected == null) ? Items.FirstOrDefault() : Items.FirstOrDefault(item => item.Item == selected);
			RaisePropertyChanged(() => ShowParam);
		}

		/// <summary> Gets a value indicating whether the parameter 1 is shown. </summary>
		/// <value> true if show parameter 1, false if not. </value>
		public bool ShowParam
		{
			get { return _items.Any(item => (item.ShowParam1 || item.ShowParam2)); }
		}

		/// <summary> Saves this object. </summary>
		private void Save()
		{
			string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), App.Company, App.ApplicationName);
			if (!Directory.Exists(defaultPath))
			{
				Directory.CreateDirectory(defaultPath);
			}

			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				CheckPathExists = true,
				CheckFileExists = false,
				FileName = "ELLSequence.ell",
				DefaultExt = ".ell",
				Filter = "Elliptec Sequence (.ell)|*.ell",
				InitialDirectory = defaultPath
			};

			bool? result = saveFileDialog.ShowDialog();
			if (!result.GetValueOrDefault(false))
			{
				return;
			}
			string path = saveFileDialog.FileName;

			ELLSequenceXML.Save(_sequence, path);
		}

		private void Load()
		{
			_sequence = new ELLSequenceXML();
			string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), App.Company, App.ApplicationName);
			if (!Directory.Exists(defaultPath))
			{
				Directory.CreateDirectory(defaultPath);
			}
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				CheckFileExists = true,
				FileName = String.IsNullOrEmpty(defaultPath) ? "EllSequence.ell" : defaultPath,
				DefaultExt = ".ell",
				Filter = "Elliptec Sequence (.ell)|*.ell",
				InitialDirectory = defaultPath
			};

			bool? result = openFileDialog.ShowDialog();
			if (!result.GetValueOrDefault(false))
			{
				return;
			}

			_sequence = ELLSequenceXML.Load(openFileDialog.FileName);
			BuildList(null);
			RaisePropertyChanged(() => RepeatRun);
			RaisePropertyChanged(() => RepeatContinuously);
			RaisePropertyChanged(() => RepeatCount);
		}

		private void InsertItem()
		{
			int index = -1;
			if (SelectedItem != null)
			{
				index = _sequence.Items.IndexOf(SelectedItem.Item);
			}
			if (index >= 0 && index < _sequence.Items.Count)
			{
				ELLSequenceItemXML newItem = new ELLSequenceItemXML();
				_sequence.Items.Insert(index, newItem);
				BuildList(newItem);
			}
			else
			{
				AddItem();
			}
		}

		private void AddItem()
		{
			ELLSequenceItemXML newItem = new ELLSequenceItemXML();
			_sequence.Items.Add(newItem);
			BuildList(newItem);
		}

		private void Remove()
		{
			if (SelectedItem != null)
			{
				int index = _sequence.Items.IndexOf(SelectedItem.Item);
				if (index >= 0 && index < _sequence.Items.Count)
				{
					_sequence.Items.RemoveAt(index);
					int lastIndex = _sequence.Items.Count - 1;
					BuildList(lastIndex >= 0 ? _sequence.Items[index <= lastIndex ? index : lastIndex] : null);
				}
			}
		}

		private void Clear()
		{
			_sequence.Items.Clear();
			BuildList(null);
		}

		/// <summary> Gets or sets a value indicating whether the running sequence. </summary>
		/// <value> true if running sequence, false if not. </value>
		public bool RunningSequence
		{
			get
			{
				lock (_lockableObject)
				{
					return _runningSequence;
				}
			}
			set
			{
				lock (_lockableObject)
				{
					_runningSequence = value;
				}
				RaisePropertyChanged(() => RunningSequence);
			}
		}

		/// <summary> Gets or sets the number of repeats. </summary>
		/// <value> The number of repeats. </value>
		public int RepeatCount
		{
			get { return _sequence.RepeatCount; }
			set
			{
				_sequence.RepeatCount = value;
				RaisePropertyChanged(() => RepeatCount);
				if (RunningSequence)
				{
					SetCycleParams(_sequence.RepeatRun, _sequence.RepeatContinuously, _sequence.RepeatCount);
				}
			}
		}

		/// <summary> Gets or sets a value indicating whether the run continuously. </summary>
		/// <value> <c>true</c> if run continuously. </value>
		public bool RepeatContinuously
		{
			get { return _sequence.RepeatContinuously; }
			set
			{
				_sequence.RepeatContinuously = value;
				RaisePropertyChanged(() => RepeatContinuously);
				if (RunningSequence)
				{
					SetCycleParams(_sequence.RepeatRun, _sequence.RepeatContinuously, _sequence.RepeatCount);
				}
			}
		}

		/// <summary> Gets or sets a value indicating whether the repeat run. </summary>
		/// <value> <c>true</c> if repeat run. </value>
		public bool RepeatRun
		{
			get { return _sequence.RepeatRun; }
			set
			{
				_sequence.RepeatRun = value;
				RaisePropertyChanged(() => RepeatRun);
				if (RunningSequence)
				{
					SetCycleParams(_sequence.RepeatRun, _sequence.RepeatContinuously, _sequence.RepeatCount);
				}
			}
		}

		/// <summary> Gets or sets zero-based index of the running. </summary>
		/// <value> The running index. </value>
		public int RunningIndex
		{
			get { return _runningIndex; }
			set
			{
				_runningIndex = value;
				RaisePropertyChanged(() => RunningIndex);
			}
		}

		/// <summary> Gets or sets the running address. </summary>
		/// <value> The running address. </value>
		public char RunningAddress
		{
			get { return _runningAddress; }
			set
			{
				_runningAddress = value;
				RaisePropertyChanged(() => RunningAddress);
			}
		}

		private void SetCycleParams(bool repeatRun, bool repeatContinuously, int repeatCount)
		{
			lock (_lockableObject)
			{
				_repeatContinuously = repeatContinuously;
				_repeatRun = repeatRun;
				_repeatCount = repeatCount;
			}
		}

		/// <summary> Gets or sets the running command. </summary>
		/// <value> The running command. </value>
		public string RunningCmd
		{
			get { return _runningCmd; }
			set
			{
				_runningCmd = value;
				RaisePropertyChanged(() => RunningCmd);
			}
		}

		/// <summary> Gets or sets the time of the running. </summary>
		/// <value> The time of the running. </value>
		public string RunningTime
		{
			get { return _runningTime; }
			set
			{
				_runningTime = value;
				RaisePropertyChanged(() => RunningTime);
			}
		}

		/// <summary> Gets or sets the time of the running. </summary>
		/// <value> The time of the running. </value>
		public int CycleCount
		{
			get { return _cycleCount; }
			set
			{
				_cycleCount = value;
				RaisePropertyChanged(() => CycleCount);
			}
		}

		private void Stop()
		{
			if (RunningSequence)
			{
				RunningSequence = false;
			}
		}

		private void Run()
		{
			if (CanRun && !RunningSequence)
			{
				SetCycleParams(RepeatRun, RepeatContinuously, RepeatCount);
				ELLDeviceSequence sequence = new ELLDeviceSequence();
				sequence.Sequence = _sequence.Items.Select(item => sequence.CreateCommand(_ellDevices, _ellDevices.ValidAddresses(item.Addresses), item.Command, item.WaitTime, item.Parameter1, item.Parameter2)).ToList();
				_owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => RunSequenceEx(sequence));
			}
		}

        /// <summary>   Gets or sets a value indicating whether we can run. </summary>
        /// <value> True if we can run, false if not. </value>
	    public bool CanRun
	    {
	        get { return _canRun; }
	        set
	        {
	            _canRun = value;
	            RaisePropertyChanged(() => CanRun);
	        }
	    }

	    private void UpdateRunningState(int? index, char address, int cycleCount, string cmd, string time)
		{
			_dispatcher.Execute(() =>
			{
				if (!RunningSequence)
				{
					RunningCmd = "";
					RunningAddress = '\0';
					RunningTime = "";
					RunningIndex = 0;
					cycleCount = 0;
				}
				else
				{
					if (index.HasValue)
					{
						RunningIndex = index.Value;
					}
					RunningAddress = address;
					if (!string.IsNullOrEmpty(cmd))
					{
						RunningCmd = cmd;
					}
					if (!string.IsNullOrEmpty(time))
					{
						RunningTime = time;
					}
					CycleCount = cycleCount;
				}
			});	
		}

		private bool KeepRunning()
		{
			lock (_lockableObject)
			{
				return (_runningCycleCount == 1) || (_repeatRun && (_repeatContinuously || (_runningCycleCount <= _repeatCount)));
			}
		}

		private void RunSequenceEx(ELLDeviceSequence sequence)
		{
			_dispatcher.Execute(() => { RunningSequence = true; });
			_runningCycleCount = 1;
			while (RunningSequence && KeepRunning())
			{
				int index = 0;
				while (RunningSequence && (index < sequence.Sequence.Count))
				{
					UpdateRunningState(index + 1, sequence.Sequence[index].Address, _runningCycleCount, sequence.Sequence[index].Description, "Active");
					sequence.Sequence[index].ELLFunction.Invoke();
					WaitForTimeElapsed(sequence.Sequence[index].WaitTime);
					index++;
				}
				_runningCycleCount++;
			}
			_dispatcher.Execute(() => { RunningSequence = false; });
			UpdateRunningState(null, '\0', _runningCycleCount, null, null);
		}

		// this is a crude wait time loop, would be better to use a Timer object to poll and a wait event
		private void WaitForTimeElapsed(TimeSpan ts)
		{
			TimeSpan timeRemaining = ts;
			DateTime endTime = DateTime.Now + timeRemaining;
			while (timeRemaining.TotalMilliseconds > 0)
			{
				UpdateRunningState(null, '\0', _runningCycleCount, null, timeRemaining.ToString(@"'Waiting - 'mm\:ss"));
				Thread.Sleep(timeRemaining.TotalMilliseconds > 1000 ? 500 : 50);
				timeRemaining = endTime - DateTime.Now;
			}
		}
	}
}
