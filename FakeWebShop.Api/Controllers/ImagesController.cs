using FakeWebShop.Domain.Abstractions.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private readonly IImageStorage _imageStorage;

    public ImagesController(IImageStorage imageStorage)
    {
        _imageStorage = imageStorage;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Geen bestand ontvangen.");

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };

        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest("Alleen jpg, png en webp toegestaan.");

        await using var stream = file.OpenReadStream();

        var imageUrl = await _imageStorage.UploadImageAsync(
            stream,
            file.FileName,
            file.ContentType);

        return Ok(new { imageUrl });
    }
}