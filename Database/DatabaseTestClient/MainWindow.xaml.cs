using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

using FDMS.DAL;

namespace FDMS.DatabaseTestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Button> connectedButtons = new List<Button>();
        private Dictionary<TextBox, Func<string, object>> inputConverters;
        private Brush defaultBorderBrush;

        private FdmsDatabase database = new FdmsDatabase();

        public MainWindow()
        {
            InitializeComponent();

            connectedButtons.AddRange(new Button[] { DisconnectButton, InsertButton, SelectButton });
            connectedButtons.ForEach(b => b.IsEnabled = false);

            inputConverters = new Dictionary<TextBox, Func<string, object>>()
            {
                { AircraftTailNumTextBox, ConvertAircraftTailNum },
                { TimestampTextBox, ConvertDateTime },
                { AccelXTextBox, ConvertFloat },
                { AccelYTextBox, ConvertFloat },
                { AccelZTextBox, ConvertFloat },
                { AltitudeTextBox, ConvertFloat },
                { PitchTextBox, ConvertFloat },
                { BankTextBox, ConvertFloat },
                { AircraftTailNumSelectTextBox, ConvertAircraftTailNum }
            };

            defaultBorderBrush = UsernameTextBox.BorderBrush;
        }

        #region TextBox Validation Methods
        
        private static object ConvertAircraftTailNum(string input)
        {
            return !(String.IsNullOrWhiteSpace(input) || input.Length > 6) ? (object)input : null;
        }

        private static object ConvertFloat(string input)
        {
            return float.TryParse(input, out float output) ? output : (object)null;
        }

        private static object ConvertDateTime(string input)
        {
            return DateTime.TryParse(input, out DateTime output) ? output : (object)null;
        }

        #endregion

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!database.Connected)
            {
                UsernameTextBox.BorderBrush = defaultBorderBrush;
                PasswordTextBox.BorderBrush = defaultBorderBrush;
                IpAddressTextBox.BorderBrush = defaultBorderBrush;
                PortNumTextBox.BorderBrush = defaultBorderBrush;

                ConnectionInput input = ParseConnectionInput();
                if (input.Valid)
                {
                    string connectionStr =
                        $"Data Source={input.IpAddress},{input.Port};" +
                        $"Database=FDMS_Server;" +
                        $"User ID={input.Username};" +
                        $"Password={input.Password};";

                    DALResult result = database.Connect(connectionStr);

                    if (result.Success && database.Connected)
                    {
                        ConnectButton.IsEnabled = false;
                        connectedButtons.ForEach(b => b.IsEnabled = true);
                    }
                    else
                    {
                        MessageBox.Show(result.FailureMessage);
                    }
                }
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (database.Connected)
            {
                DALResult result = database.Disconnect();
                if (!result.Success)
                {
                    MessageBox.Show(result.FailureMessage);
                }
            }

            ConnectButton.IsEnabled = true;
            connectedButtons.ForEach(b => b.IsEnabled = false);
        }

        private ConnectionInput ParseConnectionInput()
        {
            bool valid = true;
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                UsernameTextBox.BorderBrush = Brushes.Red;
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordTextBox.BorderBrush = Brushes.Red;
                valid = false;
            }

            IPAddress ipAddress = null;
            ushort port = 0;

            if (!IPAddress.TryParse(IpAddressTextBox.Text, out ipAddress))
            {
                IpAddressTextBox.BorderBrush = Brushes.Red;
                valid = false; 
            }

            if (!ushort.TryParse(PortNumTextBox.Text, out port))
            {
                PortNumTextBox.BorderBrush = Brushes.Red;
                valid = false;
            }

            return new ConnectionInput()
            {
                Valid = valid,
                Username = username,
                Password = password,
                IpAddress = ipAddress,
                Port = port
            };
        }

        private class ConnectionInput
        {
            public bool Valid;
            public string Username;
            public string Password;
            public IPAddress IpAddress;
            public ushort Port;
        }
    }
}
