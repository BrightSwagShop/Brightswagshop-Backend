using System;

namespace FakeWebShop.Domain.Abstractions.Storage;

public interface IImageStorage
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteImageAsync(string imagePath);
}
