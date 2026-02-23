using System;

namespace FakeWebShop.Domain.Model;

public class CategoryModel
{
      public Guid? Id { get; set; }
    public string Naam { get; set; } = string.Empty;

    public List<ProductModel> ProductModels { get; set; } = [];

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Naam);
    }

}
