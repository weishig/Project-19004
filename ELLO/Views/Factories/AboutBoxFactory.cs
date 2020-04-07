using System.Windows;
using Thorlabs.Elliptec.ELLO.ViewModel;
using Thorlabs.Elliptec.ELLO.ViewModel.IFactoryInterfaces;

// namespace: Thorlabs.Elliptec.ELLO.Views.Factories
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.Views.Factories
{
	/// <summary>AboutBox factory</summary>
	public class AboutBoxFactory : IAboutBoxFactory
	{
		private readonly Window _window;

		/// <summary>Initializes a new instance of the <see cref="AboutBoxFactory"/> class.</summary>
		/// <param name="window">The window.</param>
		public AboutBoxFactory(Window window)
		{
			_window = window;
		}
		/// <summary>Shows the about box.</summary>
		public void ShowAboutBox()
		{
			AboutBoxViewModel aboutBoxViewModel = new AboutBoxViewModel();
			AboutBoxView about = new AboutBoxView(_window, aboutBoxViewModel);
			about.ShowDialog();
		}
	}
}
