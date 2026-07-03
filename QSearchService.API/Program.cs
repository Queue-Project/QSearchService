using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using QSearchService.API;
using QSearchService.Application;
using QSearchService.Application.Consumers.ElasticSearchConsumers.BranchConsumers;
using QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyConsumers;
using QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;
using QSearchService.Application.Consumers.ElasticSearchConsumers.CustomerConsumers;
using QSearchService.Application.Consumers.ElasticSearchConsumers.EmployeeConsumers;
using QSearchService.Application.Consumers.FullTextSearchConsumers.BranchConsumers;
using QSearchService.Application.Consumers.FullTextSearchConsumers.CompanyConsumers;
using QSearchService.Application.Consumers.FullTextSearchConsumers.CompanyServiceConsumers;
using QSearchService.Application.Consumers.FullTextSearchConsumers.CustomerConsumers;
using QSearchService.Application.Consumers.FullTextSearchConsumers.EmployeeConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Application.Services;
using QSearchService.Infrastructure.Persistence.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ElasticsearchOptions>(
    builder.Configuration.GetSection(ElasticsearchOptions.SectionName));

builder.Services.AddSingleton<ElasticsearchClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;

    var settings = new ElasticsearchClientSettings(new Uri(options.Uri))
        .Authentication(new BasicAuthentication(
            options.Username!,
            options.Password!))
        .DefaultIndex(options.DefaultIndex);

    return new ElasticsearchClient(settings);
});

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
    x.AddConsumer<CustomerCreatedEventElasticSearchConsumer>();
    x.AddConsumer<CustomerUpdatedEventElasticSearchConsumer>();
    x.AddConsumer<CustomerDeletedEventElasticSearchConsumer>();
    x.AddConsumer<EmployeeCreatedEventElasticSearchConsumer>();
    x.AddConsumer<EmployeeUpdatedEventElasticSearchConsumer>();
    x.AddConsumer<EmployeeDeletedEventElasticSearchConsumer>();
    x.AddConsumer<CompanyCreatedEventElasticSearchConsumer>();
    x.AddConsumer<CompanyUpdatedEventElasticSearchConsumer>();
    x.AddConsumer<CompanyDeletedEventElasticSearchConsumer>();
    x.AddConsumer<BranchCreatedEventElasticSearchConsumer>();
    x.AddConsumer<BranchUpdatedEventElasticSearchConsumer>();
    x.AddConsumer<BranchDeletedEventElasticSearchConsumer>();
    x.AddConsumer<CompanyServiceCreatedEventElasticSearchConsumer>();
    x.AddConsumer<CompanyServiceUpdatedEventElasticSearchConsumer>();
    x.AddConsumer<CompanyServiceDeletedEventElasticSearchConsumer>();
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
builder.Services.AddScoped<IElasticSearchService, ElasticSearchService>();

builder.Services.AddDbContext<SearchServiceDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString).Build();
    options.UseNpgsql(dataSourceBuilder);
});


var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.Run();