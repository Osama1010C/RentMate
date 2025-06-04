using System;
using System.Collections.Generic;

namespace RentMateAPI.Data.Models;

public partial class PropertyImage
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public byte[]? Image { get; set; }

    public virtual Property Property { get; set; } = null!;
}
