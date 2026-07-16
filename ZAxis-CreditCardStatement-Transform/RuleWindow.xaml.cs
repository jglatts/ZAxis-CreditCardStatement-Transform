using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ZAxis_CreditCardStatement_Transform
{
    public partial class RulesWindow : Window
    {
        public RulesWindow(
            IEnumerable<(string account_number, string keyword_description)> keywordMap)
        {
            InitializeComponent();

            List<KeywordRuleDisplay> displayedRules = keywordMap
                .Select((entry, index) =>
                    new KeywordRuleDisplay(
                        index + 1,
                        entry.keyword_description,
                        entry.account_number))
                .ToList();

            rulesDataGrid.ItemsSource = displayedRules;

            ruleCountTextBlock.Text =
                $"{displayedRules.Count} mapping rule" +
                (displayedRules.Count == 1 ? "" : "s");
        }

        private void btnClose_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }

    public class KeywordRuleDisplay
    {
        public int RuleNumber { get; }

        public string Keyword { get; }

        public string AccountNumber { get; }

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