namespace RentMateAPI.Data.Models;

public partial class SavedPost
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int PropertyId { get; set; }

    public virtual Property Property { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;
}
