using System.ComponentModel.DataAnnotations;
using BankingApp.Models;

namespace BankingApp.Tests.ModelsTests;

public class BillPayTests
{
    [Theory]
    [InlineData(1001, 1, 50.25, "2023-01-01T00:00:00", 'O', BillPayStatus.Scheduled)]
    [InlineData(1002, 2, 100.50, "2023-02-01T00:00:00", 'M', BillPayStatus.Scheduled)]
    public void BillPay_ValidParameters_Success(int accountNumber, int payeeId, decimal amount, string scheduleTimeUtcString, char period, BillPayStatus status)
    {
        // Arrange
        var billPay = new BillPay
        {
            AccountNumber = accountNumber,
            PayeeID = payeeId,
            Amount = amount,
            ScheduleTimeUtc = DateTime.Parse(scheduleTimeUtcString),
            Period = period,
            Status = status
        };

        // Act
        var validationResults = ValidateModel(billPay);

        // Assert
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(0, 1, 0.0001, "2023-01-01T00:00:00", 'O', BillPayStatus.Scheduled)]
    [InlineData(1001, 0, 50.25, "2023-01-01T00:00:00", '0', BillPayStatus.Scheduled)]
    [InlineData(1001, 1, -50.25, "2023-01-01T00:00:00", 'O', BillPayStatus.Scheduled)]
    [InlineData(1001, 1, 50.25, "2023-01-01T00:00:00", 'X', BillPayStatus.Scheduled)]
    public void BillPay_InvalidProperties_ShouldFailValidation(int accountNumber, int payeeId, decimal amount, string scheduleTimeUtcString, char period, BillPayStatus status)
    {
        // Arrange
        var billPay = new BillPay
        {
            AccountNumber = accountNumber,
            Amount = amount,
            ScheduleTimeUtc = DateTime.Parse(scheduleTimeUtcString),
            Period = period,
            Status = status
        };

        // Act
        var validationResults = ValidateModel(billPay);

        // Assert
        Assert.NotEmpty(validationResults);
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}

