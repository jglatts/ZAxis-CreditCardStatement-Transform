using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
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
        private List<string[]> csvRows = new();

        private Mapper glMapper = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadAndTransform_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedFilePath))
            {
                MessageBox.Show("Please select a CSV file first");
                return;
            }

            if (csvRows.Count == 0)
            {
                MessageBox.Show("The selected CSV file does not contain any rows.");
                return;
            }

            if (!transformCSV())
            {
                MessageBox.Show($"Unable to transform the CSV file.");
            }
        }

        private bool transformCSV()
        {
            if (csvRows.Count == 0)
                return false;

            for (int rowIndex = 0; rowIndex < csvRows.Count; rowIndex++)
            {
                string[] currentRow = csvRows[rowIndex];

                if (currentRow.Length < 4)
                {
                    MessageBox.Show(
                        $"Row {rowIndex + 1} does not contain enough columns.\n\n" +
                        $"Column count: {currentRow.Length}",
                        "Transform Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return false;
                }

                var transformedRow = new List<string>(currentRow);

                // Remove the original third column first.
                transformedRow.RemoveAt(2);

                // Then remove the original second column.
                transformedRow.RemoveAt(1);

                if (rowIndex == 0)
                {
                    transformedRow[0] = "Statement Date";
                    transformedRow.Add("GL Account Number");
                }
                else
                {
                    if (transformedRow.Count <= 3)
                    {
                        MessageBox.Show(
                            $"Row {rowIndex + 1} does not contain a description column.\n\n" +
                            string.Join(" | ", transformedRow),
                            "Transform Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        return false;
                    }

                    // get GL Account Number based on the description in the second column (index 1)
                    // will need further testing 
                    string description = transformedRow[1];

                    string glAccountNumber =
                        glMapper.getGLAccountNumber("", description);

                    transformedRow.Add(glAccountNumber);
                }

                csvRows[rowIndex] = transformedRow.ToArray();
            }

            saveTransformedCSV();

            return true;
        }

        private void saveTransformedCSV()
        {
            string directory = System.IO.Path.GetDirectoryName(selectedFilePath)!;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(selectedFilePath);
            string extension = System.IO.Path.GetExtension(selectedFilePath);

            string outputPath = System.IO.Path.Combine(
                directory,
                $"{fileName}_Transformed{extension}");

            using var writer = new StreamWriter(outputPath);

            foreach (string[] row in csvRows)
            {
                writer.WriteLine(string.Join(",", row.Select(EscapeCsvField)));
            }

            MessageBox.Show(
                $"Transformed CSV saved successfully.\n\n{outputPath}",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Process.Start(new ProcessStartInfo
            {
                FileName = outputPath,
                UseShellExecute = true
            });
        }

        private static string EscapeCsvField(string field)
        {
            if (field.Contains(',') ||
                field.Contains('"') ||
                field.Contains('\n') ||
                field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
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
                txtBoxCSVFileName.Text = selectedFilePath;
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