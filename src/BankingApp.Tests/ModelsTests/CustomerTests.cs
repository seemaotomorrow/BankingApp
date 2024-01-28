using System.ComponentModel.DataAnnotations;
using BankingApp.Models;

namespace BankingApp.Tests.ModelsTests;

public class CustomerTests
{
    [Theory]
    [MemberData(nameof(GetValidCustomerData))]
    public void Customer_ValidParameters_Success(CustomerTestData testData)
    {
        // Arrange
        var customer = testData.Customer;

        var validationContext = new ValidationContext(customer);
        var validationResults = new List<ValidationResult>();
        
        // Act
        var isValid = Validator.TryValidateObject(customer, validationContext, validationResults, true);
        
        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }
    
    [Theory]
    [MemberData(nameof(GetInvalidCustomerData))]
    public void Customer_InvalidParameters_Fail(CustomerTestData testData)
    {
        // Arrange
        var customer = testData.Customer;

        var validationContext = new ValidationContext(customer);
        var validationResults = new List<ValidationResult>();
        
        // Act
        var isValid = Validator.TryValidateObject(customer, validationContext, validationResults, true);
        
        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(validationResults);
    }

    public static IEnumerable<object[]> GetValidCustomerData()
    {
        yield return new object[] { new CustomerTestData(1000, "John Doe") };
        yield return new object[] { new CustomerTestData(1111, "Jane Smith", TFN: "111 222 333") };
        yield return new object[] { new CustomerTestData(9999, "Mingjian Mao", Address: "123 Main St") };
        yield return new object[] { new CustomerTestData(1000, "Matthew Bolger", City: "Cityville") };
        yield return new object[] { new CustomerTestData(1000, "Hannah Johnson", PostCode: "3053") };
        yield return new object[] { new CustomerTestData(1000, "James Brown", State: "VIC") };
        yield return new object[] { new CustomerTestData(1000, "Sarah White", Mobile: "0411 111 111") };
    }

    public static IEnumerable<object[]> GetInvalidCustomerData()
    {
        yield return new object[] { new CustomerTestData(100, "Yahui Wang") };
        yield return new object[] { new CustomerTestData(10000, "Jane Smith", TFN: "111 222 333") };
        yield return new object[] { new CustomerTestData(99999, "Mingjian Mao", Address: "This is a very long address") };
        yield return new object[] { new CustomerTestData(1000, "Matthew Bolger", City: "This is a very long city name This is a very long city name") };
        yield return new object[] { new CustomerTestData(1000, "Hannah Johnson", PostCode: "City123") };
        yield return new object[] { new CustomerTestData(1000, "James Brown", State: "Victoria") };
        yield return new object[] { new CustomerTestData(1000, "Sarah White", Mobile: "0411 111 1111") };
    }

    public record CustomerTestData(int CustomerID, string Name, string? TFN = null, string? Address = null, string? City = null, string? PostCode = null, string? State = null, string? Mobile = null)
    {
        public Customer Customer => new Customer
        {
            CustomerID = CustomerID,
            Name = Name,
            TFN = TFN,
            Address = Address,
            City = City,
            PostCode = PostCode,
            State = State,
            Mobile = Mobile
        };
    }
}
