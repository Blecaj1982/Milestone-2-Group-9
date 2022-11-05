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
    using InputConverter = Tuple<Func<string, object>, Action<object, TelemetryRecordDAL>>;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Button> connectedButtons = new List<Button>();
        private Dictionary<TextBox, InputConverter> insertInputConverters;
        private Brush defaultBorderBrush;

        private FdmsDatabase database = new FdmsDatabase();

        public MainWindow()
        {
            InitializeComponent();

            connectedButtons.AddRange(new Button[] { DisconnectButton, InsertButton, SelectButton });
            connectedButtons.ForEach(b => b.IsEnabled = false);

            insertInputConverters = new Dictionary<TextBox, Tuple<Func<string, object>, Action<object, TelemetryRecordDAL>>>()
            {
                { AircraftTailNumTextBox, new InputConverter(ConvertAircraftTailNum, (obj, rec) => rec.AircraftTailNum = (string)obj) },
                { TimestampTextBox, new InputConverter(ConvertDateTime, (obj, rec)=> rec.Timestamp = (DateTime)obj)},
                { AccelXTextBox, new InputConverter(ConvertFloat, (obj, rec) => rec.Accel_X = (float)obj) },
                { AccelYTextBox, new InputConverter(ConvertFloat, (obj, rec) => rec.Accel_Y = (float)obj) },
                { AccelZTextBox, new InputConverter(ConvertFloat, (obj, rec) => rec.Accel_Z = (float)obj) },
                { AltitudeTextBox, new InputConverter(ConvertFloat, (obj, rec) => rec.Altitude = (float)obj) },
                { PitchTextBox, new InputConverter(ConvertFloat, (obj, rec) => rec.Pitch = (float)obj) },
                { BankTextBox, new InputConverter(ConvertFloat, (obj, rec) => rec.Bank = (float)obj) },
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

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(TextBox tb in insertInputConverters.Keys)
            {
                tb.BorderBrush = defaultBorderBrush;
            }

            if (database.Connected)
            {
                InsertInput input = ParseInsertInput();

                if (input.Valid)
                {
                    DALResult result = database.Insert(input.Record);

                    if (!result.Success)
                    {
                        MessageBox.Show(result.FailureMessage);
                    }
                    else
                    {
                        foreach (TextBox tb in insertInputConverters.Keys)
                        {
                            tb.Text = "";
                        }
                    }
                }
            }
        }

        private InsertInput ParseInsertInput()
        {
            bool valid = true;

            TelemetryRecordDAL record = new TelemetryRecordDAL();

            foreach(KeyValuePair<TextBox, InputConverter> kvp in insertInputConverters)
            {
                object convertedInput = kvp.Value.Item1(kvp.Key.Text);
                if (convertedInput != null)
                {
                    kvp.Value.Item2(convertedInput, record); 
                }
                else
                {
                    kvp.Key.BorderBrush = Brushes.Red;
                    valid = false;
                }
            }

            return new InsertInput()
            {
                Valid = valid,
                Record = record
            };
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

        private class InsertInput
        {
            public bool Valid;
            public TelemetryRecordDAL Record;
        }
    }
}
