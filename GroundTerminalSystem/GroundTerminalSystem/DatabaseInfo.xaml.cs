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

namespace GroundTerminalSystem
{
    /// <summary>
    /// Interaction logic for DatabaseInfo.xaml
    /// </summary>
    public partial class DatabaseInfo : Page
    {
        public DatabaseInfo()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseSearch dbs = new DatabaseSearch();

            var records = dbs.dbaseSearch(SearchTextBox.Text);
            DatabaseView.ItemsSource = records;
            DatabaseView.Items.Refresh();
        }
    }
}
