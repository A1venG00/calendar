using lab2.Controllers;
using Consul;
using Notifications;

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
builder.Services.AddHttpClient<CalendarNotificationsController>();
builder.Services.AddHostedService<RabbitMqListener>();

var app = builder.Build();

// Register service with Consul
//using (var serviceScope = app.Services.CreateScope())
//{
//    var consulClient = serviceScope.ServiceProvider.GetRequiredService<IConsulClient>();
//    var registration = new AgentServiceRegistration()
//    {
//        ID = "calendarnotifications-1",  // Unique ID
//        Name = "calendarnotifications",  // Service Name
//        Address = "http://calendarnotifications",  // Docker service name
//        Port = 8080  // Service port
//    };

//    try
//    {
//        await consulClient.Agent.ServiceRegister(registration);
//        Console.WriteLine("Service registered with Consul");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Error registering service with Consul: {ex.Message}");
//    }
//}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
