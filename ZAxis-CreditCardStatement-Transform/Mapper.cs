/******************************************************************************
 * ZAxis Credit Card Statement Transform
 *
 * Provides merchant description to General Ledger (GL) account mapping used
 * to automatically categorize transactions during the CSV transformation
 * process.
 * 
 *  * ToDo:
 *  - Map with both description and category to GL account number
 *  - Can try both and see if one works, or just use category if available
 *
 * Author: John Glatts
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZAxis_CreditCardStatement_Transform
{
    public class Mapper
    {
        public List<MappingRule> mappingRules = new();

        public Mapper()
        {
            loadDefaultRules();
        }

        public GLAccount? findGLAccount(string cardNumber, string description)
        {
            string normalizedCard = normalizeCardNumber(cardNumber);
            string normalizedDescription = normalizeDescription(description);

            MappingRule? matchingRule = mappingRules.FirstOrDefault(rule =>
                rule.isMatch(normalizedCard, normalizedDescription));

            return matchingRule?.Account;
        }

        private string normalizeDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return "";

            return description
                .Trim()
                .ToUpperInvariant()
                .Replace("\t", " ");
        }

        public string getGLAccountNumber(
            string cardNumber,
            string description,
            string defaultAccount = "60270")
        {
            GLAccount? account = findGLAccount(cardNumber, description);

            return account?.AccountNumber ?? defaultAccount;
        }


        // default mapping rules for common vendors and descriptions
        // uses DESCRIPTION field from the credit card statement to determine the GL account
        // should consider using CATEGORGY field as well, but this works as first-pass for common vendorss
        // nice to have:
        //  - ability to add custom rules for specific card numbers and descriptions 
        //  - save and load rules from a file 
        private void loadDefaultRules()
        {
            addRule("", "AMAZON", GLAccounts.OfficeExpense);
            addRule("", "COSTCO GAS", GLAccounts.AutoAndTravel);
            addRule("", "STRATASYS", GLAccounts.FlexibleTestExpense);
            addRule("", "TESTEQUITY", GLAccounts.ShopEquipment);
            addRule("", "PRIME VIDEO", GLAccounts.BusinessEntertainment);
            addRule("", "EBAY", GLAccounts.EbayOfficeExpense);
            addRule("", "REACHLOCAL", GLAccounts.Advertising);
            addRule("", "AUDIBLE", GLAccounts.SubscriptionsAndMagazines);
            addRule("", "LINDE GAS", GLAccounts.ShopSuppliesExpense);
            addRule("", "EZ PASS", GLAccounts.AutoAndTravel);
            addRule("", "ATT", GLAccounts.Telephone);
            addRule("", "DIGI KEY", GLAccounts.FlexibleTestExpense);
            addRule("", "FORMLABS", GLAccounts.FlexibleTestExpense);
            addRule("", "VERIZON", GLAccounts.CellularTelephone);
            addRule("", "UPS", GLAccounts.ShippingExpense);
            addRule("", "FEDEX", GLAccounts.ShippingExpense);
            addRule("", "DHL", GLAccounts.ShippingExpense);
            addRule("", "CANVA", GLAccounts.SoftwareExpense);
            addRule("", "MCMASTER", GLAccounts.ShopSuppliesExpense);
            addRule("", "STAPLES", GLAccounts.OfficeExpense);
            addRule("", "HARBOR FREIGHT", GLAccounts.Tools);
            addRule("", "HOME DEPOT", GLAccounts.ShopSuppliesExpense);
            addRule("", "LECK WASTE", GLAccounts.Utilities);
            addRule("", "HOSTING", GLAccounts.OnlineComputerServices);
            addRule("", "LINKEDIN", GLAccounts.Advertising);
            addRule("", "PCBWAY", GLAccounts.ZWireResearchMaterialsAndEquipment);
            addRule("", "ULINE", GLAccounts.ShippingSuppliesExpense);
        }

        private void addRule(
            string cardNumber,
            string descriptionKeyword,
            GLAccount account, 
            string categoryKeyword = "")
        {
            mappingRules.Add(new MappingRule(
                normalizeCardNumber(cardNumber),
                normalizeDescription(descriptionKeyword),
                categoryKeyword,
                account));
        }

        private static string normalizeCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return "";

            return new string(cardNumber.Where(char.IsDigit).ToArray());
        }
    }


}