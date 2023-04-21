using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using Microsoft.Office.Interop.Word;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace GMAH.Web.Helpers
{
    /// <summary>
    /// Sử dụng EPPlus
    /// https://toidicodedao.com/2015/11/24/series-c-hay-ho-epplus-thu-vien-excel-ba-dao-phan-1/
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// Đọc data từ excel file, chuyển thành model để import vào db
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ImportScoreExcel ReadScoreFromExcel(string filePath)
        {
            var data = new ImportScoreExcel();

            try
            {
                // Khởi tạo trình đọc excel
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Nếu ko có sheet nào thì báo lỗi
                    if (package.Workbook.Worksheets.Count == 0)
                    {
                        throw new Exception("Không tìm thấy sheet nào trong file excel");
                    }

                    // Lấy subject code từ sheet name
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    data.SubjectCode = worksheet.Name;

                    // Lấy thông tin điểm của học sinh
                    data.Student = new List<ImportScoreStudentDetail>();

                    // Kiểm tra table trong file excel
                    if (worksheet.Tables.Count != 1)
                    {
                        throw new Exception("Không tìm thấy table nào trong file excel");
                    }

                    // Đọc từ table
                    var table = worksheet.Tables[0];
                    for (var i = 1; i <= table.Range.Rows; i++)
                    {
                        for (var j = 3; j < table.Range.Columns; j++)
                        {
                            // Kiểm tra header điểm

                            // Cộng 1 để bỏ header
                            var studentData = new ImportScoreStudentDetail
                            {
                                StudentCode = worksheet.Cells[i + 1, 1].Value?.ToString(),
                                StudentName = worksheet.Cells[i + 1, 2].Value?.ToString(),
                                ScoreName = worksheet.Cells[1, j].Value?.ToString(),
                                Score = ParseToScore(worksheet.Cells[i + 1, j]),
                                // Note luôn là cột cuối
                                Note = worksheet.Cells[i + 1, table.Range.Columns].Value?.ToString(),
                            };

                            if (string.IsNullOrEmpty(studentData.StudentCode))
                            {
                                continue;
                            }

                            data.Student.Add(studentData);
                        }
                    }
                }

                data.IsSuccess = true;
            }
            catch (Exception ex)
            {
                data.IsSuccess = false;
                data.Message = ex.Message;
            }

            return data;
        }

        /// <summary>
        /// Xuất file excel
        /// https://riptutorial.com/epplus/example/27209/adding-and-formating-a-table
        /// </summary>
        public static Stream ExportScoreToExcel(List<ScoreViewModel> data, List<ScoreComponentViewModel> scoreComponents)
        {
            using (var excelPackage = new ExcelPackage(new MemoryStream()))
            {
                // Add Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("Score");

                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var workSheet = excelPackage.Workbook.Worksheets[0];

                // Tạo table
                // Header
                var columnIndex = 3;
                for (var i = 0; i < scoreComponents.Count; i++)
                {
                    var subject = scoreComponents[i];
                    workSheet.SetValue(1, columnIndex, subject.SubjectName);
                    var mergeCell = workSheet.Cells[1, columnIndex, 1, columnIndex + subject.Column.Count - 1];
                    mergeCell.Merge = true;
                    mergeCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    for (var j = 0; j < subject.Column.Count; j++)
                    {
                        var scoreName = subject.Column[j];
                        workSheet.SetValue(2, columnIndex + j, scoreName);
                    }

                    columnIndex += subject.Column.Count;
                }
                workSheet.SetValue(2, 1, "Mã số học sinh");
                workSheet.SetValue(2, 2, "Họ tên học sinh");

                // Body
                for (var i = 0; i < data.Count; i++)
                {
                    var studentData = data[i];
                    workSheet.SetValue(3 + i, 1, studentData.StudentCode);
                    workSheet.SetValue(3 + i, 2, studentData.StudentName);

                    var columnScoreIndex = 3;
                    foreach (var subject in scoreComponents)
                    {
                        var subjectData = studentData.Subjects.Where(x => x.SubjectName == subject.SubjectName).FirstOrDefault();
                        foreach (var scoreName in subject.Column)
                        {
                            var score = "";

                            // Nếu tìm thấy môn học thì tìm tiếp điểm thành phần
                            if (subjectData != null)
                            {
                                var scoreData = subjectData.Details.Where(x => x.ScoreName == scoreName).FirstOrDefault();
                                score = scoreData?.Score?.ToString() ?? "";
                                if (string.IsNullOrEmpty(score))
                                {
                                    score = scoreData?.Text ?? "";
                                }
                                else
                                {
                                    workSheet.Cells[3 + i, columnScoreIndex].Style.Numberformat.Format = "0.0";
                                }
                            }

                            // Set value cho row
                            workSheet.SetValue(3 + i, columnScoreIndex, score);

                            columnScoreIndex++;
                        }
                    }
                }

                // Format lại
                workSheet.Column(1).AutoFit();
                workSheet.Column(2).AutoFit();

                // Border
                var table = workSheet.Cells[1, 1, 2 + data.Count, 2 + scoreComponents.SelectMany(x => x.Column).Count()];
                table.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public static double? ParseToScore(ExcelRange data)
        {
            if (data is null || data.Value is null) return null;
            double score = 0;
            if (double.TryParse(data.Value.ToString(), out score))
            {
                return score;
            }

            return null;
        }

        public static ImportTimelineExcel ReadTimelineFromExcel(string filePath)
        {
            var data = new ImportTimelineExcel();

            try
            {
                // Khởi tạo trình đọc excel
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Nếu ko có sheet nào thì báo lỗi
                    if (package.Workbook.Worksheets.Count == 0)
                    {
                        throw new Exception("Không tìm thấy sheet nào trong file excel");
                    }

                    // Lấy subject code từ sheet name
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    // Lấy thông tin điểm của học sinh
                    data.TimelineData = new List<TimelineData>();

                    // Kiểm tra table trong file excel
                    if (worksheet.Tables.Count != 1)
                    {
                        throw new Exception("Không tìm thấy table nào trong file excel");
                    }

                    // Đọc từ table
                    var table = worksheet.Tables[0];
                    for (var i = 1; i <= table.Range.Rows; i++)
                    {
                        var timelineData = new TimelineData
                        {
                            SubjectCode = worksheet.Cells[i + 1, 1].Value?.ToString(),
                            Periods = worksheet.Cells[i + 1, 2].Value?.ToString().Split(',').Select(x => int.Parse(x)).ToList(),
                        };

                        if (string.IsNullOrEmpty(timelineData.SubjectCode))
                        {
                            break;
                        }

                        // Date from, to
                        timelineData.DateFrom = DateTime.Parse(worksheet.Cells[i + 1, 4].Value.ToString());
                        timelineData.DateTo = DateTime.Parse(worksheet.Cells[i + 1, 5].Value.ToString());

                        // Thứ
                        switch (worksheet.Cells[i + 1, 3].Value?.ToString())
                        {
                            case "Thứ Hai":
                                timelineData.DayOfWeek = DayOfWeek.Monday;
                                break;
                            case "Thứ Ba":
                                timelineData.DayOfWeek = DayOfWeek.Tuesday;
                                break;
                            case "Thứ Tư":
                                timelineData.DayOfWeek = DayOfWeek.Wednesday;
                                break;
                            case "Thứ Năm":
                                timelineData.DayOfWeek = DayOfWeek.Thursday;
                                break;
                            case "Thứ Sáu":
                                timelineData.DayOfWeek = DayOfWeek.Friday;
                                break;
                            case "Thứ Bảy":
                                timelineData.DayOfWeek = DayOfWeek.Saturday;
                                break;
                            case "Chủ nhật":
                                timelineData.DayOfWeek = DayOfWeek.Sunday;
                                break;
                            default:
                                throw new Exception("Vui lòng bổ xung cột Thứ tại dòng thứ " + (i + 1));
                        }

                        data.TimelineData.Add(timelineData);
                    }
                }

                data.IsSuccess = true;
            }
            catch (Exception ex)
            {
                data.IsSuccess = false;
                data.Message = ex.Message;
            }

            return data;
        }
    }
}