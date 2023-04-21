using System.IO;
using System.Web;

namespace GMAH.Web.Helpers.Formatter
{
    /// <summary>
    /// https://stackoverflow.com/questions/56896484/net-mvc-how-to-submit-multipart-form-data-to-web-api
    /// </summary>
    public class HttpPostedFileMultipart : HttpPostedFileBase
    {
        public override string FileName { get; }

        public override string ContentType { get; }

        public override Stream InputStream { get; }

        public override int ContentLength => (int)InputStream.Length;

        public HttpPostedFileMultipart(string fileName, string contentType, byte[] fileContents)
        {
            FileName = fileName;
            ContentType = contentType;
            InputStream = new MemoryStream(fileContents);
        }
    }
}