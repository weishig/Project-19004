using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Thorlabs.Elliptec.ELLO.Views
{
    /// <summary>
    /// Interaction logic for ELLPaddlePolariserView.xaml
    /// </summary>
    public partial class ELLPaddlePolariserView : UserControl
    {
        public ELLPaddlePolariserView()
        {
            InitializeComponent();
        }
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
