using Frontend.Services;
using Frontend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers;

public class ProductController(IProductService productService) : Controller
{
    public async Task<IActionResult> ProductIndex()
    {
        var products = await productService.FindAllProducts();
        return View(products);
    }

    public IActionResult ProductCreate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await productService.CreateProduct(model);
            if (response != null)
                return RedirectToAction(nameof(ProductIndex));
        }
        return View(model);
    }

    public async Task<IActionResult> ProductUpdate(long id)
    {
        var model = await productService.FindProductById(id);
        return model != null ? View(model) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductUpdate(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await productService.UpdateProduct(model);
            if (response != null)
                return RedirectToAction(nameof(ProductIndex));
        }
        return View(model);
    }

    public async Task<IActionResult> ProductDelete(int id)
    {
        var model = await productService.FindProductById(id);
        return model != null ? View(model) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductViewModel model)
    {
        var response = await productService.DeleteProductById(model.Id);
        return response ? RedirectToAction(nameof(ProductIndex)) : View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View("Error!");
}