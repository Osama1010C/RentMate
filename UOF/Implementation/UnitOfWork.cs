using RentMateAPI.Data;
using RentMateAPI.Data.Models;
using RentMateAPI.Repositories.Implementations;
using RentMateAPI.Repositories.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.UOF.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public IRepository<User> Users { get; }
        public IRepository<Property> Properties { get; }
        public IRepository<RentalRequest> RentalRequests { get; }
        public IRepository<Comment> Comments { get; }
        public IRepository<Message> Messages { get; }
        public IRepository<PendingLandlord> PendingLandlord { get; }
        public IRepository<TenantProperty> TenantProperties { get; }
        public IRepository<SavedPost> SavedPosts { get; }
        public IRepository<PropertyView> PropertyViews { get; }
        public IRepository<PropertyImage> PropertyImages { get; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Users = new Repository<User>(_db);
            Properties = new Repository<Property>(_db);
            RentalRequests = new Repository<RentalRequest>(_db);
            Comments = new Repository<Comment>(_db);
            Messages = new Repository<Message>(_db);
            PendingLandlord = new Repository<PendingLandlord>(_db);
            TenantProperties = new Repository<TenantProperty>(_db);
            SavedPosts = new Repository<SavedPost>(_db);
            PropertyViews = new Repository<PropertyView>(_db);
            PropertyImages = new Repository<PropertyImage>(_db);
        }

        public async Task<int> CompleteAsync() => await _db.SaveChangesAsync();

        public void Dispose() => _db.Dispose();

    }
}
