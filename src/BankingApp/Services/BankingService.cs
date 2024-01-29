using BankingApp.Models;
using BankingApp.Repositories;
using BankingApp.Tools.Utilities;

namespace BankingApp.Services;

public class ValidationError
{
    public required string PropertyName { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IBankingService
{
    public void Deposit(int accountNumber, decimal amount, string comment);
    public void Withdraw(int accountNumber, decimal amount, string comment);
    public void Transfer(int sourceAccountNumber, int destinationAccountNumber, decimal amount, string comment);
    public IEnumerable<ValidationError> ValidateAmount(decimal amount);
    public IEnumerable<ValidationError> ValidateComment(string? comment);
    public IEnumerable<ValidationError> ValidateBalanceSufficientToCoverServiceFee(int accountNumber, decimal amount, TransactionType transactionType);
    public IEnumerable<ValidationError> ValidateSourceAccAndDestinationAccDifferent(int accountNumber, int destinationAccountNumber);
    public IEnumerable<ValidationError> ValidateDestinationAccount(int destinationAccountNumber);
}


public class BankingService : IBankingService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private decimal AccountTransferServiceCharge { get; init; } = 0.10m;
    private decimal AtmWithdrawalServiceCharge { get; init; } = 0.05m;


    public BankingService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public void Deposit(int accountNumber, decimal amount, string? comment)
    {
        // Logic to deposit funds into the account & add transaction
        var account = _accountRepository.GetAccount(accountNumber);
        account.Balance += amount;
        _accountRepository.UpdateAccount(account);
        var transaction = new Transaction
        {
            TransactionType = TransactionType.Deposit,
            AccountNumber = account.AccountNumber,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.UtcNow
        };
        _transactionRepository.AddTransaction(transaction);
        
    }

    public void Withdraw(int accountNumber, decimal amount, string? comment)
    {
        var account = _accountRepository.GetAccount(accountNumber);
        
        // Check if apply service charge
        decimal serviceCharge = 0;
        bool applyServiceCharge = !(account.HasFreeTransaction());
        if (applyServiceCharge)
            serviceCharge = AtmWithdrawalServiceCharge;
        
        // Logic to withdraw funds from the account
        account.Balance -= (amount+ serviceCharge);
        _accountRepository.UpdateAccount(account);
        
        Transaction transaction = new Transaction
        {
            TransactionType = TransactionType.Withdraw,
            AccountNumber = account.AccountNumber,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.UtcNow
        };
        _transactionRepository.AddTransaction(transaction);

        
        var serviceChargeTransaction = account.ApplyServiceCharge(serviceCharge, applyServiceCharge);
        if(serviceChargeTransaction != null)
            _transactionRepository.AddTransaction(serviceChargeTransaction);
    }


    public void Transfer(int sourceAccountNumber, int destinationAccountNumber, decimal amount, string? comment)
    {
        var sourceAccount = _accountRepository.GetAccount(sourceAccountNumber);
        var destinationAccount = _accountRepository.GetAccount(destinationAccountNumber);
        // Check if apply service charge
        decimal serviceCharge = 0;
        bool applyServiceCharge = !(sourceAccount.HasFreeTransaction());
        if (applyServiceCharge)
            serviceCharge = AccountTransferServiceCharge;
        
        // Logic to transfer funds between accounts
        sourceAccount.Balance -= (amount + serviceCharge);
        destinationAccount.Balance += amount;
        
        _accountRepository.UpdateAccount(sourceAccount);
        _accountRepository.UpdateAccount(destinationAccount);
        
        _transactionRepository.AddTransaction(new Transaction
        {
            TransactionType = TransactionType.TransferOut,
            AccountNumber = sourceAccount.AccountNumber,
            DestinationAccountNumber = destinationAccountNumber,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.UtcNow
        });
        
        _transactionRepository.AddTransaction(new Transaction
        {
            TransactionType = TransactionType.TransferIn,
            AccountNumber = destinationAccount.AccountNumber,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.UtcNow
        });
        
        var serviceChargeTransaction = sourceAccount.ApplyServiceCharge(serviceCharge, applyServiceCharge);
        if(serviceChargeTransaction != null)
            _transactionRepository.AddTransaction(serviceChargeTransaction);
    }

    public IEnumerable<ValidationError> ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            yield return new ValidationError { PropertyName = nameof(amount), ErrorMessage = "Amount must be positive." };
        }
        else if (amount.HasMoreThanTwoDecimalPlaces())
        {
            yield return new ValidationError { PropertyName = nameof(amount), ErrorMessage = "Amount cannot have more than 2 decimal places." };
        }
    }

    public IEnumerable<ValidationError> ValidateComment(string? comment)
    {
        if (!string.IsNullOrEmpty(comment) && comment.Length > 30)
        {
            yield return new ValidationError { PropertyName = nameof(comment), ErrorMessage = "Comment exceeded maximum length of 30 characters." };
        }
    }

    public IEnumerable<ValidationError> ValidateSourceAccAndDestinationAccDifferent(int accountNumber,
        int destinationAccountNumber)
    {
        if (accountNumber == destinationAccountNumber)
        {
            yield return new ValidationError {PropertyName = nameof(destinationAccountNumber), ErrorMessage = "Source and destination accounts cannot be the same."};
        }
    }

    public IEnumerable<ValidationError> ValidateDestinationAccount(int destinationAccountNumber)
    {
        var destinationAccount = _accountRepository.GetAccount(destinationAccountNumber);
        if (destinationAccount == null)
        {
            yield return new ValidationError { PropertyName = nameof(destinationAccountNumber), ErrorMessage = "Invalid destination account."};
        }
    }
    
    // Validate if it's sufficient to cover service fee
    public IEnumerable<ValidationError> ValidateBalanceSufficientToCoverServiceFee(int accountNumber, decimal amount, TransactionType transactionType)
    {
        var account = _accountRepository.GetAccount(accountNumber);
        
        decimal serviceCharge = 0;
        bool applyServiceCharge = !(account.HasFreeTransaction());
        if (applyServiceCharge)
        {
            if(transactionType == TransactionType.Withdraw)
                serviceCharge = AtmWithdrawalServiceCharge;
            if(transactionType == TransactionType.TransferOut)
                serviceCharge = AccountTransferServiceCharge;
        }
        var sufficient = account.Balance - (amount + serviceCharge) > account.MinimumBalanceAllowed;
        if (!sufficient)
        {
            yield return new ValidationError { PropertyName = nameof(amount), ErrorMessage = "Insufficient funds to cover service fee and withdrawal." };
        }
    }
}