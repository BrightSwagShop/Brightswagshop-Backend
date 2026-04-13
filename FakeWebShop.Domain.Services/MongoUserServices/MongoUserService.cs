using System;
using FakeWebShop.Contracts.Request;
using FakeWebShop.Contracts.Response;
using FakeWebShop.Contracts.UserContracts;
using FakeWebShop.Domain.Services.MongoServicesMapping.MongoUserMapping;
using FakeWebShop.Domain.Services.MongoUserServices.MongoInterfaces;
using FakeWebShop.Persistence.Entities.PublicUser;
using FakeWebShop.Persistence.PublicUserRepo_s.MongoInterfaces;

namespace FakeWebShop.Domain.Services.MongoUserServices;

public class MongoUserService(IMongoUserRepository repo) : IMongoUserInterface
{
       public async Task<UserResponseContract> Register(UserRequestContract request)
    {
        var existing = await repo.GetByUsernameAsync(request.Username);
        if (existing != null)
            throw new Exception("User exists");

        var model = request.ToModel();
        model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var entity = model.ToEntity();

        var created = await repo.CreateAsync(entity);

        return created.ToModel().ToResponse();
    }

    public async Task<UserResponseContract?> Login(UserRequestContract request)
    {
        var user = await repo.GetByUsernameAsync(request.Username);
        if (user == null) return null;

        var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!valid) return null;

        return user.ToModel().ToResponse();
    }
    //hier moet ik een methode hebben voor de user request te behandelen
    // met user request bedoel ik dat de request een productId bevat dit request wordt omgezet naar model bij mapping 
    // en erna wordt met behulp van de repo de productId toegevoegd en wordt de model terug omgezet naar een userResponseContract die moet ik nog aanpassen
    //wat moet in de userRequest en wat moet in de userResponse

    public async Task<UserResponseContract> VoegFavoriteByUserAsync(UserRequestContract request)
    {
        //ik wil de favorites array hier updaten dus ik gebruik de methode van de repo!
        // de request.UserId de request zelf moet ik nog omzetten naar model en entity
        var user = await repo.GetByUsernameAsync(request.Username);
        user.ToModel().ToEntity();
        await repo.VoegFavoriteByUserAsync(user.Id, user.ProductId);
        return user.ToModel().ToResponse();
        
    }


}
