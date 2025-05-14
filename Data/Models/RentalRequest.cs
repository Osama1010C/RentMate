namespace RentMateAPI.Data.Models;

public partial class RentalRequest
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int PropertyId { get; set; }

    public byte[] RequirmentDocument { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public virtual Property Property { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;
}
