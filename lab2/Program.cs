using lab2;
using lab2.Controllers;
using Consul;
using lab2.Persistance;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(config =>
//{
//    config.Address = new Uri("http://consul:8500");
//}));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddHttpClient<CalendarEventsController>();

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = builder.Configuration["RabbitMQ:HostName"],
        UserName = builder.Configuration["RabbitMQ:UserName"],
        Password = builder.Configuration["RabbitMQ:Password"]
    };
    return factory.CreateConnection();
});
builder.Services.AddScoped<RabbitMqPublisher>(); // Äëÿ CalendarEvents
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<OutboxProcessorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//using (var serviceScope = app.Services.CreateScope())
//{
//    var consulClient = serviceScope.ServiceProvider.GetRequiredService<IConsulClient>();
//    var registration = new AgentServiceRegistration()
//    {
//        ID = "calendarevents", // Unique ID for this service
//        Name = "calendarevents", // Service name
//        Address = "http://calendarevents", // The address of the service
//        Port = 5000 // The port the service is running on
//    };
//    await consulClient.Agent.ServiceRegister(registration);
//}

//app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
