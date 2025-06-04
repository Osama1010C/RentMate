using System;
using System.Collections.Generic;

namespace RentMateAPI.Data.Models;

public partial class PropertyView
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PropertyId { get; set; }

    public virtual Property Property { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
