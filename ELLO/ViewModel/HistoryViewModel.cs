using Thorlabs.Elliptec.ELLO.Support;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
	/// <summary> History view model. </summary>
	/// <seealso cref="T:ELLO.Support.ObservableObject"/>
	public class HistoryViewModel : ObservableObject
	{
		private string _historyText;

		/// <summary>Initializes a new instance of the <see cref="HistoryViewModel"/> class.</summary>
		/// <param name="historyText">The history text.</param>
		public HistoryViewModel(string historyText)
		{
			_historyText = historyText;
		}

		/// <summary>Gets or sets the history text.</summary>
		/// <value>The history text.</value>
		public string HistoryText
		{
			get { return _historyText; }
			set
			{
				_historyText = value;
				RaisePropertyChanged(() => HistoryText);
			}
		}
	}
}
