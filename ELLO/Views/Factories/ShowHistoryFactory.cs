using System.Windows;
using Thorlabs.Elliptec.ELLO.ViewModel;
using Thorlabs.Elliptec.ELLO.ViewModel.IFactoryInterfaces;

// namespace: Thorlabs.Elliptec.ELLO.Views.Factories
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.Views.Factories
{
	/// <summary>Show History Factory</summary>
	public class ShowHistoryFactory : IShowHistoryFactory
	{
		private readonly Window _window;

		/// <summary>Initializes a new instance of the <see cref="ShowHistoryFactory"/> class.</summary>
		/// <param name="window">The window.</param>
		public ShowHistoryFactory(Window window)
		{
			_window = window;
		}
		/// <summary>Shows the History</summary>
		/// <param name="historyText">The history text.</param>
		public void ShowHistory(string historyText)
		{
			var historyViewModel = new HistoryViewModel(historyText);
			var historyView = new HistoryView(_window, historyViewModel);
			historyView.ShowDialog();
		}
	}
}
