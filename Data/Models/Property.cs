namespace RentMateAPI.Data.Models;

public partial class Property
{
    public int Id { get; set; }

    public int? LandlordId { get; set; }

    public string Title { get; set; } = null!;

    public byte[] MainImage { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public string Location { get; set; } = null!;

    public int? Views { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public string PropertyApproval { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User? Landlord { get; set; }

    public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();

    public virtual ICollection<PropertyView> PropertyViews { get; set; } = new List<PropertyView>();

    public virtual ICollection<RentalRequest> RentalRequests { get; set; } = new List<RentalRequest>();

    public virtual ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();

    public virtual ICollection<TenantProperty> TenantProperties { get; set; } = new List<TenantProperty>();
}
