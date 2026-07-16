using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ZAxis_CreditCardStatement_Transform
{
    public partial class RulesWindow : Window
    {
        private readonly Mapper mapper;

        private readonly ObservableCollection<KeywordRuleDisplay> displayedRules;

        public RulesWindow(Mapper mapper)
        {
            InitializeComponent();

            this.mapper = mapper;

            displayedRules =
                new ObservableCollection<KeywordRuleDisplay>(
                    mapper.keywordMap.Select((entry, index) =>
                        new KeywordRuleDisplay(
                            index + 1,
                            entry.keyword_description,
                            entry.account_number)));

            rulesDataGrid.ItemsSource = displayedRules;

            UpdateRuleNumbers();
        }

        private void btnAdd_Click(
            object sender,
            RoutedEventArgs e)
        {
            KeywordRuleDisplay newRule = new(
                displayedRules.Count + 1,
                "",
                "");

            displayedRules.Add(newRule);

            UpdateRuleNumbers();

            rulesDataGrid.SelectedItem = newRule;
            rulesDataGrid.ScrollIntoView(newRule);

            rulesDataGrid.CurrentCell =
                new DataGridCellInfo(
                    newRule,
                    rulesDataGrid.Columns[1]);

            rulesDataGrid.BeginEdit();
        }

        private void btnDelete_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (rulesDataGrid.SelectedItem
                is not KeywordRuleDisplay rule)
            {
                MessageBox.Show(
                    "Select a rule to delete.",
                    "Delete Rule",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Delete the rule for \"{rule.Keyword}\"?",
                "Delete Rule",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            displayedRules.Remove(rule);

            UpdateRuleNumbers();
        }

        private void btnSave_Click(
            object sender,
            RoutedEventArgs e)
        {
            // Commit the currently edited cell before reading its value.
            rulesDataGrid.CommitEdit(
                DataGridEditingUnit.Cell,
                true);

            rulesDataGrid.CommitEdit(
                DataGridEditingUnit.Row,
                true);

            if (!ValidateRules())
                return;

            mapper.keywordMap.Clear();

            foreach (KeywordRuleDisplay rule in displayedRules)
            {
                mapper.keywordMap.Add((
                    rule.AccountNumber.Trim(),
                    rule.Keyword.Trim().ToUpperInvariant()));
            }

            // This currently rewrites mapping-rules.txt.
            mapper.updateKeywordMap();

            DialogResult = true;
            Close();
        }

        private bool ValidateRules()
        {
            for (int index = 0;
                 index < displayedRules.Count;
                 index++)
            {
                KeywordRuleDisplay rule =
                    displayedRules[index];

                if (string.IsNullOrWhiteSpace(rule.Keyword))
                {
                    ShowInvalidRule(
                        rule,
                        $"Rule {index + 1} is missing a keyword.");

                    return false;
                }

                if (string.IsNullOrWhiteSpace(
                        rule.AccountNumber))
                {
                    ShowInvalidRule(
                        rule,
                        $"Rule {index + 1} is missing a GL account number.");

                    return false;
                }

                if (rule.Keyword.Contains(','))
                {
                    ShowInvalidRule(
                        rule,
                        $"Rule {index + 1} contains a comma.");

                    return false;
                }

                bool duplicateExists =
                    displayedRules
                        .Where(otherRule =>
                            !ReferenceEquals(otherRule, rule))
                        .Any(otherRule =>
                            otherRule.Keyword.Equals(
                                rule.Keyword,
                                System.StringComparison.OrdinalIgnoreCase));

                if (duplicateExists)
                {
                    ShowInvalidRule(
                        rule,
                        $"The keyword \"{rule.Keyword}\" is already used.");

                    return false;
                }
            }

            return true;
        }

        private void ShowInvalidRule(
            KeywordRuleDisplay rule,
            string message)
        {
            MessageBox.Show(
                message,
                "Invalid Rule",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            rulesDataGrid.SelectedItem = rule;
            rulesDataGrid.ScrollIntoView(rule);
        }

        private void btnClose_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }

        private void UpdateRuleNumbers()
        {
            for (int index = 0;
                 index < displayedRules.Count;
                 index++)
            {
                displayedRules[index].RuleNumber =
                    index + 1;
            }

            ruleCountTextBlock.Text =
                $"{displayedRules.Count} mapping rule" +
                (displayedRules.Count == 1 ? "" : "s");

            rulesDataGrid.Items.Refresh();
        }
    }

    public class KeywordRuleDisplay
    {
        public int RuleNumber { get; set; }

        public string Keyword { get; set; }

        public string AccountNumber { get; set; }

        public KeywordRuleDisplay(
            int ruleNumber,
            string keyword,
            string accountNumber)
        {
            RuleNumber = ruleNumber;
            Keyword = keyword;
            AccountNumber = accountNumber;
        }
    }
}