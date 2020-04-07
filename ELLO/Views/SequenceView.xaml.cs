using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

// namespace: Thorlabs.Elliptec.ELLO.Views
//
// summary:	Provides the UI for the Elliptec application
using System.Windows.Input;
using Thorlabs.Elliptec.ELLO.ViewModel;

namespace Thorlabs.Elliptec.ELLO.Views
{
	/// <summary>
	/// Interaction logic for SequenceView.xaml
	/// </summary>
	public partial class SequenceView : UserControl
	{
		/// <summary> Default constructor. </summary>
		public SequenceView()
		{
			InitializeComponent();
		}

		private ELLSequenceItemViewModel GetViewModel()
		{
			return DataContext as ELLSequenceItemViewModel;
		}
	}
}
