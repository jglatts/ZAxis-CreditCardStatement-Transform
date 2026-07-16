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
using System.Windows;

namespace ZAxis_CreditCardStatement_Transform
{
    public class Mapper
    {
        // working with a tuple
        // can use a class for DTOs 
        public List<(string account_number, string keyword_description)> keywordMap = new();

        public static string rulesFilePath = "mapping-rules.txt";

        public Mapper()
        {
            if (!loadKeywordMap())
                loadKeywordMapDefault();
        }

        public bool loadKeywordMap()
        {
            if (!System.IO.File.Exists(rulesFilePath))
            {
                MessageBox.Show($"Mapping rules file not found: {rulesFilePath}. Using default rules.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            string[] lines = System.IO.File.ReadAllLines(rulesFilePath);

            foreach (string line in lines)
            {
                if (line.StartsWith(";") || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                string[] parts = line.Split(',');
                if (parts.Length == 2)
                {
                    string keyword = parts[0].Trim();
                    string accountNumber = parts[1].Trim();
                    if (!string.IsNullOrWhiteSpace(keyword) && !string.IsNullOrWhiteSpace(accountNumber))
                    {
                        keywordMap.Add((accountNumber, keyword));
                    }
                }
            }

            return true;
        }

        public void loadKeywordMapDefault()
        {
            keywordMap = new List<(string account_number, string keyword_description)>
            {
                ("60150", "ATT"),
                ("60152", "VERIZON"),
                ("60160", "LECK WASTE"),
                ("60178", "WAREHOUSE RENTAL"),
                ("60179", "WAREHOUSE EXPENSE"),
                ("60180", "STAPLES"),
                ("60181", "CHECK PRINTING"),
                ("60270", "AMAZON"),
                ("60270", "COSTCO GAS"),
                ("60270", "STRATASYS"),
                ("60270", "TESTEQUITY"),
                ("60270", "PRIME VIDEO"),
                ("60270", "EBAY"),
                ("60270", "REACHLOCAL"),
                ("60270", "AUDIBLE"),
                ("60270", "LINDE GAS"),
                ("60270", "EZ PASS"),
                ("60270", "DIGI KEY"),
                ("60270", "FORMLABS"),
                ("60270", "UPS"),
                ("60270", "FEDEX"),
                ("60270", "DHL"),
                ("60270", "CANVA"),
                ("60270", "MCMASTER"),
                ("60270", "HARBOR FREIGHT"),
                ("60270", "HOME DEPOT"),
                ("60270", "HOSTING"),
                ("60270", "LINKEDIN"),
                ("60270", "PCBWAY"),
                ("60270", "ULINE")
            };
        }

        public void updateKeywordMapFile()
        {
            // also update the mapping rules based on the new keyword map
            System.IO.File.WriteAllLines(rulesFilePath, keywordMap.Select(
                entry => $"{entry.keyword_description}, {entry.account_number}"));
        }

        public void addKeywordRule(string keyword, string number)
        {
            keyword = normalizeDescription(keyword);
            number = number.Trim();

            bool alreadyExists = keywordMap.Any(entry =>
                entry.keyword_description.Equals(
                    keyword,
                    StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
            {
                MessageBox.Show($"Keyword '{keyword}' already exists in the mapping rules. Please choose a different keyword.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            keywordMap.Add((number, keyword));

            System.IO.File.AppendAllText(
                rulesFilePath,
                $"{keyword}, {number}{Environment.NewLine}");

            return;
        }

        public string findGLAccountNumberByKeyword(string description)
        {
            string normalizedDescription = normalizeDescription(description);
            var matchingEntry = keywordMap.FirstOrDefault(entry =>
                normalizedDescription.Contains(entry.keyword_description, StringComparison.OrdinalIgnoreCase));
            return matchingEntry.account_number ?? "60270"; 
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

        private static string normalizeCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return "";

            return new string(cardNumber.Where(char.IsDigit).ToArray());
        }
    }


}