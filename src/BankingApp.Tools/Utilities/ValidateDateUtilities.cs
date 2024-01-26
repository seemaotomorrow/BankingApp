using System.ComponentModel.DataAnnotations;

namespace BankingApp.Tools.Utilities;

public class ValidateDateUtilities : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime > DateTime.UtcNow)
                return true;
        }

        return false; // Return false if the value is not a DateTime
    }
    
    public static bool IsFuture(DateTime date)
    {
        return date > DateTime.UtcNow;
    }

    public static bool IsNow(DateTime date)
    {
        return date == DateTime.UtcNow;
    }
}