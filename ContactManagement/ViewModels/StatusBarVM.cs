using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace ContactManagement.ViewModels
{
    public class StatusBarVM : ViewModelBase
    {
        private DispatcherTimer dispatcherTimer;

        public StatusBarVM()
        {
            DispatcherTimeSetup();
            
        }

        private void DispatcherTimeSetup()
        {
            //  DispatcherTimer setup
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        //  System.Windows.Threading.DispatcherTimer.Tick handler
        //
        //  Updates the current seconds display and calls
        //  InvalidateRequerySuggested on the CommandManager to force 
        //  the Command to raise the CanExecuteChanged event.
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            DisplayTime = Convert.ToString(DateTime.Now);
        }

        private string displayTime;

        public string DisplayTime
        {
            get { return displayTime; }
            set
            {
                if (displayTime != value)
                {
                    displayTime = value;
                    
                }
                OnPropertyChanged("DisplayTime");
            }
        }

    }
}
