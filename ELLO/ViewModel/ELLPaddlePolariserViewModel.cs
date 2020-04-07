using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO_DLL;

namespace Thorlabs.Elliptec.ELLO.ViewModel
{
    public class ELLPaddlePolariserViewModel : ELLDeviceBaseViewModel
    {
        private decimal _targetHomeOffset;
        private decimal _targetJogStepSize;

        private ICommand _getHomeCommand;
        private ICommand _getGetPositionsCommand;
        private ICommand _getGetHomeOffsetCommand;
        private ICommand _getSetHomeOffsetCommand;
        private ICommand _getGetJogSizeCommand;
        private ICommand _getSetJogSizeCommand;
        private ICommand _getMoveAbsoluteCommand;
        private ICommand _getMoveRelativeCommand;

        private readonly Dictionary<ELLPaddlePolariser.PaddleIDs, ELLPaddleViewModel> _paddlesTable;
        private List<ELLPaddleViewModel> _paddlesList;

        /// <summary> Gets the click home command. </summary>
        /// <value> The click home command. </value>
        public ICommand ClickHomeCommand { get { return _getHomeCommand ?? (_getHomeCommand = new RelayCommand(Home)); } }

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
        /// <summary> Gets the click home command. </summary>
        /// <value> The click home command. </value>
        public ICommand ClickGetPositionsCommand { get { return _getGetPositionsCommand ?? (_getGetPositionsCommand = new RelayCommand(GetPositions)); } }

        /// <summary>   Gets the click move absolute command. </summary>
        /// <value> The click move absolute command. </value>
        public ICommand ClickMoveAbsoluteCommand { get { return _getMoveAbsoluteCommand ?? (_getMoveAbsoluteCommand = new RelayCommand(MoveAbsolute)); } }

        /// <summary>   Gets the click move relative command. </summary>
        /// <value> The click move relative command. </value>
        public ICommand ClickMoveRelativeCommand { get { return _getMoveRelativeCommand ?? (_getMoveRelativeCommand = new RelayCommand(MoveRelative)); } }

        public ELLPaddlePolariser ELLPaddleStage { get; private set; }

        /// <summary>	Constructor. </summary>
        /// <param name="owner">			The owner. </param>
        /// <param name="device">			The device. </param>
        public ELLPaddlePolariserViewModel(ELLDevicesViewModel owner, ELLPaddlePolariser device)
            : base(owner, "Paddle", device, 3)
        {
            ELLPaddleStage = device;
            Units = "deg";
            _paddlesTable = new Dictionary<ELLPaddlePolariser.PaddleIDs, ELLPaddleViewModel>()
            {
                {ELLPaddlePolariser.PaddleIDs.Paddle1, new ELLPaddleViewModel(owner, device, ELLPaddlePolariser.PaddleIDs.Paddle1)},
                {ELLPaddlePolariser.PaddleIDs.Paddle2, new ELLPaddleViewModel(owner, device, ELLPaddlePolariser.PaddleIDs.Paddle2)},
                {ELLPaddlePolariser.PaddleIDs.Paddle3, new ELLPaddleViewModel(owner, device, ELLPaddlePolariser.PaddleIDs.Paddle3)},
            };
            Paddles = _paddlesTable.Values.ToList();
            TargetHomeOffset = 90;
            TargetJogStepSize = 30;
        }
        /// <summary>   Gets a value indicating whether we can run sequence. </summary>
        /// <value> True if we can run sequence, false if not. </value>
        public override bool CanRunSequence { get { return false; } }

        public override void InitializeViewModel()
        {
            ELLPaddleStage.GetHomeOffset();
            ELLPaddleStage.GetJogstepSize();
            ELLPaddleStage.RequestPositions();
        }

        private void GetHomeOffset()
        {
            if (Owner.IsConnected)
            {
                Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLPaddleStage.GetHomeOffset());
            }
        }

        private void SetHomeOffset()
        {
            if (Owner.IsConnected)
            {
                Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLPaddleStage.SetHomeOffset(TargetHomeOffset));
            }
        }

        /// <summary> Gets or sets target home offset. </summary>
        /// <value> The target home offset. </value>
        public decimal TargetHomeOffset
        {
            get { return _targetHomeOffset; }
            set
            {
                _targetHomeOffset = value;
                RaisePropertyChanged(() => TargetHomeOffset);
                RaisePropertyChanged(() => TargetHomeOffsetStr);
                if (value < ELLPaddleStage.MinPos || value > ELLPaddleStage.MaxPos)
                {
                    throw new ArgumentOutOfRangeException($"Range {ELLPaddleStage.MinPosRounded} to {ELLPaddleStage.MaxPosRounded}");
                }
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
                _targetJogStepSize = value;
                RaisePropertyChanged(() => TargetJogStepSize);
                RaisePropertyChanged(() => TargetJogStepSizeStr);
                if ((value < 0.001m) || (value > ELLPaddleStage.DeviceInfo.Travel))
                {
                    throw new ArgumentOutOfRangeException($"Range 0.001 to {ELLPaddleStage.DeviceInfo.Travel}");
                }
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
                Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLPaddleStage.Home(ELLPaddlePolariser.PaddleHomeMask.All));
            }
        }

        private void GetJogSize()
        {
            if (Owner.IsConnected)
            {
                Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLPaddleStage.GetJogstepSize());
            }
        }

        private void SetJogSize()
        {
            if (Owner.IsConnected)
            {
                Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLPaddleStage.SetJogstepSize(TargetJogStepSize));
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

        private void GetPositions()
        {
            if (Owner.IsConnected)
            {
                Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLPaddleStage.RequestPositions());
            }
        }

        private void MoveAbsolute()
        {
            if (Owner.IsConnected)
            {
                ELLPaddlePolariser.PolarizerPaddlePositions positions = new ELLPaddlePolariser.PolarizerPaddlePositions()
                {
                    Paddle1 = _paddlesTable[ELLPaddlePolariser.PaddleIDs.Paddle1].TargetPosition,
                    Paddle2 = _paddlesTable[ELLPaddlePolariser.PaddleIDs.Paddle2].TargetPosition,
                    Paddle3 = _paddlesTable[ELLPaddlePolariser.PaddleIDs.Paddle3].TargetPosition,
                };
                Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLPaddleStage.MoveAbsolute(positions));
            }
        }
        private void MoveRelative()
        {
            if (Owner.IsConnected)
            {
                ELLPaddlePolariser.PolarizerPaddlePositions positions = new ELLPaddlePolariser.PolarizerPaddlePositions()
                {
                    Paddle1 = _paddlesTable[ELLPaddlePolariser.PaddleIDs.Paddle1].Displacement,
                    Paddle2 = _paddlesTable[ELLPaddlePolariser.PaddleIDs.Paddle2].Displacement,
                    Paddle3 = _paddlesTable[ELLPaddlePolariser.PaddleIDs.Paddle3].Displacement,
                };
                Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLPaddleStage.MoveRelative(positions));
            }
        }

        /// <summary>   Updates the paddle position described by positions. </summary>
        /// <param name="positions">    The positions. </param>
        public void UpdatePaddlePosition(ELLPaddlePolariser.PolarizerPaddlePositions positions)
        {
            _paddlesTable[ELLPaddlePolariser.PaddleIDs.Paddle1].Position = positions.Paddle1;
            _paddlesTable[ELLPaddlePolariser.PaddleIDs.Paddle2].Position = positions.Paddle2;
            _paddlesTable[ELLPaddlePolariser.PaddleIDs.Paddle3].Position = positions.Paddle3;
        }

        /// <summary>   Updates the paddle position described by positions. </summary>
        /// <param name="position"> The position. </param>
        public void UpdatePaddlePosition(ELLPaddlePolariser.PaddlePosition position)
        {
            _paddlesTable[position.PaddleId].Position = position.Position;
        }

        /// <summary>   Gets or sets the paddles. </summary>
        /// <value> The paddles. </value>
        public List<ELLPaddleViewModel> Paddles
        {
            get { return _paddlesList; }
            set
            {
                _paddlesList = value; 
                RaisePropertyChanged(() => Paddles);
            }
        }
    }
}
