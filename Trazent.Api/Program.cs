using GraphQL.AspNet.Configuration;
using Trazent.Api.DataAccess;
using Trazent.Api.GraphQL.Mapping;
using Trazent.Core.Implementations.Services;
using Trazent.Core;
using AutoMapper;

System.AppContext.SetSwitch(
    "Npgsql.EnableStoredProcedureCompatMode",
    true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDatabaseConnection, DatabaseConnection>();
builder.Services.AddCoreServices();
builder.Services.AddAutoMapper(configuration =>
    configuration.AddMaps(typeof(CompaniesMappingProfile).Assembly));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Pwa", policy =>
        policy.WithOrigins(
                "http://localhost:5500") // development
            .WithMethods("POST")
            .WithHeaders("Content-Type", "Accept"));
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Add GraphQL
builder.Services
    .AddGraphQL();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseGraphQLGraphiQL("/ui/playground");

app.UseGraphQL();

app.UseHttpsRedirection();

app.UseCors("Pwa");

app.UseAuthorization();

app.MapControllers();

app.Run();