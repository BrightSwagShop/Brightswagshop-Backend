using System;
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
    

}
