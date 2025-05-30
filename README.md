# 🧩 Custom Mediator Pattern in .NET

This project demonstrates a lightweight custom implementation of the **Mediator Pattern** in ASP.NET Core, inspired by [MediatR](https://github.com/jbogard/MediatR). It shows how to decouple business logic using `IRequest`, `IRequestHandler`, and a dynamic `Sender`.

The Mediator pattern is a behavioral design pattern aimed at reducing chaotic dependencies between objects.

It restricts direct communication between components, forcing them to communicate only through a central object: the mediator.

---

## 🚀 Features

- Custom `IRequest<TResponse>` interface for commands and queries
- `IRequestHandler<TRequest, TResponse>` to handle specific requests
- `ISender` interface and `Sender` implementation to dispatch requests
- `AddMyMediator` extension to register everything automatically
- No external dependencies – only uses .NET built-in features

---

## 📦 Interfaces & Classes

### `IRequest<TResponse>`

Marker interface that represents a request expecting a response.

```csharp
public interface IRequest<TResponse> {}
```

### `IRequestHandler<TRequest, TResponse>`

Defines a handler for a specific request.

```csharp
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}
```

### `ISender`

Exposes the method to send a request.

```csharp
public interface ISender
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
```

### `Sender`

Resolves and invokes the appropriate handler for the request dynamically using `IServiceProvider`.

```csharp
public class Sender(IServiceProvider serviceProvider) : ISender
{
    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);

        return handler.Handle((dynamic)request, cancellationToken);
    }
}
```

---

## 🧰 Dependency Injection Registration

Use the `AddMyMediator` extension method to automatically scan the assembly and register all handlers:

```csharp
public static IServiceCollection AddMyMediator(this IServiceCollection services, Assembly? assembly = null)
{
    assembly ??= Assembly.GetCallingAssembly();
    services.AddScoped<ISender, Sender>();

    var handlerInterfaceType = typeof(IRequestHandler<,>);

    var handlerTypes = assembly.GetTypes()
        .Where(type => !type.IsAbstract && !type.IsInterface)
        .SelectMany(type => type.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
            .Select(i => new { Interface = i, Implementation = type }));

    foreach (var handler in handlerTypes)
    {
        services.AddScoped(handler.Interface, handler.Implementation);
    }

    return services;
}
```

### ✅ Add to `Program.cs`

```csharp
builder.Services.AddMyMediator();
```

---

## 🧪 Example Usage

### 1. Define a Query

```csharp
public class GetUserByIdQuery : IRequest<UserViewModel>
{
    public int Id { get; set; }
}
```

### 2. Create the Handler

```csharp
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserViewModel>
{
    public Task<UserViewModel> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // Business logic here
    }
}
```

### 3. Send the Request

```csharp
var result = await sender.SendAsync(new GetUserByIdQuery { Id = 1 });
```

---

## 📌 Summary

This minimal mediator implementation enables clean and decoupled architecture in .NET without relying on external libraries. Ideal for educational purposes, lightweight projects, or deeper understanding of how MediatR works under the hood.

Feel free to fork and expand it with pipeline behaviors, or logging middleware!
