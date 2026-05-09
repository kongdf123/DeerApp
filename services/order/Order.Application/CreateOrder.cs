using Order.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application
{
    public class CreateOrder
    {
        private readonly AppDbContext _db;
        private readonly CatalogApiClient _catalog;

        public CreateOrder(
            AppDbContext db,
            CatalogApiClient catalog)
        {
            _db = db;
            _catalog = catalog;
        }

        public async Task<Domain.Order?> Execute(
            long productId,
            int quantity)
        {
            var product = await _catalog.GetProduct(productId);

            if (product is null)
                return null;

            var order = new Domain.Order
            {
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = product.Price
            };

            _db.Orders.Add(order);

            await _db.SaveChangesAsync();

            return order;
        }
    }
}
