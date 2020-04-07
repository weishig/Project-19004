using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Thorlabs.Elliptec.ELLO_DLL.Support;

namespace Thorlabs.Elliptec.ELLO_DLL
{
    public class ELLPaddlePolariser : ELLBaseDevice
    {
        /// <summary>   A paddle positions. </summary>
        /// <seealso cref="T:Thorlabs.Elliptec.ELLO_DLL.ELLBaseDevice"/>
        public struct PolarizerPaddlePositions
        {
            /// <summary>   The first paddle. </summary>
            public decimal Paddle1;
            /// <summary>   The second paddle. </summary>
            public decimal Paddle2;
            /// <summary>   The third paddle. </summary>
            public decimal Paddle3;
        }

        /// <summary>   A paddle positions. </summary>
        /// <seealso cref="T:Thorlabs.Elliptec.ELLO_DLL.ELLBaseDevice"/>
        public struct PaddlePosition
        {
            /// <summary>   The position. </summary>
            public decimal Position;
            /// <summary>   Identifier for the paddle. </summary>
            public PaddleIDs PaddleId;
        }

        /// <summary>   A bit-field of flags for specifying paddles. </summary>
        [Flags]
        public enum PaddleHomeMask
        {
            /// <summary>   A binary constant representing the paddle 1 flag. </summary>
            Paddle1 = 1,
            /// <summary>   A binary constant representing the paddle 2 flag. </summary>
            Paddle2 = 2,
            /// <summary>   A binary constant representing the paddle 3 flag. </summary>
            Paddle3 = 4,
            All = Paddle1 | Paddle2 | Paddle3,
            None = 0,
        }

        /// <summary>   Values that represent paddle i ds. </summary>
        public enum PaddleIDs
        {
            /// <summary>   An enum constant representing the paddle 1 option. </summary>
            Paddle1 = '1',
            /// <summary>   An enum constant representing the paddle 2 option. </summary>
            Paddle2 = '2',
            /// <summary>   An enum constant representing the paddle 3 option. </summary>
            Paddle3 = '3'
        }

        private decimal _jogstepSize;
        private decimal _homeOffset;


        /// <summary>   EllDevice Constructor. </summary>
        /// <param name="deviceID">         Identifier for the device. </param>
        /// <param name="messageUpdater">   Thea message handler. To receive feedback on progress, supply a meddage handler, derived from
        ///                                 <see cref="MessageUpdater"/> to process feedback. </param>
        internal ELLPaddlePolariser(DeviceID deviceID, MessageUpdater messageUpdater)
            : base(deviceID, 3, messageUpdater)
        {
            MinPos = (16m / 1023) * deviceID.Travel;
            MinPosRounded = Math.Ceiling(MinPos * 10) / 10;
            MaxPos = (1006m / 1023) * deviceID.Travel;
            MaxPosRounded = Math.Floor(MinPos * 10) / 10;
         }

        /// <summary>   EllDevice Constructor. </summary>
        /// <param name="deviceID"> Identifier for the device. </param>
        public ELLPaddlePolariser(DeviceID deviceID)
            : base(deviceID, 3)
        {
            MinPos = (16m / 1023) * deviceID.Travel;
            MinPosRounded = Math.Ceiling(MinPos * 10) / 10;
            MaxPos = (1006m / 1023) * deviceID.Travel;
            MaxPosRounded = Math.Floor(MinPos * 10) / 10;
        }

        /// <summary>   Gets the minimum position. </summary>
        /// <value> The minimum position. </value>
        public Decimal MinPos { get; }

        /// <summary>   Gets the minimum position rounded. </summary>
        /// <value> The minimum position rounded. </value>
        public Decimal MinPosRounded { get; }

        /// <summary>   Gets the maximum position. </summary>
        /// <value> The maximum position. </value>
        public Decimal MaxPos { get; }

        /// <summary>   Gets the maximum position rounded. </summary>
        /// <value> The maximum position rounded. </value>
        public Decimal MaxPosRounded { get; }

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

        /// <summary>  Homes the given paddles. </summary>
        /// <param name="paddles"> (Optional) The paddles. </param>
        /// <returns>  True if it succeeds, false if it fails. </returns>
        public bool Home(PaddleHomeMask paddles = PaddleHomeMask.All)
        {
            if (paddles == PaddleHomeMask.None)
            {
                return false;
            }
            UpdateOutput("Homing paddles ...");
            ELLDevicePort.SendString(Address, "ho" + (char)('0' + paddles));
            if (!WaitForPositions())
            {
                return false;
            }
            return true;
        }

        #endregion

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
                    //_isThermalLocked = false;
                    UpdateDeviceStatus(deviceStatus);
                    if (getter)
                    {
                        ELLDevicePort.SendString(Address, resendCmd);
                    }
                    return StatusResult.OK;
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
        public bool GetJogstepSize()
        {
            UpdateOutput("Get Jogstep Size...");
            ELLDevicePort.SendString(Address, "gj");
            return WaitForJogstepSize("gj", true);
        }

        #endregion

        #region Positions
        private const int paddleMask = 0x3FF;  // (0b0011_1111_1111);

        private PolarizerPaddlePositions GetPositions(int encodedPositions)
        {
            decimal p1 = decimal.Round(DeviceInfo.PulseToUnit(encodedPositions & paddleMask), 3);
            decimal p2 = decimal.Round(DeviceInfo.PulseToUnit((encodedPositions >> 10) & paddleMask));
            decimal p3 = decimal.Round(DeviceInfo.PulseToUnit((encodedPositions >> 20) & paddleMask));
            return new PolarizerPaddlePositions() { Paddle1 = p1, Paddle2 = p2, Paddle3 = p3 };
        }

        private PaddlePosition GetPosition(int encodedPosition, PaddleIDs paddleId)
        {
            decimal p1 = decimal.Round(DeviceInfo.PulseToUnit(encodedPosition), 3);
            return new PaddlePosition() { PaddleId = paddleId, Position = p1 };
        }

        private int UnitToPulse(decimal value, int minVal, int maxVal)
        {
            int i = DeviceInfo.UnitToPulse(value);
            if (i < minVal || i > maxVal)
            {
                throw new OverflowException("");
            }
            return i;
        }

        private int GetEncodedPosition(PolarizerPaddlePositions position, bool signed)
        {
            int i1, i2, i3;
            if (signed)
            {
                i1 = UnitToPulse(position.Paddle1, -0x200, 0x1FF) & paddleMask;
                i2 = (UnitToPulse(position.Paddle2, -0x200, 0x1FF) & paddleMask) << 10;
                i3 = (UnitToPulse(position.Paddle3, -0x200, 0x1FF) & paddleMask) << 20;
            }
            else
            {
                i1 = UnitToPulse(position.Paddle1, 0, 0x3FF) & paddleMask;
                i2 = (UnitToPulse(position.Paddle2, 0, 0x3FF) & paddleMask) << 10;
                i3 = (UnitToPulse(position.Paddle3, 0, 0x3FF) & paddleMask) << 20;
            }
            return (i1 | i2) | i3;
        }

        public bool RequestPositions()
        {
            UpdateOutput("Get Positions...");
            ELLDevicePort.SendString(Address, "gp");
            return WaitForPositions();
        }

        public bool RequestPosition(PaddleIDs paddleID)
        {
            UpdateOutput($"Get Position {(char)paddleID}...");
            ELLDevicePort.SendString(Address, $"p{(char)paddleID}");
            return WaitForPosition(paddleID);
        }
        #endregion

        #region Movement
        public bool MoveAbsolute(PolarizerPaddlePositions positions)
        {
            try
            {
                int encodedPosition = GetEncodedPosition(positions, false);
                UpdateOutput(string.Format("Move paddles to {0},{1},{2}...", positions.Paddle1, positions.Paddle2, positions.Paddle3));
                ELLDevicePort.SendStringI32(Address, "ma", encodedPosition);
                return WaitForPositions();
            }
            catch (OverflowException)
            {
                UpdateOutput(string.Format("Parameter out of range {0},{1},{2}...", positions.Paddle1, positions.Paddle2, positions.Paddle3));
            }
            return false;
        }

        public bool MoveAbsolute(PaddlePosition position)
        {
            try
            {
                short pulses = (short)(UnitToPulse(position.Position, 0, 0x3FF));
                UpdateOutput(string.Format("Move paddle {0} to {1}...", (char)position.PaddleId, position.PaddleId));
                ELLDevicePort.SendStringI16(Address, $"a{(char)position.PaddleId}", pulses);
                return WaitForPosition(position.PaddleId);
            }
            catch (OverflowException)
            {
                UpdateOutput(string.Format("Parameter out of range {0}...", position.Position));
            }
            return false;
        }

        /// <summary>   Move microsecond steps. </summary>
        /// <param name="paddleID"> Identifier for the paddle. </param>
        /// <param name="timeMS">   The time in milliseconds. </param>
        /// <param name="backward"> True to backward. </param>
        /// <returns>   True if it succeeds, false if it fails. </returns>
        public bool MoveMicrosecondSteps(PaddleIDs paddleID, short timeMS, bool backward)
        {
            try
            {
                if (timeMS < 0 || timeMS > 8000)
                {
                    throw new ArgumentOutOfRangeException($"Range 0 to 8000");
                }
                if (backward)
                {
                    timeMS = (short)((timeMS & 0x1FFF) | 0x8000);
                }
                UpdateOutput(string.Format("Move paddle {0} for {1} ms...", (char)paddleID, timeMS));
                ELLDevicePort.SendStringI16(Address, $"t{(char)paddleID}", timeMS);
                return WaitForPosition(paddleID);
            }
            catch (OverflowException)
            {
                UpdateOutput(string.Format("Parameter out of range {0}...", timeMS));
            }
            return false;
        }

        public bool MoveRelative(PolarizerPaddlePositions displacements)
        {
            try
            {
                int encodedPosition = GetEncodedPosition(displacements, true);
                UpdateOutput(string.Format("Move paddles by {0},{1},{2}...", displacements.Paddle1, displacements.Paddle2, displacements.Paddle3));
                ELLDevicePort.SendStringI32(Address, "mr", encodedPosition);
                return WaitForPositions();
            }
            catch (OverflowException)
            {
                UpdateOutput(string.Format("Parameter out of range {0},{1},{2}...", displacements.Paddle1, displacements.Paddle2, displacements.Paddle3));
            }
            return false;
        }

        public bool MoveRelative(PaddlePosition displacement)
        {
            try
            {
                short pulses = (short)(UnitToPulse(displacement.Position, -0x3FF, 0x3FF));
                UpdateOutput(string.Format("Move paddle {0} by {1}...", (char)displacement.PaddleId, displacement.PaddleId));
                ELLDevicePort.SendStringI16(Address, $"r{(char)displacement.PaddleId}", pulses);
                return WaitForPosition(displacement.PaddleId);
            }
            catch (OverflowException)
            {
                UpdateOutput(string.Format("Parameter out of range {0}...", displacement.Position));
            }
            return false;
        }
        #endregion

        #region Wait for completion methods
        private bool WaitForPositions(int msTimeout = MoveTimeout)
        {
            try
            {
                List<string> responses = new List<string> { "GS", "PO" };
                int counter = 10;
                while (true)
                {
                    string msg = ELLDevicePort.WaitForResponse(Address, responses, msTimeout);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        bool returnValue;
                        if (TestStatus(msg, "gp", true, ref counter, out returnValue))
                        {
                            return returnValue;
                        }
                        if (msg.Substring(1, 2) == "PO")
                        {
                            if (msg.Length != 11)
                            {
                                return false;
                            }
                            PolarizerPaddlePositions polarizerPaddlePositions = GetPositions(msg.Substring(3).ToBytes(8).ToInt(true));
                            UpdateParameter(MessageUpdater.UpdateTypes.PolarizerPositions, Address, polarizerPaddlePositions);
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

        private bool WaitForPosition(PaddleIDs paddleID, int msTimeout = MoveTimeout)
        {
            try
            {
                string command = $"p{(char)paddleID}";
                string response = $"P{(char)paddleID}";
                List<string> responses = new List<string> { "GS", response };
                int counter = 10;
                while (true)
                {
                    string msg = ELLDevicePort.WaitForResponse(Address, responses, msTimeout);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        bool returnValue;
                        if (TestStatus(msg, command, true, ref counter, out returnValue))
                        {
                            return returnValue;
                        }
                        if (msg.Substring(1, 2) == response)
                        {
                            if (msg.Length != 7)
                            {
                                return false;
                            }
                            PaddlePosition position = GetPosition(msg.Substring(3).ToBytes(4).ToInt(true), paddleID);
                            UpdateParameter(MessageUpdater.UpdateTypes.PaddlePosition, Address, position);
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

        private bool WaitForHomeOffset(string cmd, bool getter, int msTimeout = HomeTimeout)
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
