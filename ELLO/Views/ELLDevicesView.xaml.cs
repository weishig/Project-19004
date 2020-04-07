using System.Windows.Controls;
using System.Windows.Input;

// namespace: Thorlabs.Elliptec.ELLO.Views
//
// summary:	Provides the UI for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.Views
{
	/// <summary>
	/// Interaction logic for DisplayView.xaml
	/// </summary>
	public partial class ELLDevicesView : UserControl
	{
		/// <summary> Default constructor. </summary>
		public ELLDevicesView()
		{
			InitializeComponent();
		}

		private void Grid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				_sendBtn.Command.Execute(null);
			}
		}
	}
}
