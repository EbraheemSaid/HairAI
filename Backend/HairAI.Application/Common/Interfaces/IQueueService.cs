namespace HairAI.Application.Common.Interfaces;

public interface IQueueService
{
    Task PublishAnalysisJobAsync(Guid jobId);
}