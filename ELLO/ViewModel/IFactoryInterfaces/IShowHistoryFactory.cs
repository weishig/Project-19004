// file:	ViewModel\IFactoryInterfaces\IShowHistoryFactory.cs
//
// summary:	Declares the IShowHistoryFactory interface
namespace Thorlabs.Elliptec.ELLO.ViewModel.IFactoryInterfaces
{
	/// <summary>Show History factory interface</summary>
	public interface IShowHistoryFactory
	{
		/// <summary>Shows the History</summary>
		/// <param name="historyText">The history text.</param>
		void ShowHistory(string historyText);
	}
}
