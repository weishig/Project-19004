using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using Thorlabs.Elliptec.ELLO_DLL.Support;
#pragma warning disable 1570
#pragma warning disable 1584,1711,1572,1581,1580
#pragma warning disable 1574


// namespace: Thorlabs.Elliptec.ELLO_DLL
//
// summary:	.
namespace Thorlabs.Elliptec.ELLO_DLL
{
	/// <summary> The Elliptec Device class. </summary>
	public class ELLDevice : ELLBaseDevice
	{

		private decimal _position;
		private decimal _homeOffset;
		private decimal _jogstepSize;

		private bool _isJogging;
		private bool _isThermalLocked;
	    private bool _isCleaning;

		private Timer _jogTimer;

	    public event EventHandler<bool> CleaningUpdate;
        
        /// <summary>   EllDevice Constructor. </summary>
        /// <param name="deviceID">         Identifier for the device. </param>
        /// <param name="messageUpdater">   Thea message handler. To receive feedback on progress, supply a meddage handler, derived from
        ///                                 <see cref="MessageUpdater"/> to process feedback. </param>
		internal ELLDevice(DeviceID deviceID, MessageUpdater messageUpdater)
            : base(deviceID, 2, messageUpdater)
		{
		}

        /// <summary>   EllDevice Constructor. </summary>
        /// <param name="deviceID"> Identifier for the device. </param>
		public ELLDevice(DeviceID deviceID)
            : base(deviceID, 2)
		{
		}

        #region Homing Functions
		/// <summary> Gets the device home offset. </summary>
		/// <remarks> Use <see cref="GetHomeOffset()"/> to update the current home offset</remarks>
		/// <value> The device home offset. </value>
		/// <seealso cref="GetHomeOffset()"/>
		/// <seealso cref="SetHomeOffset(decimal)"/>
		public decimal HomeOffset
		{ get { return _homeOffset; } }

	    /// <summary> Sets the home offset for the current stage. </summary>
	    /// <param name="offset"> The stage offset. </param>
	    /// <returns> True if the Offset were set. </returns>
	    /// <seealso cref="HomeOffset"/>
	    /// <seealso cref="GetHomeOffset()"/>
	    public bool SetHomeOffset(decimal offset)
	    {
	        UpdateOutput(string.Format("Set Home Offset to {0}...", DeviceInfo.FormatPosition(offset, true, true)));
	        int pulses = DeviceInfo.UnitToPulse(offset);
	        ELLDevicePort.SendStringI32(Address, "so", pulses);
	        if (!WaitForHomeOffset("gs", false))
	        {
	            return false;
	        }
	        _homeOffset = offset;
	        return true;
	    }

	    /// <summary> Gets the current device home offset. </summary>
	    /// <remarks> Use <see cref="HomeOffset"/> to obtain the current home offset</remarks>
	    /// <returns> True if the home offset was retrieved succesfully. </returns>
	    /// <seealso cref="HomeOffset"/>
	    /// <seealso cref="SetHomeOffset(decimal)"/>
	    public bool GetHomeOffset()
	    {
	        UpdateOutput("Get Home Offset...");
	        ELLDevicePort.SendString(Address, "go");
	        return WaitForHomeOffset("go", true);
	    }
	
        /// <summary> Homes the current stage. </summary>
        /// <param name="direction"> The home direction. </param>
        /// <returns> True if the device was homed succesfully. </returns>
        /// <seealso cref="Home(List<char>, ELLDevice.DeviceDirection)"/>
        public bool Home(DeviceDirection direction = DeviceDirection.Linear)
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        UpdateOutput("Homing device ...");
	        ELLDevicePort.SendString(Address, "ho" + (char)direction);
	        if (!WaitForPosition())
	        {
	            return false;
	        }
	        return true;
	    }

        /// <summary> Homes the given stages. </summary>
        /// <param name="addresses"> The list of addresses of devices to be homed. </param>
        /// <param name="direction"> The home direction. </param>
        /// <returns> Success. </returns>
        /// <seealso cref="Home(ELLBaseDevice.DeviceDirection)"/>
        /// <seealso cref="MoveAbsolute(List<char>, decimal)"/>
        /// <seealso cref="MoveRelative(List<char>, decimal)"/>
        /// <seealso cref="JogForward(List<char>)"/>
        /// <seealso cref="JogBackward(List<char>)"/>
        public bool Home(List<char> addresses, DeviceDirection direction = DeviceDirection.Linear)
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        if (!addresses.Any())
	        {
	            return Home(direction);
	        }
	        SetToGroupAddress(addresses);
	        UpdateOutput("Homing device ...");
	        ELLDevicePort.SendString(Address, "ho" + (int)direction);
	        if (!WaitForPositions(addresses))
	        {
	            return false;
	        }
	        return true;
	    }
        #endregion

        #region Jogging Functions
		/// <summary> Gets the jog step size. </summary>
		/// <remarks> Use <see cref="GetJogstepSize()"/> to update the current jog step size</remarks>
		/// <value> The size of the jogstep. </value>
		/// <seealso cref="GetJogstepSize()"/>
		/// <seealso cref="SetJogstepSize(decimal)"/>
		public decimal JogstepSize
		{ get { return _jogstepSize; } }


	    /// <summary> Sets the current device jogstep size. </summary>
	    /// <param name="jogstepSize"> The size of the jogstep. </param>
	    /// <returns> True if the jog step were set succesfully. </returns>
	    /// <seealso cref="JogBackward()"/>
	    /// <seealso cref="JogForward()"/>
	    /// <seealso cref="GetJogstepSize()"/>
	    /// <seealso cref="JogForward(List<char>)"/>
	    /// <seealso cref="JogBackward(List<char>)"/>
	    public bool SetJogstepSize(decimal jogstepSize)
	    {
	        UpdateOutput(string.Format("Set Jog Step to {0}...", DeviceInfo.FormatPosition(jogstepSize)));
	        int pulses = DeviceInfo.UnitToPulse(jogstepSize);
	        ELLDevicePort.SendStringI32(Address, "sj", pulses);
	        if (!WaitForJogstepSize("gs", false))
	        {
	            return false;
	        }
	        _jogstepSize = jogstepSize;
	        return true;
	    }

	    /// <summary> Gets the current device jogstep size. </summary>
	    /// <remarks> Use <see cref="JogstepSize"/> to obtain the current jog step size</remarks>
	    /// <returns> True if the jogstep was retrieved succesfully. </returns>
	    /// <seealso cref="JogBackward()"/>
	    /// <seealso cref="JogForward()"/>
	    /// <seealso cref="SetJogstepSize(decimal)"/>
	    /// <seealso cref="JogForward(List<char>)"/>
	    /// <seealso cref="JogBackward(List<char>)"/>
	    public bool GetJogstepSize()
	    {
	        UpdateOutput("Get Jogstep Size...");
	        ELLDevicePort.SendString(Address, "gj");
	        return WaitForJogstepSize("gj", true);
	    }

        private void StartJogging(string command, string message)
	    {
	        _isJogging = true;
	        UpdateOutput(message);
	        ELLDevicePort.SendString(Address, command);
	        if (_jogTimer == null)
	        {
	            _jogTimer = new Timer(JogTimeUpdate, 0, 100, 0);
	        }
	    }

	    /// <summary>	Starts a forward jog and returns after position value is returned. </summary>
	    /// <returns> True if the device was jogged succesfully. </returns>
	    public bool JogForwardStart()
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        if (_isJogging)
	        {
	            return false;
	        }
	        StartJogging("fw", "Jog Forward Start");
	        return true;
	    }

	    private void JogStopInternal()
	    {
	        _isJogging = false;
	        if (_jogTimer != null)
	        {
	            _jogTimer.Dispose();
	            _jogTimer = null;
	        }
	    }

	    private void JogTimeUpdate(object state)
	    {
	        if (_isJogging)
	        {
	            GetPosition();
	            _jogTimer?.Change(500, 0);
	        }
	    }

	    /// <summary>	Starts a forward jog and returns after position value is returned. </summary>
	    /// <returns> True if the device was jogged succesfully. </returns>
	    public bool JogStop()
	    {
	        if (!_isJogging)
	        {
	            return false;
	        }
	        JogStopInternal();
	        //Debug.WriteLine($"Stop Set false {_isJogging}");
	        UpdateOutput("Jog Stop");
	        ELLDevicePort.SendString(Address, "ms");
	        return GetPosition();
	    }

	    /// <summary> Jog the current device forward. </summary>
	    /// <returns> True if the device was jogged succesfully. </returns>
	    /// <seealso cref="JogBackward()"/>
	    /// <seealso cref="SetJogstepSize(decimal)"/>
	    /// <seealso cref="GetJogstepSize()"/>
	    /// <seealso cref="JogForward(List<char>)"/>
	    /// <seealso cref="JogBackward(List<char>)"/>
	    public bool JogForward()
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        UpdateOutput(string.Format("Jog Forward {0}", DeviceInfo.FormatPosition(_jogstepSize, true, true)));
	        ELLDevicePort.SendString(Address, "fw");
	        if (!WaitForPosition())
	        {
	            return false;
	        }
	        return true;
	    }

        /// <summary> Jog the given stages forward. </summary>
        /// <param name="addresses"> The addresses of the devices to be jogged. </param>
        /// <returns> True if the device was jogged succesfully. </returns>
        /// <seealso cref="Home(List<char>, ELLBaseDevice.DeviceDirection)"/>
        /// <seealso cref="MoveAbsolute(List<char>, decimal)"/>
        /// <seealso cref="MoveRelative(List<char>, decimal)"/>
        /// <seealso cref="JogForward(List<char>)"/>
        /// <seealso cref="JogBackward(List<char>)"/>
        public bool JogForward(List<char> addresses)
	    {
	        if (!addresses.Any())
	        {
	            return JogForward();
	        }
	        SetToGroupAddress(addresses);
	        UpdateOutput(string.Format("Jog Forward {0}", DeviceInfo.FormatPosition(_jogstepSize, true, true)));
	        ELLDevicePort.SendString(Address, "fw");
	        if (!WaitForPositions(addresses))
	        {
	            return false;
	        }
	        return true;
	    }

	    /// <summary>	Starts a forward jog and returns after position value is returned. </summary>
	    /// <returns> True if the device was jogged succesfully. </returns>
	    public bool JogBackwardStart()
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        if (_isJogging)
	        {
	            return false;
	        }
	        StartJogging("bw", "Jog Backward Start");
	        return true;
	    }

	    /// <summary> Jog the current device backward. </summary>
	    /// <returns> True if the device was jogged succesfully. </returns>
	    /// <seealso cref="JogBackward()"/>
	    /// <seealso cref="SetJogstepSize(decimal)"/>
	    /// <seealso cref="GetJogstepSize()"/>
	    /// <seealso cref="JogForward(List<char>)"/>
	    /// <seealso cref="JogBackward(List<char>)"/>
	    public bool JogBackward()
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        UpdateOutput(string.Format("Jog Backward {0}", DeviceInfo.FormatPosition(_jogstepSize, true, true)));
	        ELLDevicePort.SendString(Address, "bw");
	        if (!WaitForPosition())
	        {
	            return false;
	        }
	        return true;
	    }

        /// <summary> Jog the given stages backward. </summary>
        /// <param name="addresses"> The addresses of the devices to be jogged. </param>
        /// <returns> True if the device was jogged succesfully. </returns>
        /// <seealso cref="Home(List<char>, ELLBaseDevice.DeviceDirection)"/>
        /// <seealso cref="MoveAbsolute(List<char>, decimal)"/>
        /// <seealso cref="MoveRelative(List<char>, decimal)"/>
        /// <seealso cref="JogForward(List<char>)"/>
        /// <seealso cref="JogBackward(List<char>)"/>
        public bool JogBackward(List<char> addresses)
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        if (!addresses.Any())
	        {
	            return JogBackward();
	        }
	        SetToGroupAddress(addresses);
	        UpdateOutput(string.Format("Jog Backward {0}", DeviceInfo.FormatPosition(_jogstepSize, true, true)));
	        ELLDevicePort.SendString(Address, "bw");
	        if (!WaitForPositions(addresses))
	        {
	            return false;
	        }
	        return true;
	    }
        #endregion

        #region Moving Functions
        /// <summary> Gets the current device position. </summary>
        /// <remarks> Use <see cref="GetPosition()"/> to update the current position</remarks>
        /// <value> The current device position. </value>
        /// <seealso cref="GetPosition()"/>
        public decimal Position
		{ get { return _position; } }

        /// <summary> Gets the device position. </summary>
        /// <remarks> Use <see cref="Position"/> to obtain the current position</remarks>
        /// <returns> True if the position was obtained succesfully. </returns>
        /// <seealso cref="Position"/>
        /// <seealso cref="MoveAbsolute(decimal)"/>
        /// <seealso cref="MoveRelative(decimal)"/>
        /// <seealso cref="JogForward()"/>
        /// <seealso cref="JogBackward()"/>
        /// <seealso cref="Home(ELLBaseDevice.DeviceDirection)"/>
        public bool GetPosition()
		{
			UpdateOutput("Get Positions...");
			ELLDevicePort.SendString(Address, "gp");
			return WaitForPosition();
		}

	    /// <summary> Move the current stage to an absolute position. </summary>
	    /// <param name="position"> The required position. </param>
	    /// <returns> True if the stage was moved succesfully. </returns>
	    /// <seealso cref="Position"/>
	    /// <seealso cref="GetPosition()"/>
	    /// <seealso cref="MoveRelative(decimal)"/>
	    /// <seealso cref="MoveAbsolute(List<char>, decimal)"/>
	    /// <seealso cref="MoveRelative(List<char>, decimal)"/>
	    public bool MoveAbsolute(decimal position)
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        UpdateOutput(string.Format("Move device to {0}...", DeviceInfo.FormatPosition(position, true, true)));
	        int pulses = DeviceInfo.UnitToPulse(position);
	        ELLDevicePort.SendStringI32(Address, "ma", pulses);
	        if (!WaitForPosition())
	        {
	            return false;
	        }
	        return true;
	    }

        /// <summary> Move the given stages to an absolute position. </summary>
        /// <param name="addresses"> The addresses of the devices to be moved. </param>
        /// <param name="position"> The required position. </param>
        /// <returns> True if the stages were moved succesfully. </returns>
        /// <seealso cref="Home(List<char>, ELLBaseDevice.DeviceDirection)"/>
        /// <seealso cref="MoveAbsolute(List<char>, decimal)"/>
        /// <seealso cref="MoveRelative(List<char>, decimal)"/>
        /// <seealso cref="JogForward(List<char>)"/>
        /// <seealso cref="JogBackward(List<char>)"/>
        public bool MoveAbsolute(List<char> addresses, decimal position)
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        if (!addresses.Any())
	        {
	            return MoveAbsolute(position);
	        }
	        SetToGroupAddress(addresses);
	        UpdateOutput(string.Format("Move device to {0}...", DeviceInfo.FormatPosition(position, true, true)));
	        int pulses = DeviceInfo.UnitToPulse(position);
	        ELLDevicePort.SendStringI32(Address, "ma", pulses);
	        if (!WaitForPositions(addresses))
	        {
	            return false;
	        }
	        return true;
	    }

	    /// <summary> Move the current stage by a relative distance. </summary>
	    /// <param name="step"> The relative step distance. </param>
	    /// <returns> True if the stage was moved succesfully. </returns>
	    /// <seealso cref="Position"/>
	    /// <seealso cref="GetPosition()"/>
	    /// <seealso cref="MoveAbsolute(decimal)"/>
	    /// <seealso cref="MoveAbsolute(List<char>, decimal)"/>
	    /// <seealso cref="MoveRelative(List<char>, decimal)"/>
	    public bool MoveRelative(decimal step)
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        UpdateOutput(string.Format("Move device by {0}...", DeviceInfo.FormatPosition(step, true, true)));
	        int pulses = DeviceInfo.UnitToPulse(step);
	        if (pulses == 0)
	        {
	            return true;
	        }
	        ELLDevicePort.SendStringI32(Address, "mr", pulses);
	        if (!WaitForPosition())
	        {
	            return false;
	        }
	        return true;
	    }

        /// <summary> Move the current stage by a relative distance. </summary>
        /// <param name="addresses"> The addresses of the stages to be moved. </param>
        /// <param name="step"> The relative step distance. </param>
        /// <returns> True if the stage was moved succesfully. </returns>
        /// <seealso cref="Home(List<char>, ELLBaseDevice.DeviceDirection)"/>
        /// <seealso cref="MoveAbsolute(List<char>, decimal)"/>
        /// <seealso cref="MoveRelative(List<char>, decimal)"/>
        /// <seealso cref="JogForward(List<char>)"/>
        /// <seealso cref="JogBackward(List<char>)"/>
        public bool MoveRelative(List<char> addresses, decimal step)
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        if (!addresses.Any())
	        {
	            return MoveRelative(step);
	        }
	        SetToGroupAddress(addresses);
	        UpdateOutput(string.Format("Move device by {0}...", DeviceInfo.FormatPosition(step, true, true)));
	        int pulses = DeviceInfo.UnitToPulse(step);
	        if (pulses == 0)
	        {
	            return true;
	        }
	        ELLDevicePort.SendStringI32(Address, "mr", pulses);
	        if (!WaitForPositions(addresses))
	        {
	            return false;
	        }
	        return true;
	    }
        #endregion

	    public bool IsDeviceBusy()
	    {
	        return TestThermalInitialized() || TestCleaningState();
	    }

        /// <summary>	Tests thermal initialized. </summary>
        /// <returns>	true if the test passes, false if the test fails. </returns>
        public bool TestThermalInitialized()
		{
			if (_isThermalLocked)
			{
				UpdateOutput("Thermal Lockout");
				return true;
			}
			return false;
		}

	    /// <summary>	Tests thermal initialized. </summary>
	    /// <returns>	true if the test passes, false if the test fails. </returns>
	    public bool TestCleaningState()
	    {
	        if (_isCleaning)
	        {
	            UpdateOutput("Cleaning, Please wait...");
	            return true;
	        }
	        return false;
	    }

	    void SetCleaningState(bool isCleaning)
	    {
	        _isCleaning = isCleaning;
	        CleaningUpdate?.Invoke(this, _isCleaning);
	    }

	    /// <summary>	Saves the user data. </summary>
	    /// <returns>	True if the UserData is saved by the device. </returns>
	    public bool SendCleanMachine()
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        SetCleaningState(true);
	        UpdateOutput("Performing clean mechanics...");
	        ELLDevicePort.SendString(Address, "cm");
	        bool retval = WaitForCleaning();
            SetCleaningState(false);;
	        return retval;
	    }

	    /// <summary>	Saves the user data. </summary>
	    /// <returns>	True if the UserData is saved by the device. </returns>
	    public bool SendCleanAndOptimize()
	    {
	        if (IsDeviceBusy())
	        {
	            return false;
	        }
	        SetCleaningState(true);;
	        UpdateOutput("Performing clean & optimize...");
	        ELLDevicePort.SendString(Address, "om");
	        bool retval = WaitForCleaning();
	        SetCleaningState(false);;
	        return retval;
	    }

	    /// <summary>	Saves the user data. </summary>
	    /// <returns>	True if the UserData is saved by the device. </returns>
	    public bool SendStopCleaning()
	    {
	        if (!_isCleaning)
	        {
	            return false;
	        }
	        UpdateOutput("Stop cleaning...");
	        ELLDevicePort.SendString(Address, "st");
	        SetSleepDelayCounter(2);
	        ELLDevicePort.SendString(Address, "gs");
	        return true;
	    }

		/// <summary> Sets the group address for the given devices. </summary>
		/// <param name="addresses"> The addresses of devices to join the group. </param>
		/// <returns> True if the function completed succesfully. </returns>
		public bool SetToGroupAddress(IEnumerable<char> addresses)
		{
			foreach (char address in addresses)
			{
				if (address != Address)
				{
					UpdateOutput(string.Format("Set GroupAddress {0}->{1}...", address, Address));
					ELLDevicePort.SendStringB(address, "ga", (byte)Address);
					if (!WaitForStatus(Address))
					{
						return false;
					}
				}
			}
			return true;
		}

        #region Wait for completion methods
	    /// <summary>   Process the device status. </summary>
	    /// <param name="deviceStatus"> The device status. </param>
	    /// <param name="resendCmd">    The resend command. </param>
	    /// <param name="getter">       True to getter. </param>
	    /// <returns>   A StatusResult. </returns>
	    protected override StatusResult ProcessDeviceStatus(DeviceStatus deviceStatus, string resendCmd, bool getter)
	    {
	        switch (deviceStatus.Status)
	        {
	            case DeviceStatus.DeviceStatusValues.Busy:
	                UpdateDeviceStatus(deviceStatus);
	                if (getter)
	                {
	                    ELLDevicePort.SendString(Address, resendCmd);
	                }
	                return StatusResult.Busy;
	            case DeviceStatus.DeviceStatusValues.OK:
	                _isThermalLocked = false;
	                UpdateDeviceStatus(deviceStatus);
	                if (getter)
	                {
	                    ELLDevicePort.SendString(Address, resendCmd);
	                }
	                return StatusResult.OK;
	            case DeviceStatus.DeviceStatusValues.ThermalError:
	                JogStopInternal();
	                _isThermalLocked = true;
	                UpdateDeviceStatus(deviceStatus);
	                Thread.Sleep(2000);
	                ELLDevicePort.SendString(Address, "gs");
	                break;
	            case DeviceStatus.DeviceStatusValues.ModuleIsolated:
	                UpdateDeviceStatus(deviceStatus);
	                return StatusResult.Fail;
	            default:
	                UpdateDeviceStatus(deviceStatus);
	                Thread.Sleep(500);
	                ELLDevicePort.SendString(Address, resendCmd);
	                break;
	        }
	        return StatusResult.Error;
	    }

        private bool WaitForPosition(int msTimeout = MoveTimeout)
	    {
	        return WaitForPositions(new List<char> { Address }, msTimeout);
	    }

	    private bool WaitForPositions(List<char> addresses, int msTimeout = MoveTimeout)
	    {
	        try
	        {
	            Dictionary<char, bool> _isCompletedList = addresses.ToDictionary(c => c, c => false);
	            List<string> responses = new List<string> { "GS", "PO" };
	            int counter = 10 * addresses.Count;
	            while (true)
	            {
	                string msg = ELLDevicePort.WaitForResponse(addresses, responses, msTimeout);
	                if (!string.IsNullOrEmpty(msg))
	                {
	                    bool returnValue;
	                    if (TestStatus(msg, "gp", true, ref counter, out returnValue))
	                    {
	                        return returnValue;
	                    }
	                    char address = msg[0];
	                    if ((msg.Substring(1, 2) == "PO") && addresses.Contains(address))
	                    {
	                        if (msg.Length != 11)
	                        {
	                            return false;
	                        }
	                        _position = DeviceInfo.PulseToUnit(msg.Substring(3).ToBytes(8).ToInt(true));
	                        UpdateParameter(MessageUpdater.UpdateTypes.Position, address, _position);
	                        _isCompletedList[address] = true;
	                        if (_isCompletedList.All(item => item.Value))
	                        {
	                            return true;
	                        }
	                    }
	                }
	            }
	        }
	        catch (ELLException ex)
	        {
	            UpdateOutput($"Get Device Status: {ex.Message}", true);
	        }
	        return false;
	    }

        private bool WaitForHomeOffset(string cmd, bool getter, int msTimeout = MoveTimeout)
		{
			try
			{
				List<string> responses = new List<string> { "GS", "HO" };
				int counter = 10;
				while (true)
				{
					string msg = ELLDevicePort.WaitForResponse(Address, responses, msTimeout);
					if (!string.IsNullOrEmpty(msg))
					{
						bool returnValue;
						if (TestStatus(msg, cmd, getter, ref counter, out returnValue))
						{
							return returnValue;
						}
						if (msg.Substring(1, 2) == "HO")
						{
							if (msg.Length != 11)
							{
								return false;
							}
							_homeOffset = DeviceInfo.PulseToUnit(msg.Substring(3).ToBytes(8).ToInt(true));
							UpdateParameter(MessageUpdater.UpdateTypes.HomeOffset, Address, _homeOffset);
							return true;
						}
					}
				}
			}
			catch (ELLException ex)
			{
				UpdateOutput($"Get Device Status: {ex.Message}", true);
			}
			return false;
		}

		private bool WaitForJogstepSize(string cmd, bool getter, int msTimeout = MoveTimeout)
		{
			try
			{
				List<string> responses = new List<string> { "GS", "GJ" };
				int counter = 10;
				while (true)
				{
					string msg = ELLDevicePort.WaitForResponse(Address, responses, msTimeout);
					if (!string.IsNullOrEmpty(msg))
					{
						bool returnValue;
						if (TestStatus(msg, cmd, getter, ref counter, out returnValue))
						{
							return returnValue;
						}
						if (msg.Substring(1, 2) == "GJ")
						{
							if (msg.Length != 11)
							{
								return false;
							}
							_jogstepSize = DeviceInfo.PulseToUnit(msg.Substring(3).ToBytes(8).ToInt(true));
							UpdateParameter(MessageUpdater.UpdateTypes.JogstepSize, Address, _jogstepSize);
							return true;
						}
					}
				}
			}
			catch (ELLException ex)
			{
				UpdateOutput($"Get Device Status: {ex.Message}", true);
			}
			return false;
		}
        #endregion
 	}
}
