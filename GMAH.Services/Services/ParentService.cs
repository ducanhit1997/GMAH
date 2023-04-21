using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GMAH.Services.Services
{
    /// <summary>
    /// Xử lý nghiệp vụ cho phụ huynh
    /// </summary>
    public class ParentService : BaseService
    {
        /// <summary>
        /// Lấy danh sách học sinh của phụ huynh
        /// </summary>
        public List<UserViewModel> GetListChild(int idParent)
        {
            var parentDB = _db.USERs.Where(x => x.IdUser == idParent 
            && x.IsDeleted != true 
            && x.IdRole == (int)RoleEnum.PARENT)
                .FirstOrDefault();

            // Nếu không tồn tại thì báo lỗi
            if (parentDB is null)
            {
                throw new Exception("Tài khoản phụ huynh không tồn tại");
            }

            // Lấy danh sách học sinh
            return parentDB.STUDENTs1.ToList().Select(x => ConvertToViewModel(x.USER)).ToList();
        }

        /// <summary>
        /// Lấy danh sách phụ huynh của học sinh
        /// </summary>
        /// <param name="idChild"></param>
        /// <returns></returns>
        public List<UserViewModel> GetListParent(int idChild)
        {
            var childDB = _db.USERs.AsNoTracking().Where(x => x.IdUser == idChild
                        && x.IsDeleted != true
                        && x.IdRole == (int)RoleEnum.STUDENT)
                            .FirstOrDefault();

            // Nếu không tồn tại thì báo lỗi
            if (childDB is null)
            {
                throw new Exception("Học sinh này không tồn tại");
            }

            // Lấy danh sách học sinh
            return childDB.STUDENTs.First().USERs.ToList().Select(x => ConvertToViewModel(x)).ToList();
        }

        /// <summary>
        /// Thêm học sinh khỏi danh sách quản lý của phụ huynh
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="idChild"></param>
        /// <returns></returns>
        public BaseResponse AddChildToParent(int idParent, int idChild)
        {
            // Kiểm tra parent có tồn tại trong system hay không
            var parentDB = _db.USERs
                .Where(x => x.IdUser == idParent && x.IsDeleted != true && x.IdRole == (int)RoleEnum.PARENT)
                .FirstOrDefault();

            // Nếu không tồn tại thì báo lỗi
            if (parentDB is null)
            {
                return new BaseResponse("Tài khoản phụ huynh không tồn tại");
            }

            // Kiểm tra học sinh
            var studentDB = _db.USERs
                .Where(x => x.IsDeleted != true && x.IdUser == idChild && x.IdRole == (int)RoleEnum.STUDENT)
                .SelectMany(x => x.STUDENTs)
                .FirstOrDefault();

            // Kiểm tra số lượng học sinh
            if (studentDB is null)
            {
                return new BaseResponse("Học sinh không tồn tại");
            }

            // Kiểm tra đã có quan hệ chưa
            var idStudent = studentDB.IdStudent;
            if (parentDB.STUDENTs1.Any(x => x.IdStudent == idStudent))
            {
                return new BaseResponse("Học sinh này đã tồn tại trong danh sách quản lý của phụ huynh");
            }

            parentDB.STUDENTs1.Add(studentDB);

            try
            {
                // Lưu lại
                _db.SaveChanges();

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

        /// <summary>
        /// Xoá học sinh khỏi danh sách quản lý của phụ huynh
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="idChild"></param>
        /// <returns></returns>
        public BaseResponse RemoveChildFromParent(int idParent, int idChild)
        {
            // Kiểm tra parent có tồn tại trong system hay không
            var parentDB = _db.USERs
                .Where(x => x.IdUser == idParent && x.IsDeleted != true && x.IdRole == (int)RoleEnum.PARENT)
                .FirstOrDefault();

            // Nếu không tồn tại thì báo lỗi
            if (parentDB is null)
            {
                return new BaseResponse("Tài khoản phụ huynh không tồn tại");
            }

            // Kiểm tra học sinh
            var studentDB = _db.USERs
                .Where(x => x.IsDeleted != true && x.IdUser == idChild && x.IdRole == (int)RoleEnum.STUDENT)
                .SelectMany(x => x.STUDENTs)
                .FirstOrDefault();

            // Kiểm tra số lượng học sinh
            if (studentDB is null)
            {
                return new BaseResponse("Học sinh không tồn tại");
            }

            // Kiểm tra đã có quan hệ chưa
            var idStudent = studentDB.IdStudent;
            if (!parentDB.STUDENTs1.Any(x => x.IdStudent == idStudent))
            {
                return new BaseResponse("Học sinh này chưa tồn tại trong danh sách quản lý của phụ huynh");
            }

            parentDB.STUDENTs1.Remove(studentDB);

            try
            {
                // Lưu lại
                _db.SaveChanges();

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

        /// <summary>
        /// Lưu danh sách học sinh cho phụ huynh
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="idUsers"></param>
        /// <exception cref="Exception"></exception>
        public void SetListChild(int idParent, List<int> idUsers)
        {
            // Kiểm tra parent có tồn tại trong system hay không
            var parentDB = _db.USERs
                .Where(x => x.IdUser == idParent && x.IsDeleted != true && x.IdRole == (int)RoleEnum.PARENT)
                .FirstOrDefault();

            // Nếu không tồn tại thì báo lỗi
            if (parentDB is null)
            {
                throw new Exception("Tài khoản phụ huynh không tồn tại");
            }

            // Lấy danh sách học sinh
            var studentsDB = _db.USERs
                .Where(x => x.IsDeleted != true && idUsers.Any(i => i == x.IdUser) && x.IdRole == (int)RoleEnum.STUDENT)
                .SelectMany(x => x.STUDENTs)
                .ToList();

            // Kiểm tra số lượng học sinh
            if (studentsDB.Count != idUsers.Count)
            {
                throw new Exception("Danh sách học sinh không hợp lệ");
            }

            // Xoá các học sinh không tồn tại trong list parent và add các học sinh chưa tồn tại
            var idStudents = studentsDB.Select(x => x.IdStudent).ToList();
            var listCurrentChild = parentDB.STUDENTs1.Select(x => x.IdStudent).ToList();
            var listChildNeedAddNew = idStudents.Where(x => !listCurrentChild.Any(i => i == x)).ToList();
            var listChildNeedRemove = parentDB.STUDENTs1.Where(x => !idStudents.Any(i => i == x.IdStudent)).ToList();

            // Add new
            var listStudentDBWillBeAdded = _db.STUDENTs.Where(x => listChildNeedAddNew.Any(i => x.IdStudent == i)).ToList();
            foreach (var addNew in listStudentDBWillBeAdded)
            {
                parentDB.STUDENTs1.Add(addNew);
            }

            // Remove
            foreach (var remove in listChildNeedRemove)
            {
                parentDB.STUDENTs1.Remove(remove);
            }

            try
            {
                // Lưu lại
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                throw new Exception("Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message);
            }
        }

        /// <summary>
        /// Lưu danh sách phụ huynh cho học sinh
        /// </summary>
        /// <param name="idChild"></param>
        /// <param name="idParents"></param>
        public void SetListParent(int idChild, List<int> idParents)
        {
            var childDB = _db.USERs.Where(x => x.IdUser == idChild
                               && x.IsDeleted != true
                               && x.IdRole == (int)RoleEnum.STUDENT)
                                   .FirstOrDefault();

            // Nếu không tồn tại thì báo lỗi
            if (childDB is null)
            {
                throw new Exception("Học sinh này không tồn tại");
            }

            // Lấy danh sách học sinh
            var parentsDB = _db.USERs
                .Where(x => x.IsDeleted != true && idParents.Any(i => i == x.IdUser) && x.IdRole == (int)RoleEnum.PARENT)
                .ToList();

            // Kiểm tra số lượng học sinh
            if (parentsDB.Count != idParents.Count)
            {
                throw new Exception("Danh sách phụ huynh không hợp lệ");
            }

            // Xoá các phụ huynh không tồn tại trong list child và add các phụ huynh chưa tồn tại trong db
            var studentDB = childDB.STUDENTs.First();

            var listCurrentParent = studentDB.USERs.Select(x => x.IdUser).ToList();
            var listParentNeedAddNew = idParents.Where(x => !listCurrentParent.Any(i => i == x)).ToList();
            var listParentNeedRemove = studentDB.USERs.Where(x => !idParents.Any(i => i == x.IdUser)).ToList();

            // Add new
            var listParentDBWillBeAdded = _db.USERs.Where(x => listParentNeedAddNew.Any(i => i == x.IdUser)).ToList();
            foreach (var addNew in listParentDBWillBeAdded)
            {
                studentDB.USERs.Add(addNew);
            }

            // Remove
            foreach (var remove in listParentNeedRemove)
            {
                studentDB.USERs.Remove(remove);
            }

            try
            {
                // Lưu lại
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                throw new Exception("Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message);
            }
        }

        /// <summary>
        /// Đổi thông tin học sinh
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse ChangeChildInfo(ChangeChildInfoRequest data)
        {
            // Kiểm tra model
            var validateModel = ValidationModelUtility.Validate(data);
            if (!validateModel.IsValidate)
            {
                return new SaveUserInfoResponse
                {
                    IsSuccess = false,
                    Message = validateModel.ErrorMessage,
                };
            }

            // Kiểm tra dữ liệu tồn tại
            var studentDB = _db.STUDENTs.Where(x => x.IdUser == data.IdChild && x.USERs.Any(i => i.IdUser == data.IdParent)).FirstOrDefault();

            if (studentDB is null)
            {
                return new GetClassScoreResponse
                {
                    IsSuccess = false,
                    Message = "Học sinh này không tồn tại",
                };
            }

            // Sửa info
            studentDB.USER.Email = data.Email;
            studentDB.USER.Phone = data.Phone;

            // Save lại data

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
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message,
                };
            }
        }

        public BaseResponse ViewChildInfo(int idChild, int idParent)
        {
            // Kiểm tra dữ liệu tồn tại
            var studentDB = _db.STUDENTs.Where(x => x.IdUser == idChild && x.USERs.Any(i => i.IdUser == idParent)).FirstOrDefault();

            if (studentDB is null)
            {
                return new GetClassScoreResponse
                {
                    IsSuccess = false,
                    Message = "Học sinh này không tồn tại",
                };
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Object = ConvertToViewModel(studentDB.USER),
            };
        }
    }
}
