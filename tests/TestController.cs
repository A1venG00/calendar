using FluentAssertions;
using lab2;
using lab2.Controllers;
using lab2.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace tests
{
    public class TestController
    {
        private readonly Mock<ICalendarService> _mockService;
        private readonly CalendarEventsController _controller;
        private readonly Mock<ILogger<CalendarEventsController>> _mockLogger;
        private readonly Mock<HttpClient> _mockClient;
        private readonly Mock<RabbitMqPublisher> _rabbitMq;
        private readonly ApplicationDbContext _dbContext;

        public TestController()
        {
            _mockService = new Mock<ICalendarService>();
            _mockLogger = new Mock<ILogger<CalendarEventsController>>();
            _mockClient = new Mock<HttpClient>();
            _rabbitMq = new Mock<RabbitMqPublisher>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseInMemoryDatabase("InMemoryDb")
                        .Options;

            // Now, you can create the ApplicationDbContext
            _dbContext = new ApplicationDbContext(options);

            // Then, initialize the controller
            _controller = new CalendarEventsController(_mockLogger.Object, _mockService.Object, _mockClient.Object, _rabbitMq.Object, _dbContext);
        }

        [Fact]
        public void GetEventById_ReturnsOk_WhenEventExists()
        {
            Guid eventId = Guid.NewGuid();
            var mockEvent = new CalendarEvent { Id = eventId, Title = "Test" };
            _mockService.Setup(x => x.GetCalendarEvent(eventId)).Returns(mockEvent);

            var result = _controller.GetEvent(eventId);

            result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);

            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(mockEvent);
        }

        [Fact]
        public void GetEventById_ReturnsNotFound_WhenEventDoesNotExist()
        {
            var eventId = Guid.NewGuid();
            _mockService.Setup(s => s.GetCalendarEvent(eventId)).Returns((CalendarEvent)null);

            var result = _controller.GetEvent(eventId);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        //[Fact]
        //public void CreateEvent_ReturnsCreatedResult_WhenEventIsValid()
        //{
        //    var newEvent = new CalendarEvent { Title = "Meeting" };
        //    var createdEvent = new CalendarEvent { Id = new Guid(), Title = "Meeting" };

        //    var result = _controller.CreateEventAsync(newEvent);

        //    result.Result.Should().BeOfType<CreatedAtActionResult>()
        //    .Which.StatusCode.Should().Be(201);

        //    //var createdAtActionResult = result.Result as CreatedAtActionResult;
        //    //var createdEventResult = createdAtActionResult?.Value as CalendarEvent;

        //    //// Assert that the Value in CreatedAtActionResult is equivalent to the createdEvent
        //    //createdEventResult.Should().BeEquivalentTo(createdEvent);
        //}

        [Fact]
    public void CreateEvent_ReturnsBadRequest_WhenModelIsInvalid()
    {
        // Arrange
        var newEvent = new CalendarEvent { /* Invalid state */ };
        _controller.ModelState.AddModelError("Title", "The Title field is required.");

        // Act
        var result = _controller.CreateEventAsync(newEvent);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
    }
}