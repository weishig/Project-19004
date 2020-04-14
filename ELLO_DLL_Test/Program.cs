using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thorlabs.Elliptec.ELLO_DLL;

/// ///////////dafasda
/// </summary>

namespace ELLO_DLL_Test
{
	/// <summary>	Main test program. </summary>
	public class Program
	{
		/// <summary>	Main entry-point for this application. </summary>
		/// <param name="args">	Array of command-line argument strings. [port] [minAddress] [maxAddress] </param>
		static void Main(string[] args)
		{

			
			//string port = (args.Length > 0) ? args[0] : "COM7";

			// get the range of addresses used max range is '0' to 'F'

			//char _minSearchLimit = (args.Length > 1 && ELLBaseDevice.IsValidAddress(char.ToUpper(args[1][0]))) ? char.ToUpper(args[1][0]) : '0';
			//char _maxSearchLimit = (args.Length > 2 && ELLBaseDevice.IsValidAddress(char.ToUpper(args[2][0]))) ? char.ToUpper(args[2][0]) : '1';

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
	
			Application.Run(new Form(args));
			//Console.



			// get the communication port
			
			// create ELLDevices class to maintain the collection of Elliptec devices
			//ELLDevices ellDevices = new ELLDevices();
			/*

			if (ELLDevicePort.Connect(port))
			{
				Console.WriteLine("Discover devices");
				Console.WriteLine("================");
				// scan the port for connected devices using the given range of addresses
				List<string> devices = ellDevices.ScanAddresses(_minSearchLimit, _maxSearchLimit);
				foreach (string device in devices)
				{
					// configure each device found
					if (ellDevices.Configure(device))
					{
						Console.WriteLine("");
						Console.WriteLine("Identify device " + device[0]);
						Console.WriteLine("=================");

						//get device at address 0
						ELLDevice addressedDevice1 = ellDevices.AddressedDevice(device[0]) as ELLDevice;

						if (addressedDevice1 != null)
						{
							// display the device information
							DeviceID deviceInfo = addressedDevice1.DeviceInfo;
							foreach (string str in deviceInfo.Description())
							{
								Console.WriteLine(str);
							}

							//addressedDevice1.SetJogstepSize(90);

							//addressedDevice1.JogForward();

							//addressedDevice1.Home(ELLBaseDevice.DeviceDirection.Linear);




						}
					}
					}
					*/

			/*
						// setup handler to display Transmitted data
						ELLDevicePort.DataSent += delegate(string str)
						{
							Console.ForegroundColor = ConsoleColor.Cyan;
							Console.WriteLine("Tx: " + str);///////////////////////
							Console.ResetColor();
						};
						// setup handler to display Received data
						ELLDevicePort.DataReceived += delegate(string str)
						{
							Console.ForegroundColor = ConsoleColor.Green;
							Console.WriteLine("Rx: " + str);
							Console.ResetColor();
						};

						// create ELLDevices class to maintain the collection of Elliptec devices
						ELLDevices ellDevices = new ELLDevices();
						// setup handler to process device update messages
			/*
			ellDevices.MessageUpdates.OutputUpdate += delegate(List<string> str, bool error)
					{
						//Console.ForegroundColor = ConsoleColor.Yellow;
						foreach (string s in str)
						{
							Console.WriteLine("Output->" + s);
						}
						//Console.ResetColor();
					};

			// attempt to connect to the port
			if (ELLDevicePort.Connect(port))
{
	Console.WriteLine("Discover devices");
	Console.WriteLine("================");
	// scan the port for connected devices using the given range of addresses
	List<string> devices = ellDevices.ScanAddresses(_minSearchLimit, _maxSearchLimit);

	foreach (string device in devices)
	{
		// configure each device found
		if (ellDevices.Configure(device))
		{
			// test each device found
			Console.WriteLine("");
			Console.WriteLine("Identify device " + device[0]);
			Console.WriteLine("=================");
			ELLDevice addressedDevice1 = ellDevices.AddressedDevice(device[0]) as ELLDevice;
			ELLDevice addressedDevice2 = ellDevices.AddressedDevice(device[1]) as ELLDevice;
			ELLDevice addressedDevice3 = ellDevices.AddressedDevice(device[2]) as ELLDevice;
			ELLDevice addressedDevice4 = ellDevices.AddressedDevice(device[3]) as ELLDevice;


			if (addressedDevice1 != null)
			{
				// display the device information
				DeviceID deviceInfo = addressedDevice1.DeviceInfo;
				foreach (string str in deviceInfo.Description())
				{
					Console.WriteLine(str);
				}

				// test each device according to type
				// NOTE only a shutter and a Linear stage are shown in this example
				Console.WriteLine("");
				Console.WriteLine("Test device " + device[0]);
				Console.WriteLine("=============");

				addressedDevice1.SetJogstepSize(10);
				addressedDevice1.JogBackward();
				addressedDevice1.JogBackward();
				addressedDevice1.JogBackward(); 
				addressedDevice1.JogBackward();


				//addressedDevice2.SetJogstepSize(90);
				//addressedDevice2.JogForward();
				//addressedDevice3.SetJogstepSize(90);
				//addressedDevice3.JogForward();
				//addressedDevice4.SetJogstepSize(90);
				//addressedDevice4.JogForward();

				switch (deviceInfo.DeviceType)
				{
					case DeviceID.DeviceTypes.Shutter:
						// test the shutter device
						addressedDevice1.Home(ELLBaseDevice.DeviceDirection.Linear);
						Thread.Sleep(250);
						addressedDevice1.JogForward();
						Thread.Sleep(250);
						addressedDevice1.JogBackward();
						Thread.Sleep(250);
						break;
					case DeviceID.DeviceTypes.Shutter4:
						// test the shutter device
						addressedDevice1.Home(ELLBaseDevice.DeviceDirection.Linear);
						Thread.Sleep(250);
						addressedDevice1.JogForward();
						Thread.Sleep(250);
						addressedDevice1.JogForward();
						Thread.Sleep(250);
						addressedDevice1.JogForward();
						Thread.Sleep(250);
						addressedDevice1.JogBackward();
						Thread.Sleep(250);
						addressedDevice1.JogBackward();
						Thread.Sleep(250);
						addressedDevice1.JogBackward();
						Thread.Sleep(250);
						break;
					case DeviceID.DeviceTypes.LinearStage:
					case DeviceID.DeviceTypes.LinearStage2:
					case DeviceID.DeviceTypes.LinearStage17:
					case DeviceID.DeviceTypes.LinearStage20:
						// Test the Linear stage

						// for each motor ('1' and '2' get the motor information
						for (char c = '1'; c <= '2'; c++)
						{
							if (addressedDevice1.GetMotorInfo(c))
							{
								MotorInfo motorInfo1 = addressedDevice1[c];
								foreach (string s in motorInfo1.Description())
								{
									Console.WriteLine("Output->" + s);
								}
							}
						}

						// Test the stage movement
						addressedDevice1.Home(ELLBaseDevice.DeviceDirection.Linear);
						Thread.Sleep(250);
						addressedDevice1.SetJogstepSize(1.0m);
						for (int i = 0; i < 10; i++)
						{
							addressedDevice1.JogForward();
							Thread.Sleep(100);
						}
						break;
					default:
						break;
				}
			}
		}
	}

	ELLDevicePort.Disconnect();
}
else
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("Port {0} unavailable", port);
	Console.ResetColor();
}
Console.WriteLine("Press any key to exit");
//Console.ReadKey(true);
*/
		}

	}
}
