﻿using Hangefire.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.ScheduledJob;

namespace Hangefire.API.Controllers
{
    [Route("api/scheduled-jobs")]
    [ApiController]
    public class ScheduledJobsController : ControllerBase
    {
        private readonly IBackgroundJobService _backgroundJobService;
        public ScheduledJobsController(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;

        }
        [HttpPost]
        [Route("send-email-reminder-checkout-order")]
        public IActionResult SendReminderCheckoutOrderEmail([FromBody] ReminderCheckoutOrderDto model)
        {

            var jobId = _backgroundJobService.SendEmailContent(model.email, model.subject, model.emailContent, model.enqueueAt);
            return Ok(jobId);

        }
    }
}
