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
// code version 1    15.4.2020 update at 1pm

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


       // public double xPos;
        //public double yPos;
        //public double distance;
        

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
                        if (addressedDevice1 != null)
                        {
                            addressedDevice1.Home();
                            decimal val = addressedDevice1.Position;
                            decimal roundedVal = Math.Round(val, 3);
                            RotationMount1CurrentPosition.Text = roundedVal.ToString() + "deg";
                                                 
                            OutputWindowString = OutputWindowString + "Device 1 is connected sucessfully\n";
                            OutputWindowString = OutputWindowString + "Device 1 is homed\n\n";
                            PrintOutBox.Text = OutputWindowString;
                        }
                        else {
                            OutputWindowString = OutputWindowString + "Device 1 is not connected\n";
                            PrintOutBox.Text = OutputWindowString;
                        }

                        if (addressedDevice2 != null)
                        {
                            addressedDevice2.Home();
                            decimal val = addressedDevice2.Position;
                            decimal roundedVal = Math.Round(val, 3);
                            RotationMount2CurrentPosition.Text = roundedVal.ToString() + "deg";
                            OutputWindowString = OutputWindowString + "Device 2 is connected sucessfully\n";
                            OutputWindowString = OutputWindowString + "Device 2 is homed\n\n";
                            PrintOutBox.Text = OutputWindowString;
                        }
                        else
                        {
                            OutputWindowString = OutputWindowString + "Device 2 is not connected\n";
                            PrintOutBox.Text = OutputWindowString;
                        }

                        if (addressedDevice3 != null)
                        {
                            addressedDevice3.Home();
                            decimal val = addressedDevice3.Position;
                            decimal roundedVal = Math.Round(val, 3);
                            RotationMount3CurrentPosition.Text = roundedVal.ToString() + "deg";
                            OutputWindowString = OutputWindowString + "Device 3 is connected sucessfully\n";
                            OutputWindowString = OutputWindowString + "Device 3 is homed\n\n";
                            PrintOutBox.Text = OutputWindowString;
                        }
                        else
                        {
                            OutputWindowString = OutputWindowString + "Device 3 is not connected\n";
                            PrintOutBox.Text = OutputWindowString;
                        }

                        if (addressedDevice4 != null)
                        {
                            addressedDevice4.Home();
                            decimal val = addressedDevice4.Position;
                            decimal roundedVal = Math.Round(val, 3);
                            RotationMount4CurrentPosition.Text = roundedVal.ToString() + "deg";
                            OutputWindowString = OutputWindowString + "Device 4 is connected sucessfully\n";
                            OutputWindowString = OutputWindowString + "Device 4 is homed\n\n";
                            PrintOutBox.Text = OutputWindowString;
                        }
                        else
                        {
                            OutputWindowString = OutputWindowString + "Device 4 is not connected\n";
                            PrintOutBox.Text = OutputWindowString;
                        }


                    }
                 }
             }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

// device 1 control///////////////////////////////////////////////////////////////////////////////////////////////////
       
        private void RotationMount1HomeButton_Click(object sender, EventArgs e)
        {
            OutputWindowString = OutputWindowString + "Mount 1 home button is clicked\n";
            PrintOutBox.Text = OutputWindowString;
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
            addressedDevice2.MoveRelative(i);
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
        private void MoveAll_Click(object sender, EventArgs e)
        {
            string xPosStr = XPositionTextBox.Text;
            string yPosStr = YPositionTextBox.Text;
            string distanceStr = DistanceTexBox.Text;
            decimal phi = 2.215m;
            decimal ex = 1m;
            decimal ey = 1m;
            decimal thetax = 0m;
            decimal thetay = 0m;

            if (xPosStr == "" || yPosStr == "")
            {
                return;
            }


            decimal xPos;
            decimal yPos;
            decimal distance;
            var a = decimal.TryParse(xPosStr, out xPos);
            var b = decimal.TryParse(yPosStr, out yPos);
            var c = decimal.TryParse(distanceStr, out distance);
            decimal x1;
            decimal x2;
            decimal y1;
            decimal y2;
            while (ex > 0.1m)
            {
                //((double)0 + 90)
                //double l;
                //l= Math.Cos((((double)0 + 90) * (Math.PI)) / 180);
                decimal x1_cos = (decimal)Math.Cos(((90 + (double)thetax) * (Math.PI)) / 180);
                x1 = x1_cos * phi;
                x1 = Math.Round(x1, 5);
                decimal x2_cos = (decimal)Math.Cos(((270 - (double)thetax) * (Math.PI)) / 180);
                x2 = x2_cos * phi;
                x2 = Math.Round(x2, 5);
                if (xPos > 0m)
                {
                    thetax = thetax - 0.01m;

                }
                else if (xPos < 0m)
                {
                    thetax = thetax + 0.01m;

                }
                else if (xPos == 0m)
                {
                    thetax = thetax;
                    break;

                }
                ex = Math.Abs(xPos - 60m * (x1 + x2));

            }


            while (ey > 0.1m)
            {
                //()
                decimal y1_cos = (decimal)Math.Sin(((0 + (double)thetay) * (Math.PI)) / 180);
                y1 = y1_cos * phi;
                y1 = Math.Round(y1, 5);
                decimal y2_cos = (decimal)Math.Sin(((180 - (double)thetay) * (Math.PI)) / 180);
                y2 = y2_cos * phi;
                y2 = Math.Round(y2, 5);

                if (yPos > 0m)
                {
                    thetay = thetay + 0.01m;

                }
                else if (yPos < 0m)
                {
                    thetay = thetay - 0.01m;

                }
                else if (yPos == 0m)
                {
                    thetay = thetay;
                    break;
                }
                ey = Math.Abs(yPos - 60m * (y1 + y2));

            }
            decimal mount1Rotate;
            decimal mount2Rotate;
            decimal mount3Rotate;
            decimal mount4Rotate;
            if (thetax < 0)
            {
                mount1Rotate = -thetax;
                mount2Rotate =thetax;
                
            }
            else
            {
                mount1Rotate = thetax;
                mount2Rotate = -thetax;
                
            }

            if (thetay < 0)
            {
                mount3Rotate = -thetay;
                mount4Rotate = thetay;
            }
            else
            {
                mount3Rotate = thetay;
                mount4Rotate = -thetay;
            }

            OutputWindowString = OutputWindowString + "prism 1 should rotate " + mount1Rotate + "\n";
            OutputWindowString = OutputWindowString + "prism 2 should rotate " + mount2Rotate + "\n";           
            OutputWindowString = OutputWindowString + "prism 3 should rotate " + mount3Rotate + "\n";
            OutputWindowString = OutputWindowString + "prism 4 should rotate " + mount4Rotate + "\n";

            PrintOutBox.Text = OutputWindowString;
            
                        addressedDevice1.MoveAbsolute(mount1Rotate);
                        addressedDevice2.MoveAbsolute(mount2Rotate);
                        addressedDevice3.MoveAbsolute(mount3Rotate);
                        addressedDevice4.MoveAbsolute(mount4Rotate);
                        
            XPositionTextBox.Text = "";
            YPositionTextBox.Text = "";
            DistanceTexBox.Text = "";
            
            decimal val1 = addressedDevice1.Position;
            decimal roundedVal1 = Math.Round(val1, 3);
            RotationMount1CurrentPosition.Text = roundedVal1.ToString() + "deg";

            decimal val2 = addressedDevice2.Position;
            decimal roundedVal2 = Math.Round(val2, 3);
            RotationMount2CurrentPosition.Text = roundedVal2.ToString() + "deg";
            decimal val3 = addressedDevice3.Position;
            decimal roundedVal3 = Math.Round(val3, 3);
            RotationMount3CurrentPosition.Text = roundedVal3.ToString() + "deg";
            decimal val4 = addressedDevice4.Position;
            decimal roundedVal4 = Math.Round(val4, 3);
            RotationMount4CurrentPosition.Text = roundedVal4.ToString() + "deg";
          


            return;
        }
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
