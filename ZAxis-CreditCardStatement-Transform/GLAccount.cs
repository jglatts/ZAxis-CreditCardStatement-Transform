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
    public record GLAccount(
        string AccountType,
        bool Active,
        string Description,
        string AccountNumber);

    public static class GLAccounts
    {

        public static GLAccount? getGLAccountByNumber(string accountNumber)
        {
            return typeof(GLAccounts)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Select(field => field.GetValue(null) as GLAccount)
                .FirstOrDefault(account => account?.AccountNumber == accountNumber);
        }

        public static readonly GLAccount Telephone =
            new("Expenses", true, "Telephone", "60150");

        public static readonly GLAccount CellularTelephone =
            new("Expenses", true, "Cellular Telephone", "60152");

        public static readonly GLAccount Utilities =
            new("Expenses", true, "Utilities", "60160");

        public static readonly GLAccount WarehouseRental =
            new("Expenses", true, "Warehouse Rental", "60178");

        public static readonly GLAccount WarehouseExpense =
            new("Expenses", true, "Warehouse Expense", "60179");

        public static readonly GLAccount OfficeExpense =
            new("Expenses", true, "Office Expense", "60180");

        public static readonly GLAccount CheckPrintingExpense =
            new("Expenses", true, "Check Printing Expense", "60181");

        public static readonly GLAccount SoftwareExpense =
            new("Expenses", true, "Software Expense", "60182");

        public static readonly GLAccount SoftwareSupport =
            new("Expenses", true, "Software Support", "601821");

        public static readonly GLAccount ShippingSuppliesExpense =
            new("Expenses", true, "Shipping Supplies Expense", "60183");

        public static readonly GLAccount ShippingExpense =
            new("Expenses", true, "Shipping Expense", "601831");

        public static readonly GLAccount OnlineComputerServices =
            new("Expenses", true, "Online Computer Services", "60184");

        public static readonly GLAccount ComputerRepairs =
            new("Expenses", true, "Computer Repairs", "601841");

        public static readonly GLAccount NetworkAdministration =
            new("Expenses", true, "Network Administration", "601842");

        public static readonly GLAccount ProgrammingSoftware =
            new("Expenses", true, "Programming / Software", "60186");

        public static readonly GLAccount ShopSuppliesExpense =
            new("Expenses", true, "Shop Supplies Expense", "60187");

        public static readonly GLAccount ShopEquipment =
            new("Expenses", true, "Shop Equipment", "601871");

        public static readonly GLAccount CleanRoomSupplies =
            new("Expenses", true, "Clean Room Supplies", "60188");

        public static readonly GLAccount QualityAssurance =
            new("Expenses", true, "Quality Assurance", "60189");

        public static readonly GLAccount SubscriptionsAndMagazines =
            new("Expenses", true, "Subscriptions and Magazines", "60190");

        public static readonly GLAccount AutoAndTravel =
            new("Expenses", true, "Auto and Travel", "60200");

        public static readonly GLAccount MealsOnTheRoad =
            new("Expenses", true, "Meals on the Road", "60201");

        public static readonly GLAccount HandTools =
            new("Expenses", true, "Hand Tools", "60203");

        public static readonly GLAccount Tools =
            new("Expenses", true, "Tools", "60204");

        public static readonly GLAccount MovingExpense =
            new("Expenses", true, "Moving Expense", "60208");

        public static readonly GLAccount Meals =
            new("Expenses", true, "Meals", "60209");

        public static readonly GLAccount BusinessEntertainment =
            new("Expenses", true, "Business Entertainment", "60210");

        public static readonly GLAccount BusinessMembershipDues =
            new("Expenses", true, "Business Membership Dues", "60215");

        public static readonly GLAccount Consulting =
            new("Expenses", true, "Consulting", "60225");

        public static readonly GLAccount Advertising =
            new("Expenses", true, "Advertising", "60230");

        public static readonly GLAccount SampleShippingExpense =
            new("Expenses", true, "Sample Shipping Expense", "60231");

        public static readonly GLAccount SecuritySystem =
            new("Expenses", true, "Security System", "60232");

        public static readonly GLAccount OtherOperatingExpense =
            new("Expenses", true, "Other Operating Expense", "60260");

        public static readonly GLAccount MiscellaneousExpense =
            new("Expenses", true, "Miscellaneous Expense", "60270");

        public static readonly GLAccount SeminarsAndTraining =
            new("Expenses", true, "Seminars / Training", "60291");

        public static readonly GLAccount GeneralMaintenance =
            new("Expenses", true, "General Maintenance", "60292");

        public static readonly GLAccount EquipmentStorage =
            new("Expenses", true, "Equipment Storage", "60311");

        public static readonly GLAccount EquipmentMaintenanceAndRepairs =
            new("Expenses", true, "Equipment Maintenance and Repairs", "60320");

        public static readonly GLAccount PermitsAndLicenses =
            new("Expenses", true, "Permits and Licenses", "60340");

        public static readonly GLAccount ZWireResearchMaterialsAndEquipment =
            new("Expenses", true, "Z-Wire R&D Material and Equipment", "63000");

        public static readonly GLAccount ZWireResearchConsulting =
            new("Expenses", true, "Z-Wire R&D Consulting", "63100");

        public static readonly GLAccount ResearchSoftware =
            new("Expenses", true, "R&D Software", "63200");

        public static readonly GLAccount EbayOfficeExpense =
            new("Expenses", true, "eBay Office Expense", "86600");

        public static readonly GLAccount FlexibleTestExpense =
            new("Expenses", true, "Flexible Test Expense", "96000");
    }
}
