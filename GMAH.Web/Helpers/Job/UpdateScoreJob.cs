using GMAH.Services.Services;
using Quartz;
using System.Threading.Tasks;

namespace GMAH.Web.Helpers.Job
{
    public class UpdateScoreJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            // Define service
            ScoreSemesterService scoreSemesterService = new ScoreSemesterService();

            // Execute calculate score in semester
            scoreSemesterService.CalculateSubjectAvgScoreSemester();

            return Task.CompletedTask;
        }
    }
}