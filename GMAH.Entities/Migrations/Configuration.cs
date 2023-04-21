namespace GMAH.Entities.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GMAH.Entities.GMAHEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(GMAH.Entities.GMAHEntities context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            AddDumpSystemSettingTable(context);
            AddDumpDataRoleTable(context);
            AddDumpDataReportStatusTable(context);
        }

        private void AddDumpDataReportStatusTable(GMAHEntities context)
        {
            context.REPORT_STATUS.AddOrUpdate(new REPORT_STATUS
            {
                IdReportStatus = 1,
                StatusName = "Chờ giáo viên chủ nhiệm duyệt",
            });

            context.REPORT_STATUS.AddOrUpdate(new REPORT_STATUS
            {
                IdReportStatus = 2,
                StatusName = "Chờ giám thị duyệt",
            });

            context.REPORT_STATUS.AddOrUpdate(new REPORT_STATUS
            {
                IdReportStatus = 3,
                StatusName = "Chờ giáo viên bộ môn duyệt",
            });

            context.REPORT_STATUS.AddOrUpdate(new REPORT_STATUS
            {
                IdReportStatus = 100,
                StatusName = "Đã được duyệt",
            });

            context.REPORT_STATUS.AddOrUpdate(new REPORT_STATUS
            {
                IdReportStatus = 101,
                StatusName = "Bị từ chối",
            });
        }

        private void AddDumpSystemSettingTable(GMAHEntities context)
        {
            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "LOGIN_IMAGE",
                SettingName = "Background hình ảnh trang đăng nhập",
                InputType = "1",
                SettingValue = "https://uop.scene7.com/is/image/phoenixedu/two-male-coworkers-taking-learning-and-development-courses-970x580?fmt=webp-alpha&qlt=70&fit=constrain,1&wid=970",
            });

            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "EMAIL_USERNAME",
                SettingName = "Tài khoản email",
                InputType = "1",
                SettingValue = "hethong.gmah@gmail.com",
            });

            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "EMAIL_PASSWORD",
                SettingName = "Mật khẩu email",
                InputType = "2",
                SettingValue = "adzwgwappvqkbcag",
            });

            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "EMAIL_SENDERNAME",
                SettingName = "Tên người gửi mail",
                InputType = "1",
                SettingValue = "GMAH",
            });

            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "EMAIL_SMTP",
                SettingName = "SMTP Server",
                InputType = "1",
                SettingValue = "smtp.gmail.com",
            });

            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "EMAIL_PORT",
                SettingName = "SMTP Port",
                InputType = "4",
                SettingValue = "587",
            });

            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "EMAIL_TEMPLATE_PASSWORD",
                SettingName = "Mẫu email gửi mật khẩu",
                InputType = "5",
                SettingValue = @"<p>Chào bạn,</p>
<p>Mật khẩu của bạn đã được làm mới, vui lòng sử dụng mật khẩu dưới đây để truy cập.</p>
<table style=""border-collapse: collapse; width: 100%;"" border=""1"">
<tbody>
<tr>
<td style=""width: 26.5112%;"">Tài khoản</td>
<td style=""width: 73.4888%;"">{username}</td>
</tr>
<tr>
<td style=""width: 26.5112%;"">Mật khẩu</td>
<td style=""width: 73.4888%;"">{password}</td>
</tr>
</tbody>
</table>
<p>Cám ơn bạn đã sử dụng hệ thống.</p>",
            });

            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "EMAIL_TEMPLATE_SCOREBASELINE",
                SettingName = "Mẫu email gửi thông báo sửa điểm",
                InputType = "5",
                SettingValue = @"<p>Chào bạn,</p>
<p>Điểm số của học sinh đã được thay đổi, bạn có thể truy cập vào cổng thông tin để kiểm tra lại.</p>
<table style=""border-collapse: collapse; width: 100%;"" border=""1"">
<tbody>
<tr>
<td style=""width: 26.5112%;"">Họ tên học sinh</td>
<td style=""width: 73.4888%;"">{fullname}</td>
</tr>
<tr>
<td style=""width: 26.5112%;"">Học kỳ được sửa điểm</td>
<td style=""width: 73.4888%;"">{semester}</td>
</tr>
</tbody>
</table>
<p>Cám ơn bạn đã sử dụng hệ thống.</p>",
            });

            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "EMAIL_TEMPLATE_SCORE",
                SettingName = "Mẫu email gửi thông báo điểm học kỳ",
                InputType = "5",
                SettingValue = @"<p>Chào bạn,</p>
<p>Nhà trường xin thông báo về điểm và hạnh kiểm của học sinh.</p>
<table style=""border-collapse: collapse; width: 100%;"" border=""1"">
<tbody>
<tr>
<td style=""width: 26.5112%;"">Họ tên học sinh</td>
<td style=""width: 73.4888%;"">{fullname}</td>
</tr>
		<tr>
			<td>Năm học</td>
			<td>{semester}</td>
		</tr>
		<tr>
			<td>Hạnh kiểm</td>
			<td>{behaviour}</td>
		</tr>
		<tr>
			<td>Điểm trung bình</td>
			<td>{score}</td>
		</tr>
		<tr>
			<td>Xếp hạng</td>
			<td>{rank}</td>
		</tr>
</tbody>
</table>
<p>Cám ơn bạn đã sử dụng hệ thống.</p>",
            });

            context.SYSTEMSETTINGs.AddOrUpdate(new SYSTEMSETTING
            {
                SettingKey = "EMAIL_TEMPLATE_REPORT",
                SettingName = "Mẫu email gửi thông báo báo cáo thay đổi",
                InputType = "5",
                SettingValue = @"<p>Chào bạn,</p>
<p>Báo cáo của bạn có cập nhật mới nhất</p>
<table style=""border-collapse: collapse; width: 100%;"" border=""1"">
<tbody>
<tr>
<td style=""width: 26.5112%;"">Mã báo cáo</td>
<td style=""width: 73.4888%;"">{idreport}</td>
</tr>
<tr>
<td style=""width: 26.5112%;"">Tiêu đề</td>
<td style=""width: 73.4888%;"">{reporttitle}</td>
</tr>
<tr>
<td style=""width: 26.5112%;"">Trạng thái hiện tại</td>
<td style=""width: 73.4888%;"">{status}</td>
</tr>
</tbody>
</table>
<p>Cám ơn bạn đã sử dụng hệ thống.</p>",
            });
        }

        private void AddDumpDataRoleTable(GMAHEntities context)
        {
            context.ROLEs.AddOrUpdate(new ROLE
            {
                IdRole = 1,
                RoleName = "Ban giám hiệu",
            });

            context.ROLEs.AddOrUpdate(new ROLE
            {
                IdRole = 2,
                RoleName = "Giám thị",
            });

            context.ROLEs.AddOrUpdate(new ROLE
            {
                IdRole = 3,
                RoleName = "Trưởng bộ môn",
            });

            context.ROLEs.AddOrUpdate(new ROLE
            {
                IdRole = 4,
                RoleName = "Giáo viên",
            });

            context.ROLEs.AddOrUpdate(new ROLE
            {
                IdRole = 5,
                RoleName = "Phụ huynh",
            });

            context.ROLEs.AddOrUpdate(new ROLE
            {
                IdRole = 6,
                RoleName = "Học sinh",
            });
        }
    }
}
