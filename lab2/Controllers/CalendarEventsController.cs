using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using lab2.Persistance;
using lab2;
using Microsoft.EntityFrameworkCore;

namespace lab2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarEventsController : ControllerBase
    {
        private readonly ICalendarService _calendarService;
        private readonly ILogger<CalendarEventsController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IMessagePublisher _rabbitMqPublisher;
        private readonly ApplicationDbContext _context;

        public CalendarEventsController(ILogger<CalendarEventsController> logger, ICalendarService calendarService,
            HttpClient _client, IMessagePublisher rabbitMqPublisher, ApplicationDbContext context)
        {
            _logger = logger;
            _calendarService = calendarService;
            _httpClient = _client;
            _rabbitMqPublisher = rabbitMqPublisher;
            _context = context;
        }

        [HttpGet(Name = "GetCalendarEvents")]
        public ActionResult <IEnumerable<CalendarEvent>> GetEvents()
        {
            try
            {
                _logger.LogInformation("GET /events succeeded, {Count} events returned.", _calendarService.GetCount());
                return Ok(_calendarService.GetEvents());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching events.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CalendarEvent> GetEvent(Guid id)
        {
            var selectedEvent = _calendarService.GetCalendarEvent(id);
            if (selectedEvent != null)
            {
                return Ok(selectedEvent);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEventAsync([FromBody] CalendarEvent calendarEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //await NotifyAsync(calendarEvent.Id, "created");
            _calendarService.AddEvent(calendarEvent);
            //var message = $"Event created: {calendarEvent.Id}";
            //_rabbitMqPublisher.PublishMessage(message);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.CalendarEvents.Add(calendarEvent);
                    await _context.SaveChangesAsync();

                    var outboxMessage = new OutboxMessage
                    {
                        MessageType = "CalendarEventCreated",
                        MessageBody = Newtonsoft.Json.JsonConvert.SerializeObject(calendarEvent), // Serialize the event data to JSON
                        CreatedAt = DateTime.UtcNow,
                        IsSent = false,
                    };

                    _context.OutboxMessages.Add(outboxMessage);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    
                    return CreatedAtAction(nameof(GetEvent), new { id = calendarEvent.Id }, calendarEvent);
                }
                catch (Exception)
                {
                    // If something goes wrong, rollback the transaction
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Internal server error");
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] CalendarEvent calendarEvent) 
        {
            _calendarService.UpdateEvent(id, calendarEvent);
            await NotifyAsync(id, "updated");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id)
        {
            _calendarService.DeleteEvent(id);
            await NotifyAsync(id, "deleted");

            return NoContent();
        }

        private async Task NotifyAsync(Guid eventId, string action)
        {
            var notificationRequest = new
            {
                UserId = "user-id", // Replace with actual user ID if available
                EventId = eventId,
                Message = $"Event {action}: {eventId}"
            };

            var content = new StringContent(JsonSerializer.Serialize(notificationRequest), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://calendarNotifications:8080/notify", content);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Notification sent for event {EventId} ({Action}).", eventId, action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
