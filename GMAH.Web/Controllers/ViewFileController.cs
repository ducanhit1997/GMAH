using GMAH.Web.Helpers.Attributes;
using System.Linq;
using System.Net;
using System;
using System.Web.Mvc;

namespace GMAH.Web.Controllers
{
    [RouteArea("All", AreaPrefix = "")]
    [JwtAuthentication]
    public class ViewFileController : Controller
    {
        [Route("xemfile/{filename}")]
        public void ViewFile(string filename)
        {
            var filePath = @"Assests\Report\" + filename;
            viewFilePath(filePath, attachment: "inline");
        }

        private bool viewFilePath(string filePath, bool returnNotFoundMsg = true, string attachment = "attachment")
        {
            string root = Server.MapPath("~");
            filePath = root + filePath;
            WebClient User = new WebClient();

            if (!System.IO.File.Exists(filePath))
            {
                if (!returnNotFoundMsg)
                {
                    return false;
                }

                Response.Write("Không tìm thấy file này");
                return false;
            }

            Byte[] FileBuffer = User.DownloadData(filePath);
            if (FileBuffer != null)
            {
                var fileName = System.IO.Path.GetFileName(filePath);
                var fileType = filePath.Split('.').Last();

                Response.ContentType = "application/" + fileType;
                Response.AddHeader("content-disposition", $"{attachment};filename=\"" + fileName + "\"");
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                Response.BinaryWrite(FileBuffer);

                return true;
            }

            return false;
        }
    }
}