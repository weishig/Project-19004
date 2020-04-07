using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using Thorlabs.Elliptec.ELLO.Support;
using Thorlabs.Elliptec.ELLO_DLL;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
	/// <summary> Elliptec shutter view model. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.ViewModel.ELLMotorBaseViewModel"/>
	public class ELLShutterViewModel : ELLDeviceBaseViewModel
	{
	    private readonly int _positions;
	    private List<ButtonRelayCommandViewModel> _buttons;

	    /// <summary> The get home command. </summary>
	    private ICommand _getHomeCommand;

	    /// <summary> Gets the click home command. </summary>
	    /// <value> The click home command. </value>
	    public ICommand ClickHomeCommand { get { return _getHomeCommand ?? (_getHomeCommand = new RelayCommand(Home)); } }

        /// <summary>   Gets or sets the ell device shutter. </summary>
        /// <value> The ell device shutter. </value>
        public ELLDevice ELLDeviceShutter { get; private set; }

        /// <summary>   Constructor. </summary>
        /// <param name="owner">        The owner. </param>
        /// <param name="device">       The device. </param>
        /// <param name="positions">    The positions. </param>
        public ELLShutterViewModel(ELLDevicesViewModel owner, ELLDevice device, int positions)
			: base(owner, "Shutter", device, 1)
        {
            ELLDeviceShutter = device;
            _positions = positions;
		    if (ShowPositions)
		    {
                List<ButtonRelayCommandViewModel> buttons = new List<ButtonRelayCommandViewModel>();
		        for (int i = 0; i < _positions; i++)
		        {
		            int i1 = i;
		            buttons.Add(new ButtonRelayCommandViewModel(() => { MoveToPosition(i1); }, $"Position {i}"));
		        }
		        Buttons = buttons;
		    }
		}

        /// <summary>   Gets a value indicating whether the positions is shown. </summary>
        /// <value> True if show positions, false if not. </value>
	    public bool ShowPositions
	    {
	        get { return _positions > 2; }
	    }

	    public void MoveToPosition(int i)
	    {
	        if (Owner.IsConnected)
	        {
	            Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceShutter.MoveAbsolute(i * 32));
	        }
	    }

        /// <summary>   Gets or sets the positions. </summary>
        /// <value> The positions. </value>
	    public List<ButtonRelayCommandViewModel> Buttons
	    {
	        get { return _buttons; }
	        set
	        {
	            _buttons = value;
	            RaisePropertyChanged(() => Buttons);
	        }
	    }

	    private void Home()
	    {
	        if (Owner.IsConnected)
	        {
	            Owner.BackgroundThreadManager.RunBackgroundFunction((s, e) => ELLDeviceShutter.Home());
	        }
	    }
    }
}
