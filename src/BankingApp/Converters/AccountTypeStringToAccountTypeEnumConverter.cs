using BankingApp.Models;
using Newtonsoft.Json;

namespace BankingApp.Converters;

// Reference: RMIT WDT Day7 Assignment 2 Workshop
public class AccountTypeStringToAccountTypeEnumConverter : JsonConverter<AccountType>
{
    public override void WriteJson(JsonWriter writer, AccountType value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override AccountType ReadJson(JsonReader reader, Type objectType, AccountType existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        // The type is a string in the JSON.
        var type = (string) reader.Value;

        // Convert the string to an enum.
        return type switch
        {
            "S" => AccountType.Saving,
            "C" => AccountType.Checking,
            _ => throw new InvalidOperationException($"Unknown AccountType: {type}")
        };
    }
}