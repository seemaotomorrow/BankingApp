namespace BankingApp.Tools.Utilities;

// Reference: RMIT WDT Week5-Lectorial McbaExample
public static class MiscellaneousExtensionUtilities
{
    private static bool HasMoreThanNDecimalPlaces(this decimal value, int n) => decimal.Round(value, n) != value;
    public static bool HasMoreThanTwoDecimalPlaces(this decimal value) => value.HasMoreThanNDecimalPlaces(2);
}
