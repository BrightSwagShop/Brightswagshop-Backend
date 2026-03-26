using System;
using FakeWebShop.Contracts.UserContracts;
using FakeWebShop.Persistence.Entities.PublicUser;

namespace FakeWebShop.Domain.Services.MongoUserServices.MongoInterfaces;

public interface IMongoUserInterface
{
    Task<UserResponseContract> Register(UserRequestContract request);
    Task<UserResponseContract?> Login(UserRequestContract request);

}
