using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GMAH.Services.Services
{
    public class SystemSettingService : BaseService
    {
        /// <summary>
        /// Lấy value từng 1 setting cụ thể
        /// </summary>
        /// <returns></returns>
        public string GetSetting(SettingEnum key)
        {
            var settingDB = _db.SYSTEMSETTINGs.AsNoTracking().Where(x => x.SettingKey.Equals(key.ToString(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return settingDB?.SettingValue;
        }

        /// <summary>
        /// Lấy toàn bộ setting
        /// </summary>
        /// <returns></returns>
        public List<SettingViewModel> GetAllSetting()
        {
            var settingList = _db.SYSTEMSETTINGs.AsNoTracking().ToList();

            // Chỉ lấy list theo danh sách define trong SettingEnum
            var listResult = new List<SettingViewModel>();
            foreach (SettingEnum settingKey in (SettingEnum[])Enum.GetValues(typeof(SettingEnum)))
            {
                var key = settingKey.ToString();
                var setting = settingList.Where(x => x.SettingKey.Equals(key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (setting is null)
                {
                    continue;
                }

                listResult.Add(ConvertToViewModel(setting));
            }

            return listResult;
        }

        /// <summary>
        /// Lưu toàn bộ setting
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public BaseResponse SaveAllSetting(List<SettingViewModel> settings)
        {
            foreach (var setting in settings)
            {
                // Lấy setting trong db
                var settingDB = _db.SYSTEMSETTINGs.Where(x => x.SettingKey.Equals(setting.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                // Tạo mới key nếu chưa có
                if (settingDB is null)
                {
                    settingDB = new SYSTEMSETTING
                    {
                        // Default là text
                        InputType = ((int)setting.Type).ToString(),
                    };
                    _db.SYSTEMSETTINGs.Add(settingDB);
                }

                // Gán dữ liệu mới
                settingDB.SettingValue = setting.Value;
                settingDB.SettingName = setting.Name;
            }

            // Lưu lại dữ liệu
            try
            {
                // Lưu lại
                _db.SaveChanges();

                // Thành công
                return new BaseResponse
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                return new BaseResponse("Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message);
            }
        }
    }
}
