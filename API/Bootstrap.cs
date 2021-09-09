using API.Business;
using API.Business.Utilities;
using API.Contracts.Business;
using API.Contracts.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API
{
    public static class Bootstrap
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IInventoryBusiness, InventoryBusiness>();
            services.AddScoped<IItemTypeBusiness, ItemTypeBusiness>();
            services.AddScoped<IOrderBusiness, OrderBusiness>();
            services.AddScoped<IShoppingCartItemBusiness, ShoppingCartItemBusiness>();

            services.AddScoped<IDataUtility, DataUtility>();
        }
    }
}
