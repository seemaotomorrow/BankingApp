# Info 
This is a 2 member group work
- StudentID: s3968520 Mingjian Mao/ s3911598 Yahui Wang (Postgraduate)

- GitHub Repository URL: https://github.com/rmit-wdt-summer-2024/s3968520-s3911598-a2

# Banking Application
### Application Features
- Check account balance and Statements
- Simulate transactions such as deposits and withdrawals
- Transfer money between accounts
- Modify a personal profile
- Change login password
- Schedule billpayments
- Perform Administrative tasks:
  - Modify a customer's profile
  - Lock and unlock a customer's login
 
    

### BankingApp structure
- AdminWebAPI

- AdminWebsite
  
- BankingApp
  - BackgroundServices
  - Controllers
  - Data
  - Filters
  - Migrations
  - Models
  - Repositories
  - Service
  - Utilities
  - ViewModels
  - Views

- BankingApp.Tools
      
- AdminWebAPITest
- AdminWebisteTest
- BankingApp.Test


### BillPay Feature
The bill pay functionality in our banking application allows users to schedule and manage their bill payments conveniently. We leverage Hangfire as a background job service to automate the processing of scheduled bill payments when it reach the user scheduled times

### Database/Data
- Used EF Core's LINQ syntax
- Hard-code Payees data when seeding data
- Added 'Status' column in BillPays schema
- Added 'isLocked' column in Logins schema


### Reference:
- RMIT A2 workshop: https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/Assignment2-Workshop-JSON-Helper-Code.txt
- RMIT WDT Day5-Lectorial McbaExample
- RMIT WDT Day6-Lab McbaExampleWithLogin
- RMIT WDT Day10 -Lectorial MagicInventory.Tests
- .gitignore: https://github.com/dotnet/core/blob/main/.gitignore
