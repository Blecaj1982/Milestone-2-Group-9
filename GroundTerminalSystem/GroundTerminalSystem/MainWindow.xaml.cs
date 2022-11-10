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
using FDMS.DAL;

namespace GroundTerminalSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ListenerClass serverListener = new ListenerClass("127.0.0.1", 8989);

        LiveConnection liveConnectionPage = new LiveConnection();
        DatabaseInfo databaseInfoPage = new DatabaseInfo();
        bool isConnected = false;
        object isConnectedLockObject = new object();
        List<TelemetryRecordDAL> Records = new List<TelemetryRecordDAL>();

        public MainWindow()
        {
            InitializeComponent();

            //initially live connection screen will be on the main panel
            LiveConnectionButton.Style = (Style)Application.Current.Resources["SideMenuButtonActive"];
            mainPanel.Content = liveConnectionPage;

            liveConnectionPage.LiveConnectionDataView.ItemsSource = Records;
            Thread ListenerThread = new Thread(
                () => 
                {
                    serverListener.ListenForConnection((r) =>
                       {
                           Dispatcher.Invoke(() =>
                           {
                               lock(isConnectedLockObject)
                               {
                                   if (isConnected)
                                   {
                                       Records.Insert(0, r);
                                       liveConnectionPage.LiveConnectionDataView.Items.Refresh();
                                   }
                               }
                           });
                       });
                }
                );

            ListenerThread.Start();
        }

        private void OnBorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void LiveConnectionButtonOnClick(object sender, RoutedEventArgs e)
        {
            LiveConnectionButton.Style = (Style)Application.Current.Resources["SideMenuButtonActive"];
            QueryDatabaseButton.Style = (Style)Application.Current.Resources["SideMenuButton"];
            mainPanel.Content = liveConnectionPage;
        }

        private void QueryDatabaseButtonOnClick(object sender, RoutedEventArgs e)
        {
            QueryDatabaseButton.Style = (Style)Application.Current.Resources["SideMenuButtonActive"];
            LiveConnectionButton.Style = (Style)Application.Current.Resources["SideMenuButton"];
            mainPanel.Content = databaseInfoPage;
        }

        private void ConnectionButtonOnClick(object sender, RoutedEventArgs e)
        {
            bool isConnectedTemp = false;

            lock(isConnectedLockObject)
            {
                isConnected = !isConnected;
                isConnectedTemp = isConnected;
            }

            SignalIcon.Foreground = isConnectedTemp ?
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#99cc60")) :
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));

            ConnectionText.Text = isConnectedTemp ?
                "Connection On" :
                "Connection Off";
        }

        private void QuitButtonOnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
