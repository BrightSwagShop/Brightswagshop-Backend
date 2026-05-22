using System;
using FakeWebShop.Contracts.Request.ContactRequest;

namespace FakeWebShop.Domain.Services.Interface_s;

public interface IEmailService
{
      Task SendContactMailAsync(ContactRequest request);

}
