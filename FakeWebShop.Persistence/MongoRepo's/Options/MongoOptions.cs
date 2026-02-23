using System;

namespace FakeWebShop.Persistence.MongoRepo_s.Options;

public class MongoOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Database { get; set; } = default!;
}
