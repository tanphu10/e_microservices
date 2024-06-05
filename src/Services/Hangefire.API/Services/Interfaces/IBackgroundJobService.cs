using Contracts.ScheduledJobs;

namespace Hangefire.API.Services.Interfaces
{
    public interface IBackgroundJobService
    {
        IScheduledJobService scheduledJobService { get; }
        string? SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt);
    }
}
