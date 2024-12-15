using Microsoft.AspNetCore.Mvc;
using Notifications;

namespace lab2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarNotificationsController : ControllerBase
    {
        [HttpPost("notify")]
        public IActionResult Notify([FromBody] NotificationRequest request)
        {
            // Handle the notification
            Console.WriteLine($"Notification sent to user {request.UserId} for event {request.EventId}: {request.Message}");
            return Ok(new { Status = "Notification sent successfully" });
        }
    }
}
