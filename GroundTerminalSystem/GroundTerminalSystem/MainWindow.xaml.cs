using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using FDMS.DAL;

namespace GroundTerminalSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LiveConnection liveConnectionPage = new LiveConnection();
        DatabaseInfo databaseInfoPage = new DatabaseInfo();

        private List<TelemetryRecordDAL> liveData = new List<TelemetryRecordDAL>();
        bool isShowingLiveData = false;
        object isShowingLiveDataLockObject = new object();

        public MainWindow()
        {
            InitializeComponent();

            // Activate buttons and pages based on active components
            InitializeButton(ConnectionButton, App.ListeningForTransmission);
            InitializeButton(LiveConnectionButton, App.ListeningForTransmission);
            InitializeButton(QueryDatabaseButton, App.ConnectedToDatabase);

            InitializeStartPage();

            // hook up to listener to receive live data
            liveConnectionPage.LiveConnectionDataView.ItemsSource = liveData;
            App.ServerListener.RecordReceivedEvent += (r) => AddRecordToLiveData(r);
        }

        private void InitializeStartPage()
        {
            bool shouldStartOnLiveDataPage = App.ListeningForTransmission || !App.ConnectedToDatabase;
            mainPanel.Content = (shouldStartOnLiveDataPage ? (Page)liveConnectionPage : (Page)databaseInfoPage);

            if (shouldStartOnLiveDataPage && App.ListeningForTransmission)
            {
                LiveConnectionButton.Style = (Style)Application.Current.Resources["SideMenuButtonActive"];
            }
            else if (App.ConnectedToDatabase)
            {
                QueryDatabaseButton.Style = (Style)Application.Current.Resources["SideMenuButtonActive"];
            }
        }

        private static void InitializeButton(Button button, bool enabled)
        {
            button.IsEnabled = enabled;
            button.Style = (Style)Application.Current.Resources[enabled ? "SideMenuButton" : "SideMenuButtonDisabled"];
        }

        private void AddRecordToLiveData(TelemetryRecordDAL record)
        {
           lock(isShowingLiveDataLockObject)
           {
               if (isShowingLiveData)
               {
                   Dispatcher.Invoke(() =>
                   {
                       liveData.Insert(0, record);
                       liveConnectionPage.LiveConnectionDataView.Items.Refresh();
                   });
               }
           }
        }

        private void OnBorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void LiveConnectionButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (App.ListeningForTransmission)
            {
                LiveConnectionButton.Style = (Style)Application.Current.Resources["SideMenuButtonActive"];
                QueryDatabaseButton.Style = (Style)Application.Current.Resources[App.ConnectedToDatabase ? "SideMenuButton" : "SideMenuButtonDisabled"];
                mainPanel.Content = liveConnectionPage;
            }
        }

        private void QueryDatabaseButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (App.ConnectedToDatabase)
            {
                QueryDatabaseButton.Style = (Style)Application.Current.Resources["SideMenuButtonActive"];
                LiveConnectionButton.Style = (Style)Application.Current.Resources[App.ListeningForTransmission ? "SideMenuButton" : "SideMenuButtonDisabled"];
                mainPanel.Content = databaseInfoPage;
            }
        }

        private void ConnectionButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (App.ListeningForTransmission)
            {
                bool isConnectedTemp = false;

                lock(isShowingLiveDataLockObject)
                {
                    isShowingLiveData = !isShowingLiveData;
                    isConnectedTemp = isShowingLiveData;
                }

                SignalIcon.Foreground = isConnectedTemp ?
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#99cc60")) :
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));

                ConnectionText.Text = isConnectedTemp ?
                    "Connection On" :
                    "Connection Off";
            }
        }

        private void QuitButtonOnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
