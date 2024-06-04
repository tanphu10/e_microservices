using Contracts.ScheduledJobs;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;
namespace Hangefire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IScheduledJobService _jobService;
        public HomeController(ILogger logger, IScheduledJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }
        [HttpPost("[action]")]
        public IActionResult Welcome()
        {
            var jobId = _jobService.Enqueue(() => ResponseWelcome("welcome to Hangfire API"));
            return Ok($"JobId :{jobId}-Enqueue job");

        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult DelayWelcome()
        {
            var seconds = 5;
            var jobId = _jobService.Schedule(() => ResponseWelcome("welcome to hangfire API"), TimeSpan.FromSeconds(seconds));
            return Ok($"JobId :{jobId}- delay job");
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult WelcomeAt()
        {
            var enqueueAt = DateTimeOffset.UtcNow.AddSeconds(10);
            var jobId = _jobService.Schedule(() => ResponseWelcome("welcome to hangfire API"), enqueueAt);
            return Ok($"JobId :{jobId}-schedule job");
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult ConfirmedWelcome()
        {

            const int timeInSeconds = 5;
            var parentJobId = _jobService.Schedule(() => ResponseWelcome("welcome to hangfire API"), TimeSpan.FromSeconds(5));
            var jobId = _jobService.ContinueQueueWith(parentJobId, () => ResponseWelcome("welcome message is sent"));
            return Ok($"JobId :{parentJobId}-confirm welcome will be sent in {timeInSeconds} seconds");
        }

        [NonAction]
        public void ResponseWelcome(string text)
        {
            _logger.Information(text);
        }
    }
}
