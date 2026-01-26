using AutoMapper;
using ProductAPI.DBContext;
using ProductAPI.DTO;
using ProductAPI.Models;

namespace ProductAPI.Repositories;

public class ProductRepository(PgSQLContext context, IMapper mapper) : IProductRepository
{
    public IEnumerable<ProductDTO> FindAll()
    {
        var products = context.Products.ToList();
        return mapper.Map<IEnumerable<ProductDTO>>(products);
    }

    public ProductDTO FindById(long id)
    {
        var product = context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return null;
        return mapper.Map<ProductDTO>(product);
    }

    public ProductDTO Create(ProductDTO dto)
    {
        context.Products.Add(mapper.Map<Product>(dto));
        context.SaveChanges();
        return dto;
    }

    public ProductDTO Update(ProductDTO dto)
    {
        context.Products.Update(mapper.Map<Product>(dto));
        context.SaveChanges();
        return dto;
    }

    public bool Delete(long id)
    {
        var product = context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return false;
        context.Products.Remove(product);
        context.SaveChanges();
        return true;
    }
}