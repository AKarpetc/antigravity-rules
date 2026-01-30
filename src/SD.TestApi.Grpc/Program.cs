using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc.Server;
using SD.TestApi.Application;
using SD.TestApi.Grpc.Services;
using SD.TestApi.Infrastructure;
using SD.TestApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCodeFirstGrpc();
builder.Services.AddCodeFirstGrpcReflection();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TestApiService>();
app.MapGrpcService<ImageManagementGrpcService>();

app.MapCodeFirstGrpcReflectionService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Ensure DB created (for dev convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SettingsDbContext>();
    try { db.Database.EnsureCreated(); } catch { /* Ignore */ }
}

app.Run();
