using System.Windows;
using System.Windows.Controls;

using GroundTerminalSystem.classes;

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
            DatabaseView.ItemsSource = DatabaseSearch.dbaseSearch(App.SelectionDatabase, SearchTextBox.Text);
            DatabaseView.Items.Refresh();
        }
    }
}
