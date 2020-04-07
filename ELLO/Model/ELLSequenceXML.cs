using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Thorlabs.Elliptec.ELLO_DLL;

// namespace: Thorlabs.Elliptec.ELLO.Model
//
// summary:	A Collection classes to drive the Elliptec Devices
namespace Thorlabs.Elliptec.ELLO.Model
{
	[Serializable]
	[XmlType("Sequence")]
	public class ELLSequenceXML
	{
		/// <summary> Default constructor. </summary>
		public ELLSequenceXML()
		{
			Items = new List<ELLSequenceItemXML>();
			Version = 3;
			RepeatCount = 10;
			RepeatRun = false;
			RepeatContinuously = false;
		}

		/// <summary> Gets or sets the number of repeats. </summary>
		/// <value> The number of repeats. </value>
		[XmlAttribute, DefaultValue(10)]
		public int RepeatCount { get; set; }

		/// <summary> Gets or sets a value indicating whether the run continuously. </summary>
		/// <value> <c>true</c> if run continuously. </value>
		[XmlAttribute, DefaultValue(false)]
		public bool RepeatContinuously { get; set; }

		/// <summary> Gets or sets a value indicating whether the repeat run. </summary>
		/// <value> <c>true</c> if repeat run. </value>
		[XmlAttribute, DefaultValue(false)]
		public bool RepeatRun { get; set; }

		/// <summary> Gets a collection of sequence events. </summary>
		/// <value> A Collection of sequence events. </value>
		public List<ELLSequenceItemXML> Items { get; }

		/// <summary> Gets or sets the version. </summary>
		/// <value> The version. </value>
		[XmlAttribute, DefaultValue(3)]
		public int Version { get; set; }

		/// <summary> Saves the given sequence. </summary>
		/// <param name="sequence"> The sequence. </param>
		/// <param name="filename"> The filepath. </param>
		public static void Save(ELLSequenceXML sequence, string filename)
		{
			try
			{
				using (Stream stream = File.Open(filename, FileMode.Create, FileAccess.ReadWrite))
				{
					XmlSerializer serializer = new XmlSerializer(typeof (ELLSequenceXML));
					serializer.Serialize(stream, sequence);
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Failed to save file", "File save error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary> Loads the given file. </summary>
		/// <param name="filename"> The filepath. </param>
		/// <returns> . </returns>
		public static ELLSequenceXML Load(string filename)
		{
			ELLSequenceXML xmlSequence;
			try
			{
				using (Stream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ELLSequenceXML));
					xmlSequence = (ELLSequenceXML)serializer.Deserialize(stream);
				}

			}
			catch (Exception)
			{
				MessageBox.Show("Failed to load file", "File load error", MessageBoxButton.OK, MessageBoxImage.Error);
				return new ELLSequenceXML();
			}
			foreach (ELLSequenceItemXML ellSequenceItemXml in xmlSequence.Items)
			{
				if (ellSequenceItemXml.Addresses == null || ellSequenceItemXml.Addresses.Count == 0)
				{
					ellSequenceItemXml.Addresses = new List<char> {ellSequenceItemXml.Address};
				}
			}
			return xmlSequence;
		}
	}

	/// <summary> Elliptec sequence item xml. </summary>
	[Serializable]
	[XmlType("Item")]
	public class ELLSequenceItemXML
	{
		/// <summary> Default constructor. </summary>
		public ELLSequenceItemXML()
		{
			WaitTime = new TimeSpan(0, 0, 1);
			Parameter1 = 0;
			Parameter2 = ELLBaseDevice.DeviceDirection.Linear;
			Command = ELLDeviceSequence.ELLCommands.GetPosition;
			Address = '0';
			Addresses = new List<char>();
		}

		/// <summary> Gets or sets the command. </summary>
		/// <value> The command. </value>
		public ELLDeviceSequence.ELLCommands Command { get; set; }

		/// <summary> Gets or sets the duration of the event. </summary>
		/// <value> The event duration. </value>
		[XmlIgnore]
		public TimeSpan WaitTime { get; set; }

		/// <summary>	Gets or sets the duration string. </summary>
		/// <value>	The duration string. </value>
		[Browsable(false)]
		[XmlElement("WaitTime")]
		public string WaitTimeString
		{
			get { return XmlConvert.ToString(WaitTime); }
			set { WaitTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value); }
		}

		/// <summary> Gets or sets the address. </summary>
		/// <value> The address. </value>
		public char Address { get; }

		/// <summary> Gets or sets the addresses. </summary>
		/// <value> The addresses. </value>
		[OptionalField(VersionAdded = 3)]
		public List<char> Addresses;

		/// <summary> Gets or sets the decimal parameter. </summary>
		/// <value> The parameter. </value>
		public decimal Parameter1 { get; set; }

		/// <summary> Gets or sets the direction parameter. </summary>
		/// <value> The parameter. </value>
		public ELLBaseDevice.DeviceDirection Parameter2 { get; set; }
	}
}
