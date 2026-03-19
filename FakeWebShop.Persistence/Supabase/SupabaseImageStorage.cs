using System;
using FakeWebShop.Persistence.Supabase.SupabaseSettings;
using Microsoft.Extensions.Options;
using FakeWebShop.Domain.Abstractions.Storage;
using Supabase;

namespace FakeWebShop.Persistence.Supabase;

public class SupabaseImageStorage : IImageStorage
{
    private readonly Client _client;
    private readonly SupabaseStorageSettings _settings;

    public SupabaseImageStorage(IOptions<SupabaseStorageSettings> options)
    {
        _settings = options.Value;

        var supabaseOptions = new SupabaseOptions
        {
            AutoConnectRealtime = false
        };

        _client = new Client(_settings.Url, _settings.ServiceRoleKey, supabaseOptions);
        _client.InitializeAsync().Wait();
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        var extension = Path.GetExtension(fileName);
        var newFileName = $"{Guid.NewGuid()}{extension}";
        var path = $"{_settings.FolderName}/{newFileName}";

        var tempFile = Path.GetTempFileName();

        await using (var fs = File.Create(tempFile))
        {
            await fileStream.CopyToAsync(fs);
        }

        await _client.Storage
            .From(_settings.BucketName)
            .Upload(tempFile, path);

        File.Delete(tempFile);

        return _client.Storage
            .From(_settings.BucketName)
            .GetPublicUrl(path);
    }

    public async Task DeleteImageAsync(string imagePath)
    {
        await _client.Storage
            .From(_settings.BucketName)
            .Remove(new List<string> { imagePath });
    }
}