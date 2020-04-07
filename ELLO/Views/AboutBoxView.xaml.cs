using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Thorlabs.Elliptec.ELLO.ViewModel;

// namespace: Thorlabs.Elliptec.ELLO.Views
//
// summary:	Provides the UI for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.Views
{
    /// <summary>
    /// Interaction logic for AboutBoxView.xaml
    /// </summary>
    public partial class AboutBoxView
    {
        /// <summary>
        /// Default constructor is protected so callers must use one with a parent.
        /// </summary>
        public AboutBoxView(Window parent, AboutBoxViewModel aboutBoxViewModel)
        {
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				InitializeComponent();
				return;
			}
			try
			{
				InitializeComponent();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}

			DataContext = aboutBoxViewModel;
            Owner = parent;
			aboutBoxViewModel.URLResource = TryFindResource("aboutProvider");
        }
		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}
			AboutBoxViewModel aboutBoxViewModel = DataContext as AboutBoxViewModel;
			if (aboutBoxViewModel != null)
			{
				aboutBoxViewModel.URLResource = TryFindResource("aboutProvider");
			}
		}

	    /// <summary>
        /// Handles click navigation on the hyperlink in the About dialog.
        /// </summary>
        /// <param name="sender">Object the sent the event.</param>
        /// <param name="e">Navigation events arguments.</param>
        private void HyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri != null && string.IsNullOrEmpty(e.Uri.OriginalString) == false)
            {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));
                e.Handled = true;
            }
        }

    }
}
