using System;
using System.Collections.Generic;
using System.Windows.Input;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO_DLL;

namespace Thorlabs.Elliptec.ELLO.ViewModel
{
    /// <summary>   A ViewModel for the ell paddle. </summary>
    /// <seealso cref="T:Thorlabs.Elliptec.ELLO.Support.ObservableObject"/>
    public class ELLPaddleViewModel : ObservableObject
    {
        private readonly ELLPaddlePolariser _paddleDevice;
        private readonly ELLPaddlePolariser.PaddleIDs _paddleID;
        private decimal _position;
        private decimal _targetPosition;
        private decimal _displacement;
        private short _moveTime;

        private string _title;

        private ICommand _getHomeCommand;
        private ICommand _getUpdatePositionCommand;
        private ICommand _getMoveAbsoluteCommand;
        private ICommand _getMoveRelativeCommand;
        private ICommand _getMoveTimeFwdCommand;
        private ICommand _getMoveTimeBwdCommand;

        /// <summary> Gets the click home command. </summary>
        /// <value> The click home command. </value>
        public ICommand ClickHomeCommand { get { return _getHomeCommand ?? (_getHomeCommand = new RelayCommand(Home)); } }

        /// <summary> Gets the click home command. </summary>
        /// <value> The click home command. </value>
        public ICommand ClickUpdatePositionCommand { get { return _getUpdatePositionCommand ?? (_getUpdatePositionCommand = new RelayCommand(UpdatePosition)); } }

        /// <summary>   Gets the click move absolute command. </summary>
        /// <value> The click move absolute command. </value>
        public ICommand ClickMoveAbsoluteCommand { get { return _getMoveAbsoluteCommand ?? (_getMoveAbsoluteCommand = new RelayCommand(MoveAbsolute)); } }

        /// <summary>   Gets the click move relative command. </summary>
        /// <value> The click move relative command. </value>
        public ICommand ClickMoveRelativeCommand { get { return _getMoveRelativeCommand ?? (_getMoveRelativeCommand = new RelayCommand(MoveRelative)); } }

         /// <summary>  Gets the click move time command. </summary>
         /// <value>    The click move time command. </value>
         public ICommand ClickMoveTimeFwdCommand { get { return _getMoveTimeFwdCommand ?? (_getMoveTimeFwdCommand = new RelayCommand(MoveTimeFwd)); } }

        /// <summary>  Gets the click move time command. </summary>
        /// <value>    The click move time command. </value>
        public ICommand ClickMoveTimeBwdCommand { get { return _getMoveTimeBwdCommand ?? (_getMoveTimeBwdCommand = new RelayCommand(MoveTimeBwd)); } }

        private ELLDevicesViewModel _owner;

        /// <summary>   Constructor. </summary>
        /// <param name="paddleDevice"> The paddle device. </param>
        /// <param name="paddleID">     Identifier for the paddle. </param>
        public ELLPaddleViewModel(ELLDevicesViewModel owner, ELLPaddlePolariser paddleDevice, ELLPaddlePolariser.PaddleIDs paddleID)
        {
            _owner = owner;
            _paddleDevice = paddleDevice;
            _paddleID = paddleID;
            Title = $"Paddle{_paddleID}";
            TargetPosition = 180;
            Displacement = 5;
            MoveTime = 10;
        }

        /// <summary>   Gets the format string. </summary>
        /// <value> The format string. </value>
        public string FormatStr
        { get { return _paddleDevice.DeviceInfo.FormatStr; } }

        /// <summary>   Gets or sets the title. </summary>
        /// <value> The title. </value>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        /// <summary>   Gets or sets the position. </summary>
        /// <value> The position. </value>
        public decimal Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RaisePropertyChanged(() => Position);
            }
        }

        /// <summary>   Gets or sets the position. </summary>
        /// <value> The position. </value>
        public decimal TargetPosition
        {
            get { return _targetPosition; }
            set
            {
                _targetPosition = value;
                RaisePropertyChanged(() => TargetPosition);
                RaisePropertyChanged(() => TargetPositionStr);
                if (value < _paddleDevice.MinPos || value > _paddleDevice.MaxPos)
                {
                    throw new ArgumentOutOfRangeException($"Range {_paddleDevice.MinPosRounded} to {_paddleDevice.MaxPosRounded}");
                }
            }
        }

        /// <summary> Gets or sets target absolute move. </summary>
        /// <value> The target absolute move. </value>
        public string TargetPositionStr
        {
            get
            {
                return _paddleDevice.DeviceInfo.FormatPosition(_targetPosition);
            }
            set
            {
                decimal d;
                bool result = decimal.TryParse(value, out d);
                TargetPosition = d;
            }
        }

        /// <summary>   Gets or sets the position. </summary>
        /// <value> The position. </value>
        public decimal Displacement
        {
            get { return _displacement; }
            set
            {
                _displacement = value;
                RaisePropertyChanged(() => Displacement);
                RaisePropertyChanged(() => DisplacementStr);
                if (Math.Abs(value) > _paddleDevice.DeviceInfo.Travel)
                {
                    throw new ArgumentOutOfRangeException($"Range 0.0 to {_paddleDevice.DeviceInfo.Travel}");
                }
            }
        }

        /// <summary>   Gets or sets the move time. </summary>
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside the required range. </exception>
        /// <value> The move time. </value>
        public short MoveTime
        {
            get { return _moveTime; }
            set
            {
                _moveTime = value;
                RaisePropertyChanged(() => MoveTime);
                if ((value < 1) || (value > 8000))
                {
                    throw new ArgumentOutOfRangeException($"Range 1 to 8000");
                }
            }
        }

        /// <summary> Gets or sets target absolute move. </summary>
        /// <value> The target absolute move. </value>
        public string DisplacementStr
        {
            get
            {
                return _paddleDevice.DeviceInfo.FormatPosition(_displacement);
            }
            set
            {
                decimal d;
                bool result = decimal.TryParse(value, out d);
                Displacement = d;
            }
        }

        private readonly Dictionary<ELLPaddlePolariser.PaddleIDs, ELLPaddlePolariser.PaddleHomeMask> _homeLookup =
            new Dictionary<ELLPaddlePolariser.PaddleIDs, ELLPaddlePolariser.PaddleHomeMask>()
            {
                {ELLPaddlePolariser.PaddleIDs.Paddle1, ELLPaddlePolariser.PaddleHomeMask.Paddle1},
                {ELLPaddlePolariser.PaddleIDs.Paddle2, ELLPaddlePolariser.PaddleHomeMask.Paddle2},
                {ELLPaddlePolariser.PaddleIDs.Paddle3, ELLPaddlePolariser.PaddleHomeMask.Paddle3},
            };

 
        private void Home()
        {
            if (_owner.IsConnected)
            {
                _owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _paddleDevice.Home(_homeLookup[_paddleID]));
            }
        }

        private void UpdatePosition()
        {
            if (_owner.IsConnected)
            {
                _owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _paddleDevice.RequestPosition(_paddleID));
            }
        }
        private void MoveAbsolute()
        {
            if (_owner.IsConnected)
            {
                ELLPaddlePolariser.PaddlePosition position = new ELLPaddlePolariser.PaddlePosition()
                    {PaddleId = _paddleID, Position = TargetPosition};
                _owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _paddleDevice.MoveAbsolute(position));
            }
        }
        private void MoveRelative()
        {
            if (_owner.IsConnected)
            {
                ELLPaddlePolariser.PaddlePosition displacement = new ELLPaddlePolariser.PaddlePosition()
                    { PaddleId = _paddleID, Position = Displacement };
                _owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _paddleDevice.MoveRelative(displacement));
            }
        }

        private void MoveTimeFwd()
        {
            if (_owner.IsConnected)
            {
                 _owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _paddleDevice.MoveMicrosecondSteps(_paddleID, _moveTime, false));
            }
        }
        private void MoveTimeBwd()
        {
            if (_owner.IsConnected)
            {
                _owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => _paddleDevice.MoveMicrosecondSteps(_paddleID, _moveTime, true));
            }
        }
    }
}
