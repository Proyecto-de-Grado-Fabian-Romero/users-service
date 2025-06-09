using Microsoft.AspNetCore.Mvc;
using UsersService.Src.Application.DTOs.BankPaymentData;
using UsersService.Src.Application.Interfaces;

namespace UsersService.Src.WebApi.Controllers;

[ApiController]
[Route("api/bank-payment")]
public class BankPaymentDataController(IBankPaymentDataService service) : ControllerBase
{
    private readonly IBankPaymentDataService _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBankPaymentDataDto dto)
    {
        var publicIdStr = Request.Cookies["publicId"];
        if (!Guid.TryParse(publicIdStr, out var publicId))
        {
            return Unauthorized("No public Id provided.");
        }

        await _service.CreateAsync(dto, publicId);
        return Ok("Datos bancarios guardados.");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateBankPaymentDataDto dto)
    {
        var publicIdStr = Request.Cookies["publicId"];
        if (!Guid.TryParse(publicIdStr, out var publicId))
        {
            return Unauthorized("No public Id provided.");
        }

        await _service.UpdateAsync(dto, publicId);
        return Ok("Datos bancarios actualizados.");
    }
}
