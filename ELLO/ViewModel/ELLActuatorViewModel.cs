using Thorlabs.Elliptec.ELLO_DLL;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
	/// <summary> Elliptec actuator view model. </summary>
	/// <seealso cref="T:Thorlabs.Elliptec.ELLO.ViewModel.ELLMotorBaseViewModel"/>
	public class ELLActuatorViewModel : ELLDeviceBaseViewModel
	{

		/// <summary> Constructor. </summary>
		/// <param name="owner">   The owner. </param>
		/// <param name="device">  The device. </param>
		public ELLActuatorViewModel(ELLDevicesViewModel owner, ELLDevice device)
			: base(owner, "Actuator", device, 1)
		{
		}
	}
}
