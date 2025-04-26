using System;
using System.Collections.Generic;

namespace RentMateAPI.Data.Models;

public partial class TenantProperty
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int PropertyId { get; set; }

    public virtual Property Property { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;
}
