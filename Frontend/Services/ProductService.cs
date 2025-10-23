using Frontend.Utils;
using Frontend.ViewModels;

namespace Frontend.Services;

public class ProductService(HttpClient client) : IProductService
{
    private const string BasePath = "api/v1/product";

    public async Task<IEnumerable<ProductViewModel>> FindAllProducts()
    {
        var response = await client.GetAsync(BasePath);
        return await response.ReadContentAs<List<ProductViewModel>>();
    }

    public async Task<ProductViewModel> FindProductById(long id)
    {
        var response = await client.GetAsync($"{BasePath}/{id}");
        return await response.ReadContentAs<ProductViewModel>();
    }

    public async Task<ProductViewModel> CreateProduct(ProductViewModel model)
    {
        var response = await client.PostAsJson(BasePath, model);

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<ProductViewModel>();
        else
            throw new Exception("Something went wrong when calling API");
    }

    public async Task<ProductViewModel> UpdateProduct(ProductViewModel model)
    {
        var response = await client.PutAsJson(BasePath, model);

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<ProductViewModel>();
        else
            throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> DeleteProductById(long id)
    {
        var response = await client.DeleteAsync($"{BasePath}/{id}");

        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        else
            throw new Exception("Something went wrong when calling API");
    }
}