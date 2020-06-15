using CoffeeMachine.EventHub.Sender;
using CoffeeMachine.EventHub.Sender.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace CoffeeMachine.Simulator.UI.ViewModel
{
    public class MainViewModel :BindableBase
    {
        private int _counterCappuccino;
        private int _counterEspresso;
        private string _city;
        private string _serialNumber;
        private readonly ICoffeeMachineDataSender coffeeMachineDataSender;
        private int _boilerTemp;
        private int _beanLevel;
        private bool _isSendingPeriodically;
        private DispatcherTimer _dispatcherTimer;

        public MainViewModel(ICoffeeMachineDataSender coffeeMachineDataSender)
        {
            SerialNumber = Guid.NewGuid().ToString().Substring(0, 8);
            MakeCappuccinoCommand = new DelegateCommand(MakeCappuccino);
            MakeEspressoCommand = new DelegateCommand(MakeEspresso);
            this.coffeeMachineDataSender = coffeeMachineDataSender;
            Logs = new ObservableCollection<string>();
            _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

       
        public ICommand MakeCappuccinoCommand { get; }
        public ICommand MakeEspressoCommand { get; }

        public ObservableCollection<string> Logs { get; }
        public string City
        { 
            get { return _city; }
            set
            {
                _city = value;
                RaisePropertyChanged();
            }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                RaisePropertyChanged();
            }
        }

        public int CounterCappuccino
        {
            get { return _counterCappuccino; }
            set
            {
                _counterCappuccino = value;
                RaisePropertyChanged();
            }
        }

        public int BoilerTemp
        {
            get { return _boilerTemp; }
            set
            {
                _boilerTemp = value;
                RaisePropertyChanged();
            }
        }
        public int BeanLevel
        {
            get { return _beanLevel; }
            set
            {
                _beanLevel = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSendingPeriodically
        {
            get { return _isSendingPeriodically; }
            set
            {
                if (_isSendingPeriodically != value)
                {
                    _isSendingPeriodically = value;

                    if (_isSendingPeriodically)
                    {
                        _dispatcherTimer.Start();
                    }
                    else
                    {
                        _dispatcherTimer.Stop();
                    }
                    RaisePropertyChanged();
                }
                
            }
        }

        public int CounterEspresso
        {
            get { return _counterEspresso; }
            set
            {
                _counterEspresso = value;
                RaisePropertyChanged();
            }
        }

        private async void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var boilerTempData = CreateCoffeeMachineData(nameof(BoilerTemp), BoilerTemp);
            var beanLevelData = CreateCoffeeMachineData(nameof(BeanLevel), BeanLevel);

            //Send data as batch
            await SendDataAsync(new List<CoffeeMachineData> { boilerTempData, beanLevelData });

        }
        private async void MakeCappuccino()
        {
            CounterCappuccino++;
            var coffeeMachineData = CreateCoffeeMachineData(nameof(CounterCappuccino), CounterCappuccino);

            await SendDataAsync(coffeeMachineData);
        }
        private async void MakeEspresso()
        {
            CounterEspresso++;
            var coffeeMachineData = CreateCoffeeMachineData(nameof(CounterEspresso), CounterEspresso);
            await SendDataAsync(coffeeMachineData);

        }
        private async Task SendDataAsync(CoffeeMachineData coffeeMachineData)
        {
            try
            {
                await coffeeMachineDataSender.SendDataAsync(coffeeMachineData);
                WriteLog($"Sent data: {coffeeMachineData}");
            }
            catch (Exception ex)
            {
                WriteLog($"Exception: {ex.Message}");
            }
        }

        private async Task SendDataAsync(IEnumerable<CoffeeMachineData> lstCoffeeMachineData)
        {
            try
            {
                await coffeeMachineDataSender.SendDataAsync(lstCoffeeMachineData);

                foreach (var coffeeMachineData in lstCoffeeMachineData)
                {
                    WriteLog($"Sent data: {coffeeMachineData}");
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Exception: {ex.Message}");
            }
        }

        private CoffeeMachineData CreateCoffeeMachineData(string sensorType, int sensorValue)
        {
            return new CoffeeMachineData
            {
                City = City,
                SerialNumber = SerialNumber,
                SensorType = sensorType,
                SensorValue = sensorValue,
                RecordingDateTime = DateTime.Now
            };
        }

        private void WriteLog(string logMessage)
        {
            Logs.Insert(0, logMessage);
        }


    }
}
