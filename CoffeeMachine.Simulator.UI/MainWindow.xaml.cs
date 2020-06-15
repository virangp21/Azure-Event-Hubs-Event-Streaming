using CoffeeMachine.EventHub.Sender;
using CoffeeMachine.Simulator.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoffeeMachine.Simulator.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var eventHubConnectionString = ConfigurationManager.AppSettings["EventHubConnectionString"].Trim();
            DataContext = new MainViewModel(new CoffeeMachineDataSender(eventHubConnectionString));
        }
    }
}
