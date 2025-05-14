using Madhu.Models;

namespace Madhu.Interfaces
{
    public interface IProductService
    {
        public IEnumerable<Product> GetProducts();

        public Product GetProduct(string sku);

    }
}
