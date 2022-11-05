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

namespace FDMS.DatabaseTestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Button> ConnectedButtons = new List<Button>();
        private Dictionary<TextBox, Func<string, object>> InputConverters;

        public MainWindow()
        {
            InitializeComponent();

            ConnectedButtons.AddRange(new Button[] { DisconnectButton, InsertButton, SelectButton });
            ConnectedButtons.ForEach(b => b.IsEnabled = false);

            InputConverters = new Dictionary<TextBox, Func<string, object>>()
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
    }
}
