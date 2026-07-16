using System.Windows;

namespace ZAxis_CreditCardStatement_Transform
{
    public partial class AddRuleWindow : Window
    {
        public string DescriptionKeyword { get; private set; } = "";

        public string AccountNumber { get; private set; } = "";

        public AddRuleWindow()
        {
            InitializeComponent();

            Loaded += (_, _) =>
                txtDescriptionKeyword.Focus();
        }

        private void btnAdd_Click(
            object sender,
            RoutedEventArgs e)
        {
            string descriptionKeyword =
                txtDescriptionKeyword.Text.Trim();

            string accountNumber =
                txtAccountNumber.Text.Trim();

            if (string.IsNullOrWhiteSpace(descriptionKeyword))
            {
                MessageBox.Show(
                    "Enter a description keyword.",
                    "Missing Keyword",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                txtDescriptionKeyword.Focus();
                return;
            }

            if (descriptionKeyword.Contains(','))
            {
                MessageBox.Show(
                    "The description keyword cannot contain a comma.",
                    "Invalid Keyword",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtDescriptionKeyword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                MessageBox.Show(
                    "Enter a GL account number.",
                    "Missing Account Number",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                txtAccountNumber.Focus();
                return;
            }

            DescriptionKeyword = descriptionKeyword;
            AccountNumber = accountNumber;

            DialogResult = true;
        }
    }
}