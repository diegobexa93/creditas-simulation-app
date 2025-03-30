using Yarp.ReverseProxy.Transforms;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Logging estruturado
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// Configurar headers encaminhados corretamente para preservar IPs e protocolos
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // Permite qualquer origem (ajuste para produção)
    options.KnownProxies.Clear();
});

// Health checks básicos para monitorar o estado do Gateway
builder.Services.AddHealthChecks();

// Configuração do YARP a partir do appsettings.json + transformações
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddResponseTransform(async transformContext =>
        {
            transformContext.ProxyResponse.Headers.Add("X-Gateway", "YARP .NET 9");
        });
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Middleware para log de requisições de entrada
app.Use(async (context, next) =>
{
    var method = context.Request.Method;
    var path = context.Request.Path;
    var remoteIp = context.Connection.RemoteIpAddress;
    Console.WriteLine($"[{DateTime.UtcNow}] {method} {path} from {remoteIp}");
    await next();
});

// Middleware para forwarded headers
app.UseForwardedHeaders();

// Health endpoint
app.MapHealthChecks("/health");

// Endpoint de ping para verificação externa
app.MapGet("/ping", () => Results.Ok("Gateway Alive!"));

// Endpoint principal do YARP
app.MapReverseProxy();

app.Run();
