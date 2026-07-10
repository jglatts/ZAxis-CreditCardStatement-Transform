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
                    // note SAGE excepcts GL account number to be in the CATEGORY column (index 3) of the CSV file
                    // see scanned doc for more
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

            string transformedOutputPath = System.IO.Path.Combine(
                directory,
                $"{fileName}_Transformed{extension}");

            string sageOutputPath = System.IO.Path.Combine(
                directory,
                $"{fileName}_Sage{extension}");

            // Save the standard transformed CSV.
            writeCSVFile(transformedOutputPath, csvRows);

            // Create a deep copy for the Sage version.
            List<string[]> sageRows = csvRows
                .Select(row => row.ToArray())
                .ToList();

            if (sageRows.Count == 0)
                return;

            string[] headerRow = sageRows[0];

            int categoryIndex = Array.FindIndex(
                headerRow,
                column => column.Equals(
                    "Category",
                    StringComparison.OrdinalIgnoreCase));

            int glAccountIndex = Array.FindIndex(
                headerRow,
                column => column.Equals(
                    "GL Account Number",
                    StringComparison.OrdinalIgnoreCase));

            if (categoryIndex < 0)
            {
                MessageBox.Show(
                    "The Category column could not be found.",
                    "Sage Transform Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (glAccountIndex < 0)
            {
                MessageBox.Show(
                    "The GL Account Number column could not be found.",
                    "Sage Transform Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            for (int rowIndex = 0; rowIndex < sageRows.Count; rowIndex++)
            {
                List<string> row = sageRows[rowIndex].ToList();

                if (row.Count <= categoryIndex ||
                    row.Count <= glAccountIndex)
                {
                    MessageBox.Show(
                        $"Row {rowIndex + 1} does not contain the expected columns.",
                        "Sage Transform Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                if (rowIndex > 0)
                {
                    // Copy the GL account number into Category.
                    row[categoryIndex] = row[glAccountIndex];
                }

                // Remove only the extra GL Account Number column.
                // Credit and all other columns remain untouched.
                row.RemoveAt(glAccountIndex);

                sageRows[rowIndex] = row.ToArray();
            }

            // update header
            sageRows[0][2] = "GL Account Number";

            writeCSVFile(sageOutputPath, sageRows);

            MessageBox.Show(
                $"CSV files saved successfully.\n\n" +
                $"Transformed:\n{transformedOutputPath}\n\n" +
                $"Sage:\n{sageOutputPath}",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = transformedOutputPath,
                    UseShellExecute = true
                });

                Process.Start(new ProcessStartInfo
                {
                    FileName = sageOutputPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"The files were saved, but could not be opened.\n\n{ex.Message}",
                    "Open Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private static void writeCSVFile(
            string outputPath,
            IEnumerable<string[]> rows)
        {
            using var writer = new StreamWriter(
                outputPath,
                false,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            foreach (string[] row in rows)
            {
                writer.WriteLine(
                    string.Join(",", row.Select(EscapeCsvField)));
            }
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