using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Thorlabs.Elliptec.ELLO.ViewModel;

namespace Thorlabs.Elliptec.ELLO.Views
{
	/// <summary>
	/// Interaction logic for SequenceViewItem.xaml
	/// </summary>
	public partial class SequenceItemView : UserControl
	{
		public SequenceItemView()
		{
			InitializeComponent();
		}
		private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
		private void WaitTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				bool allowZero = false;
				ELLSequenceItemViewModel vm = GetViewModel();
				if (vm != null)
				{
					allowZero = vm.AllowZeroWait;
				}
				string DPchars = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
				Regex regex = allowZero ? new Regex(string.Format("^[0-9][0-9]*[{0}]{{0,1}}[0-9]*$", DPchars)) : new Regex(string.Format("^[1-9][0-9]*[{0}]{{0,1}}[0-9]*$", DPchars));
			    string s = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
			    s = s.Insert(textBox.SelectionStart, e.Text);
			    e.Handled = !regex.IsMatch(s);
			}
        }

		private ELLSequenceItemViewModel GetViewModel()
		{
			return DataContext as ELLSequenceItemViewModel;
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox box = sender as ComboBox;

			if (box == null)
			{
				return;
			}

			if (box.SelectedItem != null)
			{
				box.SelectedItem = null;

				EventHandler layoutUpdated = null;

				layoutUpdated = new EventHandler((o, ev) =>
				{
					box.GetBindingExpression(ComboBox.TextProperty).UpdateTarget();
					box.LayoutUpdated -= layoutUpdated;
				});

				box.LayoutUpdated += layoutUpdated;
			}
		}

	}
}
