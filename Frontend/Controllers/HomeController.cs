using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Frontend.ViewModels;
using Frontend.Services;

namespace Frontend.Controllers;

public class HomeController(IProductService productService, ICartService cartService) : Controller
{
    public async Task<IActionResult> Index() => View(await productService.FindAllProducts());
    public async Task<IActionResult> ProductDetails(long id) => View(await productService.FindProductById(id));

    [HttpPost]
    [ActionName("ProductDetails")]
    public async Task<IActionResult> DetailsPost(ProductViewModel model)
    {
        CartViewModel cart = new()
        {
            Header = new()
            {
                // TODO: adjust when Identity Server will be implemented 
                UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value ?? "1"
            },
            Details = [new()
            {
                Count = model.Count,
                Product = await productService.FindProductById(model.Id)
            }]
        };

        var response = await cartService.AddItemToCart(cart);
        if (response != null)
            return RedirectToAction(nameof(Index));

        return View(model);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
