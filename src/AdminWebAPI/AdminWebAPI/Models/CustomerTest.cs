using BankingApp.Tools.Utilities;

namespace AdminWebAPI;

public class CustomerTest
{


    public int CustomerID { get; set; }

    public string Name { get; set; }

    public string? TFN { get; set; }

   
    public string? Address { get; set; }


    public string? City { get; set; }


    public string? State { get; set; }
    //public Dictionary<string, string> States { get; set; } = AustralianStates.States;

  

    public string? PostCode { get; set; }

    public string? Mobile { get; set; }

    public bool IsLocked { get; set; }

      
}

