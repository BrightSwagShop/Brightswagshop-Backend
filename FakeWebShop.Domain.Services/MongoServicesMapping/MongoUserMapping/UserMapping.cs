using FakeWebShop.Contracts.Request.UserRequest;
using FakeWebShop.Contracts.Response.UserResponse;
using FakeWebShop.Domain.Model.MongoUserModel;
using FakeWebShop.Persistence.Entities.PublicUser;

namespace FakeWebShop.Domain.Services.MongoServicesMapping.MongoUserMapping;

public static class UserMapping
{
    public static UserModel ToModel(this UserAuthRequestContract request)
    {
        return new UserModel
        {
            Username = request.Username
        };
    }

    public static User ToEntity(this UserModel model)
    {
        return new User
        {
            Id = model.Id,
            Username = model.Username,
            PasswordHash = model.PasswordHash,
            Favorites = model.Favorites
        };
    }

    public static UserModel ToModel(this User entity)
    {
        return new UserModel
        {
            Id = entity.Id,
            Username = entity.Username,
            PasswordHash = entity.PasswordHash,
            Favorites = entity.Favorites
        };
    }

    public static UserResponseContract ToResponse(this UserModel model)
    {
        return new UserResponseContract
        {
            Id = model.Id,
            Username = model.Username,
            Favorites = model.Favorites
        };
    }
}