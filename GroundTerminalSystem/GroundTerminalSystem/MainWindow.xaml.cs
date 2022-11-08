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

        LiveConnection liveConnectionPage = new LiveConnection();
        DatabaseInfo databaseInfoPage = new DatabaseInfo();
        bool isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            //initially live connection screen will be on the main panel
            LiveConnectionButton.Style = (Style)Application.Current.Resources["SideMenuButtonActive"];
            mainPanel.Content = liveConnectionPage;

            Thread ListenerThread = new Thread(serverListener.ListenForConnection);
            ListenerThread.Start();

        }

        private void OnBorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        //private void RealTimeModeOff_Checked(object sender, RoutedEventArgs e)
        //{         
        //    RealTimeModeOn.IsChecked = false;
        //}

        //private void RealTimeModeOn_Checked(object sender, RoutedEventArgs e)
        //{
        //    RealTimeModeOff.IsChecked = false;
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    DatabaseSearch dbs = new DatabaseSearch();
        //    DataShow.ItemsSource = dbs.dbaseSearch(txtSearch.Text).DefaultView;

        //}

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
            //add if statement of connection if it is connected or not
            if (!isConnected)
            {

                isConnected = true;
                //change the icon color to green 
                SignalIcon.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#99cc60"));
                ConnectionText.Text = "Connection On";
            }
            else
            {
                isConnected = false;
                //change the icon color to red 
                SignalIcon.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));
                ConnectionText.Text = "Connection Off";
            }
        }

        private void QuitButtonOnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
