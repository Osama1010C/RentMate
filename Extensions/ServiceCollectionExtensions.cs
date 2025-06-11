using RentMateAPI.Repositories.Implementations;
using RentMateAPI.Repositories.Interfaces;
using RentMateAPI.Services.Implementations;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Implementation;
using RentMateAPI.UOF.Interface;
using RentMateAPI.Validations.Implementations;
using RentMateAPI.Validations.Interfaces;


namespace RentMateAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IRentalService, RentalService>();
            services.AddScoped<ISavedPostService, SavedPostService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPendingLandlordService, PendingLandlordService>();
            services.AddScoped<IHistoryService, HistoryService>();
            services.AddScoped<INotificationService, NotificationService>();

            services.AddScoped<IImageValidator, ImageValidator>();
            services.AddScoped<IFileValidator, FileValidator>();
            services.AddScoped(typeof(IModelValidator<>), typeof(ModelValidator<>)); // typeof() because we need to use generic type

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // typeof() because we need to use generic type



            return services;
        }
    }
}
