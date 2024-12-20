using Hangfire;
using Hangfire.MySql;
using Microsoft.EntityFrameworkCore;
using TimeSync.Infrastructure;
using TimeSync.Services;
using Hangfire.MySql;
using static TimeSync.Services.TaskCronJobService;
using TimeSync.Logs;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database contexts
builder.Services.AddDbContext<UserContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 38))));

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 38))));

builder.Services.AddDbContext<NotificationContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 38))));

// Hangfire configuration
builder.Services.AddHangfire(config =>
    config.UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("DefaultConnection") ,
     new MySqlStorageOptions
     {
         TransactionIsolationLevel = (System.Transactions.IsolationLevel)System.Data.IsolationLevel.ReadCommitted,
         QueuePollInterval = TimeSpan.FromSeconds(15),
         JobExpirationCheckInterval = TimeSpan.FromHours(1),
         CountersAggregateInterval = TimeSpan.FromMinutes(5),
         PrepareSchemaIfNecessary = true,
         DashboardJobListLimit = 50000,
         TransactionTimeout = TimeSpan.FromMinutes(2)
          
     })

    ));
builder.Services.AddHangfireServer();

// Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TaskCronJobService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<ChatGPTService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    return new ChatGPTService(apiKey);
});

// Allow CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Build application
var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // URL where Swagger UI will be available
    });
}
else if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // URL where Swagger UI will be available
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAll");

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new NoAuthorizationFilter() }
});

// Scoped service for task notifications
using (var scope = app.Services.CreateScope())
{
    var cronJobService = scope.ServiceProvider.GetRequiredService<TaskCronJobService>();
    cronJobService.ScheduleTaskNotifications();
}

// Map controllers
app.MapControllers();

app.Run();
