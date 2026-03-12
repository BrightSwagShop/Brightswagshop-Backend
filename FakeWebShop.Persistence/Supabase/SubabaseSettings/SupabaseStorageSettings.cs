using System;

namespace FakeWebShop.Persistence.Supabase.SupabaseSettings;

public sealed class SupabaseStorageSettings
{
    public const string SectionName = "Supabase";

    public string Url { get; set; } = string.Empty;
    public string AnonKey { get; set; } = string.Empty;
    public string ServiceRoleKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = "images";
    public string FolderName { get; set; } = "products";
}
