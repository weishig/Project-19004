using System.ComponentModel;

// namespace: Thorlabs.Elliptec.ELLO.Support
//
// summary:	A Collection of support classes to provide general non elliptec functionality
namespace Thorlabs.Elliptec.ELLO.Support
{
	/// <summary> Manager for background threads. </summary>
	/// <seealso cref="T:ELLO.Support.ObservableObject"/>
	public class BackgroundThreadManager : ObservableObject
	{
		private bool _isProcessing;

		public delegate void PostWorkEventDelegate(object result);

		private BackgroundWorker BackgroundWorker { get; set; }

		private PostWorkEventDelegate PostWorkEvent { get; set; }

		/// <summary> Default constructor. </summary>
		public BackgroundThreadManager()
		{
			BackgroundWorker = new BackgroundWorker();
			BackgroundWorker.RunWorkerCompleted += ProcessingComplete;
			BackgroundWorker.WorkerReportsProgress = false;
			BackgroundWorker.WorkerSupportsCancellation = false;
		}

		private void ProcessingComplete(object sender, RunWorkerCompletedEventArgs e)
		{
			BackgroundWorker.DoWork -= _lastTask;
			IsProcessing = false;
			if (PostWorkEvent != null)
			{
				PostWorkEvent(e.Result);
				PostWorkEvent = null;
			}
		}

		/// <summary> Gets or sets a value indicating whether this instance is processing data. </summary>
		/// <value> <c>true</c> if this instance is processing; otherwise, <c>false</c>. </value>
		public bool IsProcessing
		{
			get { return _isProcessing; }
			set
			{
				_isProcessing = value;
				RaisePropertyChanged(() => IsProcessing);
			}
		}

		private DoWorkEventHandler _lastTask;

		/// <summary> Executes the background function operation. </summary>
		/// <param name="task">			 The task. </param>
		/// <param name="postWorkEvent"> The post work event. </param>
		public void RunBackgroundFunction(DoWorkEventHandler task, PostWorkEventDelegate postWorkEvent = null)
		{
			if (IsProcessing)
			{
				return;
			}
			_lastTask = task;
			PostWorkEvent = postWorkEvent;
			IsProcessing = true;
			BackgroundWorker.DoWork += task;
			BackgroundWorker.RunWorkerAsync();
		}
	}
}
