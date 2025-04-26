using RentMateAPI.Services.Implementations;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Implementation;
using RentMateAPI.UOF.Interface;


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

            return services;
        }
    }
}
