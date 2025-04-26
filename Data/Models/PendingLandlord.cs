using System;
using System.Collections.Generic;

namespace RentMateAPI.Data.Models;

public partial class PendingLandlord
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Image { get; set; }
}
