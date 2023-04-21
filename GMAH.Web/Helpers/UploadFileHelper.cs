using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace GMAH.Web.Helpers
{
    public static class UploadFileHelper
    {
        public static string ServerPath;
        public static string DirFileReport = "/Assests/Report";
        public static int MaxSize = 5;
        public static List<string> FileType = new List<string> { ".jpg", ".png", ".jpeg", ".bmp", ".pdf", ".doc", ".docx" };

        public static string Upload(HttpPostedFile file)
        {
            // Create folder if not exist
            string path = ServerPath + DirFileReport;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Validate file
            string fileType = "";
            bool checkType = false;
            FileType.ForEach(type =>
            {
                if (file.FileName.EndsWith(type))
                {
                    fileType = type;
                    checkType = true;
                    return;
                }
            });

            if (!checkType)
            {
                throw new Exception("Loại file không hợp lệ, vui lòng chỉ upload file hình ảnh, pdf hoặc word");
            }

            if (file.ContentLength > MaxSize * 1024 * 1024)
            {
                throw new Exception($"Vượt quá dung lượng tối đa được upload cho mỗi file là {MaxSize} MB");
            }

            // Set path save file
            string filename = "evidence_" + DateTime.Now.ToFileTimeUtc() + fileType;
            path = Path.Combine(path, filename);

            // Save file
            file.SaveAs(path);

            // Return part
            return filename;
        }

        public static void Remove(string fileURL)
        {
            var split = fileURL.Split(new char[] { '/', '\\' }).ToList();
            string fileName = split.LastOrDefault();
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string serverPath = ServerPath;
            string path = serverPath + DirFileReport + "\\" + fileName;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}