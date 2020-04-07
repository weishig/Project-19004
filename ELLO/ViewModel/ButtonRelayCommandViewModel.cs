using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thorlabs.Elliptec.ELLO.Support;

namespace Thorlabs.Elliptec.ELLO.ViewModel
{
    /// <summary>   A ViewModel for the button relay command. </summary>
    /// <seealso cref="T:Thorlabs.Elliptec.ELLO.Support.ObservableObject"/>
    public class ButtonRelayCommandViewModel : ObservableObject
    {
        /// <summary>   The get command relay. </summary>
        private ICommand _getCommandRelay;

        private string _title;
        private readonly Action _action;

        /// <summary>  Gets the click command. </summary>
         /// <value>    The click command. </value>
         public ICommand ClickCommand { get { return _getCommandRelay ?? (_getCommandRelay = new RelayCommand(DoCommand)); } }

        private void DoCommand()
        {
            _action?.Invoke();
        }

        /// <summary>   Constructor. </summary>
        /// <param name="action">   The action. </param>
        /// <param name="title">    The title. </param>
        public ButtonRelayCommandViewModel(Action action, string title)
        {
            Title = title;
            _action = action;
        }

        /// <summary>   Gets or sets the title. </summary>
        /// <value> The title. </value>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }
    }
}
