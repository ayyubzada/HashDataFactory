using DataGeneratorApi.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DataGeneratorApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HashesController : ControllerBase
{
    private readonly IHashService _hashService;

    public HashesController(IHashService hashService)
    {
        _hashService = hashService;
    }

    [HttpPost]
    public async Task<IActionResult> PostData()
    {
        var count = await _hashService.GenerateAndSendHashesAsync(40000);
        return Ok(count);
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var reuslt = await _hashService.GetGroupedDataByDates();
        return Ok(reuslt);
    }
}
