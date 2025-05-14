using Madhu.Interfaces;
using Madhu.Models;

namespace Madhu.Services
{
    public class ProductService : IProductService

    {
        public Product GetProduct(string sku)
        {
            return GetProducts().Where(w => w.Sku == sku).FirstOrDefault();
        }

        public IEnumerable<Product> GetProducts()
        {
            return new List<Product>
                {
                        new Product{Sku = "iphone 10 Max", Name="iphone 10 Max", Price=1,ImageUrl="Iphone10.jpg"},
                        new Product{Sku = "Samsung Galaxy S24 Ultra", Name="Samsung Galaxy S24 Ultra", Price=2,ImageUrl="Sasumsung.png"},
                         new Product{Sku = "Redmi 14", Name="Redmi 14", Price=3, ImageUrl = "redmi.png"},
                         new Product{Sku = "iphone 16 Max PRO", Name="iphone 16 Max PRO", Price=4,ImageUrl="iphone16.png"},
                          new Product{Sku = "Vivo V17", Name="Vivo V17", Price=1,ImageUrl="vivo.png"},
                };
        }


    }
}
