using GroundTerminalSystem.classes;
using System;
using System.Collections.Generic;
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
using System.Threading;

namespace GroundTerminalSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ListenerClass serverListener = new ListenerClass("127.0.0.1", 13000);

        public MainWindow()
        {
            InitializeComponent();

            Thread ListenerThread = new Thread(serverListener.ListenForConnection);
            ListenerThread.Start();

        }

        private void RealTimeModeOff_Checked(object sender, RoutedEventArgs e)
        {         
            RealTimeModeOn.IsChecked = false;
        }

        private void RealTimeModeOn_Checked(object sender, RoutedEventArgs e)
        {
            RealTimeModeOff.IsChecked = false;
        }
    }
}
