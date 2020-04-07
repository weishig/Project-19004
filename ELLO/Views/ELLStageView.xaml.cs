using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

// namespace: Thorlabs.Elliptec.ELLO.Views
//
// summary:	Provides the UI for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.Views
{
	/// <summary>
	/// Interaction logic for ELLRotaryStageView.xaml
	/// </summary>
	public partial class ELLStageView : UserControl
	{
		/// <summary> Default constructor. </summary>
		public ELLStageView()
		{
			InitializeComponent();
		}
		/// <summary>
		/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				string DPchars = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
				Regex regex = new Regex(string.Format("^[+-]?[{0}][0-9]+$|^[+-]?[0-9]*[{0}]{{0,1}}[0-9]*$", DPchars));
			    string s = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
			    s = s.Insert(textBox.SelectionStart, e.Text);
			    e.Handled = !regex.IsMatch(s);
			}
        }
	}
}
