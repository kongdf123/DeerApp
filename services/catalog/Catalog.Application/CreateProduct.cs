using Catalog.Domain;
using Catalog.Infrastructure;

namespace Catalog.Application
{
    public class CreateProduct
    {
        private readonly AppDbContext _db;

        public CreateProduct(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Product> Execute(string name, decimal price)
        {
            var product = new Domain.Product
            {
                Name = name,
                Price = price
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return product;
        }
    }
}
