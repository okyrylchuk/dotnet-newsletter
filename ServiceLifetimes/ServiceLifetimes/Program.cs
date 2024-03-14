var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGuid, MyGuid>();
builder.Services.AddSingleton<ISingletonService, SingletonService>();
builder.Services.AddScoped<IService1, Service1>();
builder.Services.AddScoped<IService2, Service2>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/scoped_in_singleton", (ISingletonService service) =>
{
    Console.WriteLine($"Request number: {RequestNumber.Get()}");
    Console.WriteLine(service.GetMessage());
    Console.WriteLine();
});

app.MapGet("/test", (IService1 service1, IService2 service2) =>
{
    Console.WriteLine($"Request number: {RequestNumber.Get()}");
    Console.WriteLine(service1.GetMessage());
    Console.WriteLine(service2.GetMessage());
    Console.WriteLine();
});

app.Run();

#region SingletonService with Scoped dependencies
public interface ISingletonService
{
    string GetMessage();
}

public class SingletonService(IServiceProvider serviceProvider)
    : ISingletonService
{
    private readonly IServiceProvider _serviceProvider
        = serviceProvider;

    public string GetMessage()
    {
        using var scope = _serviceProvider.CreateScope();
        var _guid = scope.ServiceProvider.GetRequiredService<IGuid>();

        return $"SingletonService has guid = {_guid.Get()}";
    }
}
#endregion

#region Service1 and Service2
public interface IService1
{
    string GetMessage();
}

public interface IService2
{
    string GetMessage();
}

public class Service1(IGuid myGuid) : IService1
{
    private readonly IGuid _guid = myGuid;

    public string GetMessage() =>
        $"Service1 has guid = {_guid.Get()}";
}

public class Service2(IGuid myGuid) : IService2
{
    private readonly IGuid _guid = myGuid;

    public string GetMessage() =>
        $"Service2 has guid = {_guid.Get()}";
}
#endregion

#region Guid provider
public interface IGuid
{
    Guid Get();
}

public class MyGuid : IGuid
{
    private readonly Guid _guid = Guid.NewGuid();

    public Guid Get() => _guid;
}
#endregion

#region RequestNumber
public static class RequestNumber
{
    private static int _count = 0;

    public static int Get() => ++_count;
}
#endregion