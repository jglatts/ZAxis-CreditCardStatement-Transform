using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZAxis_CreditCardStatement_Transform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedFilePath = "";
        private readonly List<string[]> csvRows = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadAndTransform_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Load and Transform button clicked! " + selectedFilePath);
        }

        private void btnBrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Select a Credit Card Statement",
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                DefaultExt = ".csv",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                selectedFilePath = fileDialog.FileName;
                loadCSVFile();
            }

        }

        private void loadCSVFile()
        {
            csvRows.Clear();

            if (string.IsNullOrEmpty(selectedFilePath))
                return;

            using var parser = new TextFieldParser(selectedFilePath);

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;
            parser.TrimWhiteSpace = true;

            while (!parser.EndOfData)
            {
                string[]? fields = parser.ReadFields();

                if (fields != null)
                {
                    csvRows.Add(fields);
                }
            }

            //MessageBox.Show($"Loaded {csvRows.Count} rows from the CSV file.");
            //MessageBox.Show(csvRows.Count > 0 ? string.Join(", ", csvRows[1]) : "No rows loaded");
        }

    }
}