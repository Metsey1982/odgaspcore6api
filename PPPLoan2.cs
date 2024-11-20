using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ODGAPI
{
    public class PPPLoan2
    {
        public int ID { get; set; } // Required property

        public string? LoanRange { get; set; } // Nullable property
        public string? BusinessName { get; set; } // Nullable property
        public string? Address { get; set; } // Nullable property
        public string? City { get; set; } // Nullable property
        public string? State { get; set; } // Nullable property
        public string? Zip { get; set; } // Nullable property
        public string? NaicsCode { get; set; } // Nullable property
        public string? BusinessType { get; set; } // Nullable property
        public string? RaceEthnicity { get; set; } // Nullable property
        public string? Gender { get; set; } // Nullable property
        public string? Veteran { get; set; } // Nullable property
        public string? NonProfit { get; set; } // Nullable property
        public int? JobsRetained { get; set; } // Nullable property
        public string? DateApproved { get; set; } // Nullable property
        public string? Lender { get; set; } // Nullable property
        public string? CD { get; set; } // Nullable property
    }
}
