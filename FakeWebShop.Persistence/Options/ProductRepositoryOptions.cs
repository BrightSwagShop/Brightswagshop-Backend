using System;

namespace FakeWebShop.Persistence.Options;

public class ProductRepositoryOptions
{
    public const string SectionName = "ProductRepository";
    public string ConnectionString { get; set; } = "";
}
