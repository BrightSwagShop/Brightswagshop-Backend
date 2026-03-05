using FakeWebShop.Api.Controllers;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Services.MongoInterfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FakeWebShop.Contracts.Response;
using FakeWebShop.Contracts.Request; 
using FakeWebShop.Domain.Enums; 

namespace FakeWebShop.Api.Tests.Controllers;

public class MongoProductsControllerTests
{
    // Mock van de service (fake versie van IMongoProductService)
    // Strict = test faalt als een methode wordt aangeroepen die niet gesetup is
    private readonly Mock<IMongoProductService> _service = new(MockBehavior.Strict);

    // Controller die we gaan testen
    private readonly MongoProductsController _controller;

    // Constructor van de testklasse
    // Hier injecteren we de fake service in de controller
    public MongoProductsControllerTests()
    {
        _controller = new MongoProductsController(_service.Object);
    }

    // ============================
    // TEST 1: GET ALL
    // ============================

    [Fact]
    public async Task GetAll_ReturnsOk_WithList()
    {
        // Arrange
        // We maken een fake lijst die de service zou teruggeven
        var list = new List<MongoProductResponse>
        {
            new MugProductResponse
            {
                Id = "507f1f77bcf86cd799439011",
                Kleuren = new()
            }
        };

        // We zeggen tegen Moq:
        // Als GetProducts() wordt aangeroepen, geef deze lijst terug
        _service.Setup(s => s.GetProducts()).ReturnsAsync(list);

        // Act
        // We roepen de controller methode aan
        var result = await _controller.GetAll();

        // Assert
        // We controleren dat het resultaat een 200 OK is
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;

        // Statuscode moet 200 zijn
        ok.StatusCode.Should().Be(200);

        // Body moet gelijk zijn aan de lijst die we teruggaven
        ok.Value.Should().BeEquivalentTo(list);

        // Controleer dat alle verwachte calls zijn gebeurd
        _service.VerifyAll();
    }

    // ============================
    // TEST 2: GET BY ID → NOT FOUND
    // ============================
    // We controlleren of de controller het juiste antwoord geeft 
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNull()
    {
        var id = "507f1f77bcf86cd799439011";

        // Service geeft null terug → product bestaat niet
        _service.Setup(s => s.GetProductById(id))
                .ReturnsAsync((MongoProductResponse?)null);

        var result = await _controller.GetById(id);

        // Controller moet 404 teruggeven
        result.Result.Should().BeOfType<NotFoundResult>();

        _service.VerifyAll();
    }

    // ============================
    // TEST 3: GET BY ID → OK
    // ============================
    //We controleren of de controller een 200 OK teruggeeft met het juiste product wanneer het product wél gevonden wordt.
    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var id = "507f1f77bcf86cd799439011";
       var product = new MugProductResponse
        {
            Id = id,
            Kleuren = new()
        };

        // Service geeft product terug
        _service.Setup(s => s.GetProductById(id))
                .ReturnsAsync(product);

        var result = await _controller.GetById(id);

        // Controller moet 200 OK teruggeven
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;

        ok.StatusCode.Should().Be(200);

        // Body moet gelijk zijn aan het product
        ok.Value.Should().BeEquivalentTo(product);

        _service.VerifyAll();
    }

    // ============================
    // TEST 4: CREATE → 201 Created
    // ============================
    //We controleren of de controller een 201 Created teruggeeft met de juiste route en het aangemaakte product 
    // wanneer een nieuw product succesvol wordt aangemaakt.
    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Fake request object
         var request = new MugProductRequest
        {
            Name = "Test mug",
            Price = 10m,
            Category = CategoryEnum.Mugs,
            ProductType = ProductTypeEnum.Mug,
            IsActive = true,
            Kleuren = new()
        };

        // Fake response dat service zou teruggeven
        var created = new MongoProductResponse
        {
            Id = "507f1f77bcf86cd799439011"
        };

        // Service moet created object teruggeven
        _service.Setup(s => s.CreateProduct(request))
                .ReturnsAsync(created);

        var result = await _controller.Create(request);

        // Controller moet 201 CreatedAtAction teruggeven
        var createdAt = result.Result
            .Should()
            .BeOfType<CreatedAtActionResult>()
            .Subject;

        // Statuscode moet 201 zijn
        createdAt.StatusCode.Should().Be(201);

        // ActionName moet verwijzen naar GetById
        createdAt.ActionName.Should().Be(nameof(MongoProductsController.GetById));

        // RouteValues moet id bevatten
        createdAt.RouteValues!["id"].Should().Be(created.Id);

        // Body moet het created object bevatten
        createdAt.Value.Should().BeEquivalentTo(created);



        _service.VerifyAll();
    }

    // ============================
    // TEST 5: DELETE → 404
    // ============================
    // We controleren of de controller een 404 Not Found teruggeeft wanneer het verwijderen van een product niet lukt.
    [Fact]
    public async Task Delete_ReturnsNotFound_WhenFalse()
    {
        var id = "507f1f77bcf86cd799439011";

        // Service zegt: delete mislukt
        _service.Setup(s => s.DeleteProduct(id))
                .ReturnsAsync(false);

        var result = await _controller.Delete(id);

        // Controller moet 404 teruggeven
        result.Should().BeOfType<NotFoundResult>();

        _service.VerifyAll();
    }

    // ============================
    // TEST 6: DELETE → 204
    // ============================
    //We controleren of de controller een **204 No Content** teruggeeft wanneer het product succesvol werd verwijderd.



    [Fact]
    public async Task Delete_ReturnsNoContent_WhenTrue()
    {
        var id = "507f1f77bcf86cd799439011";

        // Service zegt: delete geslaagd
        _service.Setup(s => s.DeleteProduct(id))
                .ReturnsAsync(true);

        var result = await _controller.Delete(id);

        // Controller moet 204 NoContent teruggeven
        result.Should().BeOfType<NoContentResult>();

        _service.VerifyAll();
    }
}