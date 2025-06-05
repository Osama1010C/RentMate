using System;
using System.Collections.Generic;

namespace RentMateAPI.Data.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Image { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<PropertyView> PropertyViews { get; set; } = new List<PropertyView>();

    public virtual ICollection<RentalRequest> RentalRequests { get; set; } = new List<RentalRequest>();

    public virtual ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();

    public virtual ICollection<TenantProperty> TenantProperties { get; set; } = new List<TenantProperty>();
}
