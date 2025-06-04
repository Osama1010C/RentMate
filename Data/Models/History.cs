using System;
using System.Collections.Generic;

namespace RentMateAPI.Data.Models;

public partial class History
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Description { get; set; } = null!;

    public string HistoryType { get; set; } = null!;

    public DateTime? ActionDate { get; set; }
}
