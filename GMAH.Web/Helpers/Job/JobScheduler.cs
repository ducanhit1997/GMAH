using Quartz.Impl;
using Quartz;
using System.Configuration;

namespace GMAH.Web.Helpers.Job
{
    /// <summary>
    /// Thực thi scheduler
    /// https://tedu.com.vn/lap-trinh-aspnet/tu-dong-thuc-thi-tac-vu-theo-lich-trong-aspnet-voi-quartznet-87.html
    /// </summary>
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start();

            // Job dọn rác
            IJobDetail garbageJob = JobBuilder.Create<GarbageJob>().Build();
            ITrigger garbageTrigger = TriggerBuilder.Create()
                // https://www.freeformatter.com/cron-expression-generator-quartz.html
                .WithCronSchedule(ConfigurationManager.AppSettings["JOB_GARBAGE"])
                // Debug only
                //.WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever())
                .Build();

            scheduler.ScheduleJob(garbageJob, garbageTrigger);

            // Job update điểm
            IJobDetail scoreJob = JobBuilder.Create<UpdateScoreJob>().Build();
            ITrigger scoreTrigger = TriggerBuilder.Create()
                // https://www.freeformatter.com/cron-expression-generator-quartz.html
                .WithCronSchedule(ConfigurationManager.AppSettings["JOB_UPDATESCORE"])
                // Debug only
                //.WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever())
                .Build();

            scheduler.ScheduleJob(scoreJob, scoreTrigger);
        }
    }
}