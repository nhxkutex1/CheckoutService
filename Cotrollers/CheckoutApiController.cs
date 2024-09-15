using CheckoutService_TestTask.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutService_TestTask.Cotrollers;

[ApiController]
[Route("api/checkout")]
public class CheckoutApiController : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult CalculateTotal([FromBody] Order orderModel)
    {
        var order = new Order(orderModel.Items);
        var total = order.GetTotal();

        return Ok(new { total });
    }
}
