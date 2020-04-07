using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Thorlabs.Elliptec.ELLO_DLL.Support;

namespace Thorlabs.Elliptec.ELLO_DLL
{
    /// <summary>   An ell base device. </summary>
    public abstract class ELLBaseDevice
    {
        private object _lockObject = new object();

        private static readonly List<char> _validAddresses = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>	The move timeout. </summary>
        public const int CleanTimeout = 10000;
        /// <summary>	The move timeout. </summary>
        public const int MoveTimeout = 4000;
        /// <summary>   The search timeout. </summary>
        public const int SearchTimeout = 8000;
        /// <summary>	The move timeout. </summary>
        public const int HomeTimeout = 6000;

        /// <summary>	The status timeout. </summary>
        public const int StatusTimeout = 1000;

        private readonly MessageUpdater _messageUpdater;

        /// <summary>	Values that represent status results. </summary>
        public enum StatusResult
        {
            OK,
            Busy,
            Error,
            Fail
        }

        /// <summary> Values that represent Device Direction. </summary>
        /// <remarks> These are used to set the homing direction for a device</remarks>
        public enum DeviceDirection
        {
            /// <summary> An enum constant representing the linear option. </summary>
            [StringValue("Linear")]
            Linear = '0',
            /// <summary> An enum constant representing the clockwise option. </summary>
            [StringValue("Clockwise")]
            Clockwise = '0',
            /// <summary> An enum constant representing the anti clockwise option. </summary>
            [StringValue("Anticlockwise")]
            AntiClockwise = '1',
        }

        /// <summary> Output Update delegate. </summary>
        /// <remarks> Delegate defining the callback function required to connect to the OutputUpdate callback</remarks>
        /// <param name="str">   A List of Outputs sent to the device. </param>
        /// <param name="error"> An error flag set if an error has occurred. </param>
        public delegate void OutputUpdateDelegate(List<string> str, bool error);

        /// <summary> The output Update. </summary>
        /// <remarks> Notification event used to update the world of new outputs generated</remarks>
        public OutputUpdateDelegate OutputUpdate;

        private readonly Dictionary<char, MotorInfo> _motorInfo = new Dictionary<char, MotorInfo>();

        /// <summary>   EllDevice Constructor. </summary>
        /// <param name="deviceID">         Identifier for the device. </param>
        /// <param name="motorCount">       Number of motors. </param>
        /// <param name="messageUpdater">   Thea message handler. To receive feedback on progress, supply a meddage handler, derived from
        ///                                 <see cref="MessageUpdater"/> to process feedback. </param>
        internal ELLBaseDevice(DeviceID deviceID, int motorCount, MessageUpdater messageUpdater)
        {
            _messageUpdater = messageUpdater;
            InitializeClass(deviceID, (char)('0' + motorCount));
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        /// <param name="deviceID">     Identifier for the device. </param>
        /// <param name="motorCount">   Number of motors. </param>
        protected ELLBaseDevice(DeviceID deviceID, int motorCount)
        {
            _messageUpdater = null;
            InitializeClass(deviceID, (char)('0' + motorCount));
        }

        private void InitializeClass(DeviceID deviceID, char motorCount)
        {
            for (char c = '1'; c <= motorCount; c++)
            {
                _motorInfo.Add(c, new MotorInfo(c));
            }

            DeviceInfo = deviceID;
            Address = deviceID.Address;
            DeviceStatus = new DeviceStatus();
        }

        /// <summary> Gets the information describing the device. </summary>
        /// <value> Information describing the device, see <see cref="DeviceID"/>. </value>
        public DeviceID DeviceInfo { get; protected set; }

        public static DeviceID Configure(string deviceID, MessageUpdater messageUpdater)
        {
            if (string.IsNullOrEmpty(deviceID))
            {
                return null;
            }
            char address = deviceID[0];
            if (!IsValidAddress(address))
            {
                return null;
            }
            DeviceID di = new DeviceID(deviceID);
            messageUpdater?.UpdateParameter(MessageUpdater.UpdateTypes.DeviceInfo, address, di);
            return di;
        }

        #region Device Addressing
        /// <summary> Gets the device address. </summary>
        /// <value> The device address. </value>
        public char Address
        { get; protected set; }

        /// <summary> Gets or sets a value indicating whether to automatic save the device configuration. </summary>
        /// <value> true if automatic save, false if not. </value>
        public bool AutoSave { get; set; }

        /// <summary> Gets the list of valid addresses. </summary>
        /// <value> The list of valid addresses. </value>
        public static List<char> ValidAddresses
        { get { return _validAddresses; } }

        /// <summary> Query if this address is valid address. </summary>
        /// <returns> true if a valid address, false if not. </returns>
        public static bool IsValidAddress(char address)
        {
            return _validAddresses.Contains(address);
        }

        /// <summary> Query if this object has valid address. </summary>
        /// <returns> true if the device has a valid address, false if not. </returns>
        public bool IsValidAddress()
        {
            return _validAddresses.Contains(Address);
        }

        /// <summary>	Saves the user data. </summary>
        /// <returns>	True if the UserData is saved by the device. </returns>
        public bool SaveUserData()
        {
            UpdateOutput("Save user status...");
            ELLDevicePort.SendString(Address, "us");
            return WaitForStatus();
        }

        /// <summary> Sets the new device address. </summary>
        /// <param name="newAddress"> The new device address. </param>
        /// <returns> True if the device address is changed. </returns>
        public bool SetAddress(char newAddress)
        {
            if (newAddress == Address)
            {
                return true;
            }
            UpdateOutput(string.Format("changing address to {0}...", newAddress));
            ELLDevicePort.SendStringB(Address, "ca", (byte)newAddress);
            Address = newAddress;
            return WaitForStatus();
        }
        #endregion

        #region Motor Setup
        /// <summary>	Check whether the motor ID is valid. </summary>
        /// <param name="c">	The motor ID.  Should be '1' or '2' </param>
        /// <returns>	True if motor identifier valid, False if not valid or not valid for this device. </returns>
        protected bool IsMotorIdValid(char c)
        {
            return _motorInfo.ContainsKey(c);
        }

        /// <summary>	Get the current MotorInfo for the selected channe;. </summary>
        /// <param name="indexer">	The indexer. </param>
        /// <returns>	The indexed MotorInfo item. </returns>
        public MotorInfo this[char indexer]
        {
            get { return _motorInfo.ContainsKey(indexer) ? _motorInfo[indexer] : null; }
        }

        /// <summary>	Convert the frequency into a period in seconds. </summary>
        /// <param name="frequency">	The device frequency. </param>
        /// <returns>	The device periodicity in seconds. </returns>
        public decimal NormalizePeriod(decimal frequency)
        {
            Int16 period = (Int16)(14740 / frequency);
            return 14740m / period;
        }

        /// <summary> Get the device MotorInfo structure. </summary>
        /// <returns> Success if the motor info retrieved succesfully. </returns>
        public bool GetMotorInfo(char motorID)
        {
            if (!IsMotorIdValid(motorID))
            {
                return false;
            }
            UpdateOutput(string.Format("Requesting Motor{0} info...", motorID));
            ELLDevicePort.SendString(Address, "i" + motorID);
            try
            {
                string msg = ELLDevicePort.WaitForResponse(Address, "I" + motorID, MoveTimeout);
                if (!string.IsNullOrEmpty(msg))
                {
                    MotorInfo info = new MotorInfo(msg);
                    _motorInfo[motorID] = info;
                    UpdateOutput("Get Device Status: ", info.Description());
                    UpdateParameter(MessageUpdater.UpdateTypes.MotorInfo, Address, info);
                    return true;
                }
            }
            catch (ELLException)
            {
                UpdateOutput(string.Format("Requesting Motor{0}: FAILED", motorID));
            }
            return false;
        }

        /// <summary>   Sets the device period. </summary>
        /// <param name="motorID">              Identifier for the motor. </param>
        /// <param name="fwd">                  The direction the period is applied to, true for forward, false for reverse. </param>
        /// <param name="frequency">            The device frequency. </param>
        /// <param name="permanent">            True to change the period permanently. </param>
        /// <param name="hardSaveFrequencey">   True to hard save frequencey. </param>
        /// <returns>   True if the period is set. </returns>
        public bool SetPeriod(char motorID, bool fwd, decimal frequency, bool permanent, bool hardSaveFrequencey)
        {
            if (!IsMotorIdValid(motorID))
            {
                return false;
            }
            UInt16 period = (UInt16)(14740 / frequency);
            UpdateOutput(string.Format("Motor{0} - Setting {1} period to {2:X}({3}kHz) ...", motorID, fwd ? "Fwd" : "Bwd", period, frequency));
            if (hardSaveFrequencey)
            {
                period |= 0x8000;
            }
            ELLDevicePort.SendStringI16(Address, (fwd ? "f" : "b") + motorID, period);
            if (!WaitForStatus())
            {
                return false;
            }
            if (permanent)
            {
                SaveUserData();
            }
            return true;
        }

        /// <summary> Search for the device period. </summary>
        /// <param name="motorID">   Identifier for the motor. </param>
        /// <param name="permanent"> True to change the period permanently. </param>
        /// <returns> True if the period is set. </returns>
        public bool SearchPeriod(char motorID, bool permanent)
        {
            if (!IsMotorIdValid(motorID))
            {
                return false;
            }
            UpdateOutput(string.Format("Motor{0} - Search period ...", motorID));
            ELLDevicePort.SendString(Address, "s" + motorID);
            if (!WaitForStatus(SearchTimeout))
            {
                return false;
            }
            GetMotorInfo(motorID);
            if (permanent)
            {
                SaveUserData();
            }
            return true;
        }

        public bool ResetPeriod()
        {
            List<char> _motors = _motorInfo.Where(kvp => (kvp.Value != null) && (kvp.Value.IsValid)).Select(kvp=>kvp.Key).ToList();
            foreach (char motorID in _motors)
            {
                UpdateOutput(string.Format("Motor{0} - Reset default period ...", motorID));
                UInt16 period = (Int16)(14740 / 100) | 0x8000;
                ELLDevicePort.SendStringI16(Address, "f" + motorID, period);
                if (!WaitForStatus(SearchTimeout))
                {
                    return false;
                }
                ELLDevicePort.SendStringI16(Address, "b" + motorID, period);
                if (!WaitForStatus(SearchTimeout))
                {
                    return false;
                }
                GetMotorInfo(motorID);
            }
            SaveUserData();
            return true;
        }
        /// <summary> Scan current Curve. </summary>
        /// <param name="motorID">   Identifier for the motor. </param>
        /// <returns> True if the function was performed succesfully. </returns>
        public bool ScanCurrentCurve(char motorID)
        {
            if (!IsMotorIdValid(motorID))
            {
                return false;
            }
            UpdateOutput(string.Format("Motor{0} - Scan Current ...", motorID));
            ELLDevicePort.SendString(Address, "c" + motorID);
            if (!WaitForStatus(6000))
            {
                return false;
            }
            GetMotorInfo(motorID);
            return true;
        }
        #endregion


        #region Broadcasting
        /// <summary>   Updates the parameter. </summary>
        /// <param name="updateType">   Type of the update. </param>
        /// <param name="address">      The device address. </param>
        /// <param name="data">         The data. </param>
        protected void UpdateParameter(MessageUpdater.UpdateTypes updateType, char address, object data)
        {
            _messageUpdater?.UpdateParameter(updateType, address, data);
        }

        /// <summary>   Updates the output. </summary>
        /// <param name="message">  The message. </param>
        /// <param name="error">    (Optional) True to error. </param>
        protected void UpdateOutput(string message, bool error = false)
        {
            _messageUpdater?.UpdateOutput(message, error);
        }

        /// <summary>   Updates the output. </summary>
        /// <param name="list">     The list. </param>
        /// <param name="error">    (Optional) True to error. </param>
        protected void UpdateOutput(List<string> list, bool error = false)
        {
            _messageUpdater?.UpdateOutput(list, error);
        }

        /// <summary>   Updates the output. </summary>
        /// <param name="message">  The message. </param>
        /// <param name="list">     The list. </param>
        protected void UpdateOutput(string message, IEnumerable<string> list)
        {
            _messageUpdater?.UpdateOutput(message, list);
        }
        #endregion

        #region Device Status
        protected abstract StatusResult ProcessDeviceStatus(DeviceStatus deviceStatus, string resendCmd, bool getter);

        /// <summary>   Gets or sets the device status. </summary>
        /// <value> The device status. </value>
        public DeviceStatus DeviceStatus { get; protected set; }

        /// <summary>   Updates the device status described by deviceStatus. </summary>
        /// <param name="deviceStatus"> The device status. </param>
        protected void UpdateDeviceStatus(DeviceStatus deviceStatus)
        {
            if (deviceStatus.Status != DeviceStatus.Status)
            {
                bool error = !((deviceStatus.Status == DeviceStatus.DeviceStatusValues.OK) || (deviceStatus.Status == DeviceStatus.DeviceStatusValues.Busy));
                UpdateOutput("Get Device Status: " + deviceStatus.Status.GetStringValue(), error);
                UpdateParameter(MessageUpdater.UpdateTypes.Status, Address, deviceStatus);
            }
            DeviceStatus = deviceStatus;
        }

        /// <summary> Obtains the device status from the device. </summary>
        /// <remarks> Use <see cref="DeviceStatus"/> to obtain the current status</remarks>
        /// <returns> True if the status is retrieved from the device. </returns>
        /// <seealso cref="DeviceStatus"/>
        public bool GetDeviceStatus()
        {
            UpdateOutput("Get Device Status...");
            ELLDevicePort.SendString(Address, "gs");
            return WaitForStatus();
        }

        /// <summary>   Wait for status. </summary>
        /// <param name="msTimeout">    (Optional) The milliseconds timeout. </param>
        /// <returns>   True if it succeeds, false if it fails. </returns>
        private bool WaitForStatus(int msTimeout = MoveTimeout)
        {
            return WaitForStatus(Address, msTimeout);
        }

        private int _sleepDurationCounter;

        protected void SetSleepDelayCounter(int delayCounter)
        {
            lock (_lockObject)
            {
                _sleepDurationCounter = delayCounter;
            }
        }

        private int GetSleepDelayCounter()
        {
            lock (_lockObject)
            {
                return _sleepDurationCounter;
            }
        }
        /// <summary>   Wait for status. </summary>
        /// <param name="msTimeout">    (Optional) The milliseconds timeout. </param>
        /// <returns>   True if it succeeds, false if it fails. </returns>
        protected bool WaitForCleaning(int msTimeout = CleanTimeout)
        {
            try
            {
                int counter = 60;
                while (true)
                {
                    string msg = ELLDevicePort.WaitForResponse(Address, "GS", msTimeout);
                    if (!String.IsNullOrEmpty(msg))
                    {
                        if (msg.Substring(1, 4) == "GS09")
                        {
                            SetSleepDelayCounter(10);
                            while (GetSleepDelayCounter() >= 0)
                            {
                                SetSleepDelayCounter(GetSleepDelayCounter() - 1);
                                Thread.Sleep(1000);
                            }
                            ELLDevicePort.SendString(Address, "gs");
                        }
                        else
                        {
                            bool returnValue;
                            if (TestStatus(msg, "gs", false, ref counter, out returnValue))
                            {
                                return returnValue;
                            }
                        }
                    }
                }
            }
            catch (ELLException ex)
            {
                _messageUpdater?.UpdateOutput($"Get Device Status: {ex.Message}", true);
            }
            return false;
        }

        /// <summary> Wait for status. </summary>
        /// <param name="address">   The device address. </param>
        /// <param name="msTimeout"> (Optional) The milliseconds timeout. </param>
        /// <param name="getter">    True to getter. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        protected bool WaitForStatus(char address, int msTimeout = MoveTimeout, bool getter = false)
        {
            try
            {
                int counter = 10;
                while (true)
                {
                    string msg = ELLDevicePort.WaitForResponse(address, "GS", msTimeout);
                    if (!String.IsNullOrEmpty(msg))
                    {
                        bool returnValue;
                        if (TestStatus(msg, "gs", getter, ref counter, out returnValue))
                        {
                            return returnValue;
                        }
                    }
                }
            }
            catch (ELLException ex)
            {
                _messageUpdater?.UpdateOutput($"Get Device Status: {ex.Message}", true);
            }
            return false;
        }

        /// <summary>   Tests status. </summary>
        /// <param name="msg">          The message. </param>
        /// <param name="cmd">          The command. </param>
        /// <param name="getter">       True to getter. </param>
        /// <param name="counter">      [in,out] The counter. </param>
        /// <param name="returnValue">  [out] True to return value. </param>
        /// <returns>   True if the test passes, false if the test fails. </returns>
        protected bool TestStatus(string msg, string cmd, bool getter, ref int counter, out bool returnValue)
        {
            if (msg.Substring(1, 2) == "GS")
            {
                switch (ProcessDeviceStatus(new DeviceStatus(msg), cmd, getter))
                {
                    case StatusResult.Fail:
                        returnValue = false;
                        return true;
                    case StatusResult.Busy:
                        returnValue = false;
                        return true;
                    case StatusResult.OK:
                        if (!getter)
                        {
                            returnValue = true;
                            return true;
                        }
                        break;
                    case StatusResult.Error:
                        break;
                }
                if (--counter == 0)
                {
                    returnValue = false;
                    return true;
                }
            }
            returnValue = false;
            return false;
        }
        #endregion
    }
}
