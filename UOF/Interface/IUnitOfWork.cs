using RentMateAPI.Data.Models;
using RentMateAPI.Repositories.Interfaces;

namespace RentMateAPI.UOF.Interface
{
    public interface IUnitOfWork
    {

        IRepository<User> Users { get; }
        IRepository<Property> Properties { get; }
        IRepository<RentalRequest> RentalRequests { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Message> Messages { get; }
        IRepository<PendingLandlord> PendingLandlord { get; }
        IRepository<TenantProperty> TenantProperties { get; }
        IRepository<SavedPost> SavedPosts { get; }
        IRepository<PropertyView> PropertyViews { get; }
        IRepository<PropertyImage> PropertyImages { get; }
        IRepository<History> Histories { get; }
        IRepository<Notification> Notifications { get; }

        Task<int> CompleteAsync();
    }
}
