using System;
using FakeWebShop.Contracts.UserContracts;
using FakeWebShop.Domain.Model.MongoUserModel;
using FakeWebShop.Persistence.Entities.PublicUser;

namespace FakeWebShop.Domain.Services.MongoServicesMapping.MongoUserMapping;

public static class UserMapping
{
    public static User ToEntity(UserResponseContract contract)
{
    return new User
    {
        Id = contract.Id,
        Username = contract.Username,
         
    };
}
     // Request → Model
    public static UserModel ToModel(this UserRequestContract request)
    {
        return new UserModel
        {
            Username = request.Username
        };
    }

    // Model → Entity
    public static User ToEntity(this UserModel model)
    {
        return new User
        {
            Id = model.Id,
            Username = model.Username,
            PasswordHash = model.PasswordHash,
            
        };
    }

    // Entity → Model
    public static UserModel ToModel(this User entity)
    {
        return new UserModel
        {
            Id = entity.Id,
            Username = entity.Username,
            PasswordHash = entity.PasswordHash,
             
        };
    }

    // Model → Response
    public static UserResponseContract ToResponse(this UserModel model)
    {
        return new UserResponseContract
        {
            Id = model.Id,
            Username = model.Username,
             
        };
    }

}
