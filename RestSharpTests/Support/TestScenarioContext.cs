using RestSharp;

namespace RestSharpTests.Support;

/// <summary>
/// Shared state bag for a single scenario execution.
/// Holds the last HTTP response, parsed body, and any IDs created during the scenario.
/// </summary>
public class TestScenarioContext
{
    // Last raw RestSharp response
    public RestResponse? LastResponse { get; set; }

    // Last parsed body as dynamic JSON (Newtonsoft JToken)
    public Newtonsoft.Json.Linq.JToken? LastBody { get; set; }

    // Last response body as plain text
    public string? LastBodyText { get; set; }

    // Products
    public string? CreatedProductId { get; set; }
    public object? Payload { get; set; }

    // Users
    public string? UserUsername { get; set; }
    public string? UserPassword { get; set; }

    // Shopping cart
    public object? CartRequest { get; set; }
    public string? RememberedCartId { get; set; }

    // Discount
    public string? AuthCreatedDiscountId { get; set; }
    public string? DiscountCartId { get; set; }

    // Payments
    public string? CreatedOrderId { get; set; }
}
