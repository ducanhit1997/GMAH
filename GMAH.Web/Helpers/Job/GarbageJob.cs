using Quartz;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace GMAH.Web.Helpers.Job
{
    public class GarbageJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var tempPath = HttpRuntime.AppDomainAppPath  + "/Assests/UploadTemp";

            try
            {
                // Lấy danh sách file & xoá
                DirectoryInfo di = new DirectoryInfo(tempPath);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

            }
            catch
            {
                // Do nothing
            }

            return Task.CompletedTask;
        }
    }
}