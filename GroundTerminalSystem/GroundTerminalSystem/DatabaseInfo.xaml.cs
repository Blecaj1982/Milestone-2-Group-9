using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FDMS.DAL;
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

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (TextWriter writer = File.CreateText(Path.Combine(Environment.CurrentDirectory, "SearchLog_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt")))
                {
                    foreach (TelemetryRecordDal item in (List<TelemetryRecordDal>)(DatabaseView.ItemsSource))
                    {
                        writer.WriteLine(item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
