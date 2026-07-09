using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using QSearchService.Application;
using QSearchService.Application.Consumers.BranchConsumers;
using QSearchService.Application.Consumers.CompanyConsumers;
using QSearchService.Application.Consumers.CompanyServiceConsumers;
using QSearchService.Application.Consumers.CustomerConsumers;
using QSearchService.Application.Consumers.EmployeeConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Infrastructure.Persistence.Database;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CustomerCreatedEventConsumer>();
    x.AddConsumer<CustomerUpdatedEventConsumer>();
    x.AddConsumer<CustomerDeletedEventConsumer>();
    x.AddConsumer<EmployeeCreatedEventConsumer>();
    x.AddConsumer<EmployeeUpdatedEventConsumer>();
    x.AddConsumer<EmployeeDeletedEventConsumer>();
    x.AddConsumer<CompanyCreatedEventConsumer>();
    x.AddConsumer<CompanyUpdatedEventConsumer>();
    x.AddConsumer<CompanyDeletedEventConsumer>();
    x.AddConsumer<BranchCreatedEventConsumer>();
    x.AddConsumer<BranchUpdatedEventConsumer>();
    x.AddConsumer<BranchDeletedEventConsumer>();
    x.AddConsumer<CompanyServiceCreatedEventConsumer>();
    x.AddConsumer<CompanyServiceUpdatedEventConsumer>();
    x.AddConsumer<CompanyServiceDeletedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetService<IConfiguration>();

        var host = configuration?["RabbitMQ:Host"] ?? "localhost";
        var port = configuration?.GetValue<ushort?>("RabbitMQ:Port") ?? 5672;
        var username = configuration?["RabbitMQ:Username"] ?? "guest";
        var password = configuration?["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, port, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddApplicationService();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISearchServiceDbContext, SearchServiceDbContext>();

builder.Services.AddDbContext<SearchServiceDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString).Build();
    options.UseNpgsql(dataSourceBuilder);
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.Run();