// See https://aka.ms/new-console-template for more information
using Mediator.Abstractions;
using Mediator.Extensions;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");


var services = new ServiceCollection();
services.AddMediator(typeof(Program).Assembly);
services.AddTransient<AccountRepository>();

var sevicesProvider = services.BuildServiceProvider();
var mediator = sevicesProvider.GetService<IMediator>();

var repository = new AccountRepository();
var request = new CreateAccountCommand { Username = "admin", Password = "123123" };
var result = await mediator.SendAsync(request);

Console.WriteLine(result);
public class AccountRepository
{
    public void Save()
    {
        Console.WriteLine("Saving...");
    }
}

public class CreateAccountCommand : IRequest<string>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateAccountHandler(AccountRepository repository) : IHandler<CreateAccountCommand, string>
{
    public Task<string> HandleAsync(CreateAccountCommand request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Creating {request.Username} account...");
        repository.Save();
        return Task.FromResult($"Account {request.Username} created");
    }
}