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
using System.Text;
using System.Threading.Tasks;

namespace ZAxis_CreditCardStatement_Transform
{
    public class MappingRule
    {
        public string CardNumber { get; }
        public string DescriptionKeyword { get; }
        public string CategoryKeyword { get; }

        public MappingRule(
            string cardNumber,
            string descriptionKeyword,
            string categoryKeyword)
        {
            CardNumber = cardNumber;
            DescriptionKeyword = descriptionKeyword;
            CategoryKeyword = categoryKeyword;
        }

        public bool isMatch(string cardNumber, string description)
        {
            // matches if the card number ends with the specified card number (if provided)
            // and the description contains the specified keyword (case-insensitive)
            bool cardMatches =
                string.IsNullOrWhiteSpace(CardNumber) ||
                cardNumber.EndsWith(CardNumber, StringComparison.Ordinal);

            bool descriptionMatches = description.Contains(
                DescriptionKeyword,
                StringComparison.OrdinalIgnoreCase);

            return cardMatches && descriptionMatches;
        }
    }
}
