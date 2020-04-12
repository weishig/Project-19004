using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thorlabs.Elliptec.ELLO_DLL;
using ELLO_DLL_Test;
namespace ELLO_DLL_Test
{


    public partial class Form : System.Windows.Forms.Form
    {
        public string port;
        public char _minSearchLimit;
        public char _maxSearchLimit;
        public ELLDevices ellDevices;
        public List<string> devices;
        public ELLDevice addressedDevice1;
        public ELLDevice addressedDevice2;
        public ELLDevice addressedDevice3;
        public ELLDevice addressedDevice4;

        public string OutputWindowString;

        public Form(string[] args)
        {
            InitializeComponent();
            
            port = (args.Length > 0) ? args[0] : "COM3";
            _minSearchLimit = (args.Length > 1 && ELLBaseDevice.IsValidAddress(char.ToUpper(args[1][0]))) ? char.ToUpper(args[1][0]) : '0';
            _maxSearchLimit = (args.Length > 2 && ELLBaseDevice.IsValidAddress(char.ToUpper(args[2][0]))) ? char.ToUpper(args[2][0]) : '3';
            ellDevices = new ELLDevices();

            

            if (ELLDevicePort.Connect(port))
            {
                // PrintOutBox.Text = "Ide";
                devices = ellDevices.ScanAddresses(_minSearchLimit, _maxSearchLimit);
                foreach (string device in devices)
                {
                
                // configure each device found
                    if (ellDevices.Configure(device))
                    {
                      addressedDevice1 = ellDevices.AddressedDevice(device[0]) as ELLDevice;
                        addressedDevice2 = ellDevices.AddressedDevice(device[1]) as ELLDevice;
                        addressedDevice3 = ellDevices.AddressedDevice(device[2]) as ELLDevice;
                        addressedDevice4 = ellDevices.AddressedDevice(device[3]) as ELLDevice;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OutputWindowString = OutputWindowString + "Devices are found\n";
            PrintOutBox.Text = OutputWindowString;
            if (addressedDevice1 != null)
            {
                addressedDevice1.Home();
                decimal val = addressedDevice1.Position;
                decimal roundedVal = Math.Round(val, 3);
                RotationMount1CurrentPosition.Text = roundedVal.ToString() + "deg";

                OutputWindowString = OutputWindowString + "Device 1 is homed\n";
                PrintOutBox.Text = OutputWindowString;

            }
            if (addressedDevice2 != null)
            {
                addressedDevice2.Home();
                decimal val = addressedDevice2.Position;
                decimal roundedVal = Math.Round(val, 3);
                RotationMount2CurrentPosition.Text = roundedVal.ToString() + "deg";
                OutputWindowString = OutputWindowString + "Device 2 is homed\n";
                PrintOutBox.Text = OutputWindowString;


            }
            if (addressedDevice3 != null)
            {
                addressedDevice3.Home();
                decimal val = addressedDevice3.Position;
                decimal roundedVal = Math.Round(val, 3);
                RotationMount3CurrentPosition.Text = roundedVal.ToString() + "deg";
                OutputWindowString = OutputWindowString + "Device 3 is homed\n";
                PrintOutBox.Text = OutputWindowString;
            }
            if (addressedDevice4 != null)
            {
                addressedDevice4.Home();
                decimal val = addressedDevice4.Position;
                decimal roundedVal = Math.Round(val, 3);
                RotationMount4CurrentPosition.Text = roundedVal.ToString() + "deg";
                OutputWindowString = OutputWindowString + "Device 4 is homed\n";
                PrintOutBox.Text = OutputWindowString;
            }

        }

// device 1 control///////////////////////////////////////////////////////////////////////////////////////////////////
       
        private void RotationMount1HomeButton_Click(object sender, EventArgs e)
        {
            if (addressedDevice1 != null)
            {
                addressedDevice1.Home();
                OutputWindowString = OutputWindowString + "Device 1 is homed\n";
                PrintOutBox.Text = OutputWindowString;

            }
        }
        private void RotationMount1MoveAbsoluteButton_Click(object sender, EventArgs e)
        {
            
            string input = RotationMount1MoveAbsoluteTextBox.Text;
            if (input == "")
            {
                return;
            }
            decimal i;
          
            var s = decimal.TryParse(input, out i);
            addressedDevice1.MoveAbsolute(i);
            addressedDevice1.MoveAbsolute(i);
            addressedDevice1.MoveAbsolute(i);
            addressedDevice1.MoveAbsolute(i);
            decimal val = addressedDevice1.Position;
            decimal roundedVal = Math.Round(val, 3);
            val = addressedDevice1.Position;
            roundedVal = Math.Round(val, 3);
            RotationMount1CurrentPosition.Text = roundedVal.ToString() + "deg";

            OutputWindowString = OutputWindowString + "Device 1 move absolute by " + input + "deg\n";
            PrintOutBox.Text = OutputWindowString;
            
        }

        private void RotationMount1MoveRelativeButton_Click(object sender, EventArgs e)
        {

            string input = RotationMount1MoveRelativeTextBox.Text;
            if (input == "")
            {
                return;
            }
            decimal i;
           
            var s = decimal.TryParse(input, out i);
            addressedDevice1.MoveRelative(i);
            decimal val = addressedDevice1.Position;
            decimal roundedVal = Math.Round(val, 3);
            
            RotationMount1CurrentPosition.Text = roundedVal.ToString() + "deg";

            OutputWindowString = OutputWindowString + "Device 1 move relative by " + input + "deg\n";
            PrintOutBox.Text = OutputWindowString;
            
        }
/////////////////////////////////////////////////////////////////////////////////////////////////////////////
///// device 2 controll////////////////////////////////////////////////////////////////////////////////////////////
    
        private void RotationMount2HomeButton_Click(object sender, EventArgs e)
        {
            if (addressedDevice2 != null)
            {
                addressedDevice2.Home();
                OutputWindowString = OutputWindowString + "Device 2 is homed\n";
                PrintOutBox.Text = OutputWindowString;

            }
        }
        private void RotationMount2MoveAbsoluteButton_Click(object sender, EventArgs e)
        {

            string input = RotationMount2MoveAbsoluteTextBox.Text;
            if (input == "")
            {
                return;
            }
            decimal i;

            var s = decimal.TryParse(input, out i);
            addressedDevice2.MoveAbsolute(i);
            addressedDevice2.MoveAbsolute(i);
            addressedDevice2.MoveAbsolute(i);
            addressedDevice2.MoveAbsolute(i);
            decimal val = addressedDevice2.Position;
            decimal roundedVal = Math.Round(val, 3);
            val = addressedDevice2.Position;
            roundedVal = Math.Round(val, 3);
            RotationMount2CurrentPosition.Text = roundedVal.ToString() + "deg";

            OutputWindowString = OutputWindowString + "Device 2 move absolute by " + input + "deg\n";
            PrintOutBox.Text = OutputWindowString;

        }

        private void RotationMount2MoveRelativeButton_Click(object sender, EventArgs e)
        {

            string input = RotationMount2MoveRelativeTextBox.Text;
            if (input == "")
            {
                return;
            }
            decimal i;

            var s = decimal.TryParse(input, out i);
            addressedDevice1.MoveRelative(i);
            decimal val = addressedDevice2.Position;
            decimal roundedVal = Math.Round(val, 3);

            RotationMount2CurrentPosition.Text = roundedVal.ToString() + "deg";

            OutputWindowString = OutputWindowString + "Device 2 move relative by " + input + "deg\n";
            PrintOutBox.Text = OutputWindowString;

        }


// device 1 controll/////////////////////////////////////////////////////////////////////////////////////
       
        private void RotationMount3HomeButton_Click(object sender, EventArgs e)
        {
            if (addressedDevice3 != null)
            {
                addressedDevice3.Home();
                OutputWindowString = OutputWindowString + "Device 3 is homed\n";
                PrintOutBox.Text = OutputWindowString;

            }
        }
        private void RotationMount3MoveAbsoluteButton_Click(object sender, EventArgs e)
        {

            string input = RotationMount3MoveAbsoluteTextBox.Text;
            if (input == "")
            {
                return;
            }
            decimal i;

            var s = decimal.TryParse(input, out i);
            addressedDevice3.MoveAbsolute(i);
            addressedDevice3.MoveAbsolute(i);
            addressedDevice3.MoveAbsolute(i);
            addressedDevice3.MoveAbsolute(i);
            decimal val = addressedDevice3.Position;
            decimal roundedVal = Math.Round(val, 3);
            val = addressedDevice3.Position;
            roundedVal = Math.Round(val, 3);
            RotationMount3CurrentPosition.Text = roundedVal.ToString() + "deg";

            OutputWindowString = OutputWindowString + "Device 3 move absolute by " + input + "deg\n";
            PrintOutBox.Text = OutputWindowString;

        }

        private void RotationMount3MoveRelativeButton_Click(object sender, EventArgs e)
        {

            string input = RotationMount3MoveRelativeTextBox.Text;
            if (input == "")
            {
                return;
            }
            decimal i;

            var s = decimal.TryParse(input, out i);
            addressedDevice3.MoveRelative(i);
            decimal val = addressedDevice3.Position;
            decimal roundedVal = Math.Round(val, 3);

            RotationMount3CurrentPosition.Text = roundedVal.ToString() + "deg";

            OutputWindowString = OutputWindowString + "Device 3 move relative by " + input + "deg\n";
            PrintOutBox.Text = OutputWindowString;

        }

 // device 4////////// controll///////////////////////////////////////////////////////////////////////////////////////
      
        private void RotationMount4HomeButton_Click(object sender, EventArgs e)
        {
            if (addressedDevice4 != null)
            {
                addressedDevice4.Home();
                OutputWindowString = OutputWindowString + "Device 4 is homed\n";
                PrintOutBox.Text = OutputWindowString;

            }
        }
        private void RotationMount4MoveAbsoluteButton_Click(object sender, EventArgs e)
        {

            string input = RotationMount4MoveAbsoluteTextBox.Text;
            if (input == "")
            {
                return;
            }
            decimal i;

            var s = decimal.TryParse(input, out i);
            addressedDevice4.MoveAbsolute(i);
            addressedDevice4.MoveAbsolute(i);
            addressedDevice4.MoveAbsolute(i);
            addressedDevice4.MoveAbsolute(i);
            decimal val = addressedDevice4.Position;
            decimal roundedVal = Math.Round(val, 3);
            val = addressedDevice4.Position;
            roundedVal = Math.Round(val, 3);
            RotationMount4CurrentPosition.Text = roundedVal.ToString() + "deg";

            OutputWindowString = OutputWindowString + "Device 4 move absolute by " + input + "deg\n";
            PrintOutBox.Text = OutputWindowString;

        }

        private void RotationMount4MoveRelativeButton_Click(object sender, EventArgs e)
        {

            string input = RotationMount4MoveRelativeTextBox.Text;
            if (input == "")
            {
                return;
            }
            decimal i;

            var s = decimal.TryParse(input, out i);
            addressedDevice4.MoveRelative(i);
            decimal val = addressedDevice4.Position;
            decimal roundedVal = Math.Round(val, 3);

            RotationMount4CurrentPosition.Text = roundedVal.ToString() + "deg";

            OutputWindowString = OutputWindowString + "Device 4 move relative by " + input + "deg\n";
            PrintOutBox.Text = OutputWindowString;

        }
        private void ClearOutputWindowButton_Click(object sender, EventArgs e)
        {

            OutputWindowString = "";     
            PrintOutBox.Text = OutputWindowString;

        }

        /////////////control all///////////////////////////////////////////////////////////////
        private void HomeAllButton_Click(object sender, EventArgs e)
        {
            if (addressedDevice1 != null)
            {
                addressedDevice1.Home();
                decimal val = addressedDevice1.Position;
                decimal roundedVal = Math.Round(val, 3);
                RotationMount1CurrentPosition.Text = roundedVal.ToString() + "deg";

                OutputWindowString = OutputWindowString + "Device 1 is homed\n";
                PrintOutBox.Text = OutputWindowString;

            }
            if (addressedDevice2 != null)
            {
                addressedDevice2.Home();
                decimal val = addressedDevice2.Position;
                decimal roundedVal = Math.Round(val, 3);
                RotationMount2CurrentPosition.Text = roundedVal.ToString() + "deg";
                OutputWindowString = OutputWindowString + "Device 2 is homed\n";
                PrintOutBox.Text = OutputWindowString;


            }
            if (addressedDevice3 != null)
            {
                addressedDevice3.Home();
                decimal val = addressedDevice3.Position;
                decimal roundedVal = Math.Round(val, 3);
                RotationMount3CurrentPosition.Text = roundedVal.ToString() + "deg";
                OutputWindowString = OutputWindowString + "Device 3 is homed\n";
                PrintOutBox.Text = OutputWindowString;
            }
            if (addressedDevice4 != null)
            {
                addressedDevice4.Home();
                decimal val = addressedDevice4.Position;
                decimal roundedVal = Math.Round(val, 3);
                RotationMount4CurrentPosition.Text = roundedVal.ToString() + "deg";
                OutputWindowString = OutputWindowString + "Device 4 is homed\n";
                PrintOutBox.Text = OutputWindowString;
            }


        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
