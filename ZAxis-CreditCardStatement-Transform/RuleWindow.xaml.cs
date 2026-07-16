using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ZAxis_CreditCardStatement_Transform
{
    public partial class RulesWindow : Window
    {
        public RulesWindow(IEnumerable<MappingRule> mappingRules)
        {
            InitializeComponent();

            List<MappingRuleDisplay> displayedRules = mappingRules
                .Select((rule, index) => new MappingRuleDisplay(rule, index + 1))
                .ToList();

            // sort by account #

            rulesDataGrid.ItemsSource = displayedRules;

            ruleCountTextBlock.Text =
                $"{displayedRules.Count} mapping rule" +
                (displayedRules.Count == 1 ? "" : "s");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class MappingRuleDisplay
    {
        private readonly MappingRule rule;

        public int RuleNumber { get; }

        public string CardDisplay =>
            string.IsNullOrWhiteSpace(rule.CardNumber)
                ? "Any card"
                : $"Ends in {rule.CardNumber}";

        public string DescriptionDisplay =>
            string.IsNullOrWhiteSpace(rule.DescriptionKeyword)
                ? "Any description"
                : rule.DescriptionKeyword;

        public string AccountNumber => rule.Account.AccountNumber;

        public string AccountDescription => rule.Account.Description;

        public MappingRuleDisplay(MappingRule mappingRule, int ruleNumber)
        {
            rule = mappingRule;
            RuleNumber = ruleNumber;
        }
    }
}