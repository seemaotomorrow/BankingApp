using AdminWebAPI.Models;
using AdminWebAPI.Models;
using Newtonsoft.Json;

namespace AdminWebAPI.Utilities;

// Reference: RMIT WDT Day7 Assignment 2 Workshop
public class AccountTypeStringToAccountTypeEnumConverter : JsonConverter<AccountType>
{
    public override void WriteJson(JsonWriter writer, AccountType value, JsonSerializer serializer)
    {
        string typeStr = value switch
        {
            AccountType.Saving => "S",
            AccountType.Checking => "C",
            _ => throw new InvalidOperationException($"Unknown AccountType: {value}")
        };
        writer.WriteValue(typeStr);
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